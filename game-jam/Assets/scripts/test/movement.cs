using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class movement : MonoBehaviour
{
    // [SerializeField] float torqueAmount= 1f;
    // [SerializeField] float normalSpeed=20f;
    // [SerializeField] float boostSpeed=40f;
    // Rigidbody2D rb2d;
    // SurfaceEffector2D surfaceEffector2D;
    // bool canMove = true;

    // // Start is called before the first frame update
    // void Start()
    // {
    //     rb2d = GetComponent<Rigidbody2D>();
    //     surfaceEffector2D = FindObjectOfType<SurfaceEffector2D>();
    // }

    // // Update is called once per frame
    // void Update()
    // {
    //     if (canMove)
    //     {
    //     RotatePlayer();
    //     RespondToBoost();
    //     }
    // }

    // public void DisableControls()
    // {
    //     canMove = false;
    // }

    // void RespondToBoost()
    // {
    //     if(Input.GetKey(KeyCode.UpArrow))
    //     {
    //         surfaceEffector2D.speed = boostSpeed;
    //     }
    //     else 
    //     {
    //         surfaceEffector2D.speed = normalSpeed;
    //     }
    // }

    // void RotatePlayer()
    // {
    //     if (Input.GetKey(KeyCode.LeftArrow))
    //     {
    //         rb2d.AddTorque(torqueAmount);
    //     }
    //     else if (Input.GetKey(KeyCode.RightArrow))
    //     {
    //         rb2d.AddTorque(-torqueAmount);
    //     }
    // }

    // [Header("Movement Settings")]
    // public float moveSpeed = 5f;  // Speed of the character
    // public float jumpForce = 10f; // Force applied when jumping
    // public float trickRotationSpeed = 360f; // Speed of rotation for tricks
    // public float smoothing = 0.05f;
    // public float slopeStickForce = 10f;
    // public float additionalGravity = 20f;

    // [Header("Ground Detection")]
    // public Transform groundCheck;
    // public LayerMask groundLayer;
    // public float groundCheckRadius = 0.2f;

    // private Rigidbody2D rb;
    // [SerializeField] private bool isGrounded;
    // private bool isJumping;

    // private float coyoteTime = 0.1f;
    // private float coyoteTimeCounter;
    // private Quaternion targetRotation;

    // void Start()
    // {
    //     rb = GetComponent<Rigidbody2D>();
    // }

    // void Update()
    // {
    //     // Move the character forward
    //     rb.velocity = new Vector2(moveSpeed, rb.velocity.y);

    //     // Check if the character is on the ground
    //     //isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
        
    //     // Handle jumping
    //     // if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
    //     // {
    //     //     Debug.Log("SHOULD JUMP!");
    //     //     Jump();
    //     // }

    //     // Handle jumping
    //     if (Input.GetKeyDown(KeyCode.Space) && coyoteTimeCounter > 0f)
    //     {
    //         Jump();
    //     }


    //     // Handle tricks in the air
    //     if (!isGrounded && Input.GetKey(KeyCode.LeftArrow))
    //     {
    //         PerformTrick(-1); // Rotate counter-clockwise
    //     }
    //     else if (!isGrounded && Input.GetKey(KeyCode.RightArrow))
    //     {
    //         PerformTrick(1); // Rotate clockwise
    //     }

    //     if(isGrounded){

    //         Vector2 slopeNormal = GetSlopeNormal();
    //         Vector2 moveDirection = Vector2.Perpendicular(slopeNormal) * -1;
    //         moveDirection.Normalize();
    //         rb.velocity = new Vector2(moveDirection.x * moveSpeed, rb.velocity.y);

    //         // Rotate the character to align with the slope
    //         float slopeAngle = Mathf.Atan2(slopeNormal.y, slopeNormal.x) * Mathf.Rad2Deg;
    //         targetRotation = Quaternion.Euler(0f, 0f, slopeAngle - 90f);
    //         transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, smoothing * Time.deltaTime);

    //         coyoteTimeCounter = coyoteTime;
    //         rb.AddForce(-slopeNormal * slopeStickForce, ForceMode2D.Force);
    //         rb.gravityScale = 1;
    //     }else{
    //         rb.gravityScale = 3;
    //         coyoteTimeCounter -= Time.deltaTime;
    //     }
    // }

    // private void OnCollisionEnter2D(Collision2D other)
    // {
    //     if(other.collider.CompareTag("ground")){
    //         isGrounded = true;
    //     }
    // }

    // private void OnCollisionExit2D(Collision2D other)
    // {
    //     if(other.collider.CompareTag("ground")){
    //         isGrounded = false;
    //     }
    // }

    // void Jump()
    // {
    //     rb.velocity = new Vector2(rb.velocity.x, jumpForce);
    //     isJumping = true;
    // }

    // void PerformTrick(int direction)
    // {
    //     transform.Rotate(0f, 0f, direction * trickRotationSpeed * Time.deltaTime);
    // }

    // private void OnDrawGizmos()
    // {
    //     // Draw the ground check radius for visualization
    //     Gizmos.color = Color.red;
    //     Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
    // }

    // private Vector2 GetSlopeNormal()
    // {
    //     RaycastHit2D hit = Physics2D.Raycast(groundCheck.position, Vector2.down, groundCheckRadius + 0.1f, groundLayer);
    //     if (hit)
    //     {
    //         return hit.normal;
    //     }
    //     return Vector2.up;
    // }


      [Header("Movement Settings")]
    public float moveSpeed = 5f;  // Speed of the character
    public float jumpForce = 10f; // Force applied when jumping
    public float trickRotationSpeed = 360f; // Speed of rotation for tricks
    public float smoothing = 0.05f;
    public float slopeStickForce = 10f;
    public float additionalGravity = 20f;
    public float trickBoost = 2f; // Speed boost after landing a trick
    public float trickThreshold = 0.2f; // Minimum time to hold space for a trick

    [Header("Ground Detection")]
    public Transform groundCheck;
    public LayerMask groundLayer;
    public float groundCheckRadius = 0.2f;
    public Vector2 groundCheckSize = new Vector2(0.5f, 0.5f);

    private Rigidbody2D rb;
    [SerializeField] private bool isGrounded;
    private bool isJumping;

    private float coyoteTime = 0.1f;
    private float coyoteTimeCounter;
    private Quaternion targetRotation;

    private bool isTricking;
    private float trickStartTime;

    [SerializeField] float torqueAmount= 1f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        // Move the character forward
        rb.velocity = new Vector2(moveSpeed, rb.velocity.y);
        isGrounded = checkIsGrounded();

        if (Input.GetKey(KeyCode.Space))
        {
            // Start tracking the time if we are holding space
            if (!isTricking)
            {
                if (trickStartTime == 0f)
                {
                    trickStartTime = Time.time;
                }
                else if (Time.time - trickStartTime >= trickThreshold)
                {
                    // Perform trick if held long enough
                    isTricking = true;
                }
            }
            else
            {
                // Perform trick while space is held
                PerformTrick();
            }
        }
        else
        {
            if (isTricking)
            {
                // End trick and apply boost if landed
                isTricking = false;
                if (isGrounded)
                {
                    rb.velocity = new Vector2(moveSpeed + trickBoost, rb.velocity.y);
                }
            }
            else if (Input.GetKeyDown(KeyCode.Space) && coyoteTimeCounter > 0f)
            {
                // Jump if space is tapped
                Jump();
            }

            // Reset trick tracking
            trickStartTime = 0f;
        }

        // Handle gravity and coyote time
        if (isGrounded)
        {
            Vector2 slopeNormal = GetSlopeNormal();
            Vector2 moveDirection = Vector2.Perpendicular(slopeNormal) * -1;
            moveDirection.Normalize();
            rb.velocity = new Vector2(moveDirection.x * moveSpeed, rb.velocity.y);

            // Rotate the character to align with the slope
            float slopeAngle = Mathf.Atan2(slopeNormal.y, slopeNormal.x) * Mathf.Rad2Deg;
            targetRotation = Quaternion.Euler(0f, 0f, slopeAngle - 90f);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, smoothing * Time.deltaTime);

            coyoteTimeCounter = coyoteTime;
            rb.AddForce(-slopeNormal * slopeStickForce, ForceMode2D.Force);
            rb.gravityScale = 1;
        }
        else
        {
            rb.gravityScale = 3;
            coyoteTimeCounter -= Time.deltaTime;
        }
    }

    // private void OnCollisionEnter2D(Collision2D other)
    // {
    //     if (other.collider.CompareTag("ground"))
    //     {
    //         isGrounded = true;
    //     }
    // }

    // private void OnCollisionExit2D(Collision2D other)
    // {
    //     if (other.collider.CompareTag("ground"))
    //     {
    //         isGrounded = false;
    //     }
    // }

    void Jump()
    {
        rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        
        isJumping = true;
    }

    void PerformTrick()
    {
        //float trickRotation = trickRotationSpeed * Time.deltaTime;
        //transform.Rotate(0f, 0f, trickRotation);

        rb.AddTorque(torqueAmount);
    }

    private void OnDrawGizmos()
    {
        // Draw the ground check radius for visualization
        Gizmos.color = Color.red;
        Gizmos.DrawCube(groundCheck.position, groundCheckSize);
    }

    private Vector2 GetSlopeNormal()
    {
        RaycastHit2D hit = Physics2D.Raycast(groundCheck.position, Vector2.down, groundCheckRadius + 0.1f, groundLayer);
        if (hit)
        {
            return hit.normal;
        }
        return Vector2.up;
    }

    private bool checkIsGrounded(){
        if(Physics2D.OverlapBox(groundCheck.position, groundCheckSize, 0, groundLayer)){
            return true;
        }else{
            return false;
        }
    }
}
