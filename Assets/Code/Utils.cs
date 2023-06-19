using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using UnityEngine;
using Random = UnityEngine.Random;

public static partial class Utils
{
	public static bool IsNull(this object obj) => ReferenceEquals(obj, null);

	public static bool IsNullOrEmpty<T>(this IList<T> collection) => collection == null || collection.Count < 1;

	public static T GetRandomElement<T>(this IList<T> collection) where T : class
	{
		if (collection.Count == 0) { return null; }

		int randomIndex = Random.Range(0, collection.Count);
		return collection[randomIndex];
	}

	public static T GetRandomElementStruct<T>(this IList<T> collection) where T : struct
	{
		int randomIndex = Random.Range(0, collection.Count);
		return collection[randomIndex];
	}

	public static IPAddress GetIP4Address()
	{
		IPHostEntry host = Dns.GetHostEntry(Dns.GetHostName());
		foreach (IPAddress ip in host.AddressList)
		{
			if (ip.AddressFamily == AddressFamily.InterNetwork) { return ip; }
		}

		throw new Exception("Cannot get ip4 address");
	}

	public static Transform[] GetAllChildren(this Transform parent) => parent.Cast<Transform>().ToArray();

	public static string ColorRichText(this string str, Color color) => string.Concat("<color=", ColorUtility.ToHtmlStringRGBA(color), ">",str, "</color>");
}