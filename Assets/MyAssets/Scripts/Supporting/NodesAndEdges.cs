using UnityEngine;
using System.Collections.Generic;

public class AStarNode
{
    public GameObject GameObject { get; set; }
    public List<Edge> Edges { get; set; } = new List<Edge>();
    public float g { get; set; } // Cost from start to this node
    public float h { get; set; } // Heuristic cost from this node to goal
    public float f { get; set; } // Total cost (g + h)
    public AStarNode Parent { get; set; } // Parent node in the path

    public AStarNode(GameObject gameObject)
    {
        GameObject = gameObject;
    }
}

public class Edge
{
    public AStarNode StartNode { get; set; }
    public AStarNode EndNode { get; set; }
    public float Weight { get; set; } // Optional weight for weighted graphs

    public Edge(AStarNode startNode, AStarNode endNode, float weight = 1.0f)
    {
        StartNode = startNode;
        EndNode = endNode;
        Weight = weight;
    }
}
