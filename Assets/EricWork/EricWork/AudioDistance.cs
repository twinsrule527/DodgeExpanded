using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioDistance : MonoBehaviour
{

    public AudioSource Source;
    public Player player;

    public float minDist = 1;
    public float maxDist = 100;

    public float volumeMax = 0.5f;
    // Start is called before the first frame update
    void Awake()
    {
        Source = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        player = Player.Instance;

       

        float dist = Vector3.Distance(player.transform.position, transform.position);

        if(dist < minDist)
        {
            Source.volume = 0;
        } else if(dist > maxDist)
        {
            Source.volume = volumeMax;
        } else
        {
            Source.volume = volumeMax - ((dist - minDist) / (maxDist - minDist));
        }
    }
}
