using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Follower : MonoBehaviour
{
    public GameObject[] waypoints; // List of waypoints
    public float speed = 5.0f; // Speed of movement
    private int waypointIndex = 0; // Current waypoint index

    public void StartFollowing()
    {
        // Check if waypoints are assigned
        if (waypoints.Length == 0)
        {
            Debug.LogError("No waypoints assigned.");
            return;
        }

        // Start moving towards the first waypoint
        StartCoroutine(MoveToWaypoints());
    }

    IEnumerator MoveToWaypoints()
    {
        while (waypointIndex < waypoints.Length)
        {
            Transform target = waypoints[waypointIndex].transform;

            // Move towards the current waypoint
            while (Vector3.Distance(transform.position, target.position) > 0.1f)
            {
                transform.position = Vector3.MoveTowards(transform.position, target.position, Time.deltaTime * speed);
                yield return null;
            }

            // Move to the next waypoint
            waypointIndex++;
        }

        Destroy(gameObject);

        // Optional: Loop back to the start
        // waypointIndex = 0;
        // StartCoroutine(MoveToWaypoints());
    }
}
