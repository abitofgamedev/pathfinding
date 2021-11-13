using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyingController : MonoBehaviour
{
    AStarAgent _Agent;
    [SerializeField] Transform _MoveToPoint;
    [SerializeField] Animator _Anim;
    [SerializeField] AnimationCurve _SpeedCurve;
    [SerializeField] float _Speed;
    private void Start()
    {
        _Agent = GetComponent<AStarAgent>();
        StartCoroutine(Coroutine_MoveRandom());
    }

    //private void Update()
    //{
    //    if (Input.GetKeyDown(KeyCode.Space))
    //    {
    //        _Agent.Pathfinding(_MoveToPoint.position);
    //        StopAllCoroutines();
    //        StartCoroutine(Coroutine_Animation());
    //    }
    //}

    IEnumerator Coroutine_MoveRandom()
    {
        List<Point> freePoints = WorldManager.Instance.GetFreePoints();
        Point start = freePoints[Random.Range(0, freePoints.Count)];
        transform.position = start.WorldPosition;
        while (true)
        {
            Point p = freePoints[Random.Range(0, freePoints.Count)];

            //Vector3 turnDirection = (p.WorldPosition - transform.position);
            //float totalAngle = Vector3.Angle(transform.forward, turnDirection);
            //float angleRight = Vector3.Angle(transform.right, turnDirection);
            //int turnRight = angleRight > 90 ? -1 : 1;
            //Vector3 circleCenter = transform.position + transform.right * turnRight * 2.5f;

            //Vector3 turnPosition=circleCenter+Quaternion.Euler(0,1,0) * (transform.right * turnRight * 2.5f)
            //Vector3 direction = (p.WorldPosition - transform.position).normalized;
            //Vector3 startDir = transform.forward;
            //float lerp = 0;
            //while (lerp < 1)
            //{
            //    transform.position += transform.forward * _Agent.Speed * Time.deltaTime;
            //    transform.forward = Vector3.Slerp(startDir, direction, lerp);
            //    lerp += Time.deltaTime;
            //    yield return null;
            //}

            _Agent.Pathfinding(p.WorldPosition);
            //_Anim.SetBool("Flying", true);
            while (_Agent.Status != AStarAgentStatus.Finished)
            {
                yield return null;
            }
            //transform.up = Vector3.up;
            //_Anim.SetBool("Flying", false);
            //yield return new WaitForSeconds(1f);
        }
    }

    IEnumerator Coroutine_Animation()
    {
        _Anim.SetBool("Flying", true);
        while (_Agent.Status != AStarAgentStatus.Finished)
        {
            yield return null;
        }
        _Anim.SetBool("Flying", false);
    }
}
