using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//A bullet spawn tool that spawns several bullets evenly spaced apart
public class ArrayMissingBulletSpawnTool : BulletSpawnTool
{
    //private const int NUM_MISSING_SPOTS = 
    [SerializeField] private float numBullets;
    [SerializeField] private bool startFromRight;//naturally is set false - if set true, it wull go right to left, rather than left to right (in case that maters)
    [SerializeField] private float timeBtwnBullets;//How much time there should be between each bullet spawning
    [SerializeField] private float offSetFromEdge = 0.1f;//Most of the time, this is ignored - it says how far from the edge of this object is the first bullet spawned
    

    public override void Awake() {
        myBullets = CreateBulletSpawnFromThis();
        myType = BulletSpawnType.missingSpots;
    }
    //MATH SCALING IS CURRENTLY WRONG DO NOT USE THIS UNTIL I FIX IT
    public override List<BulletSpawn> CreateBulletSpawnFromThis() {
        List<int> missingSpots = new List<int>();
        List<BulletSpawn> myBullets = new List<BulletSpawn>();
        //Figures out how far apart each bullet is
        float separation = (transform.localScale.x - offSetFromEdge * 2) / numBullets;
        float startPosX = 0;
        //The position that the first bullet spawns from is set
        if(startFromRight) {
            startPosX = transform.localScale.x / 2f + offSetFromEdge;
        }
        else {
            startPosX = transform.localScale.x / 2f - offSetFromEdge;
        }
        //Spawns all the bullets
        for(int i = 0; i < numBullets; i++) {
            if(startFromRight) { 
                myBulletSpawn.spawnPos = transform.position;
                myBulletSpawn.spawnPos += (transform.right * (startPosX - separation * i));
                myBulletSpawn.spawnRotation = transform.localEulerAngles.z;
                myBulletSpawn.startDelay = startDelay + timeBtwnBullets * i;
                //All of them can be fired at once, or they can be fired one after another
                if(timeBtwnBullets == 0) {
                    myBulletSpawn.delay = bulletDelay;
                }
                else {
                    myBulletSpawn.delay = timeBtwnBullets * numBullets;
                }
                myBulletSpawn.bulletSpeed = bulletSpeed;
                myBulletSpawn.bulletLifetime = bulletLife;
                myBulletSpawn.bullet = Bullet;
            }
            else {
                myBulletSpawn.spawnPos = transform.position;
                myBulletSpawn.spawnPos -= (transform.right * (startPosX - separation * i));
                myBulletSpawn.spawnRotation = transform.localEulerAngles.z;
                myBulletSpawn.startDelay = startDelay + timeBtwnBullets * i;
                //All of them can be fired at once, or they can be fired one after another
                if(timeBtwnBullets == 0) {
                    myBulletSpawn.delay = bulletDelay;
                }
                else {
                    myBulletSpawn.delay = timeBtwnBullets * numBullets;
                }
                myBulletSpawn.bulletSpeed = bulletSpeed;
                myBulletSpawn.bulletLifetime = bulletLife;
                myBulletSpawn.bullet = Bullet;
            }
            myBullets.Add(myBulletSpawn);
        }
        return myBullets;
    }
}
