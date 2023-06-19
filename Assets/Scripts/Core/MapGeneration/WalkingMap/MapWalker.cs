using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class MapWalker : MonoBehaviour
{
    public bool run = false;

    float recoveryTime = 0;

    [SerializeField] NewMapBuilder mapBuilder;
    [SerializeField] PlayerRig rig;

    [Header("Speed & Timing")]
    [SerializeField] float startSpeed = 3;
    [SerializeField] float endSpeed = 7;
    [SerializeField] float accelerationTime = 120;
    [Header("Map Spawn Data")]
    [SerializeField] int minSpawnAmount = 3;

    [SerializeField] float slowedSpeed = 0.1f;

    float overrideSpeed = -1;

    float distShouldRan = 0;

    float currentSpeed = 0;
    float afterCalcCurrentSpeed 
    { 
        get 
        {
            if (Player.Instance.EffectIsActive(PickupIdentifier.SlowdownRework))
                return currentSpeed - 2;
            else if (Player.Instance.EffectIsActive(PickupIdentifier.SpeedupRework))
                return currentSpeed + 2;
            return currentSpeed; 
        } 
    }
    float accelerationTimer = 0;


    private void Start()
    {
        currentSpeed = startSpeed;
        accelerationTimer = 0;
    }

    LevelElement lastActiveElement = null;

    float metersRan = 0;

    float totalMetersRan = 0;
    float metersRan250 = 0;
    int meterCount = 0;

    LevelElement activeElement;

    public float TotalMetersRan => totalMetersRan;

    private void Update()
    {
        if (rig.player.dead) { run = false; //MeterDisplayer.Instance.DisplayMeters((int)totalMetersRan, 10000);
                                            }
        if(run)
        OnUpdate();
    }

    void OnUpdate()
    {
        if (!run) return;
        if(recoveryTime >= 0)
        {
            overrideSpeed = slowedSpeed;
            recoveryTime -= Time.deltaTime;
        }
        else
        {
            overrideSpeed = -1;
        }

        if (mapBuilder == null) return;
        BuildElements();

        activeElement = mapBuilder.activeElement.baseElement;

        if (activeElement == null) return;
        HandleSpeedIncrease();
        if (activeElement != lastActiveElement)
        {
            lastActiveElement = activeElement;

            metersRan = distShouldRan;
        }
        if (Input.GetKeyDown(KeyCode.D)) activeElement?.ChoseSide(MapSides.Right);
        if (Input.GetKeyDown(KeyCode.A)) activeElement?.ChoseSide(MapSides.Left);

        HandleSingleDirection(activeElement);
    }

    void BuildElements()
    {
        if (mapBuilder.elementAmount > minSpawnAmount) return;
        else mapBuilder.BuildLevelElement();
    }

    void HandleSpeedIncrease()
    {
        accelerationTimer += Time.deltaTime;
        if (accelerationTimer >= accelerationTime) accelerationTimer = accelerationTime;
        currentSpeed = Utils.Map(accelerationTimer, 0, accelerationTime, startSpeed, endSpeed);
        if (currentSpeed > endSpeed) currentSpeed = endSpeed;
        if (overrideSpeed >= 0) currentSpeed = slowedSpeed;
    }

    Path activePath = null;

    PathNode? activeNode = null;
    PathNode? newActiveNode = null;
    float removableDistance = 0;

    void HandleSingleDirection(LevelElement activeElement)
    {
        if (rig.player.oopsIDied)
        {
            mapBuilder.MoveOverElement(rig);
            rig.player.transform.localPosition = Vector3.zero;
            rig.player.oopsIDied = false;
            if (!rig.player.EffectIsActive(PickupIdentifier.Speedup))
            {
                rig.player.AddPickup((int)PickupIdentifier.Speedup);
                recoveryTime = 3;
            }
        }

        if (activeElement != null) if (activeElement.forceRequestNewPath) activePath = activeElement.GetPath();
        if (activeNode == null)
        {
            activePath = activeElement.GetPath();
        }
        else
        {
            if (activeNode.Value.shouldProbablyRequestPathUpdate)
                activePath = activeElement.GetPath();

            if (activeNode.Value.isEnd)
            {
                mapBuilder.MoveOverElement(rig);
                metersRan = distShouldRan;
                activeNode = null;
                newActiveNode = null;
                activePath = null;
                return;
            }
        }

        if (activePath == null) return;
        if (activePath.Count <= 1) return;
        DrawPath();
        WalkPath();
        metersRan += Time.deltaTime * afterCalcCurrentSpeed;
        totalMetersRan += Time.deltaTime * afterCalcCurrentSpeed;
        metersRan250 += Time.deltaTime * afterCalcCurrentSpeed;

        if(metersRan250 >= 100)
        {
            metersRan250 = 0;
            meterCount++;
            MeterDisplayer.Instance.DisplayMeters(meterCount * 100);
        }
    }

    void WalkPath()
    {
        GetNodes();
        GetPosition();
    }

    void GetPosition()
    {
        if (activeNode == null || newActiveNode == null)
        {
            FindObjectOfType<Player>()?.Kill();
            return;
        }
        Vector3 activePos = activeNode.Value.position;
        Vector3 newActivePos = newActiveNode.Value.position;

        float usedDistance = metersRan - removableDistance;
        float calcedDistance = Vector3.Distance(activePos, newActivePos);

        float zeroToOne = usedDistance / (calcedDistance + 0.001f);

        Vector3 direction = newActivePos - activePos;

        Vector3 directionFlat = direction;
        directionFlat.y = 0;

        Vector3 directionMultiplied = direction * zeroToOne;

        Vector3 outcomePosition = activePos + directionMultiplied;

        if (directionFlat.magnitude > float.Epsilon || direction.magnitude < -float.Epsilon)
            rig.transform.rotation = Quaternion.LookRotation(directionFlat, Vector3.up);
        
        rig.transform.position = outcomePosition;
    }

    void GetNodes()
    {
        activeNode = null;
        newActiveNode = null;
        removableDistance = 0;
        float currentDistance = 0;
        

        for (int i = 0; i < activePath.Count; i++)
        {
            float distance = Vector3.Distance(activePath[i].position, activePath[i + 1].position);

            currentDistance += distance;
            if (currentDistance >= metersRan)
            {
                activeNode = activePath[i];
                newActiveNode = activePath[i + 1];
                removableDistance = currentDistance - distance;
                if (activeNode.Value.choiceNode)
                    activeElement.LockInput();
                return;
            }
        }

        activeNode = activePath[activePath.Count - 1];
        newActiveNode = activeNode;
        removableDistance = Vector3.Distance(activePath[0].position, activePath[activePath.Count - 1].position);
        distShouldRan = metersRan - removableDistance;

        if (!activeNode.Value.isEnd)
        {
            if (Player.Instance.EffectIsActive(PickupIdentifier.Speedup))
            {
                List<MapSides> winningSides = activeElement.GetWinningSides();
                activeElement.ChoseSide(winningSides.GetRandomElementStruct());
            }
            else
            {
                FindObjectOfType<Player>()?.Kill();
            }
        }
    }

    void DrawPath()
    {
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
    }
}
