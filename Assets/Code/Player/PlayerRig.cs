using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRig : MonoBehaviour
{
    [SerializeField] Player player;
    [SerializeField] CameraRig cameraRig;
    [SerializeField] float maxToSide = 1.5f;

    float moveDelta = 0;

    private void Update()
    {
        HandleInput();
        Move();
        Clampies();
    }

    void Move()
    {
        //player.transform.localPosition = new Vector3(currentToSide, player.transform.localPosition.y, player.transform.localPosition.z);

    }

    void HandleInput()
    {
        moveDelta = 0;
        if (Input.GetKey(KeyCode.LeftArrow))
            moveDelta -= (3.5f * Time.deltaTime);
        if (Input.GetKey(KeyCode.RightArrow))
            moveDelta += (3.5f * Time.deltaTime);
    }

    void Clampies()
    {
        if (player == null) return;
        Vector3 playerLocalPos = player.transform.localPosition;
        if (playerLocalPos.x > maxToSide) playerLocalPos.x = maxToSide;
        if (playerLocalPos.x < -maxToSide) playerLocalPos.x = -maxToSide;
        player.transform.localPosition = playerLocalPos;
    }
}
