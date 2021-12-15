using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DecayFruit : MonoBehaviour
{
    public float shrinkAmount = 0.05f;
    public float minShrink = 0.25f;

    System.Collections.IEnumerator Decay()
    {
        while(true)
        {
            yield return new WaitForSeconds(2);
            transform.localScale = transform.localScale - new Vector3(shrinkAmount, shrinkAmount, shrinkAmount);
            if(transform.localScale.x < minShrink){
                Destroy(gameObject);
            }
        }
    }

    public void OnEnable()
    {
        StartCoroutine(Decay());
    }
}
