using UnityEngine;

public class Enemy : MonoBehaviour
{
	public void SetView(GameObject prefab)
	{
		Instantiate(prefab, transform);
	}
}
