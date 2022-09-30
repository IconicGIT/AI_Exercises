using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlockAgent : MonoBehaviour
{
    public FlockController myManager;
    public Vector3 direction;

    [SerializeField]
    float speed;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 cohesion = Vector3.zero;
        Vector3 align = Vector3.zero;
        Vector3 separation = Vector3.zero;
        int num = 0;
        foreach (GameObject go in myManager.allAgents)
        {
            if (go != this.gameObject)
            {
                float distance = Vector3.Distance(go.transform.position,
                                                  transform.position);
                if (distance <= myManager.neighbourDistance)
                {
                    cohesion += go.transform.position;

                    align += go.GetComponent<FlockAgent>().direction;

                    separation -= (transform.position - go.transform.position) / (distance * distance);


                    num++;
                }



            }
        }
        if (num > 0)
        {
            align /= num;

            speed = Mathf.Clamp(align.magnitude, myManager.minSpeed, myManager.maxSpeed);

            cohesion = (cohesion / num - transform.position).normalized * speed;
        }

        direction = (cohesion + align + separation).normalized * speed;

        transform.rotation = Quaternion.Slerp(transform.rotation,
                                          Quaternion.LookRotation(direction),
                                          myManager.rotationSpeed * Time.deltaTime);

        transform.Translate(0.0f, 0.0f, Time.deltaTime * speed);


    }

}
