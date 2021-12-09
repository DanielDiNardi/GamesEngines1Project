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
            Debug.Log("Here");
            Instantiate(fig, new Vector3(-1.44f, 4.2f, 0.78f), transform.rotation);
            fig.transform.parent = transform;
            yield return new WaitForSeconds(1);
        }
    }

    public void OnEnable()
    {
        StartCoroutine(Drop());
    }
}
