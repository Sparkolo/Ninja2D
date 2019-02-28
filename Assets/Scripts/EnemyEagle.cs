using System.Collections;
using UnityEngine;

public class EnemyEagle : Enemy
{
    [SerializeField] private int maxHealth = 50;
    private int _curHealth;
    public int curHealth
    {
        get { return _curHealth; }
        set { _curHealth = Mathf.Clamp(value, 0, maxHealth); }
    }
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
    private float nextSearchTime = 0;

    [Header("Optional: ")]
    [SerializeField] private StatusIndicator statusInd;

    private void Awake()
    {
        m_Rigidbody2D = GetComponent<Rigidbody2D>();
        GameObject initialSmoke = Instantiate(smokePrefab, this.transform.position, this.transform.rotation);
        initialSmoke.transform.localScale = new Vector2(1.5f, 1.5f);
    }

    // Start is called before the first frame update
    void Start()
    {
        curHealth = maxHealth;
        if(statusInd != null)
        {
            statusInd.SetHealth(curHealth, maxHealth);
        }

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
        if (player == null)
        {
            if (nextSearchTime <= 0)
                FindPlayer();
            else
                nextSearchTime -= Time.deltaTime;
        }
        else
        {
            if (curDazeTime <= 0)
            {
                if (m_FacingRight)
                {
                    if (player.position.x < gameObject.transform.position.x)
                    {
                        transform.eulerAngles = new Vector3(0, 0, 0);
                        statusInd.transform.localScale = new Vector3(-statusInd.transform.localScale.x, statusInd.transform.localScale.y, statusInd.transform.localScale.z);
                        m_FacingRight = false;
                    }
                }
                else
                {
                    if (player.position.x > gameObject.transform.position.x)
                    {
                        transform.eulerAngles = new Vector3(0, -180, 0);
                        statusInd.transform.localScale = new Vector3(-statusInd.transform.localScale.x, statusInd.transform.localScale.y, statusInd.transform.localScale.z);
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
    }

    void FindPlayer()
    {
        GameObject searchResult = GameObject.FindGameObjectWithTag("Player");
        if (searchResult != null)
        {
            player = searchResult.transform;
        }
        nextSearchTime = 0.5f;
    }

    IEnumerator WaitForNextAttack()
    {
        yield return new WaitForSeconds(2f); // wait 0.1 second before checking if the player has reached ground. Time is small enough that the player cannot ground before it's elapsed.
        currentlyAttacking = true; // after 0.1 second the player can check again if it's grounded, so stop the check blocking
    }

    public override void TakeDamage(int damage, float playerRotation)
    {
        curHealth -= damage;
        camShake.Shake(0.5f);

        if (curHealth == 0)
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

            statusInd.SetHealth(curHealth, maxHealth);

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
        Physics2D.IgnoreLayerCollision(9, 9);
    }
}
