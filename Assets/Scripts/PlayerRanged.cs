using System.Collections;
using UnityEngine;

public class PlayerRanged : MonoBehaviour
{
    private float remainingAtkTime; // cooldown time to be elapsed before next attack. If <=0 you can attack.
    [SerializeField] private float defaultReloadAtkTime = 0.8f; // default cooldown time to wait after you attack

    [SerializeField] private Transform firePoint; // reference to the ranged attack start position in Unity
    [SerializeField] private GameObject fireBallPrefab;
    public Animator animator; // reference to the Animator component of the player, in order to trigger attack animations and check when not to attack
    private bool rangedAttacking = false;

    public void RangedAttack()
    {
        rangedAttacking = true;
    }
       
    // Update is called once per frame
    void Update()
    {
        if (remainingAtkTime <= 0) // if you don't have any cooldown to wait check for attacks
        {
            if (rangedAttacking) // check if the player pressed the secondary attack key
            {
                if (!animator.GetBool("isCrouching")) // only shoot the fire ball if the player isn't crouching at the moment
                {
                   StartCoroutine(Shoot());

                   animator.SetBool("isRangedAttacking", true); // start the ranged attack animation
                   StartCoroutine(WaitAtkEnd("isRangedAttacking", 0.5f)); // coroutine to end the attack animation
                   remainingAtkTime = defaultReloadAtkTime; // reset the cooldown to be waited before next attack
                   rangedAttacking = false;
                }
            }
        }
        else
        {
            remainingAtkTime -= Time.deltaTime; // If it's still in cooldown, reduce the reload time left of the ammount of time passed since last check
        }
    }


    // COROUTINE TO DISABLE THE ATK ANIMATION AFTER THE ATK RELOAD TIME PASSES
    IEnumerator WaitAtkEnd(string atkType, float timeToWait) // atkType = "isRangedAttacking", possibility to extend it in the future; timeToWait = cooldown time of the attack
    {
        yield return new WaitForSeconds(timeToWait); // wait the cooldown time of the current attack before ending the animation
        animator.SetBool(atkType, false); // end the ranged attack animation
    }

    // FUNCTION TO ACTUALLY SHOOT THE FIRE BALLS
    IEnumerator Shoot ()
    {
        yield return new WaitForSeconds(0.35f); // wait the technique to be cast before shooting
        Instantiate(fireBallPrefab, firePoint.position, firePoint.rotation);
    }
}
