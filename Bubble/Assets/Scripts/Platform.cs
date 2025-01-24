using System;
using UnityEngine;


public struct PlatformInitArgs
{
	public Vector3 Position;
}

public class Platform : MonoBehaviour
{
	public static Action<Platform> OnReachEdge;

	public void Set(PlatformInitArgs args)
	{
		transform.localPosition = args.Position;
	}
	
	private void OnTriggerEnter2D(Collider2D other)
	{
		if(other.CompareTag("Edge"))
			OnReachEdge?.Invoke(this);
	}
}
