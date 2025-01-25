using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;


public struct PlatformInitArgs
{
	public Vector3 Position;
	public float ShortValue;
	public bool ShowOnStart;
	public bool Breakable;
	public PlatformState State;
}

public enum PlatformState
{
	None,
	Short,
	Long
}

public class Platform : MonoBehaviour
{
	public static Action<Platform> OnReachEdge;
	public static Action<Platform> OnReachStart;
	public static Action<PlayerController> OnPlayerGround;
	[SerializeField] private bool _dontHidePlatform;
	[SerializeField] private Transform _view;
	[SerializeField] private BoxCollider2D _collider;
	[SerializeField] private List<SpriteRenderer> _visuals;

	public PlatformInitArgs Settings;
	public bool IsShown { get; private set; }
	public void Set(PlatformInitArgs args)
	{
		Settings = args;
		if (args.ShowOnStart)
			Show().Forget();
		var pos = args.Position;
		pos.y = PlatformMover.PlatformHideLocation;
		transform.localPosition = pos;
	}

	private void OnCollisionEnter2D(Collision2D other)
	{
		var player = other.transform.GetComponent<PlayerController>();
		if (!player) 
			return;
		OnPlayerGround?.Invoke(player);
		if (Settings.Breakable && IsShown && player.IsGrounded)
		{
			IsShown = false;
			_view.DOShakePosition(2, 0.1f).SetLink(gameObject);
			Utils.RunTimer(2, () => Hide().Forget()).Forget();
		}
	}

	private void OnCollisionExit2D(Collision2D other)
	{
		if(other.collider.CompareTag("Player"))
			OnPlayerGround?.Invoke(null);
	}

	private void OnTriggerEnter2D(Collider2D other)
	{
		if(_dontHidePlatform)
			return;
		if (other.CompareTag("Edge"))
			Hide().Forget();
		if (other.CompareTag("Start"))
		{
			OnReachStart?.Invoke(this);
			Show().Forget();
		}
	}

	private async UniTaskVoid Hide()
	{
		await transform.DOLocalMoveY(PlatformMover.PlatformHideLocation, .5f).SetEase(Ease.InOutBack).AsyncWaitForCompletion();
		IsShown = false;
		OnReachEdge?.Invoke(this);
	}
	
	private async UniTaskVoid Show()
	{
		IsShown = true;
		var width = _collider.size.x;
		if (Settings.State == PlatformState.Short)
			width *= Settings.ShortValue;
		if (Settings.State == PlatformState.Long)
			width *= (1 - Settings.ShortValue);
			
		var size = _collider.size;
		size.x = width;
		_collider.size = size;
		foreach (var visual in _visuals)
		{
			size = visual.size;
			size.x = width;
			visual.size = size;
		}
		
		await transform.DOLocalMoveY(Settings.Position.y, 1).SetEase(Ease.InOutBack).AsyncWaitForCompletion();
	}
}
