using System;
using UnityEngine;

public class NoComponentFoundException<T> : Exception where T : Component
{
	public NoComponentFoundException() : base(nameof(T)) { }

	public NoComponentFoundException(string message) : base(string.Concat(nameof(T), "\t", message)) { }
}