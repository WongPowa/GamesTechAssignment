using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalFlock : MonoBehaviour
{
    public GameObject boid;
    public int numberOfBoids;
    public bool is2D;
    static public Vector3 goalPos;

    // Start is called before the first frame update
    void Start()
    {
        if (is2D) 
        {
            for (var i = 0; i < numberOfBoids; i++)
            {
                Vector3 pos = new Vector3(Random.Range(-10, 10), 0, Random.Range(-10, 10));
                Instantiate(boid, pos, Quaternion.identity);
            }
        } else {
            for (var i = 0; i < numberOfBoids; i++)
            {
                Vector3 pos = new Vector3(Random.Range(-10, 10), Random.Range(-10, 10), Random.Range(-10, 10));
                Instantiate(boid, pos, boid.transform.rotation);
            }
        }
        
        goalPos = new Vector3(0, 0, 0);
    }

    // Update is called once per frame
    void Update()
    {
        if (is2D)
        {
            if (Random.Range(0, 1000) < 50)
            {
                goalPos = new Vector3(Random.Range(-1.0f, 1.0f), 0, Random.Range(-1.0f, 1.0f));
            }
        } else {
            if (Random.Range(0, 1000) < 50)
            {
                goalPos = new Vector3(Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f));
            }
        }
        
    }
}
