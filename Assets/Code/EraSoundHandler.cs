using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EraSoundHandler : Singleton<EraSoundHandler>
{
	private const float ONE = 1;
	private const float ZERO = 0;

	[SerializeField] private Transform eraOneParent;
	[SerializeField] private Transform eraTwoParent;
	[SerializeField] private float fadeInDuration;
	[SerializeField] private float fadeOutDuration;

	private Action fadeIn;
	private Action fadeOut;

	private Dictionary<AudioSource, float> eraOneSources;
	private Dictionary<AudioSource, float> eraTwoSources;
	private float volumeEraOne;
	private float volumeEraTwo;


	public override void Awake()
	{
		base.Awake();

		eraOneSources = new Dictionary<AudioSource, float>();
		eraTwoSources = new Dictionary<AudioSource, float>();

		foreach (AudioSource source in eraOneParent.GetComponents<AudioSource>())
		{
			eraOneSources.Add(source, source.volume);
			source.volume = 0;
		}

		foreach (AudioSource source in eraTwoParent.GetComponents<AudioSource>())
		{
			eraTwoSources.Add(source, source.volume);
			source.volume = 0;
		}
	}

	private void Update()
	{
		fadeIn?.Invoke();
		fadeOut?.Invoke();
	}

	private void FadeIn(int era)
	{
		switch (era)
		{
			case 0:
				volumeEraOne = Mathf.MoveTowards(volumeEraOne, ONE, 1 / fadeInDuration * Time.deltaTime);

				foreach (KeyValuePair<AudioSource, float> source in eraOneSources) { source.Key.volume = source.Value * volumeEraOne; }

				if (volumeEraOne >= 1) { fadeIn = null; }

				break;

			case 1:
				volumeEraTwo = Mathf.MoveTowards(volumeEraTwo, ONE, 1 / fadeInDuration * Time.deltaTime);

				foreach (KeyValuePair<AudioSource, float> source in eraTwoSources) { source.Key.volume = source.Value * volumeEraTwo; }

				if (volumeEraTwo >= 1) { fadeIn = null; }

				break;

			default: throw new IndexOutOfRangeException($"Cannot change value for era {era}");
		}
	}

	private void FadeOut(int era)
	{
		switch (era)
		{
			case 1:
				volumeEraOne = Mathf.MoveTowards(volumeEraOne, ZERO, 1 / fadeOutDuration * Time.deltaTime);
				foreach (KeyValuePair<AudioSource, float> source in eraOneSources) { source.Key.volume = source.Value * volumeEraOne; }

				if (volumeEraOne <= ZERO)
				{
					fadeOut = null;
					foreach (AudioSource source in eraOneSources.Keys) { source.Stop(); }
				}

				break;

			case 0:
				volumeEraTwo = Mathf.MoveTowards(volumeEraTwo, ZERO, 1 / fadeOutDuration * Time.deltaTime);
				foreach (KeyValuePair<AudioSource, float> source in eraTwoSources) { source.Key.volume = source.Value * volumeEraTwo; }

				if (volumeEraTwo <= ZERO)
				{
					fadeOut = null;
					foreach (AudioSource source in eraTwoSources.Keys) { source.Stop(); }
				}

				break;

			default: throw new IndexOutOfRangeException($"Cannot change value for era {era}");
		}
	}

	public void OnEraChange(int currentEra)
	{
		Debug.Log("Era changed to: " + currentEra);

		switch (currentEra)
		{
			case 0:
			{
				foreach (AudioSource source in eraOneSources.Keys)
				{
					source.Play();
					source.loop = true;
				}

				break;
			}

			case 1:
			{
				foreach (AudioSource source in eraTwoSources.Keys)
				{
					source.Play();
					source.loop = true;
				}

				break;
			}

			default: throw new IndexOutOfRangeException($"Cannot change value for era {currentEra}");
		}

		fadeIn = () => FadeIn(currentEra);
		fadeOut = () => FadeOut(currentEra);
	}
}