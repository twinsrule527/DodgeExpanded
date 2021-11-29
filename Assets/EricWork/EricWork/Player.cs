using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public static Player Instance;
    public float speed;

    public Rigidbody2D rigidbody;
    public float hitPoints;
    public bool frozen;//Whether the player is frozen from the map moving
    // Start is called before the first frame update
    void Start()
    {
        Instance = this;
        rigidbody = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if(!frozen) {
            float x = Input.GetAxis("Horizontal");
            float y = Input.GetAxis("Vertical");

       
            rigidbody.velocity = new Vector2(x * speed, y * speed);
        }

       


    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Bullet"))
        {
            //When player gets hit by bullet, increases the hit tracker
            PlayerHitTracker.Instance.PlayerHit(collision.GetComponent<BulletMovement>());
        }
    }
}
