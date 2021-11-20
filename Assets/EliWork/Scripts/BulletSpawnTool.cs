using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//A child of the BorderTool, used to create spawn conditons for bullets
public class BulletSpawnTool : MonoBehaviour
{
    private BulletSpawn myBulletSpawn;
    [Header("Need to be filled in for bullet spawning")]
    [SerializeField] private float startDelay;
    [SerializeField] private float bulletDelay;
    [SerializeField] private float bulletSpeed;
    [SerializeField] private float bulletLife;
    public BulletSpawn CreateBulletSpawnFromThis() {
        myBulletSpawn.spawnPos = transform.position;
        myBulletSpawn.spawnRotation = transform.position.z % 360;
        myBulletSpawn.startDelay = startDelay;
        myBulletSpawn.delay = bulletDelay;
        myBulletSpawn.bulletSpeed = bulletSpeed;
        myBulletSpawn.bulletLifetime = bulletLife;
        return myBulletSpawn;
    }
}
