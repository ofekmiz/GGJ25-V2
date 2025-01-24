using System;
using System.Collections;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;


public struct PlatformInitArgs
{
	public Vector3 Position;
	public bool ShowOnStart;
}

public class Platform : MonoBehaviour
{
	public static Action<Platform> OnReachEdge;

	private PlatformInitArgs _args;
	public void Set(PlatformInitArgs args)
	{
		_args = args;
		if (args.ShowOnStart)
			Show().Forget();
		var pos = args.Position;
		pos.y = PlatformMover.PlatformHideLocation;
		transform.localPosition = pos;
	}
	
	private void OnTriggerEnter2D(Collider2D other)
	{
		if (other.CompareTag("Edge"))
			Hide().Forget();
		if (other.CompareTag("Start"))
			Show().Forget();
	}

	private async UniTaskVoid Hide()
	{
		await transform.DOLocalMoveY(PlatformMover.PlatformHideLocation, .5f).AsyncWaitForCompletion();
		OnReachEdge?.Invoke(this);
	}
	
	private async UniTaskVoid Show()
	{
		await transform.DOLocalMoveY(_args.Position.y, 1).AsyncWaitForCompletion();
	}
}
