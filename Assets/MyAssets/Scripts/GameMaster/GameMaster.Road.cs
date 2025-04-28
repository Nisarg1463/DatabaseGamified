using UnityEngine;

public partial class GameMaster
{
    void InitRoad()
    {
        // Create a LayerMask for the "Road" layer
        int roadLayer;
        roadLayer = LayerMask.NameToLayer("Road");
        roadLayerMask = 1 << roadLayer;
    }

    void CreateRoad(Vector3 start, Vector3 end)
    {
        // Clear previously picked objects
        foreach (GameObject piece in pickedObjects)
        {
            DestroyImmediate(piece);
        }
        pickedObjects.Clear();

        // Determine the direction of the road (horizontal or vertical)
        if (start.x == end.x) // Vertical road
        {
            float zStart = Mathf.Min(start.z, end.z);
            float zEnd = Mathf.Max(start.z, end.z);

            for (float z = zStart; z <= zEnd; z++)
            {
                Vector3 position = new(start.x, start.y, z);
                PlaceRoadTile(position);
            }
        }
        else if (start.z == end.z) // Horizontal road
        {
            float xStart = Mathf.Min(start.x, end.x);
            float xEnd = Mathf.Max(start.x, end.x);

            for (float x = xStart; x <= xEnd; x++)
            {
                Vector3 position = new(x, start.y, start.z);
                PlaceRoadTile(position);
            }
        }
    }

    public void ToggleRoad()
    {
        if (mode == Mode.Road)
        {
            mode = Mode.None;
            DeleteGrid();
        }
        else
        {
            mode = Mode.Road;
            CreateGrid();
        }
    }

    void PlaceRoadTile(Vector3 position)
    {
        // Check if the position is already occupied by another road tile
        Collider[] hitColliders = Physics.OverlapSphere(position, checkRadius, roadLayerMask);

        if (hitColliders.Length == 0)
        {
            // Instantiate a new road tile at the given position
            GameObject roadTile = Instantiate(map["Road"], position, Quaternion.identity);
            // Add the new tile to the list of picked objects
            pickedObjects.Add(roadTile);
        }
    }
}