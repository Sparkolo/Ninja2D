using UnityEngine;
using System.Collections;

public class Spikes : MonoBehaviour
{
    [SerializeField] private int spikeDamage = 50;
    private bool collisionOn = false;
    public Animator animator;               // Reference to the player's animator in order to change animation states and perform some checks

    private void OnTriggerEnter2D(Collider2D collision)
    {
        collisionOn = false;
        if (collision.gameObject.tag == "Player")
        {
            animator.SetBool("hasCollision", true);
            Player player = collision.GetComponent<Player>();
            player.TakeDamage(spikeDamage, player.transform.rotation.y);
            collisionOn = false;

            StartCoroutine(WaitSpikeEnd(1f));
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if(collisionOn)
        {
            if (collision.gameObject.tag == "Player")
            {
                animator.SetBool("hasCollision", true);
                Player player = collision.GetComponent<Player>();
                player.TakeDamage(spikeDamage, player.transform.rotation.y);
                collisionOn = false;

                StartCoroutine(WaitSpikeEnd(1f));
            }
        }
        
    }

    IEnumerator WaitSpikeEnd(float timeToWait) // timeToWait = cooldown time of spikes
    {
        yield return new WaitForSeconds(timeToWait); // wait the cooldown time of the spikes before ending them
        animator.SetBool("hasCollision", false); // end the spikes animation
        collisionOn = true;
    }
}
