using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

[RequireComponent(typeof(CircleCollider2D))]
public class PlayerController : MonoBehaviour , IEffectable
{
    [SerializeField] private float _moveSpeed = 5f;
    [SerializeField] private float _jumpForce = 5f;
    [SerializeField] private float _trampolineJumpForce = 25f;
    [SerializeField] private Rigidbody2D _rigidbody;
    [SerializeField] private float _groundCheckRadius;
    [SerializeField] private LayerMask _groundLayer;
    [SerializeField] private Transform _groundCheck;
    [SerializeField] private float _fallIncrement = 10;
    [SerializeField] private List<Utils.SpawnPoint> _spawnPoints;
    
    public static Action OnPlayerDeath;
    private bool _alive = true;
    public static Action<EffectArgs> OnPlayerCollectEffect;
    private const string Horizontal = "Horizontal";
    
    private PlayerSettings _playerSettings; 
    public bool IsGrounded {get; private set;}
    public Rigidbody2D Rigidbody => _rigidbody;
    private float _moveDirection;
    private float _jumpDirection;
    private GameObject _shield;

    private struct PlayerSettings
    {
        public float MoveSpeed;
        public float JumpForce;
        public float FallIncrement;
        public bool HasShield;
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
        EffectsManager.Subscribe(GameModifierType.Goggles, this);
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
            AudioManager.Instance.PlayGameAudio("SheepJump");
            _rigidbody.velocity = Vector2.up * _playerSettings.JumpForce;
            _jumpDirection = 0;
        }
        Fall();
    }

    private void Fall()
    {
        if(IsGrounded)
            return;
        _rigidbody.velocity -= Vector2.up * (_playerSettings.FallIncrement * Time.fixedDeltaTime);
    }

    private void CheckOnGround()
    {
        IsGrounded = Physics2D.OverlapCircle(_groundCheck.position, _groundCheckRadius, _groundLayer);
    }

    private void Jump()
    {
        if (IsGrounded && (Input.GetButtonDown("Jump") || Input.GetKeyDown(KeyCode.W)))
        {
            _jumpDirection = _jumpForce;
        }
        
        if (Input.GetButtonUp("Jump") || Input.GetKeyUp(KeyCode.W))
        {
            if (_rigidbody.velocity.y > 0)
                _rigidbody.velocity = new Vector2(_rigidbody.velocity.x, _rigidbody.velocity.y / 2f);
        }
    }

    private void MoveControl()
    {
        if (!_alive) return;
        _moveDirection = Input.GetAxisRaw(Horizontal);
        transform.rotation = _moveDirection >= 0 ? Quaternion.Euler(0f, 0f, 0f) : Quaternion.Euler(0f, 180f, 0f);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Hazard"))
        {
            OnPlayerDeath?.Invoke();
            FlipPlayerDead();
        }
        if (other.CompareTag("Enemy"))
        {
            if (_playerSettings.HasShield)
            {
                Destroy(other.gameObject);
                Destroy(_shield);
                return;
            }
            OnPlayerDeath?.Invoke();
            FlipPlayerDead();
        }

        else if (other.CompareTag("Effect"))
        {
            var effectItem = other.GetComponent<EffectItem>();
            OnPlayerCollectEffect?.Invoke(effectItem.EffectArgs);
            Destroy(other.gameObject);
        }
    }

    private void FlipPlayerDead()
    {
        transform.localScale = new Vector3(transform.localScale.x, -transform.localScale.y);
        GetComponent<Collider2D>().enabled = false;
        _alive = false;
    }

    public void ApplyEffect(GameModifier modifier)
    {
        switch (modifier.Type)
        {
            case GameModifierType.Jump: ApplyJumpModifier(3); break; 
            case GameModifierType.JetPack: ApplyJetPack(3); break;
            case GameModifierType.Shield: AddShied(modifier); return;
        }

        Spawn(modifier);
    }

    private void AddShied(GameModifier modifier)
    {
        _playerSettings.HasShield = true;
        _shield = Spawn(modifier);
    }

    private GameObject Spawn(GameModifier modifier)
    {
        if(!modifier.Prefab)
            return null;
        var spawnPointIndex = _spawnPoints.FindIndex(p => p.Key == modifier.Type);
        if(spawnPointIndex < 0)
            return null;
        var spawnPoint = _spawnPoints[spawnPointIndex];
        return Instantiate(modifier.Prefab, spawnPoint.Parent);
    }

    private void ApplyJetPack(float duration)
    {
        Debug.Log("Player got Jet Pack!");
        _playerSettings.FallIncrement = 0;
        Utils.RunTimer(duration, () => _playerSettings.FallIncrement = _fallIncrement).Forget();
    }
    

    private void ApplyJumpModifier(float duration)
    {
        Debug.Log("Player got Jump Modifier!");
        _playerSettings.JumpForce += 2;
        Utils.RunTimer(duration, () => _playerSettings.JumpForce -= 2f).Forget();
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Trampoline"))
            _rigidbody.velocity = Vector2.up * _trampolineJumpForce;
    }
}
