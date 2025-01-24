using Domains.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CircleCollider2D))]
public class PlayerController : MonoBehaviour
{
    [SerializeField] private float _moveSpeed = 5f;
    [SerializeField] private float _jumpForce = 5f;
    [SerializeField] private Rigidbody2D _rigidbody;
    [SerializeField] private float _groundCheckRadius;
    [SerializeField] private LayerMask _groundLayer;
    [SerializeField] private Transform _groundCheck;
    [SerializeField] private float _fallIncreameant;
    
    public static Action OnPlayerDeath;
    private const string Horizontal = "Horizontal";
    
    private EffectsManager _effects;
    private bool _isGrounded;
    private float _moveDirection;
    private float _jumpDirection;

    private void OnDrawGizmos()
    {
        if(_groundCheck)
            Gizmos.DrawWireSphere(_groundCheck.position, _groundCheckRadius);
    }

    public void Update()
    {
        CheckOnGround();
        MoveControl();
        Jump();
    }

    private void FixedUpdate()
    {
        Move();
    }

    private void Move()
    {
        _rigidbody.position += new Vector2(_moveDirection * _moveSpeed * Time.fixedDeltaTime, 0f);
        if (_jumpDirection > 0)
        {
            _rigidbody.velocity = Vector2.up * _jumpForce;
            _jumpDirection = 0;
        }
        Fall();
    }

    private void Fall()
    {
        if(_isGrounded)
            return;
        _rigidbody.velocity -= Vector2.up * (_fallIncreameant * Time.fixedDeltaTime);
    }

    private void CheckOnGround()
    {
        _isGrounded = Physics2D.OverlapCircle(_groundCheck.position, _groundCheckRadius, _groundLayer);
    }

    private void Jump()
    {
        if (_isGrounded && Input.GetButtonDown("Jump"))
        {
            _jumpDirection = _jumpForce;
        }
    }

    private void MoveControl()
    {
        _moveDirection = Input.GetAxis(Horizontal);
    }
    
    // private void OnCollisionEnter2D(Collision2D collision)
    // {
    //     if (collision.gameObject.CompareTag("Hazard"))
    //     {
    //         OnPlayerDeath?.Invoke();
    //     }
    //     else if (collision.gameObject.CompareTag("Effect"))
    //     {
    //         Effect effectToTake = new Effect();
    //     }
    //     else if (collision.gameObject.CompareTag("Blocker"))
    //     {
    //         // TODO
    //     }
    // }
}
