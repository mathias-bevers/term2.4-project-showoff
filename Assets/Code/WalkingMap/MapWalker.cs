using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class MapWalker : MonoBehaviour
{
    
    [SerializeField] MapBuilder mapBuilder;
    [SerializeField] CameraRig rig;

    [Header("Speed & Timing")]
    [SerializeField] float startSpeed = 3;
    [SerializeField] float endSpeed = 7;
    [SerializeField] float accelerationTime = 120;
    [Header("Map Spawn Data")]
    [SerializeField] int minSpawnAmount = 3;

    float currentSpeed = 0;
    float accelerationTimer = 0;

    private void Start()
    {
        currentSpeed = startSpeed;
        accelerationTimer = 0;
    }

    LevelElement lastActiveElement = null;
    float metersRan = 0;

    private void Update()
    {
        if (mapBuilder == null) return;
        BuildElements();

        LevelElement activeElement = mapBuilder.activeElement;

        if (activeElement == null) return;
        HandleSpeedIncrease();
        if (activeElement != lastActiveElement)
        {
            lastActiveElement = activeElement;
            metersRan = 0;
        }
        if (Input.GetKeyDown(KeyCode.RightArrow)) activeElement?.ChoseSide(MapSides.Right);
        if (Input.GetKeyDown(KeyCode.LeftArrow)) activeElement?.ChoseSide(MapSides.Left);

        HandleSingleDirection(activeElement);
    }

    void BuildElements()
    {
        if (mapBuilder.elementAmount > minSpawnAmount) return;
        else mapBuilder.BuildElement();
    }

    void HandleSpeedIncrease()
    {
        accelerationTime += Time.deltaTime;
        if (accelerationTimer >= accelerationTime) accelerationTimer = accelerationTime;
        currentSpeed = Utils.Map(accelerationTimer, 0, accelerationTime, startSpeed, endSpeed);
    }

    Path activePath = null;

    void HandleSingleDirection(LevelElement activeElement)
    {
        activePath = activeElement.GetPath();

        if(activePath == null) return;
        if (activePath.Count <= 1) return;

        for (int i = 0; i < activePath.Count - 1; i++)
        {
            Debug.DrawLine(activePath[i].position, activePath[i + 1].position);

        }

        for (int i = 0; i < activePath.Count; i++)
        {
            PathNode node = activePath[i];
            Debug.DrawLine(node.position, node.position + new Vector3(0, 0.1f, 0), Color.white);
            if (node.isEnd) Debug.DrawLine(node.position, node.position + new Vector3(0, 0.6f, 0), Color.green);
            if (node.shouldProbablyRequestPathUpdate) Debug.DrawLine(node.position, node.position + new Vector3(0.001f, 0.2f, 0), Color.blue);
            if (node.isDeath) Debug.DrawLine(node.position, node.position + new Vector3(-0.001f, 0.4f, 0), Color.black);
        }

        metersRan += Time.deltaTime * currentSpeed;
    }
}
