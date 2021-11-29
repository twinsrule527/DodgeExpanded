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
        float curTime = 0;
        while(curTime < spawnTime) {
            curTime += Time.deltaTime;
            yield return null;
        }
        myHitBox.enabled = true;
        while(lifeTime > 0) {
            transform.position += transform.up * speed * Time.deltaTime;
            lifeTime -= Time.deltaTime;
            yield return null;
        }
        Deactivate();
    }
}
