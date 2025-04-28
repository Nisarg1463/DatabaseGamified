using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Graph
{
    private List<AStarNode> nodes = new List<AStarNode>();

    public void AddNode(GameObject gameObject)
    {
        AStarNode newNode = new AStarNode(gameObject);
        nodes.Add(newNode);
    }

    public bool Contains(GameObject other)
    {
        foreach (AStarNode node in nodes)
        {
            if (node.GameObject == other)
            {
                return true;
            }
        }
        return false;
    }

    public void AddEdge(GameObject startObject, GameObject endObject, float weight = 1.0f)
    {
        AStarNode startNode = nodes.FirstOrDefault(n => n.GameObject == startObject);
        AStarNode endNode = nodes.FirstOrDefault(n => n.GameObject == endObject);

        if (startNode != null && endNode != null)
        {
            Edge newEdge = new Edge(startNode, endNode, weight);
            startNode.Edges.Add(newEdge);
        }
        else
        {
            Debug.LogError("Both nodes must exist in the graph.");
        }
    }

    public List<GameObject> FindPath(GameObject startObject, GameObject goalObject)
    {
        AStarNode startNode = nodes.FirstOrDefault(n => n.GameObject == startObject);
        AStarNode goalNode = nodes.FirstOrDefault(n => n.GameObject == goalObject);

        if (startNode == null || goalNode == null)
        {
            Debug.LogError("Both start and goal nodes must exist in the graph.");
            return null;
        }

        return AStar(startNode, goalNode);
    }

    private List<GameObject> AStar(AStarNode startNode, AStarNode goalNode)
    {
        List<AStarNode> openList = new List<AStarNode>();
        HashSet<AStarNode> closedList = new HashSet<AStarNode>();

        startNode.g = 0;
        startNode.h = CalculateHeuristic(startNode, goalNode);
        startNode.f = startNode.g + startNode.h;
        openList.Add(startNode);

        while (openList.Count > 0)
        {
            AStarNode currentNode = openList.OrderBy(n => n.f).First();
            openList.Remove(currentNode);
            closedList.Add(currentNode);

            if (currentNode == goalNode)
            {
                return ReconstructPath(currentNode);
            }

            foreach (Edge edge in currentNode.Edges)
            {
                AStarNode neighbor = edge.EndNode;

                if (closedList.Contains(neighbor))
                {
                    continue;
                }

                float tentativeG = currentNode.g + CalculateEdgeCost(currentNode, neighbor);

                if (!openList.Contains(neighbor))
                {
                    neighbor.Parent = currentNode;
                    neighbor.g = tentativeG;
                    neighbor.h = CalculateHeuristic(neighbor, goalNode);
                    neighbor.f = neighbor.g + neighbor.h;
                    openList.Add(neighbor);
                }
                else if (tentativeG < neighbor.g)
                {
                    neighbor.Parent = currentNode;
                    neighbor.g = tentativeG;
                    neighbor.f = neighbor.g + neighbor.h;
                }
            }
        }

        return null;
    }

    private float CalculateHeuristic(AStarNode node, AStarNode goalNode)
    {
        return Vector3.Distance(node.GameObject.transform.position, goalNode.GameObject.transform.position);
    }

    private float CalculateEdgeCost(AStarNode fromNode, AStarNode toNode)
    {
        return Vector3.Distance(fromNode.GameObject.transform.position, toNode.GameObject.transform.position);
    }

    private List<GameObject> ReconstructPath(AStarNode goalNode)
    {
        List<GameObject> path = new List<GameObject>();
        AStarNode currentNode = goalNode;

        while (currentNode != null)
        {
            path.Add(currentNode.GameObject);
            currentNode = currentNode.Parent;
        }

        path.Reverse();
        return path;
    }
}

