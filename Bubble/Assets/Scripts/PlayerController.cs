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
        EffectsManager.Subscribe(GameModifierType.JetPack, this);
        EffectsManager.Subscribe(GameModifierType.Jump, this);
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
    
    public void ApplyEffect(GameModifierType modifierType)
    {
        switch (modifierType)
        {
            case GameModifierType.Jump: ApplyJumpModifier(3); break;
            case GameModifierType.JetPack: ApplyJetPack(3); break;
        }
        
    }

    private void ApplyJetPack(float duration)
    {
        _playerSettings.FallIncrement = 0;
        RunTimer(duration, () => _playerSettings.FallIncrement = _fallIncrement).Forget();
    }

    public void DisableEffect()
    {
     
    }

    private void ApplyJumpModifier(float duration)
    {
        _playerSettings.JumpForce += 2;
        RunTimer(duration, () => _playerSettings.JumpForce -= 2f).Forget();
    }

    private async UniTaskVoid RunTimer(float duration, Action onEnd)
    {
        if (duration == 0)
            return;
        
        while (duration > 0 )
        {
            duration -= Time.deltaTime;
            await UniTask.NextFrame();
        }
        onEnd?.Invoke();
    }
}
