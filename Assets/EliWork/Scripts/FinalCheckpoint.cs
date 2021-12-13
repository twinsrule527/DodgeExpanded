using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class FinalCheckpoint : MonoBehaviour
{
    [SerializeField] private Image FadeToBlack;
    [SerializeField] private float timeToLerp;
    [SerializeField] private AudioSource backgroundSound;
    private float baseSoundLevel;
    private float curTime;
    bool disabled;
    void Start()
    {
        baseSoundLevel = backgroundSound.volume;
    }

    void Update()
    {
        if(!disabled) {
            curTime += Time.deltaTime;
            curTime = Mathf.Clamp(curTime, 0, timeToLerp);
            FadeToBlack.color = Color.Lerp(new Color(0, 0, 0, 0), Color.black, curTime / timeToLerp);
            backgroundSound.volume = Mathf.Lerp(baseSoundLevel, 0, curTime / timeToLerp);
            if(timeToLerp == curTime) {
                disabled = true;
                StartCoroutine("Restart");
            }
        }
    }

    private IEnumerator Restart() {
        yield return new WaitForSeconds(2f);
        SceneManager.LoadScene(0);
    }
}
