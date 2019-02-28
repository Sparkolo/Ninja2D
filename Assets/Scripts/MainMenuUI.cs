using UnityEngine.SceneManagement;
using UnityEngine;

public class MainMenuUI : MonoBehaviour
{
    public void StartPlay()
    {
        GameMaster.ResetLives();
        SceneManager.LoadScene(1);
    }
}
