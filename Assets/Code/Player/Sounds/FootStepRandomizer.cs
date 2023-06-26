using NaughtyAttributes;
using UnityEngine;

public class FootStepRandomizer : MonoBehaviour
{
	[SerializeField] private Motor playerMotor;
	[SerializeField] private AudioSource source;

	[SerializeField, MinMaxSlider(0.1f, 2.0f)]
	private Vector2 minMaxTimer;


	[Range(0.1f, 0.5f)] public float volumeChange = 0.2f;
	[Range(0.1f, 0.5f)] public float pitchChange = 0.2f;
	public AudioClip[] sounds;

	private float timer;

	private void Awake()
	{
		if (ReferenceEquals(playerMotor, null)) { throw new UnassignedReferenceException($"{nameof(playerMotor)} is not set in the editor!"); }

		if (ReferenceEquals(source, null)) { throw new UnassignedReferenceException($"{nameof(source)} is not set in the editor!"); }

		PlaySound();
	}


	private void Update()
	{
		timer -= Time.deltaTime;

		if (timer > 0) { return; }

		if (playerMotor.motorState != MotorState.Grounded) { return; }

		PlaySound();
	}

	private void PlaySound()
	{
		source.clip = sounds[Random.Range(0, sounds.Length)];
		source.volume = Random.Range(1 - volumeChange, 1);
		source.pitch = Random.Range(1 - pitchChange, 1 + pitchChange);
		source.PlayOneShot(source.clip);

		timer = Random.Range(minMaxTimer.x, minMaxTimer.y);
	}
}