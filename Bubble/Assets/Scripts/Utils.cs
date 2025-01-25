using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine;

public static class Utils
{
	[Serializable]
	public struct SpawnPoint
	{
		public GameModifierType Key;
		public Transform Parent;
	}
	
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

	public static T GetRandom<T>(this IEnumerable<T> list)
	{
		var tmp = list.ToList();
		if (tmp.Count == 0)
			return default;
		var index = UnityEngine.Random.Range(0, tmp.Count);
		return tmp[index];
	}
}
