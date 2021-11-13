using PathCreation;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CurvePloter : MonoBehaviour
{
    [SerializeField] List<Transform> Points;
    [SerializeField] PathCreator pathCreator;

    [SerializeField] Transform movingPoint;
    [SerializeField] Transform turningPoint;
    [SerializeField] Transform targetPoint;

    private void Start()
    {
        BezierPath bezierPath = new BezierPath(Points, false, PathSpace.xyz);
        pathCreator.bezierPath = bezierPath;
    }

    private void Update()
    {
        Vector3 turnDirection = (targetPoint.position - movingPoint.position);

        float angleRight = Vector3.Angle(movingPoint.right, turnDirection);
        int turnRight = angleRight > 90 ? -1 : 1;
        turningPoint.position = movingPoint.position + movingPoint.right * turnRight * 2.5f;
        turningPoint.right = movingPoint.right;

        Vector3 turningPointDirection = (targetPoint.position - turningPoint.position);
        Vector3 movingPointDirection = (movingPoint.position - turningPoint.position);

        float angle = Vector3.Angle(movingPointDirection, turningPointDirection);
        float totalAngle = 360-angle*2;



        for(int i = 1; i < totalAngle; i++)
        {
            Vector3 p1 = turningPoint.position + (turningPoint.right * -turnRight * 2.5f);
            turningPoint.localRotation = Quaternion.Euler(0, -i, 0);
            Vector3 p2 = turningPoint.position + (turningPoint.right * -turnRight * 2.5f);
            Debug.DrawLine(p1, p2, Color.red);
        }
    }
}
