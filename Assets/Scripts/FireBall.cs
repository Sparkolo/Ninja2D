using System.Collections;
using UnityEngine;

public class FireBall : MonoBehaviour
{
    [SerializeField] private float speed = 11f; // Speed of the fire ball when created
    [SerializeField] private int damage = 20; // Damage delt by a fire ball hit
    [SerializeField] private GameObject smokePrefab; // prefab referring to the smoke explosion animation
    private float timeLeftToLive = 1f; // Countdown to destroy the fireball if it doesn't hit anything

    private Rigidbody2D _rb;

    // Start is called before the first frame update
    void Start()
    {
        _rb = GetComponent<Rigidbody2D> ();
        _rb.velocity = transform.right * speed; // set the velocity of the fireball
        StartCoroutine(DecreaseFireBall(timeLeftToLive - 0.15f)); // call a coroutine that decreses the size of the fireball when almost out of range
    }

    // When the fireball is almost out of time to live gradually reduce it to 0 size
    IEnumerator DecreaseFireBall (float timeToWait)
    {
        yield return new WaitForSeconds(timeToWait);
        transform.localScale = Vector3.Lerp(transform.localScale, transform.localScale * 0, 35f * Time.deltaTime); // Reduce the size of the fire ball
    }

    // Update is called every frame
    private void Update()
    {
        if (timeLeftToLive <= 0f) // when the time to live of the fire ball expires destroy it
        {
            //DestroyFireBall();
        }
        else
        {
            timeLeftToLive -= Time.deltaTime; // if it's not expired yet, decrease the time passed since the last check
        }
    }

    // Check if there's been any collision with the fire ball
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer != LayerMask.NameToLayer("Background")) // ignore collisions with the background elements
        {
            Debug.Log("fireball");
            Enemy enemy = collision.GetComponent<Enemy>(); // check if the collision has hit an enemy

            Debug.Log(enemy);
            if (enemy != null) // if it did hit an enemy, damage it
            {
                enemy.TakeDamage(damage, gameObject.transform.rotation.y);
            }

            DestroyFireBall(); // after a collision, destroy the fire ball
        }
    }

    // function to destroy the fireball and play an animation for it's destruction
    void DestroyFireBall ()
    {
        Instantiate(smokePrefab, this.transform.position, this.transform.rotation);
        Destroy(this.gameObject);
    }
}
