using UnityEngine;

namespace Galaxy
{
	public static class SingleExtension
	{
		public static bool IsZero(this float self)
		{
			return Mathf.Abs(self) < 0.0001f ? true : false;
		}
	}
	public static class VectorExtension
	{
		public static float Distance2D(this Vector3 self, Vector3 pos)
		{
			Vector3 temp = self;
			temp.y = 0;
			pos.y = 0;
			return Vector3.Distance(temp, pos);
		}
		public static void Normalize2D(this Vector3 self)
		{
			self.y = 0;
			self.Normalize();
		}
		public static Vector3 normalized2d(this Vector3 self)
		{
			Vector3 temp = self;
			temp.y = 0;
			return temp.normalized;
		}
	}
}
