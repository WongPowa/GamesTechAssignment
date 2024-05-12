using UnityEngine;
using System.Collections.Generic;
using System.Net;

public class FlockManager : MonoBehaviour
{
    [Header("Spawn Setup")]
    [SerializeField] private FlockUnit flockUnitPrefab;
    [SerializeField] public int flockSize;
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
    public bool isLeaderOn;
    public List<GameObject> targets = new List<GameObject>();
    public Vector3 positionToFollow;
    public Vector3 objectSpawnPosition;
    public int timeToLeader;

    [Header("V Formation")]
    public bool formVShape;
    
    [Range(0, 1)]
    public float angle;
    public float vBoundingBoxSize;
    [Header("Square Formation")]
    public bool formSquareShape;
    public float squareBoundingBoxSize;
    public FlockUnit[] allUnits { get; set; }

    private void Start()
    {
        GenerateUnits();
    }

    private void Update()
    {
      
        if (formVShape)
        {
            
            FormVShape();
        } 
        else if (formSquareShape)
        {
            FormSquareShape();
        } 
        else
        {
            for (int i = 0; i < allUnits.Length; i++)
            {
                allUnits[i].MoveUnit();
            }
        }



        DrawBox(transform.position, Quaternion.identity, spawnBounds, Color.red);
        //transform.position += Vector3.right*Time.deltaTime;
    }

    private void GenerateUnits()
    {
        allUnits = new FlockUnit[flockSize];
        for (int i = 0; i < flockSize; i++)
        {
            //var randomVector = UnityEngine.Random.insideUnitSphere;
            Vector3 spawnBox = Vector3.Scale(transform.localScale, spawnBounds);
            var randomVector = new Vector3(UnityEngine.Random.Range(-spawnBox.x / 2, spawnBox.x / 2), UnityEngine.Random.Range(-spawnBox.y / 2, spawnBox.y / 2), UnityEngine.Random.Range(-spawnBox.z / 2, spawnBox.z / 2));
            //var randomVector = new Vector3(UnityEngine.Random.value * spawnBox.x, UnityEngine.Random.value * spawnBox.y, UnityEngine.Random.value * spawnBox.z);
            //randomVector = new Vector3(randomVector.x * spawnBounds.x, randomVector.y * spawnBounds.y, randomVector.z * spawnBounds.z);
            var spawnPosition = transform.position + randomVector;
            var rotation = Quaternion.Euler(0, UnityEngine.Random.Range(0, 360), 0);
            allUnits[i] = Instantiate(flockUnitPrefab, spawnPosition, rotation);
            allUnits[i].AssignFlock(this);
            allUnits[i].InitializeSpeed(UnityEngine.Random.Range(minSpeed, maxSpeed));
        }
    }    

    public void DrawBox(Vector3 pos, Quaternion rot, Vector3 scale, Color c)
    {
        // create matrix
        Matrix4x4 m = new Matrix4x4();
        m.SetTRS(pos, rot, scale);

        var point1 = m.MultiplyPoint(new Vector3(-0.5f, -0.5f, 0.5f));
        var point2 = m.MultiplyPoint(new Vector3(0.5f, -0.5f, 0.5f));
        var point3 = m.MultiplyPoint(new Vector3(0.5f, -0.5f, -0.5f));
        var point4 = m.MultiplyPoint(new Vector3(-0.5f, -0.5f, -0.5f));

        var point5 = m.MultiplyPoint(new Vector3(-0.5f, 0.5f, 0.5f));
        var point6 = m.MultiplyPoint(new Vector3(0.5f, 0.5f, 0.5f));
        var point7 = m.MultiplyPoint(new Vector3(0.5f, 0.5f, -0.5f));
        var point8 = m.MultiplyPoint(new Vector3(-0.5f, 0.5f, -0.5f));

        Debug.DrawLine(point1, point2, c);
        Debug.DrawLine(point2, point3, c);
        Debug.DrawLine(point3, point4, c);
        Debug.DrawLine(point4, point1, c);

        Debug.DrawLine(point5, point6, c);
        Debug.DrawLine(point6, point7, c);
        Debug.DrawLine(point7, point8, c);
        Debug.DrawLine(point8, point5, c);

        Debug.DrawLine(point1, point5, c);
        Debug.DrawLine(point2, point6, c);
        Debug.DrawLine(point3, point7, c);
        Debug.DrawLine(point4, point8, c);
    }

