using UnityEngine;
using System.Collections;
using UnityEngine.Events;

public class PlayerController2D : MonoBehaviour
{
	[SerializeField] private float m_JumpForce = 400f;							// Amount of force added when the player jumps.
    [SerializeField] private float m_RunSpeed = 40f;                            // Amount of force added when the player runs.
    [Range(0, 1)] [SerializeField] private float m_CrouchSpeed = .36f;			// Amount of maxSpeed applied to crouching movement. 1 = 100%
	[Range(0, .3f)] [SerializeField] private float m_MovementSmoothing = .05f;	// How much to smooth out the movement
	[SerializeField] private bool m_AirControl = false;							// Whether or not a player can steer while jumping;
	[SerializeField] private LayerMask m_WhatIsGround;							// A mask determining what is ground to the character
	[SerializeField] private Transform m_GroundCheck;							// A position marking where to check if the player is grounded.
	[SerializeField] private Transform m_CeilingCheck;							// A position marking where to check for ceilings
	[SerializeField] private Collider2D m_CrouchDisableCollider;				// A collider that will be disabled when crouching
    public Animator animator;                                                   // Reference to the player's animator in order to change animation states and perform some checks
    [SerializeField] private GameObject smokePrefab; // prefab referring to the smoke explosion animation


    const float k_GroundedRadius = .2f; // Radius of the overlap circle to determine if grounded
	private bool m_Grounded;            // Whether or not the player is grounded.
	const float k_CeilingRadius = .2f; // Radius of the overlap circle to determine if the player can stand up
	private Rigidbody2D m_Rigidbody2D; // Reference to the player's RigidBody in order to make it move
	private bool m_FacingRight = true;  // For determining which way the player is currently facing.
	private Vector3 m_Velocity = Vector3.zero; // Vector needed to apply movement

    private bool m_JumpStarted = false; // boolean to check if the jump has recently started, in order not to collide with ground as you jump
    private float horizontalMove = 0f;  // float to check if the player is moving, and in which direction
    private bool isJumping = false; // bool to check if the player decided to jump
    private bool isCrouching = false; // bool to check if the player is crouching

    private bool m_wasCrouching = false; // bool to check if the player was crouching on last check

    // FUNCTION CALLED AT THE FIRST INSTANTIATION
    private void Awake()
	{
		m_Rigidbody2D = GetComponent<Rigidbody2D>();
        GameObject initialSmoke = Instantiate(smokePrefab, this.transform.position, this.transform.rotation);
        initialSmoke.transform.localScale = new Vector2(1.5f, 1.5f);
    }

    // FUNCTION CALLED EVERY SCREEN UPDATE
    private void Update()
    {
        horizontalMove = Input.GetAxisRaw("Horizontal") * m_RunSpeed; // -1 if moving left, 0 if not moving, 1 if moving right. Values taken from button press or analog sticks 
        if (Input.GetButtonDown("Jump")) // check if the player just tried to jump and pass that to the Move function
        {
            isJumping = true;
            m_JumpStarted = true;
            StartCoroutine(JumpAnimationFix()); // coroutine to avoid colliding with the ground as soon as you start jumping
        }
        if (Input.GetButtonDown("Crouch")) // check if the player is currently trying to crouch
        {
            if(!animator.GetBool("isJumping")) // if the player is in the middle of a jump, don't let it crouch. If not, let the crouch begin!
            {
                isCrouching = true;
            }
        }
        else if (Input.GetButtonUp("Crouch")) // check if the player isn't pressing the crouch button anymore, and pass that to the Move function
        {
            isCrouching = false;
        }
    }

    // COROUTINE TO AVOID ACCIDENTAL COLLISIONS WITH THE GROUND AS SOON AS THE PLAYER STARTS A JUMP
    IEnumerator JumpAnimationFix()
    {
        yield return new WaitForSeconds(.1f); // wait 0.1 second before checking if the player has reached ground. Time is small enough that the player cannot ground before it's elapsed.
        m_JumpStarted = false; // after 0.1 second the player can check again if it's grounded, so stop the check blocking
    }

