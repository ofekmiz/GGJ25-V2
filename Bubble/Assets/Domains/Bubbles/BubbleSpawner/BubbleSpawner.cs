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
		[SerializeField] private GameManager _gm;
		[SerializeField] private Transform _bubblesParent;
		[SerializeField] private Transform _bubbleSpawnOrigin;
		[SerializeField] [Range(0, 10f)] private float _bubbleSpawnOriginOffsetRange;
		[SerializeField] [Range(0.1f, 3f)] private float _spawnIntervalSeconds = 0.5f;
		[SerializeField] [Range(1f, 10f)] private float _bubbleFloatSpeed = 0.5f;
		[SerializeField] [Range(1f, 10f)] private float _bubbleTravelDistance = 0.5f;

		private IBubbleFactory BubbleFactory => _gm.Deps.BubbleFactory;
		private readonly List<Bubble> _bubbles = new();

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
			if (_bubbles is null || _bubbles.Count is 0)
				return;

			var removeCount = 0;
			for (var i = _bubbles.Count - 1; i >= 0; i--)
			{
				var bubble = _bubbles[i];
				bubble.transform.position += _bubbleFloatSpeed * Time.deltaTime * Vector3.up;

				if (Mathf.Abs(bubble.transform.position.y - _bubbleSpawnOrigin.position.y) < _bubbleTravelDistance)
					continue;

				removeCount++;
				_bubbles[i] = _bubbles[^removeCount];
				BubbleFactory.Recycle(bubble);
			}

			if (removeCount > 0)
				_bubbles.RemoveRange(_bubbles.Count - removeCount, removeCount);
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
						bubble.transform.position = _bubbleSpawnOrigin.position;
						var offset = Random.Range(-1.0f, 1.0f) * _bubbleSpawnOriginOffsetRange;
						bubble.transform.position += new Vector3(offset, 0, 0);
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

		// private async UniTask FloatBubble(Bubble bubble, float distance, CancellationToken ct)
		// {
		// 	try
		// 	{
		// 		var startPosition = bubble.transform.position;
		// 		while (ct.IsCancellationRequested)
		// 		{
		// 			if (Mathf.Abs(bubble.transform.position.y - startPosition.y) >= distance)
		// 				break;
		//
		// 			bubble.transform.position += Vector3.up * distance * Time.deltaTime;
		// 			await UniTask.DelayFrame(1, cancellationToken: ct);
		// 		}
		// 	}
		// 	catch (OperationCanceledException) { }
		// 	catch (Exception e) { Debug.LogException(e, this); }
		// 	finally
		// 	{
		// 		BubbleFactory.Recycle(bubble);
		// 	}
		// }
	}
}
