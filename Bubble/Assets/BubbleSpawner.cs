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

    private List<BubblePhysics> _bubbles = new();

    private int _startingBubbles = 15;

    public static BubbleSpawner Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
    }

    void Start()
    {
        for (int i = 0; i < _startingBubbles; i++) {
            SpawnRandom();
        }
    }

    private void StartSpawn()
    {

    }

    private void SpawnRandom()
    {
        Bounds spawnBounds = _spawnArea.bounds;
        Vector2 spawnPoint = new Vector2(Random.Range(spawnBounds.min.x, spawnBounds.max.x), Random.Range(spawnBounds.min.y, spawnBounds.max.y)); 
        Spawn(spawnPoint);
    }

    private void Spawn(Vector2 pos)
    {
        var bubble = Instantiate(_bubblePrefab);
        _bubbles.Add(bubble);
        bubble.transform.position = pos;
    }

    public SpriteRenderer SpawningArea => _spawnArea;
    
    //private UniTask SpawnOne()
    //{

    //}
}
