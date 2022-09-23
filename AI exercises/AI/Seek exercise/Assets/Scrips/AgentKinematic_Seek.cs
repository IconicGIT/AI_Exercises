using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgentKinematic_Seek : MonoBehaviour
{
    enum MovementMode
    {
        SEEK = 0,
        FLEE = 1

    }

    [SerializeField]
    GameObject target;

    [SerializeField]
    float maxVelocity;

    [SerializeField]
    float minDistance;

    [SerializeField]
    float turnSpeed;

    [SerializeField]
    MovementMode movementMode;
    
    


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 direction = Vector3.zero;
        Vector3 movement = Vector3.zero;
        float angle = 0;
        float distance = Vector3.Distance(target.transform.position, transform.position);

        direction = target.transform.position - transform.position;
        direction.y = 0f;    // (x, z): position in the floor

        switch (movementMode)
        {
            case MovementMode.SEEK:

                // Seek
                if (distance > minDistance)
                {
                movement = direction.normalized * maxVelocity * Time.deltaTime;
                }


                break;

            case MovementMode.FLEE:

                // Flee
                movement = direction.normalized * maxVelocity;

                break;

        }


        angle = Mathf.Rad2Deg * Mathf.Atan2(movement.x, movement.z);
        Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.up);  // up = y

        if ((distance > minDistance) || movementMode == MovementMode.FLEE)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * turnSpeed);
            transform.position += transform.forward.normalized * maxVelocity * Time.deltaTime;
        }

        



    }
}
