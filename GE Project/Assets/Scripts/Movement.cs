using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    Stats stats;

    public float moveSpeed = 0f;
    public float rotSpeed = 100f;

    public bool isWandering = false;
    public bool isRotatingLeft = false;
    public bool isRotatingRight = false;
    public bool isWalking = false;

    RaycastHit hit;

    IEnumerator Wander(){
        int rotTime = Random.Range(1, 3);
        int rotateWait = Random.Range(1, 4);
        int rotateLorR = Random.Range(0, 3);
        int walkWait = Random.Range(1, 4);
        int walkTime = Random.Range(1, 5);

        isWandering = true;

        yield return new WaitForSeconds(walkWait);
        isWalking = true;
        yield return new WaitForSeconds(walkTime);
        isWalking = false;
        yield return new WaitForSeconds(rotateWait);
        
        if(rotateLorR == 1){
            isRotatingRight = true;
            yield return new WaitForSeconds(rotTime);
            isRotatingRight = false;
        }
        if(rotateLorR == 2){
            isRotatingLeft = true;
            yield return new WaitForSeconds(rotTime);
            isRotatingLeft = false;
        }
        isWandering = false;
    }

    // Start is called before the first frame update
    void Start()
    {
        moveSpeed = Random.Range(1f, 10f);
    }

    // Update is called once per frame
    void Update()
    {
        if(isWandering == false){
            StartCoroutine(Wander());
        }
        if(isRotatingRight == true){
            Quaternion rotation = Quaternion.Euler(new Vector3(0, rotSpeed, 0) * Time.deltaTime);
            gameObject.GetComponent<Rigidbody>().MoveRotation(gameObject.GetComponent<Rigidbody>().rotation * rotation);
        }
        if(isRotatingLeft == true){
            Quaternion rotation = Quaternion.Euler(new Vector3(0, -rotSpeed, 0) * Time.deltaTime);
            gameObject.GetComponent<Rigidbody>().MoveRotation(gameObject.GetComponent<Rigidbody>().rotation * rotation);
        }
        if(isWalking == true){
            // transform.position += transform.forward * Time.deltaTime * moveSpeed;
            gameObject.GetComponent<Rigidbody>().MovePosition(transform.position + transform.forward * Time.deltaTime * moveSpeed);
            if(Physics.Raycast(new Vector3(transform.position.x, transform.position.y + 0.5f, transform.position.z), (transform.forward - transform.up).normalized, out hit)){
                Debug.DrawRay(new Vector3(transform.position.x, transform.position.y + 0.5f, transform.position.z), (transform.forward - transform.up).normalized , Color.green);
                // print(hit.collider.gameObject.tag);
                if(hit.collider.gameObject.tag == "Water"){
                    stats = gameObject.GetComponent<Stats>();
                    if(stats != null){
                        if(stats.thirsty == true){
                            stats.currentThirst = stats.thirst;
                        }
                    }
                    isWalking = false;
                }
            }
            else{
                isWalking = false;
            }
        }
        
        if(gameObject.tag == "Monkey"){
            EatFruit eat = gameObject.GetComponent<EatFruit>();
            if(eat != null){
                eat.FindVisibleFruits();
            }
        }

        Cuddle cuddle = gameObject.GetComponent<Cuddle>();
        if(cuddle != null){
            cuddle.FindVisibleMates();
        }
        // CheckIfThirsty();
    }
}
