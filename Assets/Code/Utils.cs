using System.Collections.Generic;
using UnityEngine;

public static partial class Utils
{
	public static bool IsNull(this object obj) => ReferenceEquals(obj, null);

	public static bool IsNullOrEmpty<T>(this IList<T> collection) => collection == null || collection.Count < 1; 

	public static T GetRandomElement<T>(this IList<T> collection) where T : class
	{
		if(collection.Count == 0) return null;
		int randomIndex = Random.Range(0, collection.Count);
		return collection[randomIndex];
	}

    public static T GetRandomElementStruct<T>(this IList<T> collection) where T : struct
    {
        int randomIndex = Random.Range(0, collection.Count);
        return collection[randomIndex];
    }
}