using com.ootii.Messages;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Saucer : MonoBehaviour
{
    public float screenWidth;
    public float moveSpeed = 3;

    private void Start() {

        // Calculate the screen width in world coordinates
        float distanceToCamera = transform.position.z - Camera.main.transform.position.z;
        screenWidth = Camera.main.ViewportToWorldPoint(new Vector3(1, 0, distanceToCamera)).x;

    }

    /// <summary>
    /// Move across field from right to left
    /// </summary>
    public void MoveAcrossField() {



    }

    private void Update() {
        // Move the GameObject to the left
        transform.Translate(Vector3.left * moveSpeed * Time.deltaTime);

        // If the GameObject goes beyond the left edge, reset its position to the right edge
        if (transform.position.x < -screenWidth) {
            float newX = screenWidth * 2f;
            transform.position = new Vector3(newX, transform.position.y, transform.position.z);
        }
    }
}
