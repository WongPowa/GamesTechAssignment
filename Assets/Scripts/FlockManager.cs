using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlockManager : MonoBehaviour
{
    [Header("Spawn Setup")]
    [SerializeField] private FlockUnit flockUnitPrefab;
    [SerializeField] private int flockSize;
    [SerializeField] private Vector3 spawnBounds;

    [Header("Speed Setup")]
    [Range(0, 10)]
    [SerializeField] private float _minSpeed;
    public float minSpeed { get { return _minSpeed; } }
    [Range(0, 10)]
    [SerializeField] private float _maxSpeed;
    public float maxSpeed { get { return _maxSpeed; } }

    [Header("Detection Distances")]

    [Range(0, 10)]
    [SerializeField] private float _cohesionDistance;
    public float cohesionDistance { get { return _cohesionDistance; } }

    [Range(0, 10)]
    [SerializeField] private float _avoidanceDistance;
    public float avoidanceDistance { get { return _avoidanceDistance; } }

    [Range(0, 10)]
    [SerializeField] private float _aligementDistance;
    public float aligementDistance { get { return _aligementDistance; } }

    [Range(0, 10)]
    [SerializeField] private float _obstacleDistance;
    public float obstacleDistance { get { return _obstacleDistance; } }

    [Range(0, 10)]
    [SerializeField] private float _boundsDistance;
    public float boundsDistance { get { return _boundsDistance; } }

    [Range(0, 10)]
    [SerializeField] private float _leaderDistance;
    public float leaderDistance { get { return _leaderDistance; } }

    [Range(0, 10)]
    [SerializeField] private float _arrivalSlowDistance;
    public float arrivalSlowDistance { get { return _arrivalSlowDistance; } }

    [Range(0, 10)]
    [SerializeField] private float _arrivalStopDistance;
    public float arrivalStopDistance { get { return _arrivalStopDistance; } }


    [Header("Behaviour Weights")]

    [Range(0, 10)]
    [SerializeField] private float _cohesionWeight;
    public float cohesionWeight { get { return _cohesionWeight; } }

    [Range(0, 10)]
    [SerializeField] private float _avoidanceWeight;
    public float avoidanceWeight { get { return _avoidanceWeight; } }

    [Range(0, 10)]
    [SerializeField] private float _aligementWeight;
    public float aligementWeight { get { return _aligementWeight; } }

    [Range(0, 10)]
    [SerializeField] private float _obstacleWeight;
    public float obstacleWeight { get { return _obstacleWeight; } }

    [Range(0, 10)]
    [SerializeField] private float _boundsWeight;
    public float boundsWeight { get { return _boundsWeight; } }

    [Range(0, 10)]
    [SerializeField] private float _arrivalWeight;
    public float arrivalWeight { get { return _arrivalWeight; } }

    [Range(0, 10)]
    [SerializeField] private float _avoidLeaderPathWeight;
    public float avoidLeaderPathWeight { get { return _avoidLeaderPathWeight; } }

    [Header("Targets")]
    public List<GameObject> targets = new List<GameObject>();
    public Vector3 positionToFollow;
    public Vector3 objectSpawnPosition;
    public int timeToLeader;

    public FlockUnit[] allUnits { get; set; }

    private void Start()
    {
        GenerateUnits();
    }

    private void Update()
    {
        for (int i = 0; i < allUnits.Length; i++)
        {
            allUnits[i].MoveUnit();
        }
    }

    private void GenerateUnits()
    {
        allUnits = new FlockUnit[flockSize];
        for (int i = 0; i < flockSize; i++)
        {
            var randomVector = UnityEngine.Random.insideUnitSphere;
            randomVector = new Vector3(randomVector.x * spawnBounds.x, randomVector.y * spawnBounds.y, randomVector.z * spawnBounds.z);
            var spawnPosition = transform.position + randomVector;
            var rotation = Quaternion.Euler(0, UnityEngine.Random.Range(0, 360), 0);
            allUnits[i] = Instantiate(flockUnitPrefab, spawnPosition, rotation);
            allUnits[i].AssignFlock(this);
            allUnits[i].InitializeSpeed(UnityEngine.Random.Range(minSpeed, maxSpeed));
        }
    }
}