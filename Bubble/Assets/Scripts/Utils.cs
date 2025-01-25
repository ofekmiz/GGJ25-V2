using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

public static class Utils
{
	public static async UniTaskVoid RunTimer(float duration, Action onEnd)
	{
		if (duration == 0)
			return;
        
		while (duration > 0 )
		{
			duration -= Time.deltaTime;
			await UniTask.NextFrame();
		}
		onEnd?.Invoke();
	}
}
