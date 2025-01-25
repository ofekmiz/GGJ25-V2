using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shooter : MonoBehaviour
{
    [SerializeField] private float startShootInterval = 8;
    [SerializeField] private float endShootInterval = 1;
    [SerializeField] private float gameTime = 60;

    [SerializeField] private Bullet _bulletPrefab;
    [SerializeField] private Transform _spawnPoint;
    
    

    private float _playTime;
    private float _timeToNextShot;
    
    void Awake()
    {
        _timeToNextShot = startShootInterval;
    }

    void Update()
    {
        _playTime += Time.deltaTime;

        if (Input.GetKeyDown(KeyCode.W))
        {
            shoot();
            return;
        }
        
        _timeToNextShot -= Time.deltaTime;
        if (_timeToNextShot <= 0)
            shoot();
    }

    private void shoot()
    {
        float currentShootInterval = Mathf.Lerp(startShootInterval, endShootInterval, _playTime / gameTime);
        _timeToNextShot = currentShootInterval;

        Bullet bullet = Instantiate(_bulletPrefab);
        bullet.transform.position = _spawnPoint.position;
    }
}
