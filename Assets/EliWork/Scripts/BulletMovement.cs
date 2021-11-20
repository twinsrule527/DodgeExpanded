using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//Attached to a bullet object, which will be spawned, shoot forward, and die
public class BulletMovement : MonoBehaviour
{
    public float lifeTime;
    public float speed;
    void Start()
    {
        
    }

    void Update()
    {
        transform.position += transform.up * speed * Time.deltaTime;
        lifeTime -= Time.deltaTime;
        if(lifeTime <= 0) {
            gameObject.SetActive(false);
            BorderMovement.Instance.inactiveBullets.Add(this);
        }
    }
}
