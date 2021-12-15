using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DespawnOnContact : MonoBehaviour
{
    // When a creature touches the water, it drowns.
    private void OnCollisionEnter(Collision col){
        Destroy(col.gameObject);
    }
}