    private void FormVShape()
    {
        Vector2 boundBoxSize = new Vector2(vBoundingBoxSize, vBoundingBoxSize);
        List<Vector3> midpoints = CalculateVFormationPositions(flockSize, transform.position, boundBoxSize, angle);
        for(int i = 0; i < midpoints.Count; i++) {
            Vector3 midpoint = midpoints[i];
            allUnits[i].MoveUnit(midpoint);
        }
    }
    private void FormSquareShape()
    {
        Vector2 boundBoxSize = new Vector2(squareBoundingBoxSize, squareBoundingBoxSize);
        List<Vector3> midpoints = CalculateSquareFormationPositions(flockSize, transform.position, boundBoxSize);
        for(int i = 0; i < midpoints.Count; i++) {
            Vector3 midpoint = midpoints[i];
            allUnits[i].MoveUnit(midpoint);
        }
    }
    public List<Vector3> CalculateVFormationPositions(int size, Vector3 boundingBoxCenter, Vector2 boundingBoxSize, float vAngle)
    {
        // Calculate number of squares per side
        int squaresPerSide = size/2;
        // Calculate size of each square
        float squareSize = boundingBoxSize.x / squaresPerSide;

        // Calculate V-formation offset
        //float vBaseOffset = boundingBoxSize.x * Mathf.Tan(Mathf.Deg2Rad * vAngle/2)/2;
        float vBaseOffset = vAngle/2;
        float offset = 0f;
        // List to store midpoint positions
        List<Vector3> positions = new List<Vector3>();
        for (int i = 0; i < squaresPerSide; i++)
        {
            for (int j = 0; j < squaresPerSide; j++)
            {
                
                    if (i == j)
                    {
                        float centerX = boundingBoxCenter.x + (i + 0.5f) * squareSize;
                        float centerZ = boundingBoxCenter.z + (i + 0.5f) * squareSize;

                        Vector3 left = new Vector3(centerX, 0f, centerZ  - vBaseOffset * offset);
                        Vector3 right = new Vector3(centerX, 0f, -centerZ  + vBaseOffset * offset);

                        Debug.DrawLine(boundingBoxCenter,left, Color.red, 0.5f); // Draw line from center to midpoint
                        Debug.DrawLine(boundingBoxCenter, right, Color.blue, 0.5f); // Draw line from center to midpoint
                        positions.Add(left); // Set y to 0 for 2D or maintain y position for 3D
                        positions.Add(right); // Set y to 0 for 2D or maintain y position for 3D

                    }
            }
            offset += (squareSize/squaresPerSide)*100;
        }

        return positions;
    }

    public List<Vector3> CalculateSquareFormationPositions(int size, Vector3 boundingBoxCenter, Vector2 boundingBoxSize)
    {
        // Calculate number of squares per side
        int squaresPerSide = (int)Mathf.Sqrt(size);
        if (squaresPerSide * squaresPerSide != size)
        {
            Debug.LogError("Size must be a perfect square.");
            return null;
        }

        // Calculate size of each square
        float squareSize = boundingBoxSize.x / squaresPerSide;

        // Get the GameObject's rotation around the y-axis
        Quaternion rotation = Quaternion.Euler(0, transform.eulerAngles.y, 0);

        // List to store midpoint positions
        List<Vector3> positions = new List<Vector3>();

        for (int i = 0; i < squaresPerSide; i++)
        {
            for (int j = 0; j < squaresPerSide; j++)
            {
                // Calculate center position of each square in local space relative to the bounding box center
                float localX = (i + 0.5f) * squareSize - (boundingBoxSize.x * 0.5f);
                float localZ = (j + 0.5f) * squareSize - (boundingBoxSize.y * 0.5f);
                Vector3 localPosition = new Vector3(localX, 0, localZ);

                // Transform local position to world position by applying rotation
                Vector3 worldPosition = boundingBoxCenter + rotation * localPosition;

                positions.Add(worldPosition);

                // Optional: Visual debugging to show the line from the actual center to the calculated position
                Debug.DrawLine(boundingBoxCenter, worldPosition, Color.red, 1f);
            }
        }

        return positions;
    }


}