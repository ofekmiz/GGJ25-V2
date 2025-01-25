using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Random = UnityEngine.Random;

[RequireComponent(typeof(CircleCollider2D))]
public class PlayerController : MonoBehaviour , IEffectable
{
    [SerializeField] private float _moveSpeed = 5f;
    [SerializeField] private float _jumpForce = 5f;
    [SerializeField] private Rigidbody2D _rigidbody;
    [SerializeField] private float _groundCheckRadius;
    [SerializeField] private LayerMask _groundLayer;
    [SerializeField] private Transform _groundCheck;
    [SerializeField] private float _fallIncrement = 10;
    
    public static Action OnPlayerDeath;
    public static Action<EffectArgs> OnPlayerCollectEffect;
    private const string Horizontal = "Horizontal";
    
    private PlayerSettings _playerSettings; 
    private bool _isGrounded;
    private float _moveDirection;
    private float _jumpDirection;
    
    private struct PlayerSettings
    {
        public float MoveSpeed;
        public float JumpForce;
        public float FallIncrement;
    }

    private void OnDrawGizmos()
    {
        if(_groundCheck)
            Gizmos.DrawWireSphere(_groundCheck.position, _groundCheckRadius);
    }

    private void Awake()
    {
        EffectsManager.Subscribe("jetpack", this);
        _playerSettings = new()
        {
            MoveSpeed = _moveSpeed,
            JumpForce = _jumpForce,
            FallIncrement = _fallIncrement
        };
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
        _rigidbody.position += new Vector2(_moveDirection * _playerSettings.MoveSpeed * Time.fixedDeltaTime, 0f);
        if (_jumpDirection > 0)
        {
            _rigidbody.velocity = Vector2.up * _playerSettings.JumpForce;
            _jumpDirection = 0;
        }
        Fall();
    }

    private void Fall()
    {
        if(_isGrounded)
            return;
        _rigidbody.velocity -= Vector2.up * (_playerSettings.FallIncrement * Time.fixedDeltaTime);
    }

    private void CheckOnGround()
    {
        _isGrounded = Physics2D.OverlapCircle(_groundCheck.position, _groundCheckRadius, _groundLayer);
    }

    private void Jump()
    {
        if (_isGrounded && (Input.GetButtonDown("Jump") || Input.GetKeyDown(KeyCode.W)))
        {
            _jumpDirection = _jumpForce;
        }
    }

    private void MoveControl()
    {
        _moveDirection = Input.GetAxisRaw(Horizontal);
        transform.rotation = _moveDirection >= 0 ? Quaternion.Euler(0f, 0f, 0f) : Quaternion.Euler(0f, 180f, 0f);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Hazard"))
        {
            OnPlayerDeath?.Invoke();
        }
        else if (other.CompareTag("Effect"))
        {
            var effectItem = other.GetComponent<EffectItem>();
            OnPlayerCollectEffect?.Invoke(effectItem.EffectArgs);
            Destroy(other.gameObject);
        }
    }
    
    public void ApplyEffect(EffectArgs args)
    {
        if (args is JetPackArgs jetPackArgs)
        {
            ApplyJetPack(jetPackArgs.Duration).Forget();
        }
    }

    private async UniTaskVoid ApplyJetPack(float duration)
    {
        while (duration > 0 )
        {
            _playerSettings.FallIncrement = 0;
            duration -= Time.deltaTime;
            await UniTask.NextFrame();
        }

        _playerSettings.FallIncrement = _fallIncrement;
    }

    public void DisableEffect()
    {
     
    }

    public void GameModifierCollected(GameModifierType modifierType)
    {
        if (modifierType == GameModifierType.Random)
            modifierType = (GameModifierType)Random.Range(0, Enum.GetValues(typeof(GameModifierType)).Length - 1);
        
        switch (modifierType)
        {
            case GameModifierType.Jump: applyJumpModifier(); break;
        }
    }

    private void applyJumpModifier()
    {
        _playerSettings.JumpForce += 2;
    }
}

public class JetPackArgs : EffectArgs
{
    public float Duration;
}
