using UnityEngine;

public class Trampoline : MonoBehaviour
{
	[SerializeField] private GameObject _trampolineHolder;
	
	public void SetView(GameObject trampolineVisual)
	{
		if (trampolineVisual == null) return;
		
		foreach (Transform child in _trampolineHolder.transform)
		{
			GameObject.Destroy(child.gameObject);
		}
		
		Instantiate(trampolineVisual, _trampolineHolder.transform);
	}
}
