using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using UnityEngine;
using Random = UnityEngine.Random;

public static partial class Utils
{
	public static bool IsNull(this object obj) => ReferenceEquals(obj, null);

	public static bool IsNullOrEmpty<T>(this IEnumerable<T> collection) => collection == null || !collection.Any();

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

	public static string ColorRichText(this string str, Color color) => string.Concat("<color=", ColorUtility.ToHtmlStringRGBA(color), ">", str, "</color>");

	public static T GetComponentInParents<T>(this Transform start) where T : Component
	{
		Transform parent = start.parent;
		while (parent != null)
		{
			if (parent.TryGetComponent(out T component)) { return component; }

			parent = parent.parent;
		}

		throw new NoComponentFoundException<T>("");
	}

	public static T GetComponentThrow<T>(this Component gameObject) where T : Component
	{
		if (gameObject.TryGetComponent(out T component)) { return component; }

		throw new NoComponentFoundException<T>();
	}

	public static Sprite LoadSpriteFromDisk(string filePath)
	{
		Texture2D texture = LoadTextureFromDisk(filePath);
		return Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
	}

	public static Texture2D LoadTextureFromDisk(string filePath)
	{
		byte[] inBytes = File.ReadAllBytes(filePath);
		Texture2D texture = new(1, 1);
		texture.LoadImage(inBytes);

		return texture;
	}

	public static string Repeat(this string s, int times) => string.Concat(Enumerable.Repeat(s, times));
	
	public static string Bold(this string s) => string.Concat("<b>", s, "</b>");
}