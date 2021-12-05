using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//Provides info about how the border changes

//A struct which holds the information for a border when its active
[System.Serializable] public struct Border {
    public Vector2 pos;//Bottom left corner of the border
    public Vector2 size;//Dimensions of the border
    public Shape shape;//The shape of the border - for now, must be a rectangle
    public float width;//The width of the edge of the border (if it changes, it will Lerp)
    public Vector2 checkpointPos;//Bottom left corner of the checkpoint box
    public Vector2 checkpointSize;//Dimensions of the checkpoint box
    public Sprite checkpointSprite;//Sprite of the checkpoint
    public Vector3 playerStart;//Where the player starts in this border area
    public List<BulletSpawn> BulletHell;//A list of how bullets will spawn while this Border is active - this is outdated, but I'll keep it around for posterity's sake
    public List<BulletSpawnTool> BulletHell2;//A better list of how bullets will spawn
    public TextBox RoomText;
    public List<Vector2> BorderCorners {//Gets the correct corners of the border
        get {
            List<Vector2> newBorders = new List<Vector2>();
            if(shape == Shape.rect) {
                //If the border is a rectangle, goes around clockwise, adding the vertices of its shape
                Vector2 currentPos = pos;
                newBorders.Add(currentPos);
                currentPos += new Vector2(0, size.y);
                newBorders.Add(currentPos);
                currentPos += new Vector2(size.x, 0);
                newBorders.Add(currentPos);
                currentPos -= new Vector2(0, size.y);
                newBorders.Add(currentPos);
                currentPos -= new Vector2(size.x, 0);
                newBorders.Add(currentPos);
                //Ends with 5 corners, bc it needs to go back to the start
            }
            return newBorders;
        }
    }
    //These two functions allow for the efficient start and ending of a bullet hell mode
    public void StartBulletHell() {
        List<IEnumerator> runningBulletCoroutines = new List<IEnumerator>();
        /*foreach(BulletSpawn bullets in BulletHell) {
            IEnumerator bulletCoroutine = BorderMovement.Instance.SpawnBullets(bullets);
            runningBulletCoroutines.Add(bulletCoroutine);
            BorderMovement.Instance.StartCoroutine(bulletCoroutine);
        }*/
        foreach(BulletSpawnTool bulletTool in BulletHell2) {
            if(bulletTool.myType == BulletSpawnType.normal) {
                foreach(BulletSpawn bullets in bulletTool.myBullets) {
                    IEnumerator bulletCoroutine = BorderMovement.Instance.SpawnBullets(bullets);
                    runningBulletCoroutines.Add(bulletCoroutine);
                    BorderMovement.Instance.StartCoroutine(bulletCoroutine);
                }
            }
            else if(bulletTool.myType == BulletSpawnType.screenShake) {
                ScreenShakeSpawnTool attachedTool = bulletTool.GetComponent<ScreenShakeSpawnTool>();
                IEnumerator shakeCoroutine = BorderMovement.Instance.repeatBorderShake(attachedTool.shakeIntensity);
                runningBulletCoroutines.Add(shakeCoroutine);
                BorderMovement.Instance.StartCoroutine(shakeCoroutine);
            }
            else if(bulletTool.myType == BulletSpawnType.missingSpots) {
                IEnumerator spawnCoroutine = BorderMovement.Instance.SpawnBulletsMissingArray(bulletTool.myBullets, bulletTool.numMissing, bulletTool.numOffset, bulletTool.missLoop);
                runningBulletCoroutines.Add(spawnCoroutine);
                BorderMovement.Instance.StartCoroutine(spawnCoroutine);
            }
        }
        BorderMovement.Instance.runningBulletCoroutines = runningBulletCoroutines;
    }
    public void EndBulletHell() {
        foreach(IEnumerator bulletCoroutine in BorderMovement.Instance.runningBulletCoroutines) {
            BorderMovement.Instance.StopCoroutine(bulletCoroutine);
        }
    }
}

//Contains information regarding the transition between 2 borders
[System.Serializable] public struct BorderTransition {
    public Border startBorder;
    public Border endBorder;
    public float changeTime;
}
//this enum dictates whether the border is in a rectangle or some other shape
public enum Shape {
    rect,
    other
}

//So that we can spawn bullet arrays in different ways
public enum BulletSpawnType {
    normal,
    screenShake,
    missingSpots//For when an array wants to miss certain values
}

//Contains information for repeatably spawning bullets
[System.Serializable] public struct BulletSpawn {
    public Vector3 spawnPos;//position at which the bullets spawn
    public float spawnRotation;//Direction in which the bullets are pointing when they spawn (in degrees, counter-clockwise from upwards) (currently, bullets only move in a straight line)
    public float startDelay;//Delay (in seconds) before the first bullet spawns
    public float delay;//Amount of time between bullets spawning usually
    //Attributes added to bullets
    public float bulletSpeed;//Speed of bullets (in units per second)
    public float bulletLifetime;//How long the bullets live before self-destructing (in seconds)
    public BulletMovement bullet;//The prefab which this bullet spawn summons
}

//Contains information about the textBox that appears for any level
[System.Serializable] public struct TextBox {
    public string text;
    public int textSize;
    //Both of these vector2s are set in Canvas space, rather than in world space
    public Vector2 pos;
    public Vector2 size;

    public const float WORLD_TO_CANVAS_RATIO = 225;//When changing something from a world point to a canvas point, this is the ratio by which it needs to be changed (divided by the camera size)
    public static float ConvertWorldToCanvas(float startFloat) {
        float newFloat = startFloat * WORLD_TO_CANVAS_RATIO / Camera.main.orthographicSize;
        return newFloat;
    }
}
public class BorderInfo : MonoBehaviour
{
    
}
