using UnityEngine;

public class EndTrigger : MonoBehaviour
{

    [SerializeField] private GameObject doorEnd;
    private bool canWin = true;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            gameObject.GetComponent<BoxCollider2D>().enabled = false;
            doorEnd.GetComponent<BoxCollider2D>().enabled = true;

            WinLevel();

            Player player = collision.gameObject.GetComponent<Player>();
            player.GetComponent<PlayerController2D>().canMove = false;
        }
    }

    void WinLevel()
    {
        if(canWin)
        {
            GameMaster.WinLevel();
            canWin = false;
        }
    }
}
