using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundSpin : MonoBehaviour
{
    // Start is called before the first frame update
    public Vector2 startPosition;
    //public float floatingRate;
    public float rotationSpeed;
    private SpriteRenderer renderer;

    // Start is called before the first frame update
    void Start()
    {
        //floatingRate = Random.Range(0f, 4f);
        renderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        float lerpRate = GetComponentInParent<Player>().hitPoints / GetComponentInParent<Player>().hitMax;

        //transform.position = transform.position + new Vector3(Mathf.Sin(Time.time * floatingRate), 0,0);
        transform.Rotate(0f, 0, rotationSpeed, Space.Self);


        renderer.color = Color.Lerp(Color.white, Color.black, lerpRate);

    }

}
