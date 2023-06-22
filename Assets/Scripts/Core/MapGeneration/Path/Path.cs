using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mudfish
{
    public class Path : IEnumerator
    {
        public List<PathNode> nodes;

        public Path() { nodes = new List<PathNode>(); }

        public int Count => nodes.Count;


        int position = -1;
        public object Current => this[position];

        public PathNode this[int index]
        {
            get
            {
                if (index >= Count) return new PathNode(nodes[Count - 1].position);
                else return nodes[index];
            }
        }

        public IEnumerator GetEnumerator() { return (IEnumerator)this; }

        public bool MoveNext()
        {
            position++;
            return (position < nodes.Count);
        }

        public void Reset() { position = -1; }
    }
}