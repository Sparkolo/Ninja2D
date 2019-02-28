using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameMaster : MonoBehaviour
{
    public static GameMaster gm;
    [SerializeField] private Transform playerPrefab;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private int spawnDelay;
    [SerializeField] private GameObject respawnUI;
    [SerializeField] private GameObject gameOverUI;
    [SerializeField] private GameObject levelWonUI;

    private static int _remainingLives = 3;
    public static int remainingLives
    {
        get { return _remainingLives; }
    }

    // Start is called before the first frame update
    void Awake()
    {
        if (gm == null)
        {
            gm = GameObject.FindGameObjectWithTag("GM").GetComponent<GameMaster>();
        }
    }


    public IEnumerator RespawnPlayer()
    {
        yield return new WaitForSeconds(spawnDelay);

        gm.respawnUI.GetComponent<RespawnUIController>().EndRespawn();

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public static void KillPlayer (Player player)
    {
        Destroy(player.gameObject);
        _remainingLives--;

        if(remainingLives <= 0)
        {
            gm.EndGame();
        }
        else
        {
            gm.respawnUI.SetActive(true);
            gm.StartCoroutine(gm.RespawnPlayer());
        }
    }


    public void EndGame()
    {
        gm.gameOverUI.SetActive(true);
    }

    public static void ResetLives ()
    {
        _remainingLives = 3;
    }

    public static void WinLevel()
    {
        if(SceneManager.GetActiveScene().buildIndex + 1 < 3)
        {
            gm.levelWonUI.SetActive(true);
        } 
        else
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
    }
}
