using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[System.Serializable]
public struct Link
{
    public enum Direction
    {
        UNI,
        BI
    }

    public GameObject node1;
    public GameObject node2;
    public Direction dir;

}

public class WayPointManager : MonoBehaviour
{
    public GameObject[] waypoints;
    public Link[] links;
    public Graph graph = new Graph();
    public int CurrentNumberPoints { get; private set; }
    public GameObject CurrentWayPoint { get; private set; }

    public Vector3 GetLocationOfPoint(int index)
    {
        return   CurrentWayPoint.GetComponent<WayPointDebug>().GetLocationOfPoint(index);
    }
    
    void Start()
    {
        CurrentWayPoint = GameObject.FindWithTag("wp");
        CurrentNumberPoints = CurrentWayPoint.GetComponent<WayPointDebug>().currentNumberPoints;
        CurrentWayPoint.GetComponent<WayPointDebug>().GetLocationOfPoint(0);
        
        /**
        if (waypoints.Length > 0)
        {
            foreach (var el in waypoints)
            {
                graph.AddNode(el);
            }

            foreach (var el in links)
            {
                graph.AddEdge(el.node1, el.node2);
                if (el.dir == Link.Direction.BI)
                {
                    graph.AddEdge(el.node2, el.node1);
                }
            }
        }
        **/
    }
    
    
    void Update()
    {
    //    graph.debugDraw();
    }
}
