using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;


public partial class GameMaster : MonoBehaviour
{
    public GameObject gridPiece;
    public GameObjectMap gameObjectMap;
    public GameObject centralRoad = null;
    private Transform groundPiece;
    private Dictionary<string, GameObject> map;
    private readonly List<GameObject> grid = new();
    private GameObject lastHoveredObject;
    private GameObject pickedObject;
    private GameObject roadStart;
    private readonly List<GameObject> pickedObjects = new();
    private Graph roadNetwork;
    private Graph visited;
    private int mode = Mode.None;
    private readonly float checkRadius = 0.1f;
    private LayerMask roadLayerMask;
    private int buildable = 6;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // layerMask = LayerMask.GetMask("Grid");
        map = gameObjectMap.ToDictionary();
        gameObjectMap.Clear();
        groundPiece = transform.Find("Ground");
        InitRoad();
    }

    // Update is called once per frame
    void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Input.GetKeyDown(KeyCode.B))
        {
            ToggleBuilding(null, null);
        }
        if (Input.GetKeyDown(KeyCode.G))
        {
            mode = Mode.Transport;
            pickedObjects.Clear();
            if (roadStart != null)
            {
                Destroy(roadStart);
                roadStart = null;
            }

            centralRoad.TryGetComponent(out RoadManager roadManager);
            roadNetwork = new();
            visited = new();
            roadManager.CreateEntry(DirectionData.None, roadNetwork, visited, null);
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            ToggleRoad();
        }
        if (Input.GetMouseButtonDown(0))
        {
            HandleClick();
        }

        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity))
        {
            GameObject hoveredObject = hit.collider.gameObject;
            if (hoveredObject != null && !(lastHoveredObject != null && hoveredObject.transform.position == lastHoveredObject.transform.position))
            {
                if (mode == Mode.Road && roadStart != null)
                {
                    if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Grid") || hit.collider.gameObject.layer == LayerMask.NameToLayer("Road") || hit.collider.gameObject.layer == LayerMask.NameToLayer("Node"))
                    {
                        CreateRoad(roadStart.transform.position, hoveredObject.transform.position);
                    }
                }
                else if (mode == Mode.Building && roadStart != null)
                {

                    if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Grid") && buildable - Vector3.Distance(hoveredObject.transform.position, roadStart.transform.position) > 0)
                    {
                        CreateBuilding(roadStart.transform.position, hoveredObject.transform.position);
                    }
                }
                lastHoveredObject = hoveredObject;
            }
        }
    }

    private void HandleClick()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (EventSystem.current.IsPointerOverGameObject()) return;
        if (mode == Mode.Road)
        {
            if (roadStart == null && lastHoveredObject != null && lastHoveredObject.layer != LayerMask.NameToLayer("Road"))
            {
                roadStart = Instantiate(map["Road"], lastHoveredObject.transform.position, Quaternion.identity);
            }
            else
            {
                centralRoad = roadStart;
                roadStart = null;
                pickedObjects.Clear();
            }
        }
        else if (mode == Mode.Transport)
        {
            if (Physics.Raycast(ray, out RaycastHit hit2, Mathf.Infinity))
            {
                if (hit2.collider.gameObject.layer == LayerMask.NameToLayer("Road"))
                {
                    if (hit2.collider.gameObject.GetComponent<RoadManager>().IsIntersection)
                    {
                        if (roadStart == null)
                        {
                            roadStart = hit2.collider.gameObject;
                        }
                        else
                        {
                            List<GameObject> path = roadNetwork.FindPath(roadStart, hit2.collider.gameObject);
                            if (path != null)
                            {
                                Debug.Log("Path found:");
                                foreach (GameObject obj in path)
                                {
                                    Debug.Log(obj.transform.position);
                                }
                                GameObject follower = Instantiate(map["Follower"], roadStart.transform.position, Quaternion.identity);
                                Follower follow = follower.GetComponent<Follower>();
                                if (follow != null)
                                {
                                    follow.waypoints = path.ToArray();
                                    follow.StartFollowing();
                                }
                            }
                            else
                            {
                                Debug.Log("No path found.");
                            }
                        }
                    }
                }
            }

        }
        else if (mode == Mode.Building)
        {
            if (roadStart == null)
            {
                pickedObject = Instantiate(map["BuildingBlock"]);
                roadStart = pickedObject;
                ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out RaycastHit hit2, Mathf.Infinity))
                {
                    GameObject hoveredObject = hit2.collider.gameObject;
                    if (hoveredObject != null)
                    {
                        if (hoveredObject.layer == LayerMask.NameToLayer("Grid") && pickedObject != null)
                        {
                            pickedObject.transform.position = hoveredObject.transform.position;
                        }
                    }
                }
            }
            if (buildable == pickedObjects.Count)
            {
                ToggleBuilding(null, null);
            }
        }
    }
}
