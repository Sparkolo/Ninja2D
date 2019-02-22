using System.Collections;
using UnityEngine;

public class EnemyExplosion : MonoBehaviour
{
    private float animationDuration = 0.5f; 

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(VanquishSmoke(animationDuration));
    }

    IEnumerator VanquishSmoke(float timeToWait)
    {
        yield return new WaitForSeconds(timeToWait);
        Destroy(this.gameObject);
    }
}
