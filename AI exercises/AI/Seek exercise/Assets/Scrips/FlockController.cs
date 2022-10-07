using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlockController : MonoBehaviour
{
    [SerializeField]
    GameObject flockAgent;

    [SerializeField]
    int numAgents;

    
    public GameObject[] allAgents;
    [SerializeField]
    Vector3 areaLimits;
    [SerializeField]
    bool bounded;
    [SerializeField]
    bool randomize;
    [SerializeField]
    bool followLeader;
    [SerializeField]
    GameObject leader;
    [Header("Fish Settings")]
    [Range(0, 20)]
    public float minSpeed;
    [Range(0, 20)]
    public float maxSpeed;

    [SerializeField]
    [Range(0, 0.01f)]
    float changingValueMultiplier;
    [Range(0, 100)]
    public float neighbourDistance;

    [Range(0, 20)]
    public float rotationSpeed;

    [SerializeField]
    float spawnRadius;

    float changingValue = 0;

    // Start is called before the first frame update
    void Start()
    {
        allAgents = new GameObject[numAgents];
        for (int i = 0; i < numAgents; i++)
        {
            Vector3 randomPos = new Vector3(Random.Range(-spawnRadius, spawnRadius), 0, Random.Range(-spawnRadius, spawnRadius));
            Vector3 pos = this.transform.position + randomPos;
            Vector3 randomize = new Vector3(Random.Range(0, 360), 0, Random.Range(0, 360));
            
            allAgents[i] = (GameObject)Instantiate(flockAgent, pos,
                                Quaternion.LookRotation(randomize));

            allAgents[i].name = "agent " + i;
            allAgents[i].GetComponent<FlockAgent>().speed = Random.Range(minSpeed, maxSpeed);
            allAgents[i].GetComponent<FlockAgent>().myManager = this;
            allAgents[i].transform.SetParent(this.transform);

        }

    }

    // Update is called once per frame
    void Update()
    {
        changingValue++;
        neighbourDistance = (Mathf.Sin(changingValue * changingValueMultiplier) + 1) * 50;

        print("neighbourDistance: " + neighbourDistance);
    }
}
