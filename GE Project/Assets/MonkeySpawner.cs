using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonkeySpawner : MonoBehaviour
{
    public GameObject monkey;
    public int monkeyPopulation = 100;
    public float spawnWidth = 246f;
    public float spawnHeight = 246f;

    void Start(){

        // Debug.Log(transform.position.y is float);
        
        for(int i = 0; i < monkeyPopulation; i++){
            Instantiate(monkey, new Vector3(Random.Range(10f, spawnWidth), transform.position.y, Random.Range(10f, spawnHeight)), transform.rotation);
        }

    }
}
