using System;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using Domains.Bubbles.BubbleSpawner;
using Domains.Bubbles.Factories;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace Domains.Core
{
    public class GameManager : MonoBehaviour
    {
        [Header("Bubble Pool")]
        [SerializeField] private BubbleFactory _bubbleFactory;
        [SerializeField] private BubblesManager _bubblesManager;
        [SerializeField] private Transform _bubblePoolParent;

        [SerializeField] private GameObject _scroller;
        [SerializeField] private Vector3 _scrollerStartPos;

        //[SerializeField] private PlayerController _playerController;
        [SerializeField] private TMP_Text _timer;

        [SerializeField] private BubbleSpawner _bubbleSpawner;
        [SerializeField] private GameModifiersManager _gameModifiersManager;
        [SerializeField] private GameOverScreen _gameOverScreen;
        [SerializeField] private GameObject _startMenu;
        
        [SerializeField] private AudioManager _audioManager;


        private bool _isGameOver = false;
        
        private Dependencies _dependencies;
        private float _timeInterval = 0.5f;
        private float _counter = 0f;
        public int GameTimer { get; private set; }
        private EffectsManager _effectsManager;


        public IBubbleFactory BubbleFactory => _dependencies.BubbleFactory;
        public BubblesManager BubblesManager => _dependencies.BubblesManager;

        public static GameManager Instance; //Sorry not sorry // D:


        private void Awake()
        {
            _effectsManager = new();
            Instance = this;
            
            //TODO::move to init later
            _bubbleSpawner.Init(_gameModifiersManager);
            _bubbleSpawner.BeginsSpawn();
            _audioManager.Init();
            _audioManager.PlayBgAudio();

            PlayerController.OnPlayerDeath += OnGameOver;

            _scrollerStartPos = _scroller.transform.position;
        }

        public void Init()
        {
            _dependencies = new()
            {
                GameManager = this,
                BubbleFactory = _bubbleFactory,
                BubblesManager = _bubblesManager,
            };

            _bubbleFactory.Init(this, _bubblePoolParent);
            _bubblesManager.Init(this, BubbleFactory);
        }

        private void OnDisable()
        {
            PlayerController.OnPlayerDeath -= OnGameOver;
        }
        private void OnGameOver()
        {
            _isGameOver = true;
            _gameOverScreen.gameObject.SetActive(true);
            _gameOverScreen.SetScore(int.Parse(_timer.text));
            _bubbleSpawner.DestoryAllBubbles();
			_audioManager.PlayGameAudio("Loose");
        }

        public void GameModifierCollected(GameModifier modifierType)
        {
            _effectsManager.PlayEffect(modifierType);
        }

        public void RestartGame()
        {
            SceneManager.LoadScene("GameScene");
            //_startMenu?.gameObject.SetActive(false);
            //_isGameOver = false;
            //_counter = 0f;
            //_bubbleSpawner.BeginsSpawn();
            //_gameOverScreen.gameObject.SetActive(false);
            //_scroller.transform.position = _scrollerStartPos;
        }

        private void Start()
        {
            StartReport(TimeSpan.FromSeconds(5), destroyCancellationToken).Forget(e => Debug.LogException(e, this));
            RunTimer().Forget();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                _effectsManager.PlayEffect(new()
                {
                    Type = GameModifierType.BreakablePlatforms
                });
            }

            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                _effectsManager.PlayEffect(new()
                {
                    Type = GameModifierType.ShortPlatforms
                });
            }
            
            if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                _effectsManager.PlayEffect(_gameModifiersManager.GameModifiers
                    .Where(m => m.Type == GameModifierType.JetPack)
                    .GetRandom());
            }
            
            if (Input.GetKeyDown(KeyCode.Alpha4))
            {
                _effectsManager.PlayEffect(_gameModifiersManager.GameModifiers
                    .Where(m => m.Type == GameModifierType.Jump)
                    .GetRandom());
            }
            
            if (Input.GetKeyDown(KeyCode.Alpha5))
            {
                _effectsManager.PlayEffect(_gameModifiersManager.GameModifiers
                    .Where(m => m.Type == GameModifierType.Enemy)
                    .GetRandom());
            }
            
            if (Input.GetKeyDown(KeyCode.Alpha6))
            {
                _effectsManager.PlayEffect(new()
                {
                    Type = GameModifierType.Slow
                });
            }
            
            if (Input.GetKeyDown(KeyCode.Alpha7))
            {
                _effectsManager.PlayEffect(_gameModifiersManager.GameModifiers
                    .Where(m => m.Type == GameModifierType.Shield)
                    .GetRandom());
            }
            
            if (Input.GetKeyDown(KeyCode.Alpha8))
            {
                _effectsManager.PlayEffect(new()
                {
                    Type = GameModifierType.Random
                });
            }
        }

        private async UniTaskVoid RunTimer()
        {
            GameTimer = 0;
            _counter = 0f;
            while (!_isGameOver) // add death cond
            {
                _counter += Time.deltaTime;
                await UniTask.NextFrame();
                if (!(_counter >= _timeInterval)) 
                    continue;
                GameTimer++;
                _counter = 0;
                _timer.text = GameTimer.ToString("D6");
            }
        }

        private async UniTask StartReport(TimeSpan interval, CancellationToken ct)
        {
            while (!ct.IsCancellationRequested)
            {
                _bubbleFactory.ReportStatus();
                await UniTask.Delay(interval, cancellationToken: ct).SuppressCancellationThrow();
            }
        }

        public T Spawn<T>(T original, Vector3 position, Quaternion rotation) where T : Object, IConstructed
            => TryConstruct(Instantiate(original, position, rotation));

        public T Spawn<T>(T original, Vector3 position, Quaternion rotation, Transform parent) where T : Object, IConstructed
            => TryConstruct(Instantiate(original, position, rotation, parent));

        public T Spawn<T>(T original) where T : Object, IConstructed
            => TryConstruct(Instantiate(original));

        public T Spawn<T>(T original, Scene scene) where T : Object, IConstructed
            => TryConstruct((T)Instantiate(original, scene));

        public T Spawn<T>(T original, Transform parent) where T : Object, IConstructed
            => TryConstruct(Instantiate(original, parent));

        public T Spawn<T>(T original, Transform parent, bool worldPositionStays) where T : Object, IConstructed
            => TryConstruct(Instantiate(original, parent, worldPositionStays));

        private T TryConstruct<T>(T instance) where T : Object, IConstructed
        {
            if (instance)
                instance.Construct(in _dependencies);

            return instance;
        }
    }
}
