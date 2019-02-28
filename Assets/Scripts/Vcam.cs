using UnityEngine;

public class Vcam : MonoBehaviour
{
    private Transform target;
    private float nextSearchTime = 0;

    // Start is called before the first frame update
    void Start()
    {
        target = GameObject.FindGameObjectWithTag("Player").transform;
        if (target == null)
        {
            Debug.LogError("No Player object found by the cinemachine camera");
        }

        this.GetComponent<Cinemachine.CinemachineVirtualCamera>().Follow = target;
    }

    // Update is called once per frame
    void Update()
    {
        if(target == null)
        {
            if (nextSearchTime <= 0)
                FindPlayer();
            else
                nextSearchTime -= Time.deltaTime;
        }
    }

    void FindPlayer ()
    {
        GameObject searchResult = GameObject.FindGameObjectWithTag("Player");
        if(searchResult != null)
        {
            target = searchResult.transform;
            this.GetComponent<Cinemachine.CinemachineVirtualCamera>().Follow = target;
        }
        nextSearchTime = 0.5f;
    }
}
