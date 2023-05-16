using System.Collections.Generic;
using UnityEngine;

public static partial class Utils
{
	public static bool IsNull(this object obj) => ReferenceEquals(obj, null);

	public static T GetRandomElement<T>(this IList<T> collection)
	{
		int randomIndex = Random.Range(0, collection.Count);
		return collection[randomIndex];
	}
}