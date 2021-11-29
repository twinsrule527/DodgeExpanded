using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//Testing out quaternion direction lerping with facing
public class DirectionTest : MonoBehaviour
{
    public Transform point2;
    public float finalAngle;
    public float timeToLerp;
    public bool lerping;
    private float curTime;
    Vector3 direction1;
    Vector3 direction2;
    public Vector3 testPoint;
    void Start()
    {
        Vector2 pointDiff = testPoint - transform.position;
        pointDiff = pointDiff.normalized;
        if(pointDiff.x != 0) {
            float tanAngle = Mathf.Atan(pointDiff.y / pointDiff.x);
            if(pointDiff.x >  0) {
                finalAngle = tanAngle * 360 / 2 / Mathf.PI;
            }
            else { 
                finalAngle = tanAngle * 360 / 2 / Mathf.PI + 180;
            }
        }
        else {
            finalAngle = 90 * pointDiff.y;
        }
        Debug.Log(finalAngle);
        //Now, need to turn it to adjust with up being vertical
        finalAngle -=90;
        //Now, we need to adjust it so that the object makes the fastest turn possible
        float otherAngle = 0;
        if(finalAngle > 0) {
            otherAngle = -360 + finalAngle;
        }
        else {
            otherAngle = 360 + finalAngle;
        }
        if(Mathf.Abs(transform.localEulerAngles.z - finalAngle) > Mathf.Abs(transform.localEulerAngles.z - otherAngle)) {
            finalAngle = otherAngle;
        }
        //Debug.Log(finalAngle);
        direction1 = new Vector3(0, 0, transform.localEulerAngles.z);
        direction2 = new Vector3(0, 0, finalAngle);
    }

    void Update()
    {
        if(lerping) {
            curTime += Time.deltaTime;
            curTime = Mathf.Min(curTime, timeToLerp);
            transform.rotation = Quaternion.Lerp(Quaternion.Euler(direction1), Quaternion.Euler(direction2), curTime / timeToLerp);
        }
        if(Input.GetKeyDown(KeyCode.Space)) {
            transform.position += transform.up;
        }
    }
}
