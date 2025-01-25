using System.Collections.Generic;
using DG.Tweening;
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

    [Header("Modifiers Settings")] 
    [SerializeField] private Enemy _enemyPrefab;
    [SerializeField] private Vector2 _enemyMaxMinHeight;
    [SerializeField] private float _shortState = 0.5f;
    [SerializeField] private Trampoline _trampolinePrefab;
    [SerializeField] private float _spawnDistance = 20f;
    
    [SerializeField] private PlayerController _playerController;
    
    public static float PlatformHideLocation { get; private set; }
    private float _position;
    private float _timer;
    private float _movingSpeed;
    private int _platformCount;
    private Platform _startPlatformInstance;
    
    private List<Platform> _platforms = new();

    ObjectPool<Platform> _platformPool;
    private PlayerController _playerMove;

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(new Vector3(_container.position.x, _maxMinHeight.x + _maxMinHeight.y / 2, 0), new Vector3(1, _maxMinHeight.y - _maxMinHeight.x, 0 ));
        Gizmos.DrawWireCube(new Vector3(_revealPlatform.position.x, _enemyMaxMinHeight.x + _enemyMaxMinHeight.y / 2, 0), new Vector3(1, _enemyMaxMinHeight.y - _enemyMaxMinHeight.x, 0 ));
    }

    private void Awake()
    {
        EffectsManager.Subscribe(GameModifierType.BreakablePlatforms, this);
        EffectsManager.Subscribe(GameModifierType.ShortPlatforms, this);
        EffectsManager.Subscribe(GameModifierType.LongPlatforms, this);
        EffectsManager.Subscribe(GameModifierType.Enemy, this);
        EffectsManager.Subscribe(GameModifierType.Trampoline, this);
        
        PlatformHideLocation = _maxMinHeight.x - 2;
        _platformPool = new ObjectPool<Platform>(() => Instantiate(_platform, _container),
            t => t.gameObject.SetActive(true),
            t => t.gameObject.SetActive(false));
        GenerateMap();
        _movingSpeed = _moveRate;
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
        _movingSpeed += _speedIncrement;
        _timer = 0;
    }

    private void MovePlayerOnPlatforms()
    {
        if(_playerMove is {IsGrounded: true})
            _playerMove.Rigidbody.position += new Vector2(-_movingSpeed * Time.deltaTime, 0);
    }

    private void GenerateMap()
    {
        Platform.OnReachEdge = ReleasePlatform;
        Platform.OnReachStart = ApplyEffectOnPlatform;
        Platform.OnPlayerGround = ApplyPlayerScrollMovement;
        _position = _distance;
        _startPlatformInstance = Instantiate(_startPlatform, _container);
        _startPlatformInstance.Set(new PlatformInitArgs(){Position = Vector3.zero, ShowOnStart = true, ShortValue = 1f});
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
            platform.Set(new() {Position = pos, ShowOnStart = _revealPlatform.localPosition.x > _position, ShortValue = _shortState});
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
        _container.position += new Vector3(-_movingSpeed * Time.deltaTime, 0, 0);
    }

    public void ApplyEffect(GameModifier modifier)
    {
        switch (modifier.Type)
        {
            case GameModifierType.BreakablePlatforms: ApplyBreakablePlatform(5); break;
            case GameModifierType.ShortPlatforms: ApplyModifierPlatforms(5, PlatformState.Short); break;
            case GameModifierType.LongPlatforms: ApplyModifierPlatforms(5, PlatformState.Long); break;
            case GameModifierType.Slow: SlowSpeed(); break;
            case GameModifierType.Enemy: GenerateEnemy(modifier.Prefab); break;
            case GameModifierType.Trampoline: GenerateTrampoline(modifier.Prefab); break;
        }
    }

    private void SlowSpeed()
    {
        var endValue =Mathf.Max(_moveRate, _movingSpeed - (_speedIncrement * 3)); 
        DOTween.To(() => _movingSpeed, value => _movingSpeed = value, endValue, 1);
    }

    private void GenerateEnemy(GameObject enemyVisual)
    {
        //get pos
        var pos = _revealPlatform.position;
        pos.x += 2;
        pos.y = Random.Range(_maxMinHeight.x, _maxMinHeight.y);
        var enemy = Instantiate(_enemyPrefab, _container);
        enemy.transform.position = pos;
        enemy.SetView(enemyVisual);
    }

    private void GenerateTrampoline(GameObject trampolineVisual)
    {
        var targetPlatform = getPlatformNearPlayer();
        var trampoline = Instantiate(_trampolinePrefab);
        trampoline.transform.SetParent(targetPlatform.transform);
        trampoline.transform.position = targetPlatform.TopPoint.position;
    }

    private Platform getPlatformNearPlayer()
    {
        float playerX = _playerController.transform.position.x;

        Platform nearestPlatform = _platforms[^1];
        
        for (int i = 0; i < _platforms.Count; i++)
        {
            float platformX = _platforms[i].transform.position.x;
            if (platformX > playerX + _spawnDistance && Mathf.Abs(platformX-playerX) < Mathf.Abs(nearestPlatform.transform.position.x - playerX))
            {
                nearestPlatform = _platforms[i];
            }
        }

        return nearestPlatform;
    }

    private void ApplyModifierPlatforms(float duration, PlatformState platformState)
    {
        foreach (var platform in _platforms)
            platform.Settings.State = platformState;
        Utils.RunTimer(duration, () =>
        {
            foreach (var platform in _platforms)
                platform.Settings.State = PlatformState.None;
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
