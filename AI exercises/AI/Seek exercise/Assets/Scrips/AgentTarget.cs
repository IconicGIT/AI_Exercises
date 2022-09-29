using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System.Linq;
using System;

public class AgentTarget : MonoBehaviour
{

    [SerializeField]
    NavMeshAgent me;
    [SerializeField]
    NavMeshAgent target;

    [SerializeField]
    float timerRef;
    [SerializeField]
    float timer = 0;
    float timerDecrease = 1;

    [SerializeField]
    [Range(0,50)]
    float walkDistance;

    [SerializeField]
    float walkRadius;

    [SerializeField]
    bool wander;

    [SerializeField]
    bool hide;

    GameObject[] hidingSpots;

    enum FleeOrPursueMode
    {
        NONE,
        PURSUE,
        FLEE
    }

    [SerializeField]
    FleeOrPursueMode fleeOrPursue;

    bool Timer()
    {
        if (timer > 0)
        {

            timer -= timerDecrease;
            return false;
        }
        else
        {
            timer = timerRef;
            return true;
        }


    }

    void Seek(Vector3 destination)
    {
        me.destination = destination;
        hidingSpots = GameObject.FindGameObjectsWithTag("hide");
    }

    void Wander()
    {
        //Vector3 randomDirection = Random.insideUnitSphere * walkDistance;

        //randomDirection += transform.position;
        //NavMeshHit hit;
        //NavMesh.SamplePosition(randomDirection, out hit, walkDistance, 1);
        //targetPoint = hit.position;
        //me.destination = targetPoint;



        float radius = walkRadius;
        float offset = walkDistance;

        Vector3 localTarget = UnityEngine.Random.insideUnitCircle * radius;
        localTarget += new Vector3(0, 0, walkDistance);

        Vector3 worldTarget = transform.TransformPoint(localTarget);
        worldTarget.y = 0f;

        me.destination = worldTarget;

        NavMeshHit hit;
        if (NavMesh.SamplePosition(worldTarget, out  hit, radius, NavMesh.AllAreas))
        {
            Seek(hit.position);
        }
        else
        {
            worldTarget = transform.TransformPoint(-localTarget);
            worldTarget.y = 0f;
            if (NavMesh.SamplePosition(worldTarget, out hit, radius, NavMesh.AllAreas))
            {
                Seek(hit.position);
            }
        }

        //Debug.Log("hit: " + me.destination);

    }



    void FleeOrPursue()
    {


        Vector3 targetDir = target.transform.position - transform.position;
        float lookAhead = targetDir.magnitude / me.speed;

        switch (fleeOrPursue)
        {
            case FleeOrPursueMode.PURSUE:

                
                Seek(target.transform.position + target.transform.forward * lookAhead);
                break;

            case FleeOrPursueMode.FLEE:

                FleeFrom(target.transform.position);
                break;

        }
    }

    void FleeFrom(Vector3 position)
    {
        Vector3 fleeVector = position - transform.position;
        Seek(transform.position - fleeVector);
    }

    void Hide()
    {
        Func<GameObject, float> distance =
            (hs) => Vector3.Distance(target.transform.position,
                                     hs.transform.position);
        GameObject hidingSpot = hidingSpots.Select(
            ho => (distance(ho), ho)
            ).Min().Item2;
        Vector3 dir = hidingSpot.transform.position - target.transform.position;
        Ray backRay = new Ray(hidingSpot.transform.position, -dir.normalized);
        RaycastHit info;
        hidingSpot.GetComponent<Collider>().Raycast(backRay, out info, 50f);
        Seek(info.point + dir.normalized);
    }

    // Start is called before the first frame update
    void Start()
    {
        timer = 0;
    }

    // Update is called once per frame
    void Update()
    {
        
        if (wander)
        { 
            if (Timer() ) Wander();
        }

        if (fleeOrPursue != FleeOrPursueMode.NONE)
        {
            FleeOrPursue();
        }

    }
}
