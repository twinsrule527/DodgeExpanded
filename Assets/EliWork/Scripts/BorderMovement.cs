using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
//Moves the border over a period of time - this is a test
public class BorderMovement : MonoBehaviour
{
    public static BorderMovement Instance;//Allows every object to access this (basically a singleton)
    public float timeToChange;
    private LineRenderer myLine;
    private EdgeCollider2D myEdge;
    [SerializeField] private Transform checkPointBox;//The box for when a change in transition occurs
    [SerializeField] private Transform Player;//The player needs to be able to be moved around
    public BorderLevelScriptableObject Level;
    private int curBorder;

    public List<BulletMovement> inactiveBullets;//A list of all bullets which exist but aren't active, so they can be reviewed
    [SerializeField] private BulletMovement bulletPrefab;
    public List<IEnumerator> runningBulletCoroutines = new List<IEnumerator>();//A list of all bulletSpawn IEnumarators this is currently running, so it can stop them

    
    void Start()
    {
        Instance = this;
        curBorder = 0;
        myLine = GetComponent<LineRenderer>();
        myEdge = GetComponent<EdgeCollider2D>();
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
        myLine.SetPositions(currentCorners3.ToArray());
        myEdge.SetPoints(currentCorners2);
        Player.position = gameStartB.playerStart;
        checkPointBox.gameObject.SetActive(true);
        checkPointBox.transform.localScale = gameStartB.checkpointSize;
        checkPointBox.transform.position = gameStartB.checkpointPos + gameStartB.checkpointSize / 2f;
        checkPointBox.transform.position += Vector3.forward;
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
                Player.transform.position = Level.Borders[curBorder].playerStart;
            }
        }
    }

    private IEnumerator MoveBorder(BorderTransition myTransition) {
        //Deactivates the checkpoint box until the end of the transition
        checkPointBox.gameObject.SetActive(false);
        Vector3 playerPos = Player.position;
        float curTime = 0f;
        Border startB = myTransition.startBorder;
        Border endB  = myTransition.endBorder;
        List<Vector2> startCorners = startB.BorderCorners;
        List<Vector2> endCorners = endB.BorderCorners;
        //Ends the current bullet hell
        startB.EndBulletHell();
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
            Debug.Log(currentCorners2[0] + " " + currentCorners2[1]+ " " + currentCorners2[2]+ " " + currentCorners2[3]);
            currentCorners2 = EdgeColliderOffset(currentCorners2, lineWidth, Shape.rect);
            myLine.SetPositions(currentCorners3.ToArray());
            myEdge.SetPoints(currentCorners2);
            //Player's position is lerped between start and finish
            Player.position = Vector3.Lerp(playerPos, endB.playerStart, curTime / myTransition.changeTime);
            yield return null;
        }
        checkPointBox.gameObject.SetActive(true);
        checkPointBox.transform.localScale = endB.checkpointSize;
        checkPointBox.transform.position = endB.checkpointPos + endB.checkpointSize / 2f;
        checkPointBox.transform.position += Vector3.forward;
        endB.StartBulletHell();
        yield return new WaitForSeconds(1f);
        curBorder++;
    }

    //Spawns bullets repeatadly until forcibly stopped
    public IEnumerator SpawnBullets(BulletSpawn spawnDetails) {
        yield return new WaitForSeconds(spawnDetails.startDelay);
        float curTime = 0;
        //repeats infinitely, until told to stop
        while(true != false) {
            if(curTime <= 0) {
                //Spawns a bullet, using an existing bullet if possible
                BulletMovement newBullet;
                if(inactiveBullets.Count > 0) {
                    newBullet = inactiveBullets[0];
                    inactiveBullets.RemoveAt(0);
                    newBullet.gameObject.SetActive(true);
                }
                else {
                    newBullet = Instantiate(bulletPrefab, Vector3.zero, Quaternion.identity);
                }
                newBullet.transform.position = spawnDetails.spawnPos;
                newBullet.transform.rotation = Quaternion.Euler(0, 0, spawnDetails.spawnRotation);
                newBullet.speed = spawnDetails.bulletSpeed;
                newBullet.lifeTime = spawnDetails.bulletLifetime;
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
}
