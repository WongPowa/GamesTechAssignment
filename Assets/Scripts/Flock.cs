using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using static UnityEngine.GraphicsBuffer;

public class Flock : MonoBehaviour
{
    public float speed;
    [Range(0, 360)]
    public float fov;
    public float rotationSpeed;
    public Vector3 wind;

    private string boidTag;
    private string funIncrementFollowers;
    private string funDecrementFollowers;

    //Cohesion
    [Header("Cohesion")]
    public float cohesionRadius;
    public float cohesionFactor;

    //Seperation
    [Header("Separation")]
    public float separationRadius;
    public float separationFactor;
    Vector3 separationForce;

    //Alignment
    [Header("Alignment")]
    public float alignmentRadius;
    public float alignmentFactor;

    //Obstacle Avoidance
    [Header("Obstacle Avoidance")]
    public float maxDistance;
    public LayerMask layerAvoid;
    public float avoidFactor;

    //Leader
    private HashSet<int> boidsBehind;
    private int numberOfBoidsBehind;

    private void Awake() //prevent NPE
    {
        boidsBehind = new HashSet<int>();
    }

    // Start is called before the first frame update
    void Start()
    {
        //speed = Random.Range(0.1f, 1f);
        separationForce = Vector3.zero;
        boidTag = "Boid";
        funIncrementFollowers = "IncrementFollowers";
        funDecrementFollowers = "DecrementFollowers";
    }

    // Update is called once per frame
    void Update()
    {
        //if (Random.Range(0, 5) < 1)
            //ApplyRules();
        //
        //Vector3 forward = transform.TransformDirection(Vector3.forward) * 10;
        //Debug.DrawRay(transform.position, forward, Color.green);

        //transform.Translate(0, 0, Time.deltaTime * speed);
        transform.Translate(0, 0, Time.deltaTime * speed);
    }

    private void FixedUpdate()
    {
        ApplyRules();
    }

    void ApplyRules() //missing predator & missing fov
    {
        GameObject[] gos;
        gos = GameObject.FindGameObjectsWithTag(boidTag);

        Vector3 vCentre = Vector3.zero;
        Vector3 vSeparate = Vector3.zero;
        Vector3 vAlign = Vector3.zero;
        Vector3 vAvoid = Vector3.zero;
        //Vector3 goalPos = GlobalFlock.goalPos;

        //float gSpeed = 0;
        float dist = 0;
        int cohesionGroupSize = 0;
        int alignmentGroupSize = 0;
        int separationGroupSize = 0;

        foreach (GameObject go in gos)
        {
            if (go != this.gameObject)
            {
                float angle = Vector3.Angle(go.transform.position - transform.position, transform.forward);

                if (angle * 2 <= fov)
                {
                    gameObject.SendMessage(funIncrementFollowers, go);

                    dist = Vector3.Distance(go.transform.position, transform.position);

                    //gSpeed = gSpeed + go.GetComponent<Flock>().speed;

                    if (dist <= cohesionRadius)
                    {
                        vCentre += go.transform.position; //to get average position for cohesion
                        cohesionGroupSize++;
                    }

                    if (dist <= alignmentRadius)
                    {
                        vAlign += go.transform.forward;
                        alignmentGroupSize++;
                    }

                    if (dist <= separationRadius)
                    {
                        vSeparate = vSeparate + (transform.position - go.transform.position) / dist;
                        separationGroupSize++;
                    }
                    vAvoid = ObstacleAvoid();
                    
                } else {
                    gameObject.SendMessage(funDecrementFollowers, go);
                }

            }
        }

        if (cohesionGroupSize != 0) //TODO: make 3d boids
        {
            //vCentre = vCentre / cohesionGroupSize + (goalPos - transform.position);
            vCentre = vCentre / cohesionGroupSize;
            //speed = gSpeed / groupSize;
            //Vector3 direction = (vcentre + separationForce.normalized) - transform.position;
        }

        if (alignmentGroupSize != 0)
        {
            vAlign = vAlign / alignmentGroupSize;
        }

        if (separationGroupSize != 0)
        {
            vSeparate = vSeparate / separationGroupSize;
        }

        Vector3 direction = (vCentre - transform.position).normalized * cohesionFactor + vSeparate.normalized * separationFactor + vAlign.normalized * alignmentFactor + vAvoid.normalized * avoidFactor;


        if (direction != Vector3.zero)
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), rotationSpeed * Time.deltaTime);
    }


    private Vector3 ObstacleAvoid()
    {
        Vector3 vAvoid = Vector3.zero;  
        RaycastHit hit;

        //raycast in all directions within the fov
        for (float i = -fov / 4; i <= fov; i += fov / 8)
        {
            Vector3 vForward = Quaternion.AngleAxis(i, Vector3.up) * transform.forward;

            //if (Physics.SphereCast(transform.position, avoidSphereRadius, vForward, out hit, maxDistance, layerAvoid))
            if (Physics.Raycast(transform.position, vForward, out hit, maxDistance, layerAvoid))
            {
                float dist = Vector3.Distance(hit.point, transform.position);
                vAvoid = (transform.position - hit.point) / dist;
            }
        }

        return vAvoid;
    }

    private void IncrementFollowers(GameObject go)
    {
        boidsBehind.Add(go.GetInstanceID());
        numberOfBoidsBehind = boidsBehind.Count;

    }

    private void DecrementFollowers(GameObject go)
    {
        boidsBehind.Remove(go.GetInstanceID());
        numberOfBoidsBehind = boidsBehind.Count;
    }

}
