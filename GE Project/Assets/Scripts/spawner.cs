using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class spawner : MonoBehaviour
{

    GameObject[] objects;
    public GameObject monkey;
    public GameObject marten;
    public GameObject tree;
    public int[] populations = new int[] {2, 0, 50};
    public float spawnWidth = 246f;
    public float spawnHeight = 246f;

    System.Collections.IEnumerator Spawn()
    {
        yield return new WaitForSeconds(1);
        RaycastHit hit;

        for(int i = 0; i < objects.Length; i++){
            for(int j = 0; j < populations[i]; j++){
                Vector3 spawnPoint = new Vector3(Random.Range(10f, spawnWidth), transform.position.y, Random.Range(10f, spawnHeight));
                if(Physics.Raycast(spawnPoint, transform.TransformDirection(Vector3.down), out hit)){
                    if(hit.distance < 4.43f){
                        Instantiate(objects[i], spawnPoint, transform.rotation);
                    }
                }
            }
        }

    }

    void CallCoroutine()
    {
        StartCoroutine(Spawn());
    }

    void Start(){

        objects = new GameObject[] {monkey, marten, tree};

        CallCoroutine();

    }
}
