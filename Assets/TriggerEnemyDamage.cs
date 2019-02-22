using UnityEngine;

public class TriggerEnemyDamage : MonoBehaviour
{

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            Player player = collision.gameObject.GetComponent<Player>();
            player.TakeDamage(30, gameObject.transform.rotation.y);
        }
    }
}
