using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowerAvoid : MonoBehaviour
{
    public bool isInFront;
    public bool isLeaderInFront;
    public GameObject leaderObjInFront;

    private void Start()
    {
        isInFront = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Boid")
        {
            FlockUnit fu = other.gameObject.GetComponent<FlockUnit>();

            if (fu != null)
            {
                if (!fu.isLeader) //fu is not leader
                {
                    isInFront = true;
                    isLeaderInFront = false;
                    fu.inFrontOfLeader = true;
                    fu.positionToAvoid = transform.position;
                }
                else //fu is leader
                {
                    isLeaderInFront = true;
                    leaderObjInFront = other.gameObject;
                    isInFront = true;
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Boid")
        {
            isLeaderInFront = false;
            isInFront = false;
        }
    }
}
