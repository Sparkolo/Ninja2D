using UnityEngine;

public class Vcam : MonoBehaviour
{
    private Transform player;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        if (player == null)
        {
            Debug.LogError("No Player object found by the cinemachine camera");
        }

        this.GetComponent<Cinemachine.CinemachineVirtualCamera>().Follow = player;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
