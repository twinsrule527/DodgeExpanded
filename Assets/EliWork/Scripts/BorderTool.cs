using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//Attached to a rectangle, with some child components, to determine the attributes of a BorderBox
public class BorderTool : MonoBehaviour
{
    public Border myBorder;
    private TextBox myTextBox;
    [Header("Attributes that need to be filled in inspector")]
    [SerializeField] private Shape myShape;
    [SerializeField] private float myLineWidth = 0.5f;
    
    [Header("Child objects which can be manipulated")]
    [SerializeField] private Transform myBoundingBorder;
    [SerializeField] private Transform myCheckPointBox;
    [SerializeField] private Transform myPlayerStartSpot;
    [Header("The Number Room this Is")]
    [SerializeField] private int _number;//The number border this is in the game
    public int Number {
        get {
            return _number;
        }
    }
    
    [Header("TextBox Stuff")]
    //Stuff for the text box
    [SerializeField] private Transform textBoxBounds;
    [SerializeField] private string text;
    [SerializeField] private int textSize;

    public Border CreateBorderFromThis()
    {
        myBorder.pos = myBoundingBorder.position - myBoundingBorder.localScale / 2f;
        myBorder.size = myBoundingBorder.localScale;
        myBorder.shape = myShape;
        myBorder.width = myLineWidth;
        myBorder.checkpointPos = myCheckPointBox.position - myCheckPointBox.localScale / 2f;
        myBorder.checkpointSize = myCheckPointBox.localScale;
        myBorder.playerStart = myPlayerStartSpot.position;
        //Creates a list of bullets from the bullet spawns in the room
        myBorder.BulletHell = new List<BulletSpawn>();
        myBorder.BulletHell2 = new List<BulletSpawnTool>();
        List<BulletSpawnTool> myBulletSpawners = new List<BulletSpawnTool>(GetComponentsInChildren<BulletSpawnTool>());
        foreach(BulletSpawnTool spawner in myBulletSpawners) {
            myBorder.BulletHell2.Add(spawner);
            List<BulletSpawn> curBullets = spawner.CreateBulletSpawnFromThis();
            foreach(BulletSpawn bullet in curBullets) {
                myBorder.BulletHell.Add(bullet);
            }
        }
        //Sets the TextBox in Canvas Space
        Vector2 textBorder = new Vector2(TextBox.ConvertWorldToCanvas(textBoxBounds.position.x), TextBox.ConvertWorldToCanvas(textBoxBounds.position.y));
        Vector2 textBorderSize = new Vector2(TextBox.ConvertWorldToCanvas(textBoxBounds.localScale.x), TextBox.ConvertWorldToCanvas(textBoxBounds.localScale.y));
        myTextBox.pos = textBorder;
        myTextBox.size = textBorderSize;
        myTextBox.text = text;
        myTextBox.textSize = textSize;
        myBorder.RoomText = myTextBox;
        return myBorder;
    }
}
