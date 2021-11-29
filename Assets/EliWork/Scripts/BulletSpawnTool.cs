using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//A child of the BorderTool, used to create spawn conditons for bullets
public class BulletSpawnTool : MonoBehaviour
{
    protected BulletSpawn myBulletSpawn;
    [Header("Need to be filled in for bullet spawning")]
    [SerializeField] protected float startDelay;
    [SerializeField] protected float bulletDelay;
    [SerializeField] protected float bulletSpeed;
    [SerializeField] protected float bulletLife;
    [SerializeField] protected BulletMovement Bullet;
    public virtual List<BulletSpawn> CreateBulletSpawnFromThis() {
        //Now, creates a list of bullets, so we can have different tools that work differently
        List<BulletSpawn> myBullets = new List<BulletSpawn>();
        myBulletSpawn.spawnPos = transform.position;
        myBulletSpawn.spawnRotation = transform.localEulerAngles.z % 360;
        myBulletSpawn.startDelay = startDelay;
        myBulletSpawn.delay = bulletDelay;
        myBulletSpawn.bulletSpeed = bulletSpeed;
        myBulletSpawn.bulletLifetime = bulletLife;
        myBulletSpawn.bullet = Bullet;
        myBullets.Add(myBulletSpawn);
        return myBullets;
    }
}
