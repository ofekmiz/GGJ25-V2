using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Domains.Bubbles.Factories;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Domains.Core
{
    public class GameManager : MonoBehaviour
    {
        [Header("Bubble Pool")]
        [SerializeField] private BubbleFactory _bubbleFactory;
        [SerializeField] private Transform _bubblePoolParent;


        public Dependencies Deps = new Dependencies();


        private void Awake()
        {
            _bubbleFactory.Init(this, _bubblePoolParent);
            Deps.BubbleFactory = _bubbleFactory;
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

        public T Spawn<T>(T prefab, Transform parent = null) where T : Object, IConstructed
        {
            var instance = parent ? Instantiate(prefab, parent) : Instantiate(prefab);

            if (instance)
                instance.Construct(Deps);

            return instance;
        }
    }
}
