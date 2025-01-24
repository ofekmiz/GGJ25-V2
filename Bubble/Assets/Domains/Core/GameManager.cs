using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Domains.Bubbles.BubbleSpawner;
using Domains.Bubbles.Factories;
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

        [SerializeField] private PlayerController _playerManager;

        private Dependencies _dependencies;


        public IBubbleFactory BubbleFactory => _dependencies.BubbleFactory;
        public BubblesManager BubblesManager => _dependencies.BubblesManager;


        private void Awake()
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

        private void Start()
        {
            StartReport(TimeSpan.FromSeconds(5), destroyCancellationToken).Forget(e => Debug.LogException(e, this));
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
