using System.Collections;
using UnityEngine;

public class EnemyFrog : MonoBehaviour
{
    [SerializeField] private int health = 100;
    [SerializeField] private GameObject deathEffect;
    [SerializeField] private float defaultDazeTime = 0.5f;
    private Rigidbody2D m_Rigidbody2D;
    private float curDazeTime = 0f;
    private CameraShake camShake;


    private float m_JumpForce = 250f;							// Amount of force added when the player jumps.
    private float m_RunSpeed = 40f;                            // Amount of force added when the player runs.
    [SerializeField] private LayerMask m_WhatIsGround;                          // A mask determining what is ground to the character
    [SerializeField] private Transform m_GroundCheck;                           // A position marking where to check if the player is grounded.
    [SerializeField] private Transform m_CeilingCheck;                          // A position marking where to check for ceilings
    [SerializeField] private Transform m_JumpCheck;                             // A position marking where to check if the next jump is still in the same direction
    public Animator animator;                                                   // Reference to the player's animator in order to change animation states and perform some checks
    [SerializeField] private GameObject smokePrefab; // prefab referring to the smoke explosion animation


    const float k_GroundedRadius = .2f; // Radius of the overlap circle to determine if grounded
    private bool m_Grounded = true;            // Whether or not the player is grounded.
    const float k_CeilingRadius = .2f; // Radius of the overlap circle to determine if the player can stand up
    private bool m_FacingRight = false;  // For determining which way the player is currently facing.
    private Vector3 m_Velocity = Vector3.zero; // Vector needed to apply movement

    private bool m_JumpStarted = false; // boolean to check if the jump has recently started, in order not to collide with ground as you jump
    private float horizontalMove = 0f;  // float to check if the player is moving, and in which direction
    private bool isJumping = false; // bool to check if the player decided to jump

    [SerializeField] private float reloadJumpTime = 3f;
    private float _curReloadJump = 0f;


    private void Awake()
    {
        m_Rigidbody2D = GetComponent<Rigidbody2D>();
        GameObject initialSmoke = Instantiate(smokePrefab, this.transform.position, this.transform.rotation);
        initialSmoke.transform.localScale = new Vector2(1.5f, 1.5f);
    }

    private void Start()
    {
        camShake = GameMaster.gm.GetComponent<CameraShake>();
        if(camShake == null)
        {
            Debug.LogError("No CameraShake object found on the GM object!");
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (_curReloadJump <= 0)
        {
            if (curDazeTime <= 0)
            {
                int layerMask = 1 << 8;
                layerMask = ~layerMask;

                RaycastHit2D groundInfo = Physics2D.Raycast(m_JumpCheck.position, Vector2.down, 0.5f, ~layerMask);

                if (!groundInfo.collider || groundInfo.collider.name == "BackgroundTiles")
                {
                    Debug.Log(groundInfo.collider);
                    if (m_FacingRight == true)
                    {
                        transform.eulerAngles = new Vector3(0, -180, 0);
                        m_FacingRight = false;
                    }
                    else
                    {
                        transform.eulerAngles = new Vector3(0, 0, 0);
                        m_FacingRight = true;
                    }
                }

                if(m_Grounded)
                {
                    isJumping = true;
                    _curReloadJump = reloadJumpTime;
                }
            }
            else
            {
                curDazeTime -= Time.deltaTime;
            }
        }
        else
        {
            _curReloadJump -= Time.deltaTime;
        }
    }

    IEnumerator JumpAnimationFix()
    {
        yield return new WaitForSeconds(.1f); // wait 0.1 second before checking if the player has reached ground. Time is small enough that the player cannot ground before it's elapsed.
        m_JumpStarted = false; // after 0.1 second the player can check again if it's grounded, so stop the check blocking
    }

    private void FixedUpdate()
    {
        bool wasGrounded = m_Grounded; // store the previous state about if the player was on the ground or not
        m_Grounded = false; // assume the player isn't on the ground right now to check if it actually is

        // The player is grounded if a circlecast to the groundcheck position hits anything designated as ground
        // This can be done using layers instead but Sample Assets will not overwrite your project settings.
        Collider2D[] colliders = Physics2D.OverlapCircleAll(m_GroundCheck.position, k_GroundedRadius, m_WhatIsGround);

        for (int i = 0; i < colliders.Length && !m_Grounded; i++) // cycle through the collisions until you find something that is a ground object
        {
            if (colliders[i].gameObject != gameObject) // if the player is colliding with the ground, note it and stop any eventual jump animation
            {
                m_Grounded = true; // set that the player is currently on ground, for exiting the for loop and future checks
                if (!wasGrounded && !m_JumpStarted) // check if the player was not on the ground on last check, but also didn't just start a jump less than 0.1 s ago 
                {
                    animator.SetBool("isJumping", false); // if the player was jumping and now is on the ground, stop the jump animation
                }  
            }
        }

        if (isJumping)
        {
            Jump();
            _curReloadJump = reloadJumpTime;
        }
            

        isJumping = false; // after you make the player move, disable the jumping variable so that the player doesn't continuosly jump if you hold down the jump key
    }

    void Jump()
    {
        StartCoroutine(JumpAnimationFix());
        m_Grounded = false;
        if(transform.rotation.y != 0)
        {
            m_Rigidbody2D.AddForce(new Vector2(20f, 30f), ForceMode2D.Impulse);
        }
        else
        {
            m_Rigidbody2D.AddForce(new Vector2(-20f, 30f), ForceMode2D.Impulse);
        }
        
        animator.SetBool("isJumping", true);
    }

    public void TakeDamage(int damage, float playerRotation)
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

            if(playerRotation < 0)
                m_Rigidbody2D.AddForce(new Vector2(-200f, 0) * Time.deltaTime, ForceMode2D.Impulse);
            else
                m_Rigidbody2D.AddForce(new Vector2(200f, 0) * Time.deltaTime, ForceMode2D.Impulse);

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
        Physics2D.IgnoreLayerCollision(9,10);
    }
}
