using UnityEngine;
using UnityEngine.Events;
using System.Collections;

public class Jeremy : MonoBehaviour {

    [Header("Position and Layer Checks")]
    [SerializeField] private LayerMask whatIsGround;
    [SerializeField] private Transform groundCheckFront;                            // A position marking where to check if the player is grounded.
    [SerializeField] private Transform groundCheckBack;

    [Header("Attributes")]
    [SerializeField] public bool hasControl;
    [Range(0, 100f)] [SerializeField] public float runSpeed = 40f;
    [SerializeField] private float jumpVelocity;                                    // The vertical velocity of the player after a jumps.
    [Range(0, .3f)] [SerializeField] private float movementSmoothing = .05f;        // How much to smooth out the movement
    [SerializeField] private bool airControl;                                       // Whether the player can steer while jumping;
    [SerializeField] public float fallMultiplier = 4f;                              // Affects falling speed
    [SerializeField] public float lowJumpMultiplier = 2f;                           // Affects speed during the ascension of a jump
    [SerializeField] private float speedMod = 1f;                                   // Affects the horizontal speed
    [SerializeField] public Color green;
    [SerializeField] public Color brown;
    [SerializeField] public Color gold;

    [Header("References")]
    public Animator animator;                       // The player's animator
    public Rigidbody2D rb;                          // The player's rigidbody
    public InputManager im;                         // The InputManager
    public Material crownColor;
    public ParticleSystem jumpEffect1;
    public ParticleSystem jumpEffect2;
    public ParticleSystem runEffect;
    public SoundManager sm;
    public AudioClip[] steps;
    public AudioClip jump;
    public AudioClip land;
    public AudioClip candy;
    public AudioClip trill;

    private const float k_GroundedRadius = .1f;     // Radius of the overlap circle to determine if grounded
    public bool grounded;                           // Whether the player is grounded.
    private bool wasGrounded;                       // Whether the player was grounded.
    private int facing = 1;                         // For determining which way the player is currently facing
    private bool isJumping;                         // Whether the player is jumping (Holding down the jump button)
    public bool canJump;                           // Whether the player is allowed to jump
    private float horizontalMove = 0;
    private bool jumpWhenReady = false;
    private bool canMove = true;                    // Whether the player is allowed to move
    private Vector3 velocity = Vector3.zero;        // Serves as a default reference velocity
    private int extraJumps = 0;
    private Transform spawnPoint;
    private ArrayList candies;

    private void Update() {
        if (transform.position.y < -10) {
            rb.velocity = Vector2.zero;
            transform.SetPositionAndRotation(spawnPoint.position, Quaternion.identity);
            foreach (GreenCandy candy in candies) {
                candy.Refresh();
            }
            extraJumps = 1;
            crownColor.SetColor("_CrownColor", brown);
        }

        wasGrounded = grounded;
        grounded = false;
        // The player is grounded if a circlecast to the groundcheck position hits anything designated as ground
        Collider2D[] cBack = Physics2D.OverlapCircleAll(groundCheckBack.position, k_GroundedRadius, whatIsGround);
        Collider2D[] cFront = Physics2D.OverlapCircleAll(groundCheckFront.position, k_GroundedRadius, whatIsGround);
        Collider2D[] colliders = new Collider2D[cFront.Length + cBack.Length];
        cFront.CopyTo(colliders, 0);
        cBack.CopyTo(colliders, cFront.Length);
        for (int i = 0; i < colliders.Length; i++) {
            if (colliders[i].gameObject != gameObject && rb.velocity.y < 0.01f) {
                grounded = true;
                if (!wasGrounded) {
                    jumpEffect1.Play();
                    sm.PlaySound(land);
                    foreach (GreenCandy candy in candies) {
                        candy.RefreshAura();
                    }
                    if (jumpWhenReady) {
                        jumpWhenReady = false;
                        Jump(jumpVelocity);
                    } else {
                        animator.SetBool("Jumping", false);
                        //if (rb.velocity.y > 0.01f)
                            //rb.velocity = new Vector2(rb.velocity.x, 0f);
                    }

                    animator.SetBool("Falling", false);
                    break;
                }
            }
        }

        if (!grounded && rb.velocity.y < 0 && !canJump) {
            animator.SetBool("Falling", true);
            animator.SetBool("Jumping", false);
        }
        if (grounded && !isJumping) {
            canJump = true;
        }

        if (wasGrounded && !grounded && !isJumping) {
            StartCoroutine(JumpDelay());
        }
    }

