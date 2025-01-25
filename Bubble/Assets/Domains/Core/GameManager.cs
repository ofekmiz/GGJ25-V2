using System;
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

        [SerializeField] private PlayerController _playerController;
        [SerializeField] private TMP_Text _timer;

        [SerializeField] private BubbleSpawner _bubbleSpawner;
        [SerializeField] private GameModifiersManager _gameModifiersManager;
        
        private Dependencies _dependencies;
        private float _timeInterval = 0.5f;
        public int GameTimer { get; private set; }


        public IBubbleFactory BubbleFactory => _dependencies.BubbleFactory;
        public BubblesManager BubblesManager => _dependencies.BubblesManager;

        public static GameManager Instance; //Sorry not sorry


        private void Awake()
        {
            Instance = this;
            _bubbleSpawner.Init(_gameModifiersManager); //TODO::move to init later
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

        public void GameModifierCollected(GameModifierType modifierType)
        {
            _playerController.GameModifierCollected(modifierType);
        }

        private void Start()
        {
            StartReport(TimeSpan.FromSeconds(5), destroyCancellationToken).Forget(e => Debug.LogException(e, this));
            RunTimer().Forget();
        }

        private async UniTaskVoid RunTimer()
        {
            GameTimer = 0;
            var counter = 0f;
            while (true)
            {
                counter += Time.deltaTime;
                await UniTask.NextFrame();
                if (!(counter >= _timeInterval)) 
                    continue;
                GameTimer++;
                counter = 0;
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
