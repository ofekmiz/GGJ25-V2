using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Domains.Bubbles.BubbleEntity;
using Domains.Bubbles.Factories;
using Domains.Core;
using Domains.Utils;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Domains.Bubbles.BubbleSpawner
{
	public interface IBubblesManager
	{
	}

	public class BubblesManager : MonoBehaviour, IBubblesManager
	{
		[Header("Refs")]
		[SerializeField] private Transform _bubblesParent;
		[SerializeField] private Transform _bubbleSpawnOrigin;

		[Header("Config")]
		[SerializeField] [Min(0)] private float _bubbleSpawnOriginOffsetRange;
		[SerializeField] [Min(0)] private float _spawnIntervalSeconds = 0.5f;
		[SerializeField] [Min(0.1f)] private float _bubbleFloatForce = 0.1f;
		[SerializeField] [Min(0)] private float _bubbleTravelDistance = 0.5f;
		[SerializeField] [Min(float.Epsilon)] private float _bubbleLifeSeconds = 4f;

		// dependencies
		private IBubbleFactory _bubbleFactory;

		// bubbles
		private readonly List<Bubble> _bubbles = new();


		public void Init(GameManager gm, IBubbleFactory bubbleFactory)
		{
			_bubbleFactory = bubbleFactory;
		}

		private void Start()
		{
			StartBubbling(destroyCancellationToken).Forget();
		}

		private void Update()
		{
			ForeachBubble(entry =>
			{
				entry.bubble.SecondsAlive += Time.deltaTime;

				if (entry.bubble.SecondsAlive >= _bubbleLifeSeconds)
					return HandleBubbleResult.RemoveAndContinue;

				var exceededDistance = _bubbleTravelDistance <= Mathf.Abs(entry.bubble.transform.position.y - _bubbleSpawnOrigin.position.y);
				if (exceededDistance)
					return HandleBubbleResult.RemoveAndContinue;

				return HandleBubbleResult.Continue;
			});

			if (Input.GetMouseButtonDown(0))
				HandleMouseClick();
		}

		private void OnDrawGizmos()
		{
			Gizmos.color = Color.red;
			ForeachBubble(entry =>
			{
				Gizmos.DrawCube(entry.bubble.transform.position, 0.3f * entry.bubble.transform.localScale);
				return HandleBubbleResult.Continue;
			});
			var mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);	
			Gizmos.color = Color.green;
			Gizmos.DrawCube(mousePosition, 0.3f * Vector3.one);
		}

		private void HandleMouseClick()
		{
			var mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition).Xy();
			Debug.Log($"MouseDown: {mousePosition}");
			var bubbleIndex = -1;
			var intersectionScore = float.MinValue;
			ForeachBubble(entry =>
			{
				var bubblePosition = entry.bubble.transform.position.Xy();
				var bubbleRadius = entry.bubble.Radius;
				var sqrDelta = (mousePosition - bubblePosition).sqrMagnitude;

				if (sqrDelta > bubbleRadius * bubbleRadius)
				{
					Debug.Log("Nope 1");
					return HandleBubbleResult.Continue;
				}

				if (intersectionScore > sqrDelta)
				{
					Debug.Log("Nope 2");
					return HandleBubbleResult.Continue;
				}

				Debug.Log("Maybe");
				intersectionScore = sqrDelta;
				bubbleIndex = entry.index;

				return HandleBubbleResult.Continue;
			});

			if (bubbleIndex == -1)
				return;

			Debug.Log("Clicked a bubble!!!!!!!");
			var bubble = _bubbles[bubbleIndex];
			RemoveBubbleAndLoseOrder(bubbleIndex);
		}

		private void FixedUpdate()
		{
			ForeachBubble(entry =>
			{
				entry.bubble.Rigidbody.AddForce(Vector3.up * _bubbleFloatForce, ForceMode2D.Force);
				return HandleBubbleResult.Continue;
			});
		}

		private async UniTask StartBubbling(CancellationToken ct)
		{
			try
			{
				await UniTask.WaitUntil(() => _bubbleFactory != null, cancellationToken: ct);
				while (!ct.IsCancellationRequested)
				{
					await UniTask.WaitForSeconds(_spawnIntervalSeconds, cancellationToken: ct);

					var bubble = _bubbleFactory.GetBubble(_bubblesParent);
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
			catch (Exception e) { Debug.LogException(e, this); }
		}

		private void ForeachBubble(Func<(Bubble bubble, int index), HandleBubbleResult> bubbleHandler)
		{
			if (_bubbles is null || _bubbles.Count is 0)
				return;

			var removeCount = 0;
			for (var i = _bubbles.Count - 1; i >= 0; i--)
			{
				var bubble = _bubbles[i];
				switch (bubbleHandler((bubble, i)))
				{
					case HandleBubbleResult.Continue: continue;
					case HandleBubbleResult.Break: break;
					case HandleBubbleResult.RemoveAndContinue:
						RecycleBubble();
						continue;
					case HandleBubbleResult.RemoveAndBreak:
						RecycleBubble();
						break;
				}

				void RecycleBubble()
				{
					_bubbles[i] = _bubbles[^++removeCount];
					_bubbleFactory.Recycle(bubble);
				}
			}

			if (removeCount > 0)
				_bubbles.RemoveRange(_bubbles.Count - removeCount, removeCount);
		}

		private void RemoveBubbleAndLoseOrder(int index)
		{
			var bubble = _bubbles[index];
			_bubbles[index] = _bubbles[^1];
			_bubbleFactory.Recycle(bubble);
		}
	}

	public enum HandleBubbleResult
	{
		Continue,
		Break,
		RemoveAndContinue,
		RemoveAndBreak,
	}
}
