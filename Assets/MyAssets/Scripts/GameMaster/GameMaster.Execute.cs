using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public partial class GameMaster
{

    public List<string> questions;
    public List<int> cols;
    private Label prompt;
    public void Execute(string command, Label hint, TextField question)
    {
        this.prompt = hint;
        // string[] tokens = command.Split(' ');

        // switch (tokens[0])
        // {
        //     case "c":
        //         Debug.Log("create");
        //         prompt.value = "";
        //         Create(tokens[1], int.Parse(tokens[2]));
        //         break;
        //     case "s":
        //         Debug.Log("select");
        //         prompt.value = "";
        //         Select(tokens[1], tokens[2]);
        //         break;
        //     case "i":
        //         Debug.Log("insert");
        //         prompt.value = "";
        //         Insert(tokens[1], tokens[2]);
        //         break;
        //     case "d":
        //         Debug.Log("delete");
        //         prompt.value = "";
        //         Delete(tokens[1], tokens[2]);
        //         break;
        //     case "t":
        //         Debug.Log("test");
        //         prompt.value = "";
        //         Travel(tokens[1], tokens[2]);
        //         break;
        //     default:
        //         prompt.value = "";
        //         Debug.LogError("Invalid command: " + tokens[0]);
        //         break;
        // }
        SQLValidationAPI sQLValidationAPI = GetComponent<SQLValidationAPI>();
        StartCoroutine(sQLValidationAPI.ValidateSQL(question.text, command));
    }

    public void Travel(string table1, string table2)
    {
        if (buildings[table1].road == null || buildings[table1].road == null)
        {
            prompt.text = "Buildings are not connected.";
            return;
        }
        GameObject start = buildings[table1].road;
        GameObject end = buildings[table2].road;

        start.TryGetComponent(out RoadManager roadManager);
        roadNetwork = new();
        visited = new();
        roadManager.CreateEntry(DirectionData.None, roadNetwork, visited, null);
        GameObject follower = Instantiate(map["Follower"], start.transform.position, Quaternion.identity);
        Follower follow = follower.GetComponent<Follower>();
        if (follow != null)
        {
            Debug.Log("following...");
            follow.waypoints = roadNetwork.FindPath(start, end).ToArray();
            follow.StartFollowing();
        }
    }

    public void Delete(string tableName, string id)
    {
        if (!buildings[tableName].ids.Contains(id))
        {
            prompt.text = $"ID '{id}' doesn't exists for table '{tableName}'.";
            return;
        }
        int index = buildings[tableName].ids.IndexOf(id);
        buildings[tableName].ids.RemoveAt(index);
        Debug.Log("index: " + index);
        Debug.Log("length: " + buildings[tableName].floors.Count);
        foreach (GameObject block in buildings[tableName].floors[index])
        {
            Destroy(block);
        }
        buildings[tableName].floors.RemoveAt(index);
    }

    public void Create(string tableName, int cols)
    {
        buildable = cols;
        Debug.Log(cols + " " + tableName);
        if (!buildings.ContainsKey(tableName))
            ToggleBuilding(tableName, "1");
        else
            prompt.text = $"Table '{tableName}' already exists.";
    }

    public void Select(string tableName, string id)
    {
        if (!buildings[tableName].ids.Contains(id))
        {
            prompt.text = $"ID '{id}' doesn't exists for building '{tableName}'.";
            return;
        }
        int index = buildings[tableName].ids.IndexOf(id);
        foreach (GameObject block in buildings[tableName].floors[index])
        {
            block.GetComponent<Renderer>().material.color = Color.red;
        }
    }

    public void Insert(string tableName, string id)
    {
        if (buildings[tableName].ids.Contains(id))
        {
            prompt.text = $"ID '{id}' already exists for table '{tableName}'.";
            return;
        }
        buildings[tableName].ids.Add(id);
        List<GameObject> floor = new();
        GameObject temp;
        foreach (GameObject block in buildings[tableName].floors[^1])
        {
            temp = Instantiate(map["BuildingBlock"], block.transform.position + new Vector3(0, 5f, 0), Quaternion.identity);
            Rigidbody rb = temp.AddComponent<Rigidbody>();
            rb.useGravity = true;
            rb.constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotation;
            floor.Add(temp);
        }
        buildings[tableName].floors.Add(floor);
    }

    public void Failed(string suggestion)
    {
        prompt.text = suggestion;
    }
}