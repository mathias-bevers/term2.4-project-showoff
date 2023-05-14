using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Path 
{
    public List<PathNode> nodes;

    public Path()
    {
        nodes = new List<PathNode>();
    }

    public int Count => nodes.Count;

    public PathNode this[int index]
    {
        get => nodes[index];
    }
}
