using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StopWallKillBulletTool : BulletSpawnTool
{
    [Header("Used in StopWallKillTool")]
    public float timeToStop;
    public override void Awake() {
        myType = BulletSpawnType.endWallKill;
        myBullets = CreateBulletSpawnFromThis();
    }

    public override List<BulletSpawn> CreateBulletSpawnFromThis()
    {
        return new List<BulletSpawn>();
    }
}
