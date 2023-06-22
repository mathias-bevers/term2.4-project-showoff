using System;
using UnityEngine;

public class DemonstrationCamera : MonoBehaviour
{
	[SerializeField] private float rotationSpeed;

	private Transform cameraTransform;
	private Transform cachedTransform;

	private void Start()
	{
		if (Camera.main == null) { throw new UnassignedReferenceException("\'MainCamera\' tag is not assigned!"); }

		cameraTransform = Camera.main.transform;
		cachedTransform = transform;
	}

	private void Update()
	{
		float horizontal = Input.GetAxis("Horizontal") * Time.deltaTime * rotationSpeed;
		horizontal *= -1;
		cameraTransform.RotateAround(Vector3.zero, Vector3.up, horizontal);

		float vertical = Input.GetAxis("Vertical") * Time.deltaTime * rotationSpeed;
		cameraTransform.RotateAround(Vector3.zero, cameraTransform.right, vertical);
	}

	private void LateUpdate()
	{
		cameraTransform.LookAt(cachedTransform.position);
	}

	private void OnValidate()
	{
		if (rotationSpeed >= 0) { return; }

		rotationSpeed = 0;
	}
}