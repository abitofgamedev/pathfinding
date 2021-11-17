using PathCreation;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AStarAgent))]
public class CharacterMoveAB : MonoBehaviour
{
    AStarAgent _Agent;
    [SerializeField] Transform pointA;
    [SerializeField] Transform pointB;

    private void Start()
    {
        _Agent = GetComponent<AStarAgent>();
        transform.position = pointA.position;
        StartCoroutine(Coroutine_MoveAB());
    }

    IEnumerator Coroutine_MoveAB()
    {
        yield return null;
        while (true)
        {
            _Agent.Pathfinding(pointB.position);
            while (_Agent.Status == AStarAgentStatus.Invalid)
            {
                Transform pom1 = pointA;
                pointA = pointB;
                pointB = pom1;
                transform.position = pointA.position;
                _Agent.Pathfinding(pointB.position);
                yield return new WaitForSeconds(0.2f);
            }
            while (_Agent.Status != AStarAgentStatus.Finished)
            {
                yield return null;
            }
            Transform pom = pointA;
            pointA = pointB;
            pointB = pom;
            yield return null;
        }
    }
}
