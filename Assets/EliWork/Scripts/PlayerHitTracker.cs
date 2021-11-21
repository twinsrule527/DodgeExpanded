using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
//keeps track of how many times the player has been hit, and shows it
public class PlayerHitTracker : MonoBehaviour
{
    public static PlayerHitTracker Instance;
    [SerializeField] private TMP_Text HitText;
    private int numTimesHit;
    void Start()
    {
        Instance = this;  
        numTimesHit = 0; 
    }

    //Calls this whenever the player is hit
    public void PlayerHit() {
        numTimesHit++;
        HitText.text = "Times Hit: " + numTimesHit.ToString();
    }
}
