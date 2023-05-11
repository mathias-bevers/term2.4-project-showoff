using UnityEngine;

public static class Utils
{
	public static bool IsNull(this object obj) => ReferenceEquals(obj, null);
}