using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeSpawner : MonoBehaviour
{
    public GameObject tree;
    public int treePopulation = 50;
    public float spawnWidth = 246f;
    public float spawnHeight = 246f;

    void Start(){
        
        for(int i = 0; i < treePopulation; i++){
            Instantiate(tree, new Vector3(Random.Range(10f, spawnWidth), transform.position.y, Random.Range(10f, spawnHeight)), transform.rotation);
        }

    }
}
