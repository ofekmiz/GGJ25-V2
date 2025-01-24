using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Domains.Bubbles.BubbleEntity;
using Domains.Bubbles.Factories;
using Domains.Core;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Domains.Bubbles.BubbleSpawner
{
	public class BubbleSpawner : MonoBehaviour
	{
		[Header("Dependencies")]
		[SerializeField] private GameManager _gm;

		[Header("Refs")]
		[SerializeField] private Transform _bubblesParent;
		[SerializeField] private Transform _bubbleSpawnOrigin;

		[Header("Config")]
		[SerializeField] [Min(0)] private float _bubbleSpawnOriginOffsetRange;
		[SerializeField] [Min(0)] private float _spawnIntervalSeconds = 0.5f;
		[SerializeField] [Min(0.1f)] private float _bubbleFloatForce = 0.1f;
		[SerializeField] [Min(0)] private float _bubbleTravelDistance = 0.5f;
		[SerializeField] [Min(float.Epsilon)] private float _bubbleLifeSeconds = 4f;


		private readonly List<Bubble> _bubbles = new();


		private IBubbleFactory BubbleFactory => _gm.BubbleFactory;


		private void Awake()
		{
			if (_gm == null)
				Debug.LogError("game manager is missing", this);
		}

		private void Start()
		{
			StartBubbling(destroyCancellationToken).Forget();
		}

		private void Update()
		{
			ForeachBubble(bubble =>
			{
				bubble.SecondsAlive += Time.deltaTime;

				if (bubble.SecondsAlive >= _bubbleLifeSeconds)
					return HandleBubbleResult.CanRemove;

				if (_bubbleTravelDistance > Mathf.Abs(bubble.transform.position.y - _bubbleSpawnOrigin.position.y))
					return HandleBubbleResult.Nothing;

				return HandleBubbleResult.CanRemove;
			});
		}

		private void FixedUpdate()
		{
			if (_bubbles is null || _bubbles.Count is 0)
				return;

			ForeachBubble(bubble =>
			{
				bubble.Rigidbody.AddForce(Vector3.up * _bubbleFloatForce, ForceMode2D.Force);
				return HandleBubbleResult.Nothing;
			});
		}

		private async UniTask StartBubbling(CancellationToken ct)
		{
			try
			{
				while (!ct.IsCancellationRequested)
				{
					await UniTask.WaitForSeconds(_spawnIntervalSeconds, cancellationToken: ct);

					var bubble = BubbleFactory.GetBubble(_bubblesParent);
					{
						bubble.gameObject.SetActive(true);
						bubble.transform.position = _bubbleSpawnOrigin.position;
						var offset = Random.Range(-1.0f, 1.0f) * _bubbleSpawnOriginOffsetRange;
						bubble.transform.position += new Vector3(offset, 0, 0);

						bubble.SecondsAlive = 0;
					}

					_bubbles.Add(bubble);
				}
			}
			catch (OperationCanceledException) { }
			catch (Exception e)
			{
				Debug.LogException(e, this);
			}
		}

		private void ForeachBubble(Func<Bubble, HandleBubbleResult> bubbleHandler)
		{
			var removeCount = 0;
			for (var i = _bubbles.Count - 1; i >= 0; i--)
			{
				var bubble = _bubbles[i];
				switch (bubbleHandler(bubble))
				{
					case HandleBubbleResult.Nothing: continue;
					case HandleBubbleResult.CanRemove:
						_bubbles[i] = _bubbles[^++removeCount];
						BubbleFactory.Recycle(bubble);
						continue;
				}
			}

			if (removeCount > 0)
				_bubbles.RemoveRange(_bubbles.Count - removeCount, removeCount);
		}
	}

	public enum HandleBubbleResult
	{
		Nothing,
		CanRemove,
	}
}
