using UnityEngine;
using UnityEngine.Pool;
using Random = UnityEngine.Random;

public class PlatformMover : MonoBehaviour
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
    public static float PlatformHideLocation { get; private set; }
    private float _position;
    private int _platformCount;
    private Platform _startPlatformInstance;

    ObjectPool<Platform> _platformPool;

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(new Vector3(_container.position.x, _maxMinHeight.x + _maxMinHeight.y / 2, 0), new Vector3(1, _maxMinHeight.y - _maxMinHeight.x, 0 ));
        
    }

    private void Awake()
    {
        PlatformHideLocation = _maxMinHeight.x - 2;
        _platformPool = new ObjectPool<Platform>(() => Instantiate(_platform, _container),
            t => t.gameObject.SetActive(true),
            t => t.gameObject.SetActive(false));
        GenerateMap();
    }
    private void Update()
    {
        MovePlatform();
    }

    private void GenerateMap()
    {
        Platform.OnReachEdge = ReleasePlatform;
        _position = _distance;
        _startPlatformInstance = Instantiate(_startPlatform, _container);
        _startPlatformInstance.Set(new PlatformInitArgs(){Position = Vector3.zero});
        GenerateNext();
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
        _platformCount--;
        if (_platformCount <= _minAlivePlatforms)
            GenerateNext();
    }

    private void MovePlatform()
    {
        _container.position += new Vector3(-_moveRate * Time.deltaTime, 0, 0);
    }
}
