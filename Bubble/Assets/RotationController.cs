using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotationController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 0.1f;
    [SerializeField] private float deceleration = 0.15f;
    private float currentSpeed = 0f;

    void Update()
    {
        float input = Input.GetAxis("Horizontal2");
        if (input != 0)
        {
            currentSpeed += input * moveSpeed * Time.deltaTime;
        }
        else
        {
            if (currentSpeed > 0)
                currentSpeed -= deceleration * Time.deltaTime;
            else if (currentSpeed < 0)
                currentSpeed += deceleration * Time.deltaTime;
        }

        transform.Rotate(Vector3.forward * currentSpeed);
    }
}