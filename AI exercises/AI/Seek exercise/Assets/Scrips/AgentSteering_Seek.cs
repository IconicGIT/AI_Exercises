using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AgentSteering_Seek : MonoBehaviour
{

    public NavMeshAgent agent;
    public GameObject target;
    void Seek()
    {
        agent.destination = target.transform.position;
    }

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("hola");
    }

    // Update is called once per frame
    void Update()
    {
        Seek();
    }
}
