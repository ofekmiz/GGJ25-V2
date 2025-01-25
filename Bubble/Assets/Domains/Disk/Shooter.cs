using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class Shooter : MonoBehaviour
{
    [SerializeField] private float startShootInterval = 8;
    [SerializeField] private float endShootInterval = 1;
    [SerializeField] private float gameTime = 60;

    [SerializeField] private GoodBullet _goodBulletPrefab;
    [SerializeField] private DestroyingBullet _destroyingBulletPrefab;
    [SerializeField] private Transform _spawnPoint;

    [SerializeField] private ProgressBarController _goodShotProgress;
    [SerializeField] private ProgressBarController _destroyingShotProgress;

    private float _playTime;
    private float _timeToNextShot;
    private float _currentShotInterval;
    
    void Awake()
    {
        _timeToNextShot = startShootInterval;
        _currentShotInterval = startShootInterval;
    }

    void Update()
    {
        _playTime += Time.deltaTime;

        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            shootDestroyingBullet();
            return;
        }
        
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            shootGoodBullet();
            return;
        }
        
        _timeToNextShot -= Time.deltaTime;
        if (_timeToNextShot <= 0)
            shootGoodBullet();
        
        _goodShotProgress.SetProgress(1 - _timeToNextShot / _currentShotInterval);
    }

    private void shootGoodBullet()
    {
        //AudioManager.Instance.PlayGameAudio("CollectingShoot");
        _currentShotInterval = Mathf.Lerp(startShootInterval, endShootInterval, _playTime / gameTime);
        _timeToNextShot = _currentShotInterval;

        GoodBullet goodBullet = Instantiate(_goodBulletPrefab);
        goodBullet.transform.position = _spawnPoint.position;
    }
    
    private void shootDestroyingBullet()
    {
        //AudioManager.Instance.PlayGameAudio("DestroyingShoot");
        DestroyingBullet destroyingBullet = Instantiate(_destroyingBulletPrefab);
        destroyingBullet.transform.position = _spawnPoint.position;
    }
}
