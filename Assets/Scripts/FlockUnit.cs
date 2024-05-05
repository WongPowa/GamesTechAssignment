using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class FlockUnit : MonoBehaviour
{
    [SerializeField] private float FOVAngle;
    [SerializeField] private float smoothDamp;
    [SerializeField] private LayerMask obstacleMask;
    [SerializeField] private LayerMask leaderViewMask;
    [SerializeField] private Vector3[] directionsToCheckWhenAvoidingObstacles;

    private List<FlockUnit> cohesionNeighbours = new List<FlockUnit>();
    private List<FlockUnit> avoidanceNeighbours = new List<FlockUnit>();
    private List<FlockUnit> aligementNeighbours = new List<FlockUnit>();
    private List<FlockUnit> leaderNeighbours = new List<FlockUnit>();
    private FlockManager assignedFlock;
    private Vector3 currentVelocity;
    private Vector3 currentObstacleAvoidanceVector;
    private float speed;

    public bool isLeader;
    public bool inFrontOfLeader;
    public Vector3 positionToAvoid;
    public GameObject objectInFrontOfLeader;
    private FollowerAvoid followerAvoid;
    private bool isTimerRunning;

    private void Start()
    {
        followerAvoid = objectInFrontOfLeader.GetComponent<FollowerAvoid>();
        isLeader = false;
        isTimerRunning = false;
    }

    public Transform myTransform { get; set; }

    private void Awake()
    {
        myTransform = transform;
    }

    public void AssignFlock(FlockManager flock)
    {
        assignedFlock = flock;
    }

    public void InitializeSpeed(float speed)
    {
        this.speed = speed;
    }

    public void MoveUnit()
    {
        FindNeighbours();
        CalculateSpeed();

        if (assignedFlock.isLeaderOn)
            DetermineLeader();

        var cohesionVector = CalculateCohesionVector() * assignedFlock.cohesionWeight;
        var avoidanceVector = CalculateAvoidanceVector() * assignedFlock.avoidanceWeight;
        var aligementVector = CalculateAligementVector() * assignedFlock.aligementWeight;
        var boundsVector = CalculateBoundsVector() * assignedFlock.boundsWeight;
        var obstacleVector = CalculateObstacleVector() * assignedFlock.obstacleWeight;
        var arrivalVector = Vector3.zero;
        var avoidLeaderPathVector = Vector3.zero;

        if (assignedFlock.isLeaderOn)
        {
            if (isLeader && assignedFlock.targets.Count != 0) //only when there is targets, there can be a leader
            {
                gameObject.GetComponentInChildren<Renderer>().material.color = Color.yellow; //to change color of capsule its needs to be first in hierarchy
                arrivalVector = CalculateArrivalVector() * assignedFlock.arrivalWeight;
            }
            else if (assignedFlock.targets.Count != 0) //not leader but target is present
            {
                avoidLeaderPathVector = CalculateAvoidLeaderPathVector() * assignedFlock.avoidLeaderPathWeight;
                gameObject.GetComponentInChildren<Renderer>().material.color = Color.white;
                arrivalVector = CalculateArrivalVectorBehindLeader() * assignedFlock.arrivalWeight;
            }
        } else if (assignedFlock.targets.Count != 0 )
        {
            arrivalVector = CalculateArrivalVector() * assignedFlock.arrivalWeight;
        }


        var moveVector = cohesionVector + avoidanceVector + aligementVector + boundsVector + obstacleVector + arrivalVector + avoidLeaderPathVector;
        moveVector = Vector3.SmoothDamp(myTransform.forward, moveVector, ref currentVelocity, smoothDamp);
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(moveVector), smoothDamp);
        moveVector = moveVector.normalized * speed;

        if (moveVector == Vector3.zero)
        {
            moveVector = transform.forward;
        }

        myTransform.forward = moveVector;
        myTransform.position += moveVector * Time.deltaTime;

    }
        
    private void FindNeighbours()
    {
        cohesionNeighbours.Clear();
        avoidanceNeighbours.Clear();
        aligementNeighbours.Clear();
        leaderNeighbours.Clear();

        var allUnits = assignedFlock.allUnits;
        for (int i = 0; i < allUnits.Length; i++)
        {
            var currentUnit = allUnits[i];
            if (currentUnit != this)
            {
                float currentNeighbourDistanceSqr = Vector3.SqrMagnitude(currentUnit.myTransform.position - myTransform.position);
                if (currentNeighbourDistanceSqr <= assignedFlock.cohesionDistance * assignedFlock.cohesionDistance)
                {
                    cohesionNeighbours.Add(currentUnit);
                }
                if (currentNeighbourDistanceSqr <= assignedFlock.avoidanceDistance * assignedFlock.avoidanceDistance)
                {
                    avoidanceNeighbours.Add(currentUnit);
                }
                if (currentNeighbourDistanceSqr <= assignedFlock.aligementDistance * assignedFlock.aligementDistance)
                {
                    aligementNeighbours.Add(currentUnit);
                }
                if (currentNeighbourDistanceSqr <= assignedFlock.leaderDistance * assignedFlock.leaderDistance && assignedFlock.isLeaderOn)
                {
                    leaderNeighbours.Add(currentUnit);
                }
            }
        }
    }

    private void CalculateSpeed()
    {
        if (cohesionNeighbours.Count == 0)
            return;
        speed = 0;
        for (int i = 0; i < cohesionNeighbours.Count; i++)
        {
            speed += cohesionNeighbours[i].speed;
        }

        speed /= cohesionNeighbours.Count;
        speed = Mathf.Clamp(speed, assignedFlock.minSpeed, assignedFlock.maxSpeed);
    }

    private Vector3 CalculateCohesionVector()
    {
        var cohesionVector = Vector3.zero;
        if (cohesionNeighbours.Count == 0)
            return Vector3.zero;
        int neighboursInFOV = 0;
        for (int i = 0; i < cohesionNeighbours.Count; i++)
        {
            if (IsInFOV(cohesionNeighbours[i].myTransform.position))
            {
                neighboursInFOV++;
                cohesionVector += cohesionNeighbours[i].myTransform.position;
            }
        }

        if (neighboursInFOV > 0)
        {
            cohesionVector /= neighboursInFOV;
            cohesionVector -= myTransform.position;
            cohesionVector = cohesionVector.normalized;
        }

        return cohesionVector;
    }

    private Vector3 CalculateAligementVector()
    {
        var aligementVector = myTransform.forward;
        if (aligementNeighbours.Count == 0)
            return myTransform.forward;
        int neighboursInFOV = 0;
        for (int i = 0; i < aligementNeighbours.Count; i++)
        {
            if (IsInFOV(aligementNeighbours[i].myTransform.position))
            {
                neighboursInFOV++;
                aligementVector += aligementNeighbours[i].myTransform.forward;
            }
        }

        if (neighboursInFOV > 0)
        {
            aligementVector /= neighboursInFOV;
        }

        aligementVector = aligementVector.normalized;
        return aligementVector;
    }

    private Vector3 CalculateAvoidanceVector()
    {
        var avoidanceVector = Vector3.zero;
        if (aligementNeighbours.Count == 0)
            return Vector3.zero;
        int neighboursInFOV = 0;
        for (int i = 0; i < avoidanceNeighbours.Count; i++)
        {
            if (IsInFOV(avoidanceNeighbours[i].myTransform.position))
            {
                neighboursInFOV++;
                avoidanceVector += (myTransform.position - avoidanceNeighbours[i].myTransform.position);
            }
        }

        if (neighboursInFOV > 0)
        {
            avoidanceVector /= neighboursInFOV;
            avoidanceVector = avoidanceVector.normalized;
        }

        return avoidanceVector;
    }

    private Vector3 CalculateBoundsVector()
    {
        var offsetToCenter = assignedFlock.transform.position - myTransform.position;
        bool isNearCenter = (offsetToCenter.magnitude >= assignedFlock.boundsDistance * 0.9f);
        return isNearCenter ? offsetToCenter.normalized : Vector3.zero;
    }

    private Vector3 CalculateAvoidLeaderPathVector() //TODO: only avoids leader when within the trigger, however the the box is within the FOV of the leader. So it never happens. Solution: have timer
    {
        Vector3 avoidLeaderPathVector = Vector3.zero;

        inFrontOfLeader = positionToAvoid != Vector3.zero;

        if (!inFrontOfLeader)
        {
            return Vector3.zero;
        }

        avoidLeaderPathVector = myTransform.position - positionToAvoid;

        return avoidLeaderPathVector.normalized;
    }

    private Vector3 CalculateObstacleVector()
    {
        var obstacleVector = Vector3.zero;
        RaycastHit hit;

        //for (float i = -FOVAngle / 4; i <= FOVAngle; i += FOVAngle / 8)
        //{
        //    Vector3 vForward = Quaternion.AngleAxis(i, Vector3.up) * transform.forward;

            //if (Physics.SphereCast(myTransform.position, 0.5f, vForward, out hit, assignedFlock.obstacleDistance, obstacleMask))
            //Rotate the raycast around the y-axis
            if (Physics.Raycast(myTransform.position, myTransform.forward, out hit, assignedFlock.obstacleDistance, obstacleMask))
            {
                obstacleVector = FindBestDirectionToAvoidObstacle();
            }

            else
            {
                currentObstacleAvoidanceVector = Vector3.zero;
            }
        //}

        return obstacleVector;
    }

    private Vector3 FindBestDirectionToAvoidObstacle()
    {
        if (currentObstacleAvoidanceVector != Vector3.zero)
        {
            RaycastHit hit;
            if (!Physics.Raycast(myTransform.position, myTransform.forward, out hit, assignedFlock.obstacleDistance, obstacleMask))
            {
                return currentObstacleAvoidanceVector;
            }
        }
        float maxDistance = int.MinValue;
        var selectedDirection = Vector3.zero;
        for (int i = 0; i < directionsToCheckWhenAvoidingObstacles.Length; i++)
        {
            RaycastHit hit;
            var currentDirection = myTransform.TransformDirection(directionsToCheckWhenAvoidingObstacles[i].normalized);
            if (Physics.Raycast(myTransform.position, currentDirection, out hit, assignedFlock.obstacleDistance, obstacleMask))
            {

                float currentDistance = (hit.point - myTransform.position).sqrMagnitude;
                if (currentDistance > maxDistance)
                {
                    maxDistance = currentDistance;
                    selectedDirection = currentDirection;
                }
            }
            else
            {
                selectedDirection = currentDirection;
                currentObstacleAvoidanceVector = currentDirection.normalized;
                return selectedDirection.normalized;
            }
        }
        return selectedDirection.normalized;
    }

    private bool IsInFOV(Vector3 position)
    {
        //return Vector3.Angle(myTransform.forward, position - myTransform.position) <= FOVAngle;

        float angle = Vector3.Angle(myTransform.forward, position - myTransform.position);

        if (angle >= -FOVAngle/2 && angle <= FOVAngle/2)
        {
            return true;
        } else
        {
            return false;
        }
    }

    private Vector3 CalculateArrivalVector()
    {
        var arrivalVector = Vector3.zero;

        if (assignedFlock.targets.Count == 0)
            return Vector3.zero;

        if (assignedFlock.targets.Count != 0)
        {
            arrivalVector = assignedFlock.targets[0].transform.position - myTransform.position;
            CalculateSlowDownSpeed(assignedFlock.targets[0].transform.position);
        }

        return arrivalVector.normalized;
    } 

    private Vector3 CalculateArrivalVectorBehindLeader()
    {
        var arrivalVector = Vector3.zero;

        if (leaderNeighbours.Count == 0)
            return Vector3.zero;

        for (int i = 0; i < leaderNeighbours.Count; i++)
        {
            if (IsInFOV(leaderNeighbours[i].myTransform.position) && leaderNeighbours[i].isLeader)
            {
                Vector3 positionBehindLeader = leaderNeighbours[i].myTransform.position + assignedFlock.positionToFollow;

                arrivalVector = (positionBehindLeader)  - myTransform.position;
                CalculateSlowDownSpeed(positionBehindLeader);
                
                return arrivalVector.normalized;
            }
        }

        return arrivalVector.normalized;
    }

    private void CalculateSlowDownSpeed(Vector3 position)
    {
        float dist = Vector3.Distance(myTransform.position, position);

        if (dist > assignedFlock.arrivalSlowDistance)
        {
            speed = assignedFlock.maxSpeed;
        }
        else
        {
            speed = assignedFlock.maxSpeed * dist / assignedFlock.arrivalSlowDistance;
        }

        if (dist < assignedFlock.arrivalStopDistance)
        {
            speed = 0;
        }
    }

    private void DetermineLeader()
    {
        if (leaderNeighbours.Count != 0)
        {
            for (int i = 0; i < leaderNeighbours.Count; i++)
            {
                bool isBoidInFront = IsInFOV(leaderNeighbours[i].myTransform.position);

                if (!isBoidInFront && assignedFlock.targets.Count != 0)// && !followerAvoid.isLeaderInFront) //if no followers and leaders infront become leader
                {
                    BecomeLeader();

                    if (leaderNeighbours[i].isLeader)
                    {
                        var posToAvoid = leaderNeighbours[i].myTransform.position;

                        float distSelf = Vector3.Distance(assignedFlock.targets[0].transform.position, myTransform.position);
                        float distOther = Vector3.Distance(assignedFlock.targets[0].transform.position, posToAvoid);

                        if (distSelf > distOther) //if distance of leaderobjinfront is closer to target, become a follower
                        {
                            BecomeFollower();
                        }
                    }
                }
                else if (isBoidInFront && assignedFlock.targets.Count != 0) //if has follower infront dont be leader
                {
                    BecomeFollower();
                }
            }
        }
    }

    private void BecomeFollower()
    {
        if (!isTimerRunning) isLeader = false;
    }

    private void BecomeLeader()
    {
        if (!isTimerRunning) StartCoroutine(StartTimerDecay()); 
    }

    IEnumerator StartTimerDecay() //after 5 seconds, become leader
    {
        isTimerRunning = true;
        int counter = assignedFlock.timeToLeader;
        while (counter > 0)
        {
            yield return new WaitForSeconds(1);
            counter--;
        }

        isTimerRunning = false;
        isLeader = true;
    }
}