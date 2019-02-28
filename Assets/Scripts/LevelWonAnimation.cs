using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelWonAnimation : MonoBehaviour
{
    void NextLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}
