﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Serialization;

public class PowerupSounds : Singleton<PowerupSounds>
{
	private readonly List<IdSourcePair> activeEffects = new();
	
	[SerializeField] private AudioSource pickupSource;
	[SerializeField] private AudioClip pickupClip;
	[SerializeField] private IdClipPair[] idClipPairs;

	private PickupIdentifier lastPickupIdentifier;
	private Transform cachedTransform;

	public override void Awake()
	{
		base.Awake();
		cachedTransform = transform;
	}

	private void Update()
	{
		foreach (IdClipPair idClipPair in idClipPairs)
		{
			PickupIdentifier pickupIdentifier = idClipPair.identifier;

			if (Player.Instance.EffectIsActive(pickupIdentifier))
			{
				if (activeEffects.Any(pair => pair.id == pickupIdentifier)) { continue; }

				IdSourcePair idSourcePair = InitIDSourcePair(idClipPair);
				activeEffects.Add(idSourcePair);
				continue;
			}

			if (activeEffects.All(pair => pair.id != pickupIdentifier)) { continue; }

			IdSourcePair toRemove = activeEffects.Find(pair => pair.id == pickupIdentifier);
			Destroy(toRemove.go, 2f);
			activeEffects.Remove(toRemove);
		}
	}

	private IdSourcePair InitIDSourcePair(IdClipPair icp)
	{
		if (icp.clip == null || icp.volume <= 0) { throw new ArgumentException("sound will not be audible", nameof(icp)); }

		GameObject go = new(icp.identifier.ToString());
		go.transform.SetParent(cachedTransform);

		AudioSource addedSource = go.AddComponent<AudioSource>();
		addedSource.priority = 0;
		addedSource.clip = icp.clip;
		addedSource.volume = icp.volume;
		addedSource.loop = false;
		
		addedSource.Play();

		return new IdSourcePair(icp.identifier, addedSource, go);
	}

	public void ForcePlaySound(PickupIdentifier identifier)
	{
		IdClipPair clipPair = idClipPairs.FirstOrDefault(p => p.identifier == identifier);
		IdSourcePair idSourcePair = InitIDSourcePair(clipPair);
		activeEffects.Add(idSourcePair);
		Debug.Log($"playing clip: {clipPair?.clip.name}");
	}

	public void PlayPickupSound() =>  pickupSource.PlayOneShot(pickupClip);

	[Serializable]
	private class IdClipPair
	{
		[field: SerializeField] public PickupIdentifier identifier { get; private set; }
		[field: SerializeField] public AudioClip clip { get; private set; }

		[field: SerializeField, Range(0.0f, 1.0f)]
		public float volume { get; private set; }
	}

	private struct IdSourcePair
	{
		public PickupIdentifier id { get; }
		public AudioSource source { get; }
		public GameObject go { get; }

		public IdSourcePair(PickupIdentifier id, AudioSource source, GameObject go)
		{
			this.id = id;
			this.source = source;
			this.go = go;
		}
	}
}