using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
//Moves the border over a period of time - this is a test
public class BorderMovement : MonoBehaviour
{
    public static BorderMovement Instance;//Allows every object to access this (basically a singleton)
    public float timeToChange;
    private LineRenderer myLine;
    private EdgeCollider2D myEdge;
    [SerializeField] private Transform checkPointBox;//The box for when a change in transition occurs
    [SerializeField] private Player player;//The player needs to be able to be moved around
    public BorderLevelScriptableObject Level;
    private int curBorder;
    public int CurBorder {
        get {
            return curBorder;
        }
    }

    public Dictionary<string, List<BulletMovement>> inactiveBullets;//A Dictionary of all bullets which exist but aren't active, so they can be reviewed - Each string is the name of a BulletPrefab Type
    private List<BulletMovement> activeBullets;//A List of all bullets being fired - deactivates them when the map changes
    [SerializeField] private BulletMovement bulletPrefab;
    public List<IEnumerator> runningBulletCoroutines = new List<IEnumerator>();//A list of all bulletSpawn IEnumarators this is currently running, so it can stop them
    [SerializeField] private TMP_Text GameTextBox;
    private RectTransform TextBoxTransform;
    
    void Start()
    {
        Instance = this;
        curBorder = 0;
        myLine = GetComponent<LineRenderer>();
        myEdge = GetComponent<EdgeCollider2D>();
        TextBoxTransform = GameTextBox.GetComponent<RectTransform>();
        Level.Borders = new List<Border>();
        Level.SetBordersFromTool();
        Level.SetBorderTransitions(Level.transitionTimes);
        //Sets the start border/player position and bullet hells, etc
        Border gameStartB = Level.Borders[0];
        List<Vector2> startCorners = gameStartB.BorderCorners;
        List<Vector2> currentCorners2 = new List<Vector2>();
        List<Vector3> currentCorners3 = new List<Vector3>();
        for(int i = 0; i < startCorners.Count; i++) {
            currentCorners2.Add(startCorners[i]);
            currentCorners3.Add(currentCorners2[i]);
        }
        inactiveBullets = new Dictionary<string, List<BulletMovement>>();
        activeBullets = new List<BulletMovement>();
        myLine.SetPositions(currentCorners3.ToArray());
        myEdge.SetPoints(currentCorners2);
        player.transform.position = gameStartB.playerStart;
        checkPointBox.gameObject.SetActive(true);
        checkPointBox.transform.localScale = gameStartB.checkpointSize;
        checkPointBox.transform.position = gameStartB.checkpointPos + gameStartB.checkpointSize / 2f;
        checkPointBox.transform.position += Vector3.forward;
        //Sets the starting TextBox
        TextBox gameStartText = gameStartB.RoomText;
        TextBoxTransform.anchoredPosition = gameStartText.pos;
        TextBoxTransform.sizeDelta = gameStartText.size;
        GameTextBox.text = gameStartText.text;
        gameStartB.StartBulletHell();
    }

    void Update()
    {
        //Can press R to go back to last checkpoint
            //Can press Shift + R to reload level
        if(Input.GetKeyDown(KeyCode.R)) {
            if(Input.GetKey(KeyCode.LeftShift)) {
                SceneManager.LoadScene(0);
            }
            else {
                player.transform.position = Level.Borders[curBorder].playerStart;
            }
        }
    }

