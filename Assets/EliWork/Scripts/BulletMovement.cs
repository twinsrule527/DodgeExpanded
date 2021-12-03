using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//Attached to a bullet object, which will be spawned, shoot forward, and die
public class BulletMovement : MonoBehaviour
{
    public float lifeTime;
    public float speed;
    [SerializeField] protected float spawnTime;//How long the bullet's spawn animation takes, before it moves
    protected Collider2D myHitBox;
    [SerializeField] private DmgType _playerDamage;//The type of damage this deals to the player
    public DmgType PlayerDamage {
        get {
            return _playerDamage;
        }
    }
    [SerializeField] protected bool invisibleOutsideBox;//Whether the Bullet should be invisible when outside the bounding box
    [SerializeField] private bool _destroyOnCollision;//if enabled, this bullet will be destroyed when it collides with a player
    public bool DestroyOnCollision {
        get {
            return _destroyOnCollision;
        }
    }

    protected void Awake() {
        myHitBox = GetComponent<Collider2D>();
    }
    /*protected virtual void Update()
    {
        position += transform.up * speed * Time.deltaTime;
        lifeTime -= Time.deltaTime;
        if(lifeTime <= 0) {
            Deactivate();
        }
    }*/

    //Deactivates the bullet, until it might need to be used again
    public void Deactivate() {
        //Deactivates Coroutine if its running
        StopCoroutine("Fire");
        gameObject.SetActive(false);
        if(!BorderMovement.Instance.inactiveBullets.ContainsKey(name)) {
            BorderMovement.Instance.inactiveBullets.Add(name, new List<BulletMovement>());
        }
        BorderMovement.Instance.inactiveBullets[name].Add(this);
    }

    //Whenever a bullet is created, it will be fired, following this IEnumerator until it deactivates or is destroyed
    public virtual IEnumerator Fire() {
        //Spawn animation happens prior to everything else
            //Can't damage anyone while it's spawning
        myHitBox.enabled = false;
        if(invisibleOutsideBox) {
            InvisOnBox();
        }
        yield return new WaitForSeconds(spawnTime);
        myHitBox.enabled = true;
        while(lifeTime > 0) {
            transform.position += transform.up * speed * Time.deltaTime;
            if(invisibleOutsideBox) {
                InvisOnBox();
            }
            lifeTime -= Time.deltaTime;
            yield return null;
        }
        Deactivate();
    }
    
    //Checks to see if its outside the bounding box - if it is, it becomes invisible (only if bool is active)
    public void InvisOnBox() {
        float x = transform.position.x;
        float y = transform.position.y;
        Vector3[] boxCorners = new Vector3[5];
        BorderMovement.Instance.MyLine.GetPositions(boxCorners);
        if(x > boxCorners[2].x || x < boxCorners[0].x || y > boxCorners[1].y || y < boxCorners[3].y) {
            transform.position = new Vector3(transform.position.x, transform.position.y, -1000);
        }
        else {
            transform.position = new Vector3(transform.position.x, transform.position.y, 0);
        }
    }

    //Triggers when this deals damage to a player
    public void DealDamage(Player player) {
        if(PlayerDamage == DmgType.OneShot) {
            player.transform.position = BorderMovement.Instance.Level.Borders[BorderMovement.Instance.CurBorder].playerStart;
            StartCoroutine(BorderMovement.Instance.BorderShake(0.1f));
        }
        else if(PlayerDamage == DmgType.Knockback) {
            //Has to deal knockback over a certain amt of time
                //NEED TO ATTACH COROUTINE TO THE PLAYER, rather than the bullet
            //StartCoroutine(DealKnockback(player));
        }
    }

    /*private IEnumerator DealKnockback(Player player) {
        player.frozen = true;
        player.rigidbody.velocity = transform.up * 15f;
        yield return new WaitForSeconds(0.25f);
        player.frozen = false;
    }*/
}

//Types of damage a bullet can deal 
public enum DmgType {
    Normal, //Doesn't really do anything except increase hit counter
    Knockback, //Deals knockback to the player
    OneShot //Kills the player and sends them back to the beginning of this room
}
