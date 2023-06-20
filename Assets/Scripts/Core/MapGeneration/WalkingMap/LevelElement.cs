using CurveLib.Interpolation;
using System;
using System.Collections.Generic;
using UnityEngine;
using Mudfish;


public class LevelElement : MonoBehaviour
{
    [SerializeField] bool hasSideToChose = false;
    const float stepCount = 0.001f;
    const int stepAmount = (int)(1 / stepCount);

    [SerializeField] LevelPoint startPoint;
    [SerializeField] LevelPoint[] endPoints;

    public LevelPoint StartPoint => startPoint;
    public LevelPoint[] EndPoints => endPoints;

    int drawCount = 0;

    LevelPoint _takenLevelPoint = null;
    public LevelPoint TakenLevelPoint => _takenLevelPoint;

    MapSides chosenSide = 0;
    bool hasChosenSide = false;

    [HideInInspector]
    public bool lockInput = false;

    bool _forceRequestNewPath = false;
    public bool forceRequestNewPath => _forceRequestNewPath;

    public List<MapSides> GetWinningSides()
    {
        List<MapSides> sides = new List<MapSides>();
        LoopThroughPoints(startPoint, (p, lp) =>
        {
            if (p.isChoiceNode)
            {
               foreach(ConnectionPoint pp in p.connectionPoints)
                {
                    sides.Add(pp.requiredSides);
                }
            }
        }, true, false, null);

        return sides;
    }

    private void Awake()
    {
        if (endPoints.Length == 1) SetTakenPoint(endPoints[0]);

        ChoseSide(0);
    }

    public void LockInput()
    {
        lockInput = true;
    }

    public void ChoseSide(MapSides side)
    {
        if (lockInput) return;
        if (!hasSideToChose) return;
        chosenSide = side;
        if (side != 0) hasChosenSide = true;
        else hasChosenSide = false;

        LoopThroughPoints(startPoint, (p, lp) =>
        {
            if (!p.isChoiceNode) return;
            foreach (ConnectionPoint cp in p.connectionPoints)
                if (cp.requiredSides.HasFlag(chosenSide) && cp.requiredSides != MapSides.Nothing && chosenSide != MapSides.Nothing)
                {
                    SetTakenPoint(cp.nextPoint);
                    return;
                }else if(cp.requiredSides == MapSides.Nothing &&  cp.requiredSides == MapSides.Nothing)
                {
                    SetTakenPoint(cp.nextPoint);
                    return;
                }
        }, true, false, null);

        _forceRequestNewPath = true;
    }

    void SetTakenPoint(LevelPoint point)
    {
        _takenLevelPoint = point;
        if (_takenLevelPoint == null) return;
        if (_takenLevelPoint.transform == null) return;
        Transform pTrans = _takenLevelPoint.transform.GetChild(0);
        if (pTrans == null) return;
        LevelElement levelEl = pTrans.GetComponent<LevelElement>();
        if (levelEl == null) return;
        levelEl.gameObject.SetActive(true);
    }

    public Path GetPath()
    {
        _forceRequestNewPath = false;
        Path path = new Path();
        LoopThroughPoints(startPoint, (p, lp) =>
        {
            PathNode node = new PathNode();
            node.isEnd = p.isEnd;
            node.shouldProbablyRequestPathUpdate = true;
            node.position = p.position;
            node.choiceNode = p.isChoiceNode;

            if (lp != null)
                if (lp.isSmooth)
                {
                    List<Vector3> points = GetBezierPoints(p, lp);
                    for (int i = 1; i < points.Count; i++)
                    {
                        PathNode internalNode = new PathNode();
                        internalNode.position = points[i];
                        internalNode.choiceNode = lp.isChoiceNode;
                        path.nodes.Add(internalNode);
                    }
                }
            path.nodes.Add(node);
        },
        true, true);
        return path;
    }

    private void OnDrawGizmos()
    {

        if (startPoint == null) return;

        drawCount = 0;

        RecursiveDraw(startPoint);
    }


