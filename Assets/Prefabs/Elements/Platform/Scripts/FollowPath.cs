using System.Collections.Generic;
using UnityEngine;

public class FollowPath : MonoBehaviour
{
    [Header("Assign all waypoints in order (e.g., child objects).")]
    public List<Transform> waypoints = new List<Transform>();

    [Header("Movement speed (units per second).")]
    public float speed = 2f;

    [Header("Should the path loop indefinitely?")]
    public bool loop = true;

    [Header("If looping, should it reverse direction at endpoints (ping-pong)?")]
    public bool pingPong = true;

    private int currentIndex = 0;
    private bool goingBackward = false;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (waypoints.Count < 2)
            return; // Need at least 2 points to move

        Transform targetPoint = waypoints[currentIndex];
        // Move toward the target waypoint
        transform.position = Vector3.MoveTowards(
            transform.position,
            targetPoint.position,
            speed * Time.deltaTime
        );

        // If we reached (or almost reached) the waypoint:
        if (Vector3.Distance(transform.position, targetPoint.position) < 0.01f)
        {
            if (!goingBackward)
            {
                if (currentIndex < waypoints.Count - 1)
                {
                    currentIndex++;
                }
                else
                {
                    // We’re at the last waypoint
                    if (loop)
                    {
                        if (pingPong)
                        {
                            goingBackward = true;
                            currentIndex = waypoints.Count - 2;
                        }
                        else
                        {
                            // Teleport back to the first waypoint
                            transform.position = waypoints[0].position;
                            currentIndex = 1;
                        }
                    }
                    else
                    {
                        enabled = false; // Stop moving once the end is reached
                    }
                }
            }
            else
            {
                // We’re moving backward through the list of waypoints
                if (currentIndex > 0)
                {
                    currentIndex--;
                }
                else
                {
                    // We’re at the first waypoint again
                    if (loop)
                    {
                        if (pingPong)
                        {
                            goingBackward = false;
                            currentIndex = 1;
                        }
                        else
                        {
                            // Teleport to the last waypoint
                            transform.position = waypoints[waypoints.Count - 1].position;
                            currentIndex = waypoints.Count - 2;
                        }
                    }
                    else
                    {
                        enabled = false;
                    }
                }
            }
        }
    }
}
