using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowerAvoid : MonoBehaviour
{
    private FlockUnit otherFU;
    public FlockUnit ownFU;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Boid" && ownFU.isLeader)
        {
            otherFU = other.gameObject.GetComponent<FlockUnit>();

            if (otherFU != null)
            {
                otherFU.positionToAvoid = transform.position;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Boid" && ownFU.isLeader)
        {
            if (otherFU != null)
            {
                otherFU.positionToAvoid = Vector3.zero;
            }
        }
    }
}
