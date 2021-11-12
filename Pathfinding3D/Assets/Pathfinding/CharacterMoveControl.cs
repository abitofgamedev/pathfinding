using PathCreation;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointData
{
    public float GScore;
    public float FScore;
    public Vector3Int CameFrom;
    public Vector3Int Coords;
    public float TimeToReach;

    public PointData(Point point)
    {
        GScore = Mathf.Infinity;
        FScore = Mathf.Infinity;
        CameFrom = new Vector3Int(-1, -1, -1);
        Coords = point.Coords;
    }
}

[RequireComponent(typeof(AStarAgent))]
public class CharacterMoveControl : MonoBehaviour
{
    AStarAgent _Agent;
    [SerializeField] Transform _MoveToPoint;

    private void Start()
    {
        _Agent = GetComponent<AStarAgent>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            AStarAgentStatus status= _Agent.Pathfinding(_MoveToPoint.position);
            Debug.Log(status);
        }
    }
}
