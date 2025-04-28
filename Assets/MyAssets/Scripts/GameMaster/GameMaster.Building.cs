using System;
using System.Collections.Generic;
using UnityEngine;

public class Building
{
    public List<List<GameObject>> floors = new();
    public List<string> ids;
    public GameObject road = null;
}


public partial class GameMaster
{
    public Dictionary<string, Building> buildings = new();
    private string buildingName = null;

    void ToggleBuilding(string tableName, string id)
    {
        if (mode != Mode.Building)
        {
            buildingName = tableName;
            CreateGrid();
            mode = Mode.Building;
        }
        else
        {
            Building building = new();
            building.floors.Add(new List<GameObject>());
            mode = Mode.None;
            Destroy(pickedObject);
            roadStart = null;
            foreach (GameObject gameObject in pickedObjects)
            {
                if (gameObject != null)
                {
                    gameObject.TryGetComponent(out Placement placement);
                    if (placement != null)
                    {
                        gameObject.layer = LayerMask.NameToLayer("Building");
                        if (building.floors[0].Count == 0) placement.canConnect = true;
                        else placement.canConnect = false;
                        building.floors[0].Add(gameObject);
                        placement.GenerateBorder(building);
                    }
                }
            }
            building.ids = new()
            {
                id
            };
            buildings[buildingName] = building;
            pickedObjects.Clear();
            DeleteGrid();
        }
    }
    void CreateBuilding(Vector3 start, Vector3 end)
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
                if (!PlaceBuidingTile(position))
                {
                    break;
                }
            }
        }
        else if (start.z == end.z) // Horizontal road
        {
            float xStart = Mathf.Min(start.x, end.x);
            float xEnd = Mathf.Max(start.x, end.x);

            for (float x = xStart; x <= xEnd; x++)
            {
                Vector3 position = new(x, start.y, start.z);
                if (!PlaceBuidingTile(position))
                {
                    foreach (GameObject block in pickedObjects)
                    {
                        Destroy(block);
                        pickedObjects.Remove(block);
                    }
                    break;
                }

            }
        }
    }
    bool PlaceBuidingTile(Vector3 position)
    {
        // Check if the position is already occupied by another road tile
        Collider[] hitColliders = Physics.OverlapSphere(position, checkRadius, roadLayerMask);

        if (hitColliders.Length == 0)
        {
            // Instantiate a new road tile at the given position
            GameObject roadTile = Instantiate(map["BuildingBlock"], position, Quaternion.identity);

            // Add the new tile to the list of picked objects
            pickedObjects.Add(roadTile);
            return true;
        }
        return false;
    }
}