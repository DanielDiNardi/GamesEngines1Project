using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropFruit : MonoBehaviour
{
    public GameObject fig;

    System.Collections.IEnumerator Drop()
    {
        // Continuously drops fruits in between 5 to 30 seconds
        // in the range of the tree's position.
        while(true)
        {
            yield return new WaitForSeconds(Random.Range(5, 30));
            Instantiate(fig, new Vector3(transform.position.x + Random.Range(-2f, 2f), transform.position.y + 4, transform.position.z + Random.Range(-2f, 2f)), transform.rotation);
        }
    }

    public void OnEnable()
    {
        StartCoroutine(Drop());
    }
}
