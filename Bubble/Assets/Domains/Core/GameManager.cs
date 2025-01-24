using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Domains.Bubbles.Factories;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace Domains.Core
{
    public class GameManager : MonoBehaviour
    {
        [Header("Bubble Pool")]
        [SerializeField] BubbleFactory _bubbleFactory;
        [SerializeField] private Transform _bubblePoolParent;

        [SerializeField] private PlayerController _playerManager;

        // private List<PlatformerItem> _platformItems;
        // private EffectsManager _effectsManager;

        public IBubbleFactory BubbleFactory => _dependencies.BubbleFactory;

        private Dependencies _dependencies;


        private void Awake()
        {
            _dependencies.BubbleFactory = _bubbleFactory;

            _bubbleFactory.Init(this, _bubblePoolParent);

            // _platformItems = new List<PlatformerItem>();
            //
            // _effectsManager = new EffectsManager(_dependencies);
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