    private IEnumerator MoveBorder(BorderTransition myTransition) {
        //Player becomes frozen for the length of the transition
        player.frozen = true;
        //Deactivates the checkpoint box until the end of the transition
        checkPointBox.gameObject.SetActive(false);
        Vector3 playerPos = player.transform.position;
        float curTime = 0f;
        Border startB = myTransition.startBorder;
        Border endB  = myTransition.endBorder;
        List<Vector2> startCorners = startB.BorderCorners;
        List<Vector2> endCorners = endB.BorderCorners;
        //Ends the current bullet hell
        startB.EndBulletHell();
        //Removes all bullets from the current bullet hell and deactivates them
        while(activeBullets.Count > 0) {
            if(activeBullets[0].gameObject.activeInHierarchy) {
                activeBullets[0].Deactivate();
            }
            activeBullets.RemoveAt(0);
        }
        //Creates the new text box
        TextBox endTextBox = endB.RoomText;
        TextBoxTransform.anchoredPosition = endTextBox.pos;
        TextBoxTransform.sizeDelta = endTextBox.size;
        //TODO: Have the text be gradually written out
        GameTextBox.text = endTextBox.text;
        //Moves the Border as needed
        while(curTime < myTransition.changeTime) {
            curTime += Time.deltaTime;
            if(curTime > myTransition.changeTime) {
                curTime = myTransition.changeTime;
            }
            List<Vector2> currentCorners2 = new List<Vector2>();
            List<Vector3> currentCorners3 = new List<Vector3>();
            //Lerps between their linewidths NOTE: currently doesn't lerp line-width
            float lineWidth = Mathf.Lerp(startB.width, endB.width, curTime / myTransition.changeTime);
            //myLine.widthMultiplier = lineWidth;
            for(int i = 0; i < startCorners.Count; i++) {
                currentCorners2.Add(Vector2.Lerp(startCorners[i], endCorners[i], curTime / myTransition.changeTime));
                currentCorners3.Add(currentCorners2[i]);
            }
            currentCorners2 = EdgeColliderOffset(currentCorners2, lineWidth, Shape.rect);
            myLine.SetPositions(currentCorners3.ToArray());
            myEdge.SetPoints(currentCorners2);
            //Player's position is lerped between start and finish
            player.transform.position = Vector3.Lerp(playerPos, endB.playerStart, curTime / myTransition.changeTime);
            yield return null;
        }
        checkPointBox.gameObject.SetActive(true);
        checkPointBox.transform.localScale = endB.checkpointSize;
        checkPointBox.transform.position = endB.checkpointPos + endB.checkpointSize / 2f;
        checkPointBox.transform.position += Vector3.forward;
        endB.StartBulletHell();
        player.frozen = false;
        curBorder++;
    }

    //Spawns bullets repeatedly until forcibly stopped
    public IEnumerator SpawnBullets(BulletSpawn spawnDetails) {
        yield return new WaitForSeconds(spawnDetails.startDelay);
        float curTime = 0;
        //repeats infinitely, until told to stop
        while(true != false) {
            if(curTime <= 0) {
                //Spawns a bullet, using an existing bullet if possible
                BulletMovement newBullet;
                //Checks to see if the inactiveBullets even contains that named bullet
                if(!inactiveBullets.ContainsKey(spawnDetails.bullet.name)) {
                    //If it doesn't, creates the corresponding list
                    inactiveBullets.Add(spawnDetails.bullet.name, new List<BulletMovement>());
                }
                if(inactiveBullets[spawnDetails.bullet.name].Count > 0) {
                    newBullet = inactiveBullets[spawnDetails.bullet.name][0];
                    inactiveBullets[spawnDetails.bullet.name].RemoveAt(0);
                    newBullet.gameObject.SetActive(true);
                }
                else {
                    newBullet = Instantiate(spawnDetails.bullet, Vector3.zero, Quaternion.identity);
                    newBullet.name = spawnDetails.bullet.name;
                }
                activeBullets.Add(newBullet);
                newBullet.transform.position = spawnDetails.spawnPos;
                newBullet.transform.rotation = Quaternion.Euler(0, 0, spawnDetails.spawnRotation);
                newBullet.speed = spawnDetails.bulletSpeed;
                newBullet.lifeTime = spawnDetails.bulletLifetime;
                newBullet.StartCoroutine("Fire");
                curTime = spawnDetails.delay;
            }
            else {
                curTime -= Time.deltaTime;
            }
            yield return null;
        }
    }

    //Allows the checkpoint box to call when to start a new transition
    public void StartNewBorderTransition() {
        //A transition only occurs if it is able to occur
        if(curBorder < Level.BorderTransitions.Count) {
            IEnumerator newMove = MoveBorder(Level.BorderTransitions[curBorder]);
            StartCoroutine(newMove);
        }
    }

    //Makes it so the edge colliders all occur on the inside of the box
    private List<Vector2> EdgeColliderOffset(List<Vector2> points, float lineWidth = 0.5f, Shape shape = Shape.rect) {
        //Only does something if the shape is a rectangle
        if(shape == Shape.rect) {
            List<Vector2> newPointsList = new List<Vector2>();
            newPointsList.Add(points[0] + new Vector2(lineWidth / 2f, lineWidth / 2f));
            newPointsList.Add(points[1] + new Vector2(lineWidth / 2f, -lineWidth / 2f));
            newPointsList.Add(points[2] + new Vector2(-lineWidth / 2f, -lineWidth / 2f));
            newPointsList.Add(points[3] + new Vector2(-lineWidth / 2f, lineWidth / 2f));
            newPointsList.Add(newPointsList[0]);
            return newPointsList;
        }
        else {
            return points;
        }
    }

    float waitTime = 0.5f;
    public IEnumerator writeText(string myString) {
        string newString = "";
        while(newString.Length < myString.Length) {
            newString += myString[newString.Length];
            yield return new WaitForSeconds(waitTime);
        }
        yield return null;
    }
}
