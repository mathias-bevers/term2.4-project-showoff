using UnityEngine;

public struct PositionData
{
    public Vector3 position;
    public bool finished;
    public bool outOfBounds;

    public PositionData(Vector3 position, bool finished, bool outOfBounds) 
    {
        this.position = position;
        this.finished = finished;
        this.outOfBounds = outOfBounds;
    }
}
