using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//Attached to a checkpoint object - if the player is inside the checkpoint for a long enough period of time, transition occurs
public class CheckpointTransition : MonoBehaviour
{
    [SerializeField] private Transform player;
    public float timeInCheckpoint = 0;//How long the player has been in the checkpoint box
    [SerializeField] private float maxTimeInCheckpoint;//How long a player has to be in a checkpoint to be able to go to the next transition

    //Two colors to signify transitioning when the player is in its box
    [SerializeField] private Color emptyColor;
    [SerializeField] private Color fullColor;

    private SpriteRenderer mySprite;
    void Start()
    {
        mySprite = GetComponent<SpriteRenderer>();
        mySprite.color = emptyColor;
    }

    void Update()
    {
        //Is constantly updating to see if there's a player in the same location as this
        Collider2D[] CheckpointArea = Physics2D.OverlapBoxAll(transform.position, transform.localScale, 0);
        bool playerInBox = false;
        //Checks to see if the player is one of the objects in the area
        foreach(Collider2D col in CheckpointArea) {
            if(col.CompareTag("Player")) {
                playerInBox = true;
            }
        }
        //If the player is in the box, advances the checkpoint counter
        if(playerInBox) {
            timeInCheckpoint += Time.deltaTime;
            mySprite.color = Color.Lerp(emptyColor, fullColor, timeInCheckpoint / maxTimeInCheckpoint);
            if(timeInCheckpoint >= maxTimeInCheckpoint) {
                mySprite.color = fullColor;
                //Goes to the next transition
                BorderMovement.Instance.StartNewBorderTransition();
                timeInCheckpoint = 0;
                mySprite.color = emptyColor;
            }
        }
        else {
            if(timeInCheckpoint > 0) {
                timeInCheckpoint -= Time.deltaTime;
                timeInCheckpoint = Mathf.Max(timeInCheckpoint, 0);
                mySprite.color = Color.Lerp(emptyColor, fullColor, timeInCheckpoint / maxTimeInCheckpoint);
            }
        }
    }
}
