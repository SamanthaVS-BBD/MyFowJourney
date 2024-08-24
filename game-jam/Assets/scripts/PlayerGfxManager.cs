using UnityEngine;

public class CharacterRotationOnSlope : MonoBehaviour
{
    public string groundTag = "ground"; // Tag to define what is considered ground.
    [SerializeField]
    public float rayDistance = 20; // Distance of the raycast.
    public float rotationSpeed = 8f; 
    Rigidbody2D rb;
    float tiltAngle = 15f; 
    private void Start()
    {
        rayDistance = 4;
        rb = this.GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if(!this.GetComponent<characterScript>().IsGrounded){
            float verticalVelocity = rb.velocity.y;

            // Determine the tilt based on vertical velocity
            float targetRotation = 0f;
            if (verticalVelocity > 0)
            {
                // Tilting upwards when jumping
                targetRotation = tiltAngle;
            }
            else if (verticalVelocity < 0)
            {
                // Tilting downwards when falling
                targetRotation = -tiltAngle;
            }

            // Apply the rotation to the character
            transform.rotation = Quaternion.Euler(0f, 0f, targetRotation);
        }else{
            RotateCharacterToGround();
        }
        
    }

    void RotateCharacterToGround()
    {
        // Cast a ray downwards from the character's position to detect the ground.
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, rayDistance);


        if (hit.collider != null && hit.collider.CompareTag(groundTag)) // Check if the ray hit something with the ground tag
        {
            Vector2 surfaceNormal = hit.normal;

            float angle = Mathf.Atan2(surfaceNormal.y, surfaceNormal.x) * Mathf.Rad2Deg;
            if(angle > 10 || angle < -10){
                transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0, 0, angle - 90f), rotationSpeed * Time.deltaTime);
            }

            
        }
    }
}
