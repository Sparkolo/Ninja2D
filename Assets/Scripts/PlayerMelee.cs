using System.Collections;
using UnityEngine;

public class PlayerMelee : MonoBehaviour
{
    private float remainingAtkTime; // cooldown time to be elapsed before next attack. If <=0 you can attack.
    private float jumpReloadAtkDelay = 0.1f; // extra cooldown to be added after you do a jump attack. It's tougher to pull out than a ground attack, you know?
    [SerializeField] private float defaultReloadAtkTime = 0.3f; // default cooldown time to wait after you attack

    [SerializeField] private Transform atkPosition; // reference to the attack start position in Unity
    [SerializeField] private LayerMask identifyEnemy; // reference to the enemies' type, in order to know what to check to be hit from an attack
    [SerializeField] private float atkRange = 0.9f; // radius of the attack range circle
    [SerializeField] private int atkDamage = 40; // damage inflicted by the attack if you hit
    public Animator animator; // reference to the Animator component of the player, in order to trigger attack animations and check when not to attack
    bool _enemyDamaged = false;

    // Update is called once per frame
    void Update()
    {
        if (remainingAtkTime <= 0) // if you don't have any cooldown to wait check for attacks
        {
            if (Input.GetButtonDown("Fire1")) // check if the player pressed the main attack key
            {
                if (!animator.GetBool("isCrouching")) // only attack if the player isn't crouching at the moment
                {
                    Collider2D[] enemiesToDamage = Physics2D.OverlapCircleAll(atkPosition.position, atkRange, identifyEnemy); // check if there's any enemy inside the attack range
                    for (int i = 0; i < enemiesToDamage.Length; i++) // loop though all the enemies inside the attack range and inflict them the attack damage
                    {
                        if(enemiesToDamage[i].GetComponent<EnemyFrog>())
                        {
                            enemiesToDamage[i].GetComponent<EnemyFrog>().TakeDamage(atkDamage, gameObject.transform.rotation.y);
                            _enemyDamaged = true;
                        }
                    }

                    if(animator.GetBool("isJumping")) // if the player is jumping, play the jump attack animation. Also the cooldown will be a bit longer. 
                    {
                        animator.SetBool("isJumpAttacking", true); // start the jump attack animation
                        StartCoroutine(WaitAtkEnd("isJumpAttacking", defaultReloadAtkTime + jumpReloadAtkDelay)); // coroutine to end the jump attack animation
                        remainingAtkTime = defaultReloadAtkTime + jumpReloadAtkDelay; // reset the cooldown to be waited before next attack
                    }
                    else  // if the player is not jumping, just play the standard attack animation
                    {
                        animator.SetBool("isAttacking", true); // start the attack animation
                        StartCoroutine(WaitAtkEnd("isAttacking", defaultReloadAtkTime)); // coroutine to end the attack animation
                        remainingAtkTime = defaultReloadAtkTime; // reset the cooldown to be waited before next attack
                    }
                }
            }
        }
        else
        {
            remainingAtkTime -= Time.deltaTime; // If it's still in cooldown, reduce the reload time left of the ammount of time passed since last check
        }

        //  METHOD TO VISUALLY SEE THE ATTACK RADIUS OF EFFECT
        /*  
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(atkPosition.position, atkRange);
        }
        */
    }


    // COROUTINE TO DISABLE THE ATK ANIMATION AFTER THE ATK RELOAD TIME PASSES
    IEnumerator WaitAtkEnd(string atkType, float timeToWait) // atkType = "isAttacking" || "isJumpAttacking"; timeToWait = cooldown time of the attack
    {
        yield return new WaitForSeconds(timeToWait); // wait the cooldown time of the current attack before ending the animation
        animator.SetBool(atkType, false); // end the attack animation

        // check again if you hit an enemy at the end of the animation, so that you don't miss it 
        Collider2D[] enemiesToDamage = Physics2D.OverlapCircleAll(atkPosition.position, atkRange, identifyEnemy); // check if there's any enemy inside the attack range
        for (int i = 0; i < enemiesToDamage.Length; i++) // loop though all the enemies inside the attack range and inflict them the attack damage
        {
            if (!_enemyDamaged) 
                enemiesToDamage[i].GetComponent<EnemyFrog>().TakeDamage(atkDamage, gameObject.transform.rotation.y);
            _enemyDamaged = false;
        }
    }
}
