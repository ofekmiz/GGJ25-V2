using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotationController : MonoBehaviour
{
    [SerializeField] private float _speed = 1.0f;

    private void Update()
    {
        transform.Rotate(new Vector3(0, 0, Input.GetAxis("Horizontal2") * _speed));
    }
}
