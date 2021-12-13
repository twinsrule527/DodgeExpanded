using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class FinalCheckpoint : MonoBehaviour
{
    [SerializeField] private Image FadeToBlack;
    [SerializeField] private float timeToLerp;
    [SerializeField] private AudioSource backgroundSound;
    private float baseSoundLevel;
    private float curTime;
    void Start()
    {
        baseSoundLevel = backgroundSound.volume;
    }

    void Update()
    {
        curTime += Time.deltaTime;
        curTime = Mathf.Clamp(curTime, 0, timeToLerp);
        FadeToBlack.color = Color.Lerp(new Color(0, 0, 0, 0), Color.black, curTime / timeToLerp);
        backgroundSound.volume = Mathf.Lerp(baseSoundLevel, 0, curTime / timeToLerp);
        if(timeToLerp == curTime) {
            gameObject.SetActive(false);
        }
    }
}
