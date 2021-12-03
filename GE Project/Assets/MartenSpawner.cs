using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MartenSpawner : MonoBehaviour
{
    public GameObject marten;
    public int martenPopulation = 10;
    public float spawnWidth = 246f;
    public float spawnHeight = 246f;

    void Start(){
        
        for(int i = 0; i < martenPopulation; i++){
            Instantiate(marten, new Vector3(Random.Range(10f, spawnWidth), transform.position.y, Random.Range(10f, spawnHeight)), transform.rotation);
        }

    }
}
