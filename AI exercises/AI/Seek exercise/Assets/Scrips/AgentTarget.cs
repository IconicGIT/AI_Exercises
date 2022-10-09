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
    float walkDistance;

    [SerializeField]
    [Range(0, 50)]
    float maxWalkDistance;

    [SerializeField]
    float walkRadius;

    [SerializeField]
    GameObject[] hidingSpots;

    [SerializeField]
    bool debugPointWander;

    [SerializeField]
    bool debugHide;

    [SerializeField]
    Vector3 st, end;

    enum MovementMode
    {
        NONE,
        PURSUE,
        FLEE,
        WANDER,
        HIDE,
    }

    [SerializeField]
    MovementMode movMode;

    bool Timer()
    {
        if (timer > 0)
        {

            timer -= timerDecrease;
            return false;
        }
        else
        {
            timer = (int)UnityEngine.Random.Range(timerRef - timerRef/2, timerRef + timerRef/2);
           
            return true;
        }


    }

    void Seek(Vector3 destination)
    {
        me.destination = destination;
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

        Vector3 localTarget = UnityEngine.Random.insideUnitCircle * radius;
        localTarget += new Vector3(0, 0, walkDistance);


        

        Vector3 worldTarget = transform.TransformPoint(localTarget);

        worldTarget.y = 0f;

        st = transform.position;
        end = worldTarget;

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

    void FleeFrom(Vector3 position)
    {
        Vector3 fleeVector = position - transform.position;
        Seek(transform.position - fleeVector);
    }

    void Hide()
    {
        Func<GameObject, float> distance = (hs) => Vector3.Distance(target.transform.position, hs.transform.position);

        GameObject hidingSpot = hidingSpots.Select(ho => (distance(ho), ho)).ElementAt(UnityEngine.Random.Range(0, hidingSpots.Length - 1)).Item2;


        Vector3 dir = hidingSpot.transform.position - target.transform.position * 2;

        //print("target.transform.position: " + target.transform.position * 2);

        Ray backRay = new Ray(hidingSpot.transform.position, -dir.normalized);

        st = hidingSpot.transform.position;
        end = -dir;

        RaycastHit info;
        hidingSpot.GetComponent<Collider>().Raycast(backRay, out info, 50f);

        Vector3 destination = hidingSpot.transform.position + info.point + dir.normalized;
        destination.y = 0;
        Seek(destination);
        print("me.destination: " + me.destination);
        print("destination: " + destination);
        //print("hidingSpot: " + hidingSpot);
        //print("info.point + dir.normalized: " + " " + info.point + " " + dir.normalized + " " + (info.point + dir.normalized));

    }

    // Start is called before the first frame update
    void Start()
    {

        timer = 0;
        hidingSpots = GameObject.FindGameObjectsWithTag("hide");

    }

    // Update is called once per frame
    void Update()
    {
        
        

        

        switch (movMode)
        {
            case MovementMode.NONE:
                break;

            case MovementMode.PURSUE:

                Vector3 targetDir = target.transform.position - transform.position;
                float lookAhead = targetDir.magnitude / me.speed;

                Seek(target.transform.position + target.transform.forward * lookAhead);
                break;

            case MovementMode.FLEE:

                FleeFrom(target.transform.position);
                break;

            case MovementMode.WANDER:

               
                if (Timer())
                {
                    Wander();
                    walkDistance = (int)UnityEngine.Random.Range(maxWalkDistance - maxWalkDistance / 2, maxWalkDistance + maxWalkDistance / 2);
                }

               if (debugPointWander) Debug.DrawRay(st, end, Color.red);


                break;

            case MovementMode.HIDE:

                if (Timer())
                {
                    Hide();
                }

                if (debugHide)
                {
                    Debug.DrawLine(st, end, Color.red);
                }

                break;
        }

    }
}
