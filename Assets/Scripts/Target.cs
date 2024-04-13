using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Target : MonoBehaviour
{
    public FlockManager flockManager;

    private void OnCollisionEnter(Collision collision)
    {
        //if (collision.gameObject.tag == "Boid" && flockManager.targets.IndexOf(gameObject) == 0)
        //{
        //    Destroy(gameObject);

        //    if (flockManager.targets.Count != 0)
        //    {
        //        flockManager.targets.RemoveAt(0);
        //    } else
        //    {
        //        flockManager.targets.Clear();
        //    }
        //}
    }
}
