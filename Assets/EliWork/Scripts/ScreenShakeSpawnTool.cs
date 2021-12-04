using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenShakeSpawnTool : BulletSpawnTool
{
    public float shakeIntensity;
    public override void Awake() {
        myType = BulletSpawnType.screenShake;
        myBullets = CreateBulletSpawnFromThis();
    }

    public override List<BulletSpawn> CreateBulletSpawnFromThis()
    {
        return new List<BulletSpawn>();
    }
}
