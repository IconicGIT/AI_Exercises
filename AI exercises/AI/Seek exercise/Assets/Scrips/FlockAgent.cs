using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlockAgent : MonoBehaviour
{
    public FlockController myManager;
    public Vector3 direction;

    public Vector3 cohesion = Vector3.zero;
    public Vector3 align = Vector3.zero;
    public Vector3 separation = Vector3.zero;

    [SerializeField]
    Vector3 center;

    [SerializeField]
    float distanceToCenterMultiplier;

    [SerializeField]
    bool debugPig;

    public float speed;

    [SerializeField]
    [Range(0.0f, 5.0f)]
    float timerRef;

    [SerializeField]
    float timerRefVar;
    float timer;

    [SerializeField]
    [Range(0, 180)]
    float randomAdditionalAngle;

    [SerializeField]
    [Range(0, 200)]
    float separationMultiplier;

    [SerializeField]
    GameObject point;

    // Start is called before the first frame update
    void Start()
    {
        if (debugPig) point = Instantiate(point);
    }

    // Update is called once per frame
    void Update()
    {
        if (timer <= 0)
        {
                timer = Random.Range(timerRef - timerRefVar, timerRef + timerRefVar);
                UpdateFlock();
            
        }

        timer--;


        if (debugPig)
        {
            Debug.DrawLine(transform.position, point.transform.position, Color.magenta);

            Debug.DrawRay(transform.position, direction, Color.white);
            Debug.DrawRay(transform.position, cohesion, Color.red);
            Debug.DrawRay(transform.position, align, Color.green);
            Debug.DrawRay(transform.position, separation, Color.blue);
        }

    }

   public void UpdateFlock()
    {
        cohesion = Vector3.zero;
        align = Vector3.zero;
        separation = Vector3.zero;


        int num = 0;
        //Debug.Log("me: " + this);
        foreach (GameObject go in myManager.allAgents)
        {
            if (go != this.gameObject)
            {
                float distance = Vector3.Distance(go.transform.position, transform.position);

                if (distance <= myManager.neighbourDistance)
                {
                    

                    cohesion += go.transform.position;

                    align += go.GetComponent<FlockAgent>().direction;

                    separation -= (transform.position - go.transform.position) / (distance);
                    num++;
                }

                
            }
           
        }
        if (num > 0)
        {
            align /= num;


            speed = Mathf.Clamp(align.magnitude, myManager.minSpeed, myManager.maxSpeed);

            cohesion = (cohesion / num - transform.position).normalized * speed;

            point.transform.position = cohesion;

        }

        float randomAngle = Random.Range(-randomAdditionalAngle, randomAdditionalAngle);
        Vector3 randomVec = new Vector3(Mathf.Cos(randomAngle * Mathf.Deg2Rad), 0, Mathf.Sin(randomAngle * Mathf.Deg2Rad));
        Vector3 vectorToCenter = center - transform.position;

        if (debugPig) Debug.DrawRay(transform.position, vectorToCenter.normalized * distanceToCenterMultiplier, Color.black);

        direction = (
            (cohesion + align + separation * separationMultiplier).normalized +
            randomVec ) 
            * speed 
            + vectorToCenter.normalized * distanceToCenterMultiplier;

        //print("me: " + this + " cohesion:" + cohesion + " align: " + align + " separation: " + separation);
        //print("vector to center: " + vectorToCenter);

        transform.rotation = Quaternion.Slerp(transform.rotation,
                                          Quaternion.LookRotation(direction),
                                          myManager.rotationSpeed * Time.deltaTime);
        transform.Translate(0.0f, 0.0f, Time.deltaTime * speed);

    }

}
