using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class dropFruit : MonoBehaviour
{
    public GameObject fig;

    System.Collections.IEnumerator Drop()
    {
        while(true)
        {
            yield return new WaitForSeconds(Random.Range(10, 30));
            Instantiate(fig, new Vector3(transform.position.x + Random.Range(-2f, 2f), transform.position.y + 4, transform.position.z + Random.Range(-2f, 2f)), transform.rotation);
        }
    }

    public void OnEnable()
    {
        StartCoroutine(Drop());
    }
}
