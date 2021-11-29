using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//A child of the Bullet Movement - this type of bullet will change direction to shoot towards the player after a certain amount of time
public class TargetBulletMovement : BulletMovement
{
    [SerializeField] private float timeTilTarget;//how much time passes before this changes direction and targets the player
    [SerializeField] private float timeToTurn;//How long it takes for the bullet to turn before shooting straight
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
        //Gets the directional vector from this to the player
        Vector2 pointDiff = Player.Instance.transform.position - transform.position;
        pointDiff = pointDiff.normalized;
        float finalAngle = 0;
        if(pointDiff.x != 0) {
            float tanAngle = Mathf.Atan(pointDiff.y / pointDiff.x);
            if(pointDiff.x >  0) {
                finalAngle = tanAngle * 360 / 2 / Mathf.PI;
            }
            else { 
                finalAngle = tanAngle * 360 / 2 / Mathf.PI + 180;
            }
        }
        else {
            finalAngle = 90 * pointDiff.y;
        }
        //Now, need to turn it to adjust with up being vertical
        finalAngle -=90;
        //Now, we need to adjust it so that the object makes the fastest turn possible
        float otherAngle = 0;
        if(finalAngle > 0) {
            otherAngle = -360 + finalAngle;
        }
        else {
            otherAngle = 360 + finalAngle;
        }
        //Check to see if its faster to turn clockwise or counterclockwise
        if(Mathf.Abs(transform.localEulerAngles.z - finalAngle) > Mathf.Abs(transform.localEulerAngles.z - otherAngle)) {
            finalAngle = otherAngle;
        }
        Vector3 direction1 = new Vector3(0, 0, transform.localEulerAngles.z);
        Vector3 direction2 = new Vector3(0, 0, finalAngle);
        float currentTime = 0;
        //Need to figure out how to get this to work
        while(lifeTime > timeStartTargetting - timeToTurn && lifeTime > 0) {
            //Needs to slowly lerp between 2 positions
            lifeTime -= Time.deltaTime;
            currentTime += Time.deltaTime;
            currentTime = Mathf.Min(currentTime, timeToTurn);
            transform.rotation = Quaternion.Lerp(Quaternion.Euler(direction1), Quaternion.Euler(direction2), currentTime / timeToTurn);
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
