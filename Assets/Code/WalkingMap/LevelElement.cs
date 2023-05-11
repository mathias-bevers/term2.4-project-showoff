using CurveLib.Interpolation;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LevelElement : MonoBehaviour
{
    [SerializeField] bool markAsProbableWaitingTile = false;
    const float stepCount = 0.05f;
    const int stepAmount = (int)(1 / stepCount);

    [SerializeField] LevelPoint startPoint;
    [SerializeField] LevelPoint[] endPoints;

    public LevelPoint[] EndPoints => endPoints;

    int drawCount = 0;

    LevelPoint _takenLevelPoint;
    public LevelPoint TakenLevelPoint => _takenLevelPoint;


    private void Awake()
    {
        if(endPoints.Length == 1) _takenLevelPoint = endPoints[0];
    }

    public void ChoseSide(MapSides side)
    {
        foreach(LevelPoint p in endPoints)
        {
            if(p.winningSides.HasFlag(side)) _takenLevelPoint = p;
        }
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
        LoopThroughPoints(point);
        drawCount++;
    }

    void LoopThroughPoints(LevelPoint point)
    {
        foreach (LevelPoint p in point.connectionPoints)
            RecursiveDraw(p, point);
    }

    void DrawEnd(LevelPoint point)
    {
        Gizmos.color = new Color(255 / 255.0f, 0 / 255.0f, 0 / 255.0f);
        Gizmos.DrawWireSphere(point.position, 0.1f);
    }

    void DrawLine(LevelPoint point, LevelPoint lastPoint)
    {
        Gizmos.color = new Color(0, 1, 0);
        Vector3 lastPointPos = lastPoint.position;
        Vector3 pointPosition = point.position;
        Vector3 forwardPoint = point.transform.forward * point.forwardStrength;
        Vector3 forwardLastPoint = lastPoint.transform.forward * lastPoint.forwardStrength;
        Vector3 pointForward = pointPosition - forwardPoint;
        Vector3 lastPointForward = lastPointPos + forwardLastPoint;

        Vector3 drawPoint1 = pointPosition;
        Vector3 drawPoint2 = lastPointPos;

        if (!lastPoint.isSmooth) Gizmos.DrawLine(drawPoint1, drawPoint2);
        else for (int i = 0; i < stepAmount; i++)
        {
            Vector3 point1 = CurveInterpolations.CubicBezier(stepCount * i, lastPointPos, lastPointForward, pointForward, pointPosition);
            Vector3 point2 = CurveInterpolations.CubicBezier(stepCount * (i + 1), lastPointPos, lastPointForward, pointForward, pointPosition);
            Gizmos.DrawLine(point1, point2);
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
