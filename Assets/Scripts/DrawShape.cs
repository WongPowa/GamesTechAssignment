
using System.Collections.Generic;
using UnityEngine;

public class DrawShape : MonoBehaviour
{
    public LineRenderer lineRenderer;
    public int desiredSquares = 25;
    private List<Vector3> points;

    void Start()
    {
        points = new List<Vector3>();
        lineRenderer.positionCount = 0;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                points.Add(hit.point);
            }
        }
        if (Input.GetMouseButtonDown(1))
        {
            points.Clear();
            lineRenderer.positionCount = 0;
        }
        if (points.Count > 0)  // Check if there are any points
        {
            lineRenderer.positionCount = points.Count;
            lineRenderer.SetPositions(points.ToArray());
        }
        // Calculate extents, width, height, and average square dimensions
        float minX = Mathf.Infinity, maxX = Mathf.NegativeInfinity;
        float minZ = Mathf.Infinity, maxZ = Mathf.NegativeInfinity;
        foreach (Vector3 point in points)
        {
            minX = Mathf.Min(minX, point.x);
            maxX = Mathf.Max(maxX, point.x);
            minZ = Mathf.Min(minZ, point.z);
            maxZ = Mathf.Max(maxZ, point.z);
        }
        float width = maxX - minX;
        float height = maxZ - minZ;
        float avgSquareWidth = width / desiredSquares;
        float avgSquareHeight = height / desiredSquares;

        // Estimate center points for each captured point (approximate)
        List<Vector3> centerPoints = new List<Vector3>();
        for (int i = 0; i < points.Count; i++)
        {
            Vector3 center = points[i];
            centerPoints.Add(center);
        }

        //if (Input.GetMouseButton(0))
        //{
        //    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        //    RaycastHit hit;
        //    if (Physics.Raycast(ray, out hit))
        //    {
        //        points.Add(hit.point);
        //        lineRenderer.positionCount = points.Count;
        //        lineRenderer.SetPositions(points.ToArray());
        //    }
        //}


    }
}
