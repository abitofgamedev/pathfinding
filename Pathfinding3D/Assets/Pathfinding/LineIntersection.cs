using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineIntersection : MonoBehaviour
{
    [SerializeField] LineRenderer lineRenderer1;
    [SerializeField] Transform _Obj1;
    [SerializeField] Transform _Obj2;
    [SerializeField] LineRenderer lineRenderer2;
    [SerializeField] Transform _Obj3;
    [SerializeField] Transform _Obj4;
    [SerializeField] GameObject IntersectionObj;


    private void Update()
    {
        lineRenderer1.SetPosition(0, _Obj1.transform.position);
        lineRenderer1.SetPosition(1, _Obj2.transform.position);
        lineRenderer2.SetPosition(0, _Obj3.transform.position);
        lineRenderer2.SetPosition(1, _Obj4.transform.position);
        Vector3 outVector;
        if (LineLineIntersection(out outVector, _Obj1.transform.position,
             _Obj2.transform.position - _Obj1.transform.position,
             _Obj3.transform.position,
             _Obj4.transform.position - _Obj3.transform.position))
        {
            IntersectionObj.SetActive(true);
            IntersectionObj.transform.position = outVector;
        }
        else
        {
            IntersectionObj.SetActive(false);
        }
    }

    public static bool LineLineIntersection(out Vector3 intersection, Vector3 linePoint1,
       Vector3 lineVec1, Vector3 linePoint2, Vector3 lineVec2)
    {

        Vector3 lineVec3 = linePoint2 - linePoint1;
        Vector3 crossVec1and2 = Vector3.Cross(lineVec1, lineVec2);
        Vector3 crossVec3and2 = Vector3.Cross(lineVec3, lineVec2);

        float planarFactor = Vector3.Dot(lineVec3, crossVec1and2);

        //is coplanar, and not parallel
        if (Mathf.Abs(planarFactor) < 0.0001f
                && crossVec1and2.sqrMagnitude > 0.0001f)
        {
            float s = Vector3.Dot(crossVec3and2, crossVec1and2) / crossVec1and2.sqrMagnitude;
            intersection = linePoint1 + (lineVec1 * s);
            Vector3 point2 = linePoint1 + lineVec1;
            Vector3 dir1 = intersection - linePoint1;
            Vector3 dir2 = intersection - point2;

            Vector3 point4 = linePoint2 + lineVec2;

            Vector3 dir3 = intersection - linePoint2;
            Vector3 dir4 = intersection - point4;

            return Vector3.Dot(dir1, dir2) < 0 && Vector3.Dot(dir3,dir4)<0;
        }
        else
        {
            intersection = Vector3.zero;
            return false;
        }
    }

}
