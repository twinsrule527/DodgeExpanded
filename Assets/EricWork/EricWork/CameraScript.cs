using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour
{

    public Camera cam;
    public GameObject goal;

    public Vector3 origin;
    // Start is called before the first frame update
    void Start()
    {
        cam = GetComponent<Camera>();
        origin = cam.transform.position;
        goal = GameObject.Find("CheckpointBox");

    }

    // Update is called once per frame
    void Update()
    {
        Vector3 target = Vector3.Cross(goal.transform.position, cam.transform.position).normalized ;

        cam.orthographicSize = 30 - Vector3.Distance(target, Player.Instance.transform.position);
        //cam.transform.position = Vector3.Lerp(cam.transform.position, target, Time.deltaTime);
       
    }
}
