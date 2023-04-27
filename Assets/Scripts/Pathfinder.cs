using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pathfinder : MonoBehaviour
{
    [Range(0f, 2f)]
    [SerializeField] private float pointSize = 1f;

    //Create Waypoints for pathfinding
    private void OnDrawGizmos()
    {
        //Create a waypoint for each child object
        foreach (Transform t in transform)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(t.position, pointSize);
        }

        //Draw lines conecting waypoints
        Gizmos.color = Color.blue;
        for (int i = 0; i < transform.childCount - 1; i++)
        {
            Gizmos.DrawLine(transform.GetChild(i).position, transform.GetChild(i + 1).position);
        }
        Gizmos.DrawLine(transform.GetChild(transform.childCount - 1).position, transform.GetChild(0).position);
    }


    public Transform GetPoint(Transform point)
    {
        //If there is no current next point, start from the begining
        if(point == null)
        {
            return transform.GetChild(0);
        }

        //Get next point
        if (point.GetSiblingIndex() < transform.childCount - 1)
        {
            return transform.GetChild(point.GetSiblingIndex() + 1);
        }
        //Otherwise start over
        else
        {
            return transform.GetChild(0);
        }
    }
}
