using System.Collections;
using UnityEngine;

public class RespawnUIController : MonoBehaviour
{
    [SerializeField] Animator animator;

    public void EndRespawn()
    {
        animator.SetBool("endRespawn", true);
        StartCoroutine(DisableRespawnUI());
    }

    IEnumerator DisableRespawnUI()
    {
        yield return new WaitForSeconds(0.5f);
        this.gameObject.SetActive(false);
    }

}
