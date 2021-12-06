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
    public LineRenderer MyLine {
        get {
            return myLine;
        }
    }
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
    [SerializeField] private TextEffect myTextEffect;
    [SerializeField] private bool writeTextAsMoveStarts;
    
    //Needs to immediately become the singleton - and set some objects
    void Awake() {
        Instance = this;
        myLine = GetComponent<LineRenderer>();
        myEdge = GetComponent<EdgeCollider2D>();
        TextBoxTransform = GameTextBox.GetComponent<RectTransform>();
    }

    void Start()
    {
        curBorder = 0;
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
        IEnumerator myEffect = myTextEffect.BuildText(GameTextBox, gameStartText.text);
        myTextEffect.StartCoroutine(myEffect);
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
                ResetRoom();
            }
        }
        if(Input.GetKeyDown(KeyCode.Space)) {
            StartCoroutine(BorderShake(2, 0.5f));
        }
    }

    private IEnumerator MoveBorder(BorderTransition myTransition) {
        //Player becomes frozen for the length of the transition
        player.frozen = true;
        //They also regain their health
        player.hitPoints = 0;
        //Deactivates the checkpoint box until the end of the transition
        checkPointBox.gameObject.SetActive(false);
        Vector3 playerPos = player.transform.position;
        float curTime = 0f;
        Border startB = myTransition.startBorder;
        Border endB  = myTransition.endBorder;
        //Trying to just take current position

        List<Vector2> startCorners = new List<Vector2>();// = startB.BorderCorners;
        Vector3[] startPos = new Vector3[5];
        myLine.GetPositions(startPos);
        for(int i = 0; i < startPos.Length; i++) {
            startCorners.Add(startPos[i]);
        }
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
        GameTextBox.text = "";
        //TODO: Have the text be gradually written out
        if(writeTextAsMoveStarts) {
            IEnumerator writeTextCoroutine = myTextEffect.BuildText(GameTextBox, endB.RoomText.text);
            StartCoroutine(writeTextCoroutine);
        }
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
            //currentCorners2 = EdgeColliderOffset(currentCorners2, lineWidth, Shape.rect);
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
        checkPointBox.GetComponent<SpriteRenderer>().sprite = endB.checkpointSprite;
        endB.StartBulletHell();
        //Writes text out at the end if that's what it's supposed to do
        if(!writeTextAsMoveStarts) {
            IEnumerator writeTextCoroutine = myTextEffect.BuildText(GameTextBox, endB.RoomText.text);
            StartCoroutine(writeTextCoroutine);
        }
        player.frozen = false;
        curBorder++;
    }

    //Spawns bullets repeatedly until forcibly stopped
    public IEnumerator SpawnBullets(BulletSpawn spawnDetails) {
        yield return new WaitForSeconds(spawnDetails.startDelay);
        float curTime = 0;
        //repeats infinitely, until told to stop
        while(true != false) {
            //if(curTime <= 0) {
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
            //}
            /*
            else {
                curTime -= Time.deltaTime;
            }*/
            yield return new WaitForSeconds(spawnDetails.delay);
        }
    }

    //Spawns an array (or circle) of bullets, missing a certain number in a group
        //Automatically assumes all bullets fire simultaneously
    public IEnumerator SpawnBulletsMissingArray(List<BulletSpawn> spawnDetails, int numMissing, int offset, bool loops) {
        yield return new WaitForSeconds(spawnDetails[0].startDelay);
        int firstMissing = 0;
        if(loops) {
            firstMissing = Random.Range(0, spawnDetails.Count - 1);
        }
        else {
            firstMissing = Random.Range(0, spawnDetails.Count - 1 - numMissing);
            Debug.Log(firstMissing);
            Debug.Log(numMissing);
            Debug.Log(spawnDetails.Count);
        }
        while(true != false) {
            List<BulletSpawn> newSpawnDetails = new List<BulletSpawn>(spawnDetails);
            if(!loops) {
                newSpawnDetails.RemoveRange(firstMissing, numMissing);
            }
            else {
                if(firstMissing >= spawnDetails.Count - 1 - numMissing) {
                    newSpawnDetails.RemoveRange(firstMissing, firstMissing - spawnDetails.Count - 1 - numMissing);
                    newSpawnDetails.RemoveRange(0, spawnDetails.Count - firstMissing);
                }
                else {
                    newSpawnDetails.RemoveRange(firstMissing, numMissing);
                }
            }
            foreach(BulletSpawn bullet in newSpawnDetails) {
                BulletMovement newBullet;
                //Checks to see if the inactiveBullets even contains that named bullet
                if(!inactiveBullets.ContainsKey(bullet.bullet.name)) {
                    //If it doesn't, creates the corresponding list
                    inactiveBullets.Add(bullet.bullet.name, new List<BulletMovement>());
                }
                if(inactiveBullets[bullet.bullet.name].Count > 0) {
                    newBullet = inactiveBullets[bullet.bullet.name][0];
                    inactiveBullets[bullet.bullet.name].RemoveAt(0);
                    newBullet.gameObject.SetActive(true);
                }
                else {
                    newBullet = Instantiate(bullet.bullet, Vector3.zero, Quaternion.identity);
                    newBullet.name = bullet.bullet.name;
                }
                activeBullets.Add(newBullet);
                newBullet.transform.position = bullet.spawnPos;
                newBullet.transform.rotation = Quaternion.Euler(0, 0, bullet.spawnRotation);
                newBullet.speed = bullet.bulletSpeed;
                newBullet.lifeTime = bullet.bulletLifetime;
                newBullet.StartCoroutine("Fire");
            }
            if(loops) {
                firstMissing = Random.Range(0, spawnDetails.Count - 1);
            }
            else {
                firstMissing = Random.Range(0, spawnDetails.Count - 1 - numMissing);
            }
            yield return new WaitForSeconds(spawnDetails[0].delay);
        }
    }

    //If the room is meant to change while the player is able to move, it is done through this coroutine
    public IEnumerator ChangingShapeRoom(List<List<Vector3>> roomShapes, List<float> timesToChange, bool loops = true) {
        if(loops) {//Whether or not it should loop back to the same shape
            Vector3[] startPos = new Vector3[5];
            myLine.GetPositions(startPos);
            List<Vector3> startPoses = new List<Vector3>(startPos);
            roomShapes.Add(startPoses);
        }
        int curRoom = -1;
        while(curRoom < roomShapes.Count) {
            if(!loops && curRoom == roomShapes.Count - 1) {
                break;
            }
            float curTime = 0;
            //Gets the current room position
            Vector3[] startPos = new Vector3[5];
            myLine.GetPositions(startPos);
            List<Vector3> startCorners = new List<Vector3>(startPos);
            List<Vector3> endCorners = new List<Vector3>(roomShapes[(curRoom + 1) % roomShapes.Count]);
            while(curTime < timesToChange[(curRoom + 1) % timesToChange.Count]) {
                curTime += Time.deltaTime;
                curTime = Mathf.Min(curTime, timesToChange[(curRoom + 1) % timesToChange.Count]);
                List<Vector2> currentCorners2 = new List<Vector2>();
                List<Vector3> currentCorners3 = new List<Vector3>();
                //myLine.widthMultiplier = lineWidth;
                for(int i = 0; i < startCorners.Count; i++) {
                    currentCorners2.Add(Vector2.Lerp(startCorners[i], endCorners[i], curTime / timesToChange[(curRoom + 1) % timesToChange.Count]));
                    currentCorners3.Add(currentCorners2[i]);
                }
                currentCorners2 = EdgeColliderOffset(currentCorners2, 1, Shape.rect);
                myLine.SetPositions(currentCorners3.ToArray());
                myEdge.SetPoints(currentCorners2);
                yield return null;
            }
            yield return null;
            curRoom++;
            //If it loops, it will go back to the beginning
            if(loops) {
                curRoom = curRoom % roomShapes.Count;
            }
        }
        yield return null;
    }


    //Allows the checkpoint box to call when to start a new transition
    public void StartNewBorderTransition() {
        //A transition only occurs if it is able to occur
        if(curBorder < Level.BorderTransitions.Count) {
            IEnumerator newMove = MoveBorder(Level.BorderTransitions[curBorder]);
            StartCoroutine(newMove);
        }
    }

    //Resets the current room
    public void ResetRoom() {
        player.hitPoints = 0;
        player.transform.position = Level.Borders[curBorder].playerStart;
        //Ends the current bullet hell
        Level.Borders[curBorder].EndBulletHell();
        //Removes all bullets from the current bullet hell and deactivates them
        while(activeBullets.Count > 0) {
            if(activeBullets[0].gameObject.activeInHierarchy) {
                activeBullets[0].Deactivate();
            }
            activeBullets.RemoveAt(0);
        }
        List<Vector2> curCorners = Level.Borders[curBorder].BorderCorners;
        List<Vector3> curCorners3 = new List<Vector3>();
        for(int i = 0; i < curCorners.Count; i++) {
            curCorners3.Add(curCorners[i]);
        }
        myLine.SetPositions(curCorners3.ToArray());
        Level.Borders[curBorder].StartBulletHell();
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

    //Repeatedly shakes the screen until stopped
    public IEnumerator repeatBorderShake(float intensity) {
        while(true != false) {
            StartCoroutine(BorderShake(0.1f, intensity));
            yield return new WaitForSeconds(0.1f);
        }
    }
    public IEnumerator BorderShake(float duration, float intensity = 0.5f) {
        List<Vector2> borderBaseCorners = new List<Vector2>(Level.Borders[curBorder].BorderCorners);
        float curTime = 0;
        while(curTime < duration) {
            curTime+= Time.deltaTime;
            List<Vector3> currentCorners = new List<Vector3>();
            Vector2 shakeAmt = new Vector2(Random.Range(-1f, 1f) * intensity, Random.Range(-1f, 1f) * intensity);
            for(int i = 0; i < borderBaseCorners.Count; i++) {
                Vector3 newPos = borderBaseCorners[i] + shakeAmt;
                currentCorners.Add(newPos);
            }
            myLine.SetPositions(currentCorners.ToArray());
            yield return null;
        }
        List<Vector3> curCorners = new List<Vector3>();
        for(int i = 0; i < borderBaseCorners.Count; i++) {
                Vector3 newPos = borderBaseCorners[i];
                curCorners.Add(newPos);
            }
        myLine.SetPositions(curCorners.ToArray());
        yield return null;
    }
}
