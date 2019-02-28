using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class LivesCounter : MonoBehaviour
{
    private Text lives;
    
    void Awake()
    {
        lives = GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        lives.text = GameMaster.remainingLives.ToString();
    }
}
