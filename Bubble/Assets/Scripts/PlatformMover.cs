using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using Random = UnityEngine.Random;

public class PlatformMover : MonoBehaviour, IEffectable
{
    [SerializeField] private Platform _platform;
    [SerializeField] private Platform _startPlatform;
    [SerializeField] private Transform _container;
    [SerializeField] private Transform _revealPlatform;
    [SerializeField] private int _alivePlatforms = 20;
    [SerializeField] private int _minAlivePlatforms = 10;
    [SerializeField] private float _moveRate = 1f;
    [SerializeField] private float _distance = 1f;
    [SerializeField] private Vector2 _maxMinHeight;
    [Header("Incremental Settings")]
    [SerializeField] private float _speedIncrement = 0.5f;
    [SerializeField] private float _intervals = 3;
    public static float PlatformHideLocation { get; private set; }
    private float _position;
    private float _timer;
    private int _platformCount;
    private Platform _startPlatformInstance;
    
    private List<Platform> _platforms = new();

    ObjectPool<Platform> _platformPool;
    private PlayerController _playerMove;

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(new Vector3(_container.position.x, _maxMinHeight.x + _maxMinHeight.y / 2, 0), new Vector3(1, _maxMinHeight.y - _maxMinHeight.x, 0 ));
        
    }

    private void Awake()
    {
        EffectsManager.Subscribe(GameModifierType.BreakablePlatforms, this);
        EffectsManager.Subscribe(GameModifierType.ShortPlatforms, this);
        PlatformHideLocation = _maxMinHeight.x - 2;
        _platformPool = new ObjectPool<Platform>(() => Instantiate(_platform, _container),
            t => t.gameObject.SetActive(true),
            t => t.gameObject.SetActive(false));
        GenerateMap();
    }
    private void Update()
    {
        MovePlatform();
        MovePlayerOnPlatforms();
        LevelProgress();
    }

    private void LevelProgress()
    {
        _timer += Time.deltaTime;
        if (_timer < _intervals) 
            return;
        _moveRate += _speedIncrement;
        _timer = 0;
    }

    private void MovePlayerOnPlatforms()
    {
        if(_playerMove is {IsGrounded: true})
            _playerMove.Rigidbody.position += new Vector2(-_moveRate * Time.deltaTime, 0);
    }

    private void GenerateMap()
    {
        Platform.OnReachEdge = ReleasePlatform;
        Platform.OnReachStart = ApplyEffectOnPlatform;
        Platform.OnPlayerGround = ApplyPlayerScrollMovement;
        _position = _distance;
        _startPlatformInstance = Instantiate(_startPlatform, _container);
        _startPlatformInstance.Set(new PlatformInitArgs(){Position = Vector3.zero, ShowOnStart = true});
        GenerateNext();
    }

    private void ApplyEffectOnPlatform(Platform platform)
    {
        
    }

    private void ApplyPlayerScrollMovement(PlayerController player)
    {
        _playerMove = player;
    }

    private void GenerateNext()
    {
        for (int i = _platformCount; i < _alivePlatforms; i++)
        {
            var platform = _platformPool.Get();
            _position += _distance;
            
            var pos = Vector3.right * _position;
            pos.y = Random.Range(_maxMinHeight.x, _maxMinHeight.y);
            platform.Set(new() {Position = pos, ShowOnStart = _revealPlatform.localPosition.x > _position});
            _platforms.Add(platform);
            _platformCount++;
        }
    }

    private void ReleasePlatform(Platform platform)
    {
        if (_startPlatformInstance == platform)
        {
            _startPlatformInstance.gameObject.SetActive(false);
            return;
        }
        _platformPool.Release(platform);
        _platforms.Remove(platform);
        _platformCount--;
        if (_platformCount <= _minAlivePlatforms)
            GenerateNext();
    }

    private void MovePlatform()
    {
        _container.position += new Vector3(-_moveRate * Time.deltaTime, 0, 0);
    }

    public void ApplyEffect(GameModifierType type)
    {
        switch (type)
        {
            case GameModifierType.BreakablePlatforms:
                ApplyBreakablePlatform(5); break;
            case GameModifierType.ShortPlatforms:
                ApplyShortPlatforms(5); break;
        }
    }

    private void ApplyShortPlatforms(float duration)
    {
        foreach (var platform in _platforms)
            platform.Settings.Short = true;
        Utils.RunTimer(duration, () =>
        {
            foreach (var platform in _platforms)
                platform.Settings.Short = false;
        }).Forget();
    }

    private void ApplyBreakablePlatform(float duration)
    {
        foreach (var platform in _platforms)
            platform.Settings.Breakable = true;
        Utils.RunTimer(duration, () =>
        {
            foreach (var platform in _platforms)
                platform.Settings.Breakable = false;
        }).Forget();
    }
}
