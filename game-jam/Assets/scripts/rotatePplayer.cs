using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class rotatePplayer : MonoBehaviour
{
    Rigidbody2D rb;
    float tiltAngle = 15f; 
    // Start is called before the first frame update
    void Start()
    {
        rb = this.GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
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
        }
        
    }
}
