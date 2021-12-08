using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public static Player Instance;
    public float speed;

    public AudioSource hitSound;
    public List<Sprite> arraySprites;
    

    [HideInInspector] public Rigidbody2D rigidbody;
    private BoxCollider2D myCollider;
    public float hitPoints;
    public float hitMax;
    public SpriteRenderer renderer;
    public bool useFixedUpdate;
    [SerializeField] private bool wallKills;//A bool for whether the wall resets the player's position
    [HideInInspector] public bool frozen;//Whether the player is frozen from the map moving
    // Start is called before the first frame update

    private void Awake()
    {
        renderer = GetComponent<SpriteRenderer>();

    }
    void Start()
    {
        Instance = this;
        myCollider = GetComponent<BoxCollider2D>();
        rigidbody = GetComponent<Rigidbody2D>();
        renderer = GetComponent<SpriteRenderer>();
        if(wallKills) {
            BorderMovement.Instance.MyLine.GetComponent<EdgeCollider2D>().isTrigger = true;
        }

        hitSound.GetComponent<AudioSource>();

        //arraySprites.Add
    }

    private Vector3 input;
    // Update is called once per frame
    void Update()
    {
        float lerpRate = hitPoints / hitMax;
        renderer.color = Color.Lerp(Color.white, Color.black, lerpRate);


        if (!frozen) {

            float x = Input.GetAxis("Horizontal");
            float y = Input.GetAxis("Vertical");

            input = new Vector3(x, y, 0).normalized;

            /*if (wallKills == false)
            {*/
                

                if (!useFixedUpdate)
                {
                    if (!Input.GetKey(KeyCode.None))
                    {
                        rigidbody.velocity = new Vector2(x * speed, y * speed);
                        //rigidbody.MovePosition(transform.position + input * Time.deltaTime * speed);
                    }
                    else
                    {
                        rigidbody.velocity = Vector2.zero;
                        rigidbody.position = rigidbody.position;
                    }
                }
            /*} else {
                RaycastHit2D[] boxHits = Physics2D.BoxCastAll(transform.position, myCollider.size, 0, input, speed * Time.deltaTime);
                bool hitsWall = false;
                foreach(RaycastHit2D hit in boxHits) {
                    if(hit.collider.CompareTag("Border")) {
                        hitsWall = true;
                    }
                }
                transform.position += input * speed * Time.deltaTime;
                Debug.Log(speed * Time.deltaTime);
                if(hitsWall) {
                    BorderMovement.Instance.ResetRoom();
                    hitPoints = 0;
                }

            }*/
        }
        else {
            rigidbody.velocity = Vector2.zero;
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
            if(hitSound != null) {
                hitSound.Play();
            }
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

    public IEnumerator doAnimation()
    {
        List<Sprite> spritearray = arraySprites;
        for (int i = 0; i < spritearray.Count; i++)
        {
            yield return new WaitForSeconds(0.03F);
            renderer.sprite = spritearray[i];
        }

    }
}
