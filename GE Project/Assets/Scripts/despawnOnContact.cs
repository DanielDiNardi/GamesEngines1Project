using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class despawnOnContact : MonoBehaviour
{
    private void OnCollisionEnter(Collision col){
        Destroy(col.gameObject);
    }
}
