using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//Attached to a rectangle, with some child components, to determine the attributes of a BorderBox
public class BorderTool : MonoBehaviour
{
    public Border myBorder;
    [Header("Attributes that need to be filled in inspector")]
    [SerializeField] private Shape myShape;
    [SerializeField] private float mylineWidth = 0.5f;
    
    [Header("Child objects which can be manipulated")]
    [SerializeField] private Transform myBoundingBorder;
    [SerializeField] private Transform myCheckPointBox;
    [SerializeField] private Transform myPlayerStartSpot;
    [SerializeField] private int _number;//The number border this is in the game
    public int Number {
        get {
            return _number;
        }
    }
    

    public Border CreateBorderFromThis()
    {
        myBorder.pos = myBoundingBorder.position - myBoundingBorder.localScale / 2f;
        myBorder.size = myBoundingBorder.localScale;
        myBorder.shape = myShape;
        myBorder.width = mylineWidth;
        myBorder.checkpointPos = myCheckPointBox.position - myCheckPointBox.localScale / 2f;
        myBorder.checkpointSize = myCheckPointBox.localScale;
        myBorder.playerStart = myPlayerStartSpot.position;
        myBorder.BulletHell = new List<BulletSpawn>();
        List<BulletSpawnTool> myBulletSpawners = new List<BulletSpawnTool>(GetComponentsInChildren<BulletSpawnTool>());
        foreach(BulletSpawnTool spawner in myBulletSpawners) {
            myBorder.BulletHell.Add(spawner.CreateBulletSpawnFromThis());
        }
        return myBorder;
    }
}
