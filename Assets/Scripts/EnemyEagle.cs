using System.Collections;
using UnityEngine;

public class EnemyEagle : Enemy
{
    [SerializeField] private int health = 50;
    [SerializeField] private GameObject deathEffect;
    private Rigidbody2D m_Rigidbody2D;
    [SerializeField] private float defaultDazeTime = 0.5f;
    private float curDazeTime = 0f;

    [SerializeField] private float speed = 3f;
    [SerializeField] private float stopDistance = 6f;
    public Animator animator;                                                   // Reference to the player's animator in order to change animation states and perform some checks
    [SerializeField] private GameObject smokePrefab; // prefab referring to the smoke explosion animation

    private Transform player;
    private CameraShake camShake;
    private float criticalDistance = 2.5f;
    private float criticalSpeed = 2f;
    private float retreatDistance = 0.5f;
    private float _curDist;
    private bool m_FacingRight = false;  // For determining which way the player is currently facing.
    private bool currentlyAttacking = true;

    private void Awake()
    {
        m_Rigidbody2D = GetComponent<Rigidbody2D>();
        GameObject initialSmoke = Instantiate(smokePrefab, this.transform.position, this.transform.rotation);
        initialSmoke.transform.localScale = new Vector2(1.5f, 1.5f);
    }

    // Start is called before the first frame update
    void Start()
    {
        camShake = GameMaster.gm.GetComponent<CameraShake>();
        if (camShake == null)
        {
            Debug.LogError("No CameraShake object found on the GM object!");
        }
        player = GameObject.FindGameObjectWithTag("Player").transform;
        if (player == null)
        {
            Debug.LogError("No Player object found by the eagle enemy");
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(curDazeTime <= 0)
        {
            if (m_FacingRight)
            {
                if (player.position.x < gameObject.transform.position.x)
                {
                    transform.eulerAngles = new Vector3(0, 0, 0);
                    m_FacingRight = false;
                }
            }
            else
            {
                if (player.position.x > gameObject.transform.position.x)
                {
                    transform.eulerAngles = new Vector3(0, -180, 0);
                    m_FacingRight = true;
                }
            }

            if (currentlyAttacking)
            {
                _curDist = Vector2.Distance(transform.position, player.position);
                if (_curDist < stopDistance)
                {
                    if (_curDist <= retreatDistance)
                    {
                        currentlyAttacking = false;
                        StartCoroutine(WaitForNextAttack());
                    }
                    else if (_curDist < criticalDistance)
                    {
                        transform.position = Vector2.MoveTowards(transform.position, player.position, criticalSpeed * Time.deltaTime);
                    }
                    else
                    {
                        transform.position = Vector2.MoveTowards(transform.position, player.position, speed * Time.deltaTime);
                    }
                }
            }
            else
            {
                    transform.position = Vector2.MoveTowards(transform.position, transform.position + new Vector3(1f, 1f, 0), criticalSpeed * Time.deltaTime);
            }
        }
        else
        {
            curDazeTime -= Time.deltaTime;
        }
               
    }

    IEnumerator WaitForNextAttack()
    {
        yield return new WaitForSeconds(2f); // wait 0.1 second before checking if the player has reached ground. Time is small enough that the player cannot ground before it's elapsed.
        currentlyAttacking = true; // after 0.1 second the player can check again if it's grounded, so stop the check blocking
    }

    public override void TakeDamage(int damage, float playerRotation)
    {
        health -= damage;
        camShake.Shake(0.5f);

        if (health <= 0)
        {
            Die();
        }
        else
        {
            curDazeTime = defaultDazeTime;

            if (gameObject.transform.rotation.y == 0)
            {
                //transform.Translate(new Vector2(-1f, 0.5f));
                m_Rigidbody2D.AddForce(new Vector2(1f, 1f), ForceMode2D.Impulse);
            }  
            else
            {
                //transform.Translate(new Vector2(1f, 0.5f));
                m_Rigidbody2D.AddForce(new Vector2(-1f, 1f), ForceMode2D.Impulse);
            }

            animator.SetBool("hasBeenDamaged", true);
            StartCoroutine(DamageEndAnimation());

        }
    }

    IEnumerator DamageEndAnimation()
    {
        yield return new WaitForSeconds(0.5f); // wait 0.1 second before checking if the player has reached ground. Time is small enough that the player cannot ground before it's elapsed.
        animator.SetBool("hasBeenDamaged", false); // after 0.1 second the player can check again if it's grounded, so stop the check blocking
    }

    void Die()
    {
        Instantiate(deathEffect, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        Physics2D.IgnoreLayerCollision(9, 10);
    }
}
