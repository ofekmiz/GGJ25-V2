using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;

public class BubbleSpawner : MonoBehaviour
{
    [SerializeField]
    private BubblePhysics _bubblePrefab;

    [SerializeField]
    private SpriteRenderer _spawnArea;

    [SerializeField]
    private List<Transform> _spawnPoints;

    private List<BubblePhysics> _bubbles = new();

    [SerializeField]
    private int _startingBubbles = 100;

    [SerializeField]
    private int _delayBetweenBubbles = 500;

    [SerializeField]
    private Transform _center;

    [SerializeField]
    private float _radius;

    public static BubbleSpawner Instance { get; private set; }

    private GameModifiersManager _gameModifiersManager;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
    }

    void Start()
    {
        for (int i = 0; i < _startingBubbles; i++)
        {
            SpawnRandom();
            UniTask.Delay(_delayBetweenBubbles);
        }
    }

    public void Init(GameModifiersManager gameModifiersManager)
    {
        _gameModifiersManager = gameModifiersManager;
    }

    private void StartSpawn()
    {

    }

    private void SpawnRandom()
    {
        Bounds spawnBounds = _spawnArea.bounds;
        var angle = Random.Range(0, Mathf.PI * 2);
        var radMin = _radius;
        var radMax = _radius - 2f;
        var radius = Random.Range(radMin, radMax);
        var pointToSpawn = _center.position + new Vector3(Mathf.Cos(angle) * radius, Mathf.Sin(angle) * radius);
        //Vector2 spawnPoint = new Vector2(Random.Range(spawnBounds.min.x, spawnBounds.max.x), Random.Range(spawnBounds.min.y, spawnBounds.max.y)); 
        Spawn(pointToSpawn);
    }

    private void Spawn(Vector2 pos)
    {
        var bubble = Instantiate(_bubblePrefab, this.transform);
        ModifierBubble modifierBubble = bubble.GetComponent<ModifierBubble>();
        modifierBubble.Set(_gameModifiersManager.GetRandomModifier());
        bubble.SetCenter(_center, _radius);
        _bubbles.Add(bubble);
        bubble.transform.position = pos;
    }

    public SpriteRenderer SpawningArea => _spawnArea;
}
