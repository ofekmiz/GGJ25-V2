using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class PlatformMover : MonoBehaviour
{
    private Transform _trn;
    private float _moveRate = 1f;



    private void Awake()
    {
        _trn = transform;

        GenerateMap();
        

    }
    private void FixedUpdate()
    {


        MovePlatform();
        StrechTail(); // generate the continuation of the map

    }

    private void StrechTail()
    {
    }

    private void GenerateMap()
    {
    }

    private void MovePlatform()
    {
        _trn.position += new Vector3(_moveRate, 0, 0);
    }
}
