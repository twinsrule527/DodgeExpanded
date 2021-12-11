using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//Special effects occur when the wall deactivates
public class FlickerBorder : MonoBehaviour
{
    private float curTime;
    [SerializeField] private float flickerTime;
    [SerializeField] private Color baseColor;
    [SerializeField] private Color flickerColor;
    [SerializeField] private float flickerChangeTime;
    private LineRenderer myRenderer;

    void Start() {
        myRenderer = GetComponent<LineRenderer>();
    }
    void Update()
    {
        if(!Player.Instance.WallKills) {
            curTime += Time.deltaTime;
            if(curTime >= flickerTime) {
                StartCoroutine("FlickerCoroutine");
                curTime = 0;
            }
        }
    }

    private IEnumerator FlickerCoroutine() {
        myRenderer.startColor = flickerColor;
        myRenderer.endColor = flickerColor;
        if(flickerChangeTime > 0) {
            yield return new WaitForSeconds(flickerChangeTime);
        }
        else {
            yield return null;
        }
        myRenderer.startColor = baseColor;
        myRenderer.endColor = baseColor;

    }
}
