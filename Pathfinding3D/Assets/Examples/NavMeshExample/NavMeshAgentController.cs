using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NavMeshAgentController : MonoBehaviour
{
    [SerializeField] NavMeshAgent _Agent;
    [SerializeField] List<Transform> _Destination;
    int count = 0;
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            _Agent.SetDestination(_Destination[count].position);
            count++;
            if (count >= _Destination.Count)
            {
                count = 0;
            }
        }
    }

}
