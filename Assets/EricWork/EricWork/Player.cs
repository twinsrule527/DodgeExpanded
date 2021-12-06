using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public static Player Instance;
    public float speed;

    public AudioSource hitSound;

    [HideInInspector] public Rigidbody2D rigidbody;
    [HideInInspector] public float hitPoints;
    public float hitMax;
    SpriteRenderer renderer;
    public bool useFixedUpdate;
    [SerializeField] private bool wallKills;//A bool for whether the wall resets the player's position
    [HideInInspector] public bool frozen;//Whether the player is frozen from the map moving
    // Start is called before the first frame update
    void Start()
    {
        Instance = this;
        rigidbody = GetComponent<Rigidbody2D>();
        //renderer = GetComponent<SpriteRenderer>();
        renderer = GetComponentInChildren<SpriteRenderer>();
        if(wallKills) {
            BorderMovement.Instance.MyLine.GetComponent<EdgeCollider2D>().isTrigger = true;
        }

        hitSound.GetComponent<AudioSource>();
    }

    private Vector3 input;
    // Update is called once per frame
    void Update()
    {
        float lerpRate = hitPoints / hitMax;

        if (!frozen) {
            float x = Input.GetAxis("Horizontal");
            float y = Input.GetAxis("Vertical");

            input = new Vector3(x, y, 0);

            if(!useFixedUpdate) {
            if (!Input.GetKey(KeyCode.None))
            {
                rigidbody.velocity = new Vector2(x * speed, y * speed);
                //rigidbody.MovePosition(transform.position + input * Time.deltaTime * speed);
            } else
            {
                rigidbody.velocity = Vector2.zero;
                rigidbody.position = rigidbody.position;
            }
            }
        }


        //renderer.color = Color.Lerp(Color.white, Color.black, lerpRate);



        //DEATH CHECK
        //hitpoints tracks the number of times hit
        if(hitPoints >= hitMax)
        {
            //Add reset position code here
            transform.position = BorderMovement.Instance.Level.Borders[BorderMovement.Instance.CurBorder].playerStart;
            hitPoints = 0;
        }
    }

    void FixedUpdate() {
        if(useFixedUpdate) {
            rigidbody.velocity = input * speed;
        }
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Bullet"))
        {
            if(hitSound != null) {
                hitSound.Play();
            }
            hitPoints++;
            //When player gets hit by bullet, increases the hit tracker
            PlayerHitTracker.Instance.PlayerHit(collision.GetComponent<BulletMovement>());
            collision.GetComponent<BulletMovement>().DealDamage(this);
        }
        //If the wall is supposed to kill the player, it resets their position
        if(wallKills && collision.CompareTag("Border")) {
            BorderMovement.Instance.ResetRoom();
            hitPoints = 0;
        }
    }

    private const float NORMAL_KNOCKBACK_TIME = 0.25f;
    private const float NORMAL_KNOCKBACK_AMT = 10f;
    //This coroutine is called whenever the player is dealt knockback
    public IEnumerator DealtKnockback(Vector2 direction, float knockTime = NORMAL_KNOCKBACK_TIME, float knockbackAmt = NORMAL_KNOCKBACK_AMT) {
        frozen = true;
        rigidbody.velocity = direction.normalized * knockbackAmt;
        yield return new WaitForSeconds(knockTime);
        frozen = false;
    }
}
