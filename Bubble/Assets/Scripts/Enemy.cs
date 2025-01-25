using UnityEngine;

public class Enemy : MonoBehaviour
{
	public void SetView(GameObject prefab)
	{
		if(prefab)
			Instantiate(prefab, transform);
	}
}