    private void Start() {
        crownColor.SetColor("_CrownColor", brown);
        candies = new ArrayList();
    }

    public void Move(float move, bool jump, bool jumpCancel) {
        horizontalMove = runSpeed * move * Time.fixedDeltaTime;
        animator.SetFloat("Speed", Mathf.Abs(horizontalMove));

        if (canMove) {
            if (rb.velocity.y < 2 && !grounded)
                rb.velocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
            else if (rb.velocity.y > 0 && !grounded && !isJumping)
                rb.velocity += Vector2.up * Physics2D.gravity.y * (lowJumpMultiplier - 1) * Time.deltaTime;

            if (grounded && horizontalMove == 0)
                rb.velocity = Vector3.up * rb.velocity;

            //only control the player if grounded or airControl is turned on
            if (grounded || airControl) {
                // Move the character by finding the target velocity
                Vector3 targetVelocity = new Vector2(horizontalMove * 10f * speedMod, rb.velocity.y);
                // Smooth it out and apply it to the character
                if (grounded)
                    rb.velocity = Vector3.SmoothDamp(rb.velocity, targetVelocity, ref velocity, .1f);
                // When turning mid-air, apply smoothed movement
                else if (Mathf.Sign(horizontalMove) != runSpeed)
                    rb.velocity = Vector3.SmoothDamp(rb.velocity, targetVelocity, ref velocity, movementSmoothing);
                // If the input is moving the player right and the player is facing left...
                if (horizontalMove > 0 && facing < 0)
                    Flip(); // ... flip the player.
                // Otherwise if the input is moving the player left and the player is facing right...
                else if (horizontalMove < 0 && facing > 0)
                    Flip(); // ... flip the player.
            }
            if (jumpCancel) {
                isJumping = false;
                jumpWhenReady = false;
            }
            if (jump)
                isJumping = true;

            // If the player should jump...
            if (jump) {
                if (canJump) {
                    Jump(jumpVelocity); // Jump
                    jumpEffect1.Play();
                } else if (extraJumps > 0) {
                    Jump(jumpVelocity); // Jump
                    crownColor.SetColor("_CrownColor", brown);
                    extraJumps--;
                    jumpEffect2.Play();
                    foreach (GreenCandy candy in candies) {
                        candy.Refresh();
                    }
                } else
                    StartCoroutine(BufferJump());
            }
        }
    }

    void Flip() {
        // Switch the way the player is labelled as facing.
        facing *= -1;
        // Multiply the player's x local scale by -1.
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
        theScale = runEffect.transform.localScale;
        theScale.x *= -1;
        runEffect.transform.localScale = theScale;
    }

    void Jump(float jumpVelocity) {
        canJump = false;
        isJumping = true;
        rb.velocity = new Vector2(rb.velocity.x, 0);
        rb.AddForce(Vector2.up * jumpVelocity, ForceMode2D.Impulse);
        animator.SetBool("Jumping", true);
        animator.SetBool("Falling", false);
        sm.PlaySound(jump);
    }

    public void PlayRunEffect() {
        runEffect.Play();
        int num = Random.Range(0, 3);
        sm.PlaySound(steps[num]);
    }

    IEnumerator JumpDelay() {
        yield return new WaitForSeconds(.07f);
        canJump = false;
    }

    IEnumerator BufferJump() {
        jumpWhenReady = true;
        yield return new WaitForSeconds(.07f);
        jumpWhenReady = false;
    }

    public void GCandy(GreenCandy gc) {
        candies.Add(gc);
        crownColor.SetColor("_CrownColor", green);
        extraJumps = 1;
        sm.PlaySound(candy);
        if (grounded) {
            foreach (GreenCandy candy in candies) {
                candy.RefreshAura();
            }
        }
    }

    public void Loot(Transform t) {
        spawnPoint = t;
        sm.PlaySound(trill);
    }
}