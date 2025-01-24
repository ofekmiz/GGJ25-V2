using System.Collections.Generic;
using Domains.Bubbles.BubbleEntity;
using Domains.Core;
using Domains.Utils;
using UnityEngine;

namespace Domains.Bubbles.Factories
{
    public interface IBubbleFactory
    {
        Bubble GetBubble(Transform parent = null);
        void Recycle(Bubble bubble);
    }

    [CreateAssetMenu(fileName = "BubbleFactory", menuName = "Bubbles/Factory")]
    public class BubbleFactory : ScriptableObject, IBubbleFactory, IStatusReporter
    {
        [SerializeField] private Bubble _bubblePrefab;
        [SerializeField] private Transform _bubblePoolParent;


        private GameManager _gameManager;
        private readonly Stack<Bubble> _pool = new();


        public void Init(GameManager gameManager, Transform bubblePoolParent)
        {
            _gameManager = gameManager;
            _bubblePoolParent = bubblePoolParent;
        }


        public Bubble GetBubble(Transform parent = null)
        {
            if (!_pool.TryPop(out var bubble))
                return _gameManager.Spawn(_bubblePrefab, parent);

            if (parent != null)
                bubble.transform.SetParent(parent);

            return bubble;
        }

        public void Recycle(Bubble bubble)
        {
            _pool.Push(bubble);
            bubble.transform.SetParent(_bubblePoolParent);
        }

        public void ReportStatus()
        {
            Debug.Log($"Bubble factory status report: pool count: {_pool.Count}, prefab name: {_bubblePrefab.name}");
        }
    }
}