    // FUNCTION CALLED EVERY SAME AMOUNT OF TIME, APPLY PHYSICS STUFF HERE
    private void FixedUpdate()
	{
        bool wasGrounded = m_Grounded; // store the previous state about if the player was on the ground or not
		m_Grounded = false; // assume the player isn't on the ground right now to check if it actually is

        // The player is grounded if a circlecast to the groundcheck position hits anything designated as ground
        // This can be done using layers instead but Sample Assets will not overwrite your project settings.
        Collider2D[] colliders = Physics2D.OverlapCircleAll(m_GroundCheck.position, k_GroundedRadius, m_WhatIsGround);

		for (int i = 0; i < colliders.Length && !m_Grounded; i++) // cycle through the collisions until you find something that is a ground object
		{
			if (colliders[i].gameObject != gameObject) // if the player is colliding with the ground, note it and stop any eventual jump animation
			{
				m_Grounded = true; // set that the player is currently on ground, for exiting the for loop and future checks
                if (!wasGrounded && !m_JumpStarted) // check if the player was not on the ground on last check, but also didn't just start a jump less than 0.1 s ago
                    animator.SetBool("isJumping", false); // if the player was jumping and now is on the ground, stop the jump animation
            }
		}

        Move(horizontalMove * Time.fixedDeltaTime, isCrouching, isJumping); // pass the infos about movement, jump and crouch inputs to the Move function
        isJumping = false; // after you make the player move, disable the jumping variable so that the player doesn't continuosly jump if you hold down the jump key
    }

    // FUNCTION TO MANAGE THE PLAYER MOVEMENTS
	public void Move(float move, bool crouch, bool jump)
	{
        animator.SetFloat("Speed", Mathf.Abs(move)); // in the animator set the speed of the character as the absolute speed right now, in order to change from idle to run and vice versa

        // If crouching, check to see if the character can stand up
        if (!crouch)
		{
			// If the character has a ceiling preventing them from standing up, keep them crouching
			if (Physics2D.OverlapCircle(m_CeilingCheck.position, k_CeilingRadius, m_WhatIsGround))
			{
				crouch = true;
                jump = false; // if the character is crouching under a ceiling, it cannot jump
            }
		}

		//only control the player if grounded or airControl is turned on
		if (m_Grounded || m_AirControl)
		{
			// If crouching
			if (crouch)
			{
                // if the player wasn't crouching before, start the crouch animation
				if (!m_wasCrouching)
				{
					m_wasCrouching = true;
                    animator.SetBool("isCrouching", true);
                }

				// Reduce the speed by the crouchSpeed multiplier
				move *= m_CrouchSpeed;

				// Disable one of the colliders when crouching
				if (m_CrouchDisableCollider != null)
					m_CrouchDisableCollider.enabled = false;
			}
            else // If not crouching
			{
				// Enable the collider when not crouching
				if (m_CrouchDisableCollider != null)
					m_CrouchDisableCollider.enabled = true;

                // if the player was crouching before, end the crouch animation
				if (m_wasCrouching)
				{
					m_wasCrouching = false;
                    animator.SetBool("isCrouching", false);
                }
			}

			// Move the character by finding the target velocity
			Vector3 targetVelocity = new Vector2(move * 10f, m_Rigidbody2D.velocity.y);
			// And then smoothing it out and applying it to the character
			m_Rigidbody2D.velocity = Vector3.SmoothDamp(m_Rigidbody2D.velocity, targetVelocity, ref m_Velocity, m_MovementSmoothing);

			// If the input is moving the player right and the player is facing left...
			if (move > 0 && !m_FacingRight)
			{
				// ... flip the player.
				Flip();
			}
			// Otherwise if the input is moving the player left and the player is facing right...
			else if (move < 0 && m_FacingRight)
			{
				// ... flip the player.
				Flip();
			}
		}
		// If the player should jump...
		if (m_Grounded && jump)
		{
            // Add a vertical force to the player.
            m_Grounded = false;
			m_Rigidbody2D.AddForce(new Vector2(0f, m_JumpForce));
            animator.SetBool("isJumping", true); // start the jump animation as soon as the force is applied
        }
	}

    // FUNCTION TO FLIP THE PLAYER WHEN CHANGING THE DIRECTION OF ITS MOVEMENT
	private void Flip()
	{
		// Switch the way the player is labelled as facing.
		m_FacingRight = !m_FacingRight;

        transform.Rotate(0f, 180f, 0f); // rotate the player 180° on the y axis. This way its children object will rotate with it.
	}
}
