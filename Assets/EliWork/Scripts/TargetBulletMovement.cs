using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//A child of the Bullet Movement - this type of bullet will change direction to shoot towards the player after a certain amount of time
public class TargetBulletMovement : BulletMovement
{
    [SerializeField] private float timeTilTarget;//how much time passes before this changes direction and targets the player
    [SerializeField] private float timeToTurn;//How long it takes for the bullet to turn before shooting straight
    private bool targeting;
    private float timeStartTargetting;
    private float targetRot = -360;
    private float startLife;
    void Start()
    {
    }

    // Update is called once per frame
    /*protected override void Update()
    {
        if(!targeting) {
            transform.position += transform.up * speed * Time.deltaTime;
            lifeTime -= Time.deltaTime;
            if(lifeTime <= 0) {
                Deactivate();
            }
            if(lifeTime <= startLife - timeTilTarget && targetRot == -360) {
                //Gets its rotation amt, and begins to turn
                //targeting = true;
                timeStartTargetting = lifeTime;
                targetRot = 0;
                //Need to figure out how to get this to work
                transform.LookAt(Player.Instance.transform.position, Vector3.forward);
            }
        }
        else {
            //Spends a certain amt of time turning to target the player
        }
    }*/
    public override IEnumerator Fire() {
        //Spawn animation happens prior to everything else
            //Can't damage anyone while it's spawning
        myHitBox.enabled = false;
        float curTime = 0;
        while(curTime < spawnTime) {
            curTime += Time.deltaTime;
            yield return null;
        }
        myHitBox.enabled = true;
        startLife = lifeTime;
        //Shoots straight until it gets ordered to turn
        while(lifeTime > startLife - timeTilTarget && lifeTime > 0) {
            transform.position += transform.up * speed * Time.deltaTime;
            lifeTime -= Time.deltaTime;
            yield return null;
        }
        timeStartTargetting = lifeTime;
        targetRot = 0;
        //Need to figure out how to get this to work
        transform.LookAt(Player.Instance.transform.position, Vector3.forward);
        while(lifeTime > timeStartTargetting - timeToTurn && lifeTime > 0) {
            //Needs to slowly lerp between 2 positions
            lifeTime -= Time.deltaTime;
            yield return null;
        }
        while(lifeTime > 0) {
            transform.position += transform.up * speed * Time.deltaTime;
            lifeTime -= Time.deltaTime;
            yield return null;
        }
        Deactivate();
    }
}
