using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private int maxHealth = 100;
    private int _curHealth;
    public int curHealth
    {
        get { return _curHealth; }
        set { _curHealth = Mathf.Clamp(value, 0, maxHealth); }
    }
    private Rigidbody2D m_Rigidbody2D; // Reference to the player's RigidBody in order to make it move
    private CameraShake camShake;
    public Animator animator;
    [SerializeField] private GameObject smokePrefab; // prefab referring to the smoke explosion animation
    [SerializeField] private float invincibilityTime = 2f;
    private float _curInvincibilityCountdown = 0;
    [SerializeField] private float fallBoundary = -20f;


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
        camShake = GameMaster.gm.GetComponent<CameraShake>();
        if (camShake == null)
        {
            Debug.LogError("No CameraShake object found on the GM object!");
        }
    }

    private void Update()
    {
        if(transform.position.y <= fallBoundary)
        {
            this.TakeDamage(9999, 0);
        }

        if(_curInvincibilityCountdown > 0)
        {
            _curInvincibilityCountdown -= Time.deltaTime;
        }
    }

    public void TakeDamage(int damage, float enemyRotation)
    {
        if (_curInvincibilityCountdown <= 0)
        {
            curHealth -= damage;
            _curInvincibilityCountdown = invincibilityTime;
            camShake.Shake(1f);

            if (_curHealth <= 0)
            {
                statusInd.SetHealth(curHealth, maxHealth);
                Die();
            }
            else
            {
                this.GetComponent<PlayerController2D>().canMove = false;
                if (enemyRotation == 0)
                {
                    m_Rigidbody2D.AddForce(new Vector2(-1f, 0), ForceMode2D.Impulse);
                }
                else
                {
                    m_Rigidbody2D.AddForce(new Vector2(1f, 0), ForceMode2D.Impulse);
                }

                statusInd.SetHealth(curHealth, maxHealth);

                animator.SetBool("hasBeenDamaged", true);
                StartCoroutine(DamageEndAnimation());
            }
        }
    }

    IEnumerator DamageEndAnimation()
    {
        yield return new WaitForSeconds(0.5f); // wait 0.1 second before checking if the player has reached ground. Time is small enough that the player cannot ground before it's elapsed.
        this.GetComponent<PlayerController2D>().canMove = true;
        animator.SetBool("hasBeenDamaged", false); // after 0.1 second the player can check again if it's grounded, so stop the check blocking
    }

    void Die()
    {
        animator.SetBool("hasDied", true);
        StartCoroutine(DeathExplosion());
    }

    IEnumerator DeathExplosion()
    {
        yield return new WaitForSeconds(1.5f);
        GameObject deathSmoke = Instantiate(smokePrefab, this.transform.position, this.transform.rotation);
        deathSmoke.transform.localScale = new Vector2(1.5f, 1.5f);
        GameMaster.KillPlayer(this);
    }
}
