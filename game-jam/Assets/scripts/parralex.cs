using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class parralex : MonoBehaviour
{
    public float multiplier;
    public bool horizontalOnly;
    public bool infiniteHorizontal;
    public bool infiniteVertical;
    public bool isInfinite;

    public GameObject camera;
    private Vector3 startPosition;
    private Vector3 cameraStartPos;
    
    private float length;

    private void Start()
    {
        startPosition = transform.position;
        cameraStartPos = camera.transform.position;
        if(isInfinite) {
            length = GetComponent<SpriteRenderer>().bounds.size.x;
        }

        CalculateStartPos();
    }

    private void CalculateStartPos(){
        float distX = (camera.transform.position.x - transform.position.x) * multiplier;
        float distY = (camera.transform.position.y - transform.position.y) * multiplier;
        Vector3 tmp = new Vector3(startPosition.x, startPosition.y, startPosition.z);

        if(infiniteHorizontal){
            tmp.x = transform.position.x + distX;
        }

        if(infiniteVertical){
            tmp.y = transform.position.y + distY;
        }

        startPosition = tmp;

    }

    private void FixedUpdate()
    {
        Vector3 position = startPosition;

        if(horizontalOnly){
            position.x += multiplier * (camera.transform.position.x - cameraStartPos.x);
        }else{
            position += multiplier * (camera.transform.position - cameraStartPos);
        }

        transform.position = position;

        if(isInfinite){
            float tmp = camera.transform.position.x * (1-multiplier);
            if(tmp > startPosition.x + length){
                startPosition.x += length;
            }else if(tmp < startPosition.x - length){
                startPosition.x -= length;
            }
        }

        Vector3 cameraY = transform.position;

        // Set the y position of the object to the y position of the camera
        cameraY.y = camera.transform.position.y;

        // Apply the new position back to the object
        transform.position = cameraY;
    }
    

}