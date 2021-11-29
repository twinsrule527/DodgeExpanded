using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//A bulletSpawn tool that spawns several bullet spawners in a spiral - can be used in a circle as well
public class SpiralBulletSpawnTool : BulletSpawnTool
{
    [SerializeField] private int numBullets;//How many bullets should be in the circle
    [SerializeField] private float timeBtwnBullets;//How much time there should be between each bullet spawning
    [SerializeField] private float radius;//Set to 0 for a typical spiral - if you want to have a circle shooting inwards, set this larger
    public override List<BulletSpawn> CreateBulletSpawnFromThis() {
        float initialDirection = transform.localEulerAngles.z % 360;
        float degreesPerBullet = 360f / numBullets;
        //Creates a list of all bullets
        List<BulletSpawn> myBullets = new List<BulletSpawn>();
        for(int i = 0; i < numBullets; i++) {
            //Sets this obj rotation to give the bullet spawn its rotation
            transform.rotation = Quaternion.Euler(0, 0, initialDirection + i * degreesPerBullet);
            myBulletSpawn.spawnPos = transform.position - transform.up * radius;
            myBulletSpawn.spawnRotation = transform.localEulerAngles.z % 360;
            myBulletSpawn.startDelay = startDelay + i * timeBtwnBullets;
            if(timeBtwnBullets != 0) {
                myBulletSpawn.delay = timeBtwnBullets * numBullets;
            }
            else {
                myBulletSpawn.delay = bulletDelay;
            }
            myBulletSpawn.bulletSpeed = bulletSpeed;
            myBulletSpawn.bulletLifetime = bulletLife;
            myBulletSpawn.bullet = Bullet;
            myBullets.Add(myBulletSpawn);
        }
        return myBullets;
    }
}
