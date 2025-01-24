using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class WobbleBobble : MonoBehaviour
{
    [SerializeField] private Rigidbody2D _rb;

    private Vector2 _prevVelocity;

    private Vector2 _wobbleAcceleration;
    private Vector2 _wobbleSpeed;
    private Vector2 _wobbleAmount = Vector2.one;

    private const float extenralWobbleFactor = 10;
    private const float maxWobble = 0.2f;
    private const float maxWobbleFactor = 100f;
    private const float wobbleDamping = 140f;

    private void Awake()
    {
        //wobbleFactor = Random.Range(maxWobbleFactor / 2, maxWobbleFactor);
        //_wobbleAmount = new Vector2(1 - maxWobble, 1 + maxWobble);
    }

    private void Update()
    {
        float dt = Time.deltaTime;
        Vector2 velocity = _rb.velocity;
        
        Vector2 acceleration = (velocity - _prevVelocity) / dt;
        
        _wobbleAcceleration = acceleration * dt * extenralWobbleFactor; //from outside forces
        _wobbleAcceleration += new Vector2(-Mathf.Pow((_wobbleAmount.x - 1)/maxWobble, 5), -Mathf.Pow((_wobbleAmount.y - 1)/maxWobble, 5)) * maxWobbleFactor; //from internal wobble
        _wobbleAcceleration -= _wobbleSpeed * wobbleDamping * Time.deltaTime; //damping by wobble speed
        
        _wobbleSpeed += _wobbleAcceleration * dt;
        _wobbleAmount += _wobbleSpeed * dt;
        
        if (_wobbleAmount.x > 1 + maxWobble)
            _wobbleAmount.x = 1 + maxWobble;
        if (_wobbleAmount.x < 1 - maxWobble)
            _wobbleAmount.x = 1 - maxWobble;
        if (_wobbleAmount.y > 1 + maxWobble)
            _wobbleAmount.y = 1 + maxWobble;
        if (_wobbleAmount.y < 1 - maxWobble)
            _wobbleAmount.y = 1 - maxWobble;

        transform.localScale = new Vector3(_wobbleAmount.x, _wobbleAmount.y, transform.localScale.z);
        
        _prevVelocity = velocity;
    }
}
