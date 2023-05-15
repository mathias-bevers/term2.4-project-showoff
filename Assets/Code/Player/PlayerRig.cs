using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRig : MonoBehaviour
{
    [SerializeField] Player player;
    [SerializeField] CameraRig cameraRig;
    [SerializeField] float maxToSide = 1.5f;

    float currentToSide = 0;

    private void Update()
    {
        HandleInput();
        Move();
        Clampies();
    }

    void Move()
    {
        player.transform.localPosition = new Vector3(currentToSide, player.transform.localPosition.y, player.transform.localPosition.z);
    }

    void HandleInput()
    {
        if (Input.GetKey(KeyCode.LeftArrow))
            currentToSide -= (3.5f * Time.deltaTime);
        if (Input.GetKey(KeyCode.RightArrow))
            currentToSide += (3.5f * Time.deltaTime);
    }

    void Clampies()
    {
        if (currentToSide > maxToSide)
            currentToSide = maxToSide;
        if (currentToSide < -maxToSide)
            currentToSide = -maxToSide;
    }
}
