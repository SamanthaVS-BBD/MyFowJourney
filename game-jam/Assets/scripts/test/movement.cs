using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class movement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;  // Speed of the character
    public float maxMoveSpeed = 40;
    public float jumpForce = 10f; // Force applied when jumping
    public float trickRotationSpeed = 360f; // Speed of rotation for tricks
    public float smoothing = 0.05f;
    public float slopeStickForce = 10f;
    public float additionalGravity = 20f;
    public float trickBoost = 2f; // Speed boost after landing a trick
    public float trickThreshold = 0.3f; // Minimum time to hold space for a trick

    [Header("Ground Detection")]
    public Transform groundCheck;
    public LayerMask groundLayer;
    public float groundCheckRadius = 0.2f;
    public Vector2 groundCheckSize = new Vector2(0.5f, 0.5f);
    public float rayDistance;

    private Rigidbody2D rb;
    [SerializeField] private bool isGrounded;
    private bool isJumping;

    private float coyoteTimeCounter;
    private Quaternion targetRotation;

    private bool isTricking;
    private float trickStartTime;

    [SerializeField] float torqueAmount = 1f;


    public float RotationalSpeed = 325;
    public bool IsGrounded = false;
    public int NumFlips = 0;
    public int MaxFlips = 10;
    public float FlipBoost = 500;
    public float rotationSpeed = 8f;

    [Header("Score")]
    private int score;
    public int getScore() => score;
    private float prevScoreDistance;
    private bool isGameOver;

    [Header("Sound Clips")]
    [SerializeField] private AudioClip landingSound;
    [SerializeField] private AudioClip deathSound;
    [SerializeField] private AudioClip bgm;


    [Header("Sound Sources")]
    [SerializeField] private AudioSource landingSource;
    [SerializeField] private AudioSource deathSource;
    [SerializeField] private AudioSource bgmSource;

    void Start()
    {
        playClip(bgmSource, bgm, true, 0.1f);
        isGameOver = false;
        rb = GetComponent<Rigidbody2D>();
        prevScoreDistance = transform.position.x;
        score = 0;
    }

    void Update()
    {
        if (!isGameOver)
        {
            rb.velocity = new Vector2(moveSpeed, rb.velocity.y);
        }
        else
        {
            rb.velocity = new Vector2() { x = 0, y = 0 };
        }
        Boolean inAir = !isGrounded;
        isGrounded = checkIsGrounded();
        if (inAir == isGrounded && !isGameOver)
        {
            playClip(landingSource, landingSound, volume: 0.005f);
        }

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


        if (isGrounded)
        {
            Vector2 slopeNormal = GetSlopeNormal();
            Vector2 moveDirection = Vector2.Perpendicular(slopeNormal) * -1;
            moveDirection.Normalize();
            rb.velocity = new Vector2(moveDirection.x * moveSpeed, rb.velocity.y);
            RotateCharacterToGround();
            rb.AddForce(-slopeNormal * slopeStickForce, ForceMode2D.Force);
            rb.gravityScale = 1;
        }
        else
        {
            rb.gravityScale = 3;
        }

        if (transform.position.x - prevScoreDistance > 20)
        {
            score++;
            prevScoreDistance = transform.position.x;
        }
    }

    void Jump()
    {
        Debug.Log("WE ARE JUMPING");
        if (rb.rotation > 10)
            rb.velocity = new Vector2(rb.velocity.x, jumpForce * 2.2f);
        else
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);

        isJumping = true;
    }

    private float WrapAngle(float angle)
    {
        var answer = ((angle + 180) % 360) - 180;
        return answer;
    }

    void PerformTrick()
    {

        var prevRotation = rb.rotation;
        rb.rotation += RotationalSpeed * Time.deltaTime;
        rb.rotation = WrapAngle(rb.rotation);
        if (prevRotation < -45 && rb.rotation > -45)
        {
            if (NumFlips < MaxFlips) NumFlips++;
            if (moveSpeed != maxMoveSpeed)
            {
                moveSpeed += trickBoost;
            }

        }
    }

    private void OnDrawGizmos()
    {
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

    private bool checkIsGrounded()
    {
        if (Physics2D.OverlapBox(groundCheck.position, groundCheckSize, 0, groundLayer))
        {

            RotateCharacterToGround();
            return true;
        }
        else
        {
            return false;
        }
    }
    float jumpStartTime;
    public void jump(InputAction.CallbackContext context)
    {

        if (context.performed)
        {
            if (!isGameOver)
            {
                if (isTricking && isGrounded && Time.time - jumpStartTime >= trickThreshold)
                {
                    isTricking = false;
                    rb.velocity = new Vector2(moveSpeed + trickBoost, rb.velocity.y);
                }
                else if (isGrounded)
                {
                    Jump();
                    isTricking = false;
                }
            }

        }

        if (context.canceled)
        {
            if (isGrounded && !isGameOver)
            {
                Jump();
                jumpStartTime = Time.time;
                isTricking = true;
            }
            else if (!isGrounded && !isGameOver)
            {
                rb.AddForce(new Vector2(5, 0), ForceMode2D.Force);
            }
        }
    }

    void RotateCharacterToGround()
    {
        // // Cast a ray downwards from the character's position to detect the ground.
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, rayDistance);


        if (hit.collider != null && hit.collider.CompareTag("ground")) // Check if the ray hit something with the ground tag
        {
            Vector2 surfaceNormal = hit.normal;


            float angle = Mathf.Atan2(surfaceNormal.y, surfaceNormal.x) * Mathf.Rad2Deg;
            if (Math.Abs(angle) < 30)
            {
                GameOver();
            }
            if (angle > 10 || angle < -10)
            {
                transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0, 0, angle - 90f), rotationSpeed * Time.deltaTime);
            }
        }

    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("ground") || other.gameObject.CompareTag("obstacle"))
        {
            GameOver();
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("obstacle"))
        {
            GameOver();
        }
    }

    private void GameOver()
    {
        playClip(deathSource, deathSound, volume: 0.35f);
        GameObject UIM = GameObject.FindGameObjectWithTag("UIManager");
        UIM.GetComponent<UIManager>().setGameScore(false);
        UIM.GetComponent<UIManager>().gameOverUI(score.ToString());
        isGameOver = true;
        rb.velocity = new Vector2() { x = 0, y = 0 };
    }

    private void playClip(AudioSource source, AudioClip clip, Boolean loop = false, float volume = 1f)
    {
        source.clip = clip;
        source.volume = volume;
        source.Play();
        source.loop = loop;
    }
}
