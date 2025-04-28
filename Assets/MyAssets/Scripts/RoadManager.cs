using UnityEngine;

public class RoadManager : MonoBehaviour
{
    public bool IsIntersection = false;
    private LayerMask layerMask;
    void Start()
    {
        layerMask = LayerMask.GetMask("Road");
    }
    public void CreateEntry(int directionData, Graph roadNetwork, Graph visited, GameObject Connection)
    {
        int fr = 0, bk = 0, ri = 0, lf = 0;
        if (visited.Contains(gameObject))
        {
            return;
        }
        visited.AddNode(gameObject);

        // Precompute layer mask
        int roadLayer = LayerMask.NameToLayer("Road");
        LayerMask roadLayerMask = 1 << roadLayer;

        // Declare hit variables outside scope
        bool hasForward = false, hasBack = false, hasRight = false, hasLeft = false;

        // Forward check
        Ray ray = new(transform.position, transform.forward);
        if (Physics.Raycast(ray, out RaycastHit forwardHit, 1, roadLayerMask))
        {
            fr++;
            hasForward = true;
        }

        // Back check
        ray = new(transform.position, -transform.forward);
        if (Physics.Raycast(ray, out RaycastHit backHit, 1, roadLayerMask))
        {
            bk++;
            hasBack = true;
        }

        // Right check
        ray = new(transform.position, transform.right);
        if (Physics.Raycast(ray, out RaycastHit rightHit, 1, roadLayerMask))
        {
            ri++;
            hasRight = true;
        }

        // Left check
        ray = new(transform.position, -transform.right);
        if (Physics.Raycast(ray, out RaycastHit leftHit, 1, roadLayerMask))
        {
            lf++;
            hasLeft = true;
        }

        // Existing conditional logic remains the same
        if ((fr + bk + ri + lf) != 2 ||
            ((directionData == DirectionData.Forward || directionData == DirectionData.Back) && lf + ri != 0) ||
            ((directionData == DirectionData.Right || directionData == DirectionData.Left) && fr + bk != 0))
        {
            IsIntersection = true;
        }
        //Update the node color based on intersection status and intersection can be updated from another script as well
        GetComponent<Renderer>().material.color = IsIntersection ? Color.red : Color.green;
        if (IsIntersection)
        {
            roadNetwork.AddNode(gameObject);
            if (Connection != null)
            {
                roadNetwork.AddEdge(gameObject, Connection);
                roadNetwork.AddEdge(Connection, gameObject);
            }
            Connection = gameObject;
        }
        // Recursive calls using raycast results
        if (directionData != DirectionData.Forward && hasForward)
        {
            if (forwardHit.collider.gameObject.TryGetComponent<RoadManager>(out var forwardManager))
            {
                forwardManager.CreateEntry(DirectionData.Back, roadNetwork, visited, Connection);
            }
        }

        if (directionData != DirectionData.Back && hasBack)
        {
            if (backHit.collider.gameObject.TryGetComponent<RoadManager>(out var backManager))
            {
                backManager.CreateEntry(DirectionData.Forward, roadNetwork, visited, Connection);
            }
        }

        if (directionData != DirectionData.Right && hasRight)
        {
            if (rightHit.collider.gameObject.TryGetComponent<RoadManager>(out var rightManager))
            {
                rightManager.CreateEntry(DirectionData.Left, roadNetwork, visited, Connection);
            }
        }

        if (directionData != DirectionData.Left && hasLeft)
        {
            if (leftHit.collider.gameObject.TryGetComponent<RoadManager>(out var leftManager))
            {
                leftManager.CreateEntry(DirectionData.Right, roadNetwork, visited, Connection);
            }
        }


    }
}
