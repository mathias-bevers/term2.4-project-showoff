using CurveLib.Interpolation;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelElement : MonoBehaviour
{
    const float stepCount = 0.05f;
    const int stepAmount = (int)(1/stepCount);

    [SerializeField] LevelPoint startPoint;

    int drawCount = 0;


    private void OnDrawGizmos()
    {
        if (startPoint == null) return;

        drawCount = 0;
        RecursiveDraw(startPoint);
    }

    void RecursiveDraw(LevelPoint point, LevelPoint lastPoint = null)
    {
        if(drawCount == 0) DrawPointStart(point);
        DrawPointForward(point);
        if(lastPoint != null) DrawLine(point, lastPoint);
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
        if (lastPoint.isSmooth)
        {
            for(int i = 0; i < stepAmount; i++)
            {
                Vector3 point1 = CurveInterpolations.CubicBezier(stepCount * i, lastPoint.position, lastPoint.position + lastPoint.transform.forward * lastPoint.forwardStrength, point.position - point.transform.forward * point.forwardStrength, point.position);
                Vector3 point2 = CurveInterpolations.CubicBezier(stepCount * (i + 1), lastPoint.position, lastPoint.position + lastPoint.transform.forward * lastPoint.forwardStrength, point.position - point.transform.forward * point.forwardStrength, point.position);
                Gizmos.DrawLine(point1, point2);
            }
            //TODO: Implement smooth line!
        }
        else
        {

            Gizmos.DrawLine(point.position, lastPoint.position);
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
