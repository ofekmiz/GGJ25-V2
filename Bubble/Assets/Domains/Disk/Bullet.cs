using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private float _moveSpeed = 5;

    private void Update()
    {
        transform.position +=  _moveSpeed * Time.deltaTime * Vector3.up;

        if (transform.position.y > 10)
            die();
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        Destroy(other.gameObject);
        die();
    }

    private void die()
    {
        Destroy(gameObject);
    }
}
