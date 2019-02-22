using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private int maxHealth = 100;
    private int _curHealth;
    private Rigidbody2D m_Rigidbody2D; // Reference to the player's RigidBody in order to make it move
    private CameraShake camShake;
    public Animator animator;
    [SerializeField] private GameObject smokePrefab; // prefab referring to the smoke explosion animation
    [SerializeField] private float invincibilityTime = 2f;
    private float _curInvincibilityCountdown = 0;


    private void Awake()
    {
        m_Rigidbody2D = GetComponent<Rigidbody2D>();
        GameObject initialSmoke = Instantiate(smokePrefab, this.transform.position, this.transform.rotation);
        initialSmoke.transform.localScale = new Vector2(1.5f, 1.5f);
    }

    // Start is called before the first frame update
    void Start()
    {
        _curHealth = maxHealth;
        camShake = GameMaster.gm.GetComponent<CameraShake>();
        if (camShake == null)
        {
            Debug.LogError("No CameraShake object found on the GM object!");
        }
    }

    private void Update()
    {
        if(_curInvincibilityCountdown > 0)
        {
            _curInvincibilityCountdown -= Time.deltaTime;
        }
    }

    public void TakeDamage(int damage, float playerRotation)
    {
        if (_curInvincibilityCountdown <= 0)
        {
            _curHealth -= damage;
            _curInvincibilityCountdown = invincibilityTime;
            camShake.Shake(1f);

            if (_curHealth <= 0)
            {
                Die();
            }
            else
            {
                if (playerRotation < 0)
                    m_Rigidbody2D.AddForce(new Vector2(-200f, 0) * Time.deltaTime, ForceMode2D.Impulse);
                else
                    m_Rigidbody2D.AddForce(new Vector2(200f, 0) * Time.deltaTime, ForceMode2D.Impulse);

                animator.SetBool("hasBeenDamaged", true);
                StartCoroutine(DamageEndAnimation());
            }
        }
    }

    IEnumerator DamageEndAnimation()
    {
        yield return new WaitForSeconds(0.5f); // wait 0.1 second before checking if the player has reached ground. Time is small enough that the player cannot ground before it's elapsed.
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
        Destroy(gameObject);
    }
}
