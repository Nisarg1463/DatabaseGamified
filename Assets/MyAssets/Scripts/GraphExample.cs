using System.Collections.Generic;
using UnityEngine;

public class GraphExample : MonoBehaviour
{
    private Graph graph;

    void Start()
    {
        graph = new Graph();

        // Create GameObjects
        GameObject obj1 = new("Object1");
        GameObject obj2 = new("Object2");
        GameObject obj3 = new("Object3");

        // Add nodes to the graph
        graph.AddNode(obj1);
        graph.AddNode(obj2);
        graph.AddNode(obj3);

        // Add edges between nodes
        graph.AddEdge(obj1, obj2);
        graph.AddEdge(obj2, obj3);
        graph.AddEdge(obj3, obj1);

        // Find path
        List<GameObject> path = graph.FindPath(obj1, obj3);

        if (path != null)
        {
            Debug.Log("Path found:");
            foreach (GameObject obj in path)
            {
                Debug.Log(obj.name);
            }
        }
        else
        {
            Debug.Log("No path found.");
        }
        Debug.Log(graph.Contains(obj1));
    }
}