    void RecursiveDraw(LevelPoint point, LevelPoint lastPoint = null)
    {
        if (drawCount == 0) DrawPointStart(point);
        DrawPointForward(point);
        if (lastPoint != null) DrawLine(point, lastPoint);
        if (point.isEnd) DrawEnd(point);
        for (int i = 0; i < point.connectionPoints.Length; i++)
        {
            RecursiveDraw(point.connectionPoints[i].nextPoint, point);
        }
        drawCount++;
    }

    void LoopThroughPoints(LevelPoint point, Action<LevelPoint, LevelPoint> callback, bool recursive = false, bool stopIfNotChosen = true, LevelPoint lastPoint = null)
    {
        List<ConnectionPoint> connections = new List<ConnectionPoint>();
        foreach (ConnectionPoint conPoint in point.connectionPoints)
            connections.Add(conPoint);

        if (!stopIfNotChosen) callback(point, lastPoint);
        else
        {
            if (lastPoint == null) callback(point, lastPoint);
            else
            {
                if (!lastPoint.isChoiceNode) callback(point, lastPoint);
                else
                {
                    for (int i = lastPoint.connectionPoints.Length - 1; i >= 0; i--)
                    {
                        ConnectionPoint conPoint = lastPoint.connectionPoints[i];

                        if (conPoint.nextPoint == point)
                        {
                            if (((chosenSide != MapSides.Nothing && conPoint.requiredSides.HasFlag(chosenSide)) || (conPoint.requiredSides == MapSides.Nothing && !hasChosenSide)))
                                callback(point, lastPoint);
                            else connections.Remove(conPoint);
                        }
                    }
                }
            }
        }


        if (!recursive) return;

        foreach (ConnectionPoint p in connections)
            LoopThroughPoints(p.nextPoint, callback, recursive, stopIfNotChosen, point);
    }

    List<Vector3> GetBezierPoints(LevelPoint point, LevelPoint lastPoint)
    {
        List<Vector3> points = new List<Vector3>();
        if (lastPoint == null) return points;

        Vector3 lastPointPos = lastPoint.position;
        Vector3 pointPosition = point.position;
        Vector3 forwardPoint = point.transform.forward * point.forwardStrength;
        Vector3 forwardLastPoint = lastPoint.transform.forward * lastPoint.forwardStrength;
        Vector3 pointForward = pointPosition - forwardPoint;
        Vector3 lastPointForward = lastPointPos + forwardLastPoint;

        for (int i = 0; i < stepAmount; i++)
        {
            points.Add(CurveInterpolations.CubicBezier(stepCount * i, lastPointPos, lastPointForward, pointForward, pointPosition));
        }

        return points;
    }

    void DrawEnd(LevelPoint point)
    {
        Gizmos.color = new Color(255 / 255.0f, 0 / 255.0f, 0 / 255.0f);
        Gizmos.DrawWireSphere(point.position, 0.1f);
    }

    void DrawLine(LevelPoint point, LevelPoint lastPoint)
    {
        Gizmos.color = new Color(0, 1, 0);

        if (!lastPoint.isSmooth) Gizmos.DrawLine(point.position, lastPoint.position);
        else
        {
            List<Vector3> points = GetBezierPoints(point, lastPoint);
            for (int i = 0; i < points.Count - 1; i++) Gizmos.DrawLine(points[i], points[i + 1]);
        }
    }

    void DrawPointStart(LevelPoint point)
    {
        Gizmos.color = new Color(152 / 255.0f, 114 / 255.0f, 255 / 255.0f);
        Gizmos.DrawWireSphere(point.position, 0.1f);
    }

    void DrawPointForward(LevelPoint point)
    {
        Gizmos.color = new Color(0 / 255.0f, 40 / 255.0f, 255 / 255.0f);
        Vector3 newPos = point.position + point.transform.forward * point.forwardStrength;
        Gizmos.DrawLine(point.position, newPos);
        Gizmos.DrawWireSphere(newPos, 0.05f);
    }
}
