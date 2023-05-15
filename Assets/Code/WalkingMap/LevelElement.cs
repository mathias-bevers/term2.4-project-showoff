using CurveLib.Interpolation;
using System;
using System.Collections.Generic;
using UnityEngine;

public class LevelElement : MonoBehaviour
{
    [SerializeField] bool hasSideToChose = false;
    const float stepCount = 0.01f;
    const int stepAmount = (int)(1 / stepCount);

    [SerializeField] LevelPoint startPoint;
    [SerializeField] LevelPoint[] endPoints;

    public LevelPoint[] EndPoints => endPoints;

    int drawCount = 0;

    LevelPoint _takenLevelPoint = null;
    public LevelPoint TakenLevelPoint => _takenLevelPoint;

    MapSides chosenSide;
    bool hasChosenSide = false;

    [HideInInspector]
    public bool lockInput = false;

    private void Awake()
    {
        if (endPoints.Length == 1) _takenLevelPoint = endPoints[0];
    }

    public void ChoseSide(MapSides side)
    {
        if (lockInput) return;
        if (!hasSideToChose) return;
        chosenSide = side;
        hasChosenSide = true;
        foreach (LevelPoint p in endPoints)
            if (p.winningSides.HasFlag(side)) 
                _takenLevelPoint = p;
    }

    public Path GetPath()
    {
        Path path = new Path();
        LoopThroughPoints(startPoint, (p, lp) =>
        {
            PathNode node = new PathNode();
            node.isEnd = p.isEnd;
            node.shouldProbablyRequestPathUpdate = true;
            node.position = p.position;

            if (lp != null)
                if (lp.isSmooth)
                {
                    List<Vector3> points = GetBezierPoints(p, lp);
                    for (int i = 1; i < points.Count; i++)
                    {
                        PathNode internalNode = new PathNode();
                        internalNode.position = points[i];
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
       // return;
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
            RecursiveDraw(point.connectionPoints[i], point);
        }
        drawCount++;
    }

    void LoopThroughPoints(LevelPoint point, Action<LevelPoint, LevelPoint> callback, bool recursive = false, bool stopIfNotChosen = true, LevelPoint lastPoint = null)
    {
        if(HandleStop(stopIfNotChosen, lastPoint, point))
        HandleDefault(point, callback, recursive, stopIfNotChosen, lastPoint);
    }

    bool HandleStop(bool stopIfNotChosen, LevelPoint lastPoint, LevelPoint point)
    {
        if (!stopIfNotChosen) return true;
        if (lastPoint == null) return true;
        if (!lastPoint.isChoiceNode) return true;
        if (!hasChosenSide && hasSideToChose) return false;
        else if (!point.winningSides.HasFlag(chosenSide)) return false;
        return true;
    }

    void HandleDefault(LevelPoint point, Action<LevelPoint, LevelPoint> callback, bool recursive, bool stopIfNotChosen, LevelPoint lastPoint)
    {
        callback(point, lastPoint);

        if (!recursive) return;
        foreach (LevelPoint p in point.connectionPoints)
            LoopThroughPoints(p, callback, recursive, stopIfNotChosen, point);
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
