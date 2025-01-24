using UnityEngine;

namespace Domains.Utils
{
	public static class VectorUtils
	{
		public static Vector2 Xy(this Vector3 v) => new(v.x, v.y);
		public static Vector3 Xy(this Vector2 v) => new(v.x, v.y);
	}
}
