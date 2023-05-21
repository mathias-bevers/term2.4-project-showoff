using UnityEngine;

public struct PathNode
{
    public Vector3 position;
    public bool isEnd;
    public bool isDeath;

    public bool shouldProbablyRequestPathUpdate;
    public bool choiceNode;

    public PathNode(Vector3 position) : this(position, false, false, true) { }
    public PathNode(Vector3 position, bool isEnd) : this(position, isEnd, false, true) { }
    public PathNode(Vector3 position, bool isEnd, bool isDeath) : this(position, isEnd, isDeath, true) { }
    public PathNode(Vector3 position, bool isEnd, bool isDeath, bool shouldProbablyRequestPathUpdate) : this(position, isEnd, isDeath, shouldProbablyRequestPathUpdate, false) { }
    public PathNode(Vector3 position, bool isEnd, bool isDeath, bool shouldProbablyRequestPathUpdate, bool choiceNode)
    {
        this.position = position;
        this.isEnd = isEnd;
        this.isDeath = isDeath;
        this.shouldProbablyRequestPathUpdate = shouldProbablyRequestPathUpdate;
        this.choiceNode = choiceNode;
    }
}