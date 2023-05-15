using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRig : MonoBehaviour
{
    [SerializeField] Player player;
    [SerializeField] CameraRig rig;

    [SerializeField] float maxToSide = 1.5f;

    float currentToSide = 0;

    private void Update()
    {
        Stick();
        HandleInput();
        Move();
        Clampies();
    }

    void Move()
    {
        player.transform.localPosition = transform.right * currentToSide;
    }

    void HandleInput()
    {
        if (Input.GetKey(KeyCode.LeftArrow))
            currentToSide += (3.5f * Time.deltaTime);
        if (Input.GetKey(KeyCode.RightArrow))
            currentToSide -= (3.5f * Time.deltaTime);
    }

    void Clampies()
    {
        if (currentToSide > maxToSide)
            currentToSide = maxToSide;
        if (currentToSide < -maxToSide)
            currentToSide = -maxToSide;
    }

    void Stick()
    {
        if (rig == null) return;
        transform.position = rig.transform.position;
        transform.rotation = rig.transform.rotation;
    }
}
