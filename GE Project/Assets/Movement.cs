using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    Stats stats;

    public float moveSpeed = 0f;
    public float rotSpeed = 100f;

    private bool isWandering = false;
    private bool isRotatingLeft = false;
    private bool isRotatingRight = false;
    private bool isWalking = false;

    RaycastHit hit;

    public float viewRadius = 100f;
    [Range(0, 360)]
    public float viewAngle;

    public LayerMask fruitMask;
    public LayerMask treeMask;

    public LayerMask waterMask;
    public LayerMask terrainMask;

    [HideInInspector]
    public List<Transform> visibleFruits = new List<Transform>();

    [HideInInspector]
    public List<Transform> visibleWater = new List<Transform>();

    Vector3 closestFruit;

    Vector3 closestWater;

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
        
        FindVisibleFruits();
        // CheckIfThirsty();
    }

    void FindVisibleFruits(){
        visibleFruits.Clear();
        Collider[] fruitsInView = Physics.OverlapSphere(transform.position, viewRadius, fruitMask);

        for(int i = 0; i < fruitsInView.Length; i++){
            if(closestFruit == null){
                closestFruit = fruitsInView[i].gameObject.transform.position;
            }
            else if((transform.position - fruitsInView[i].gameObject.transform.position).magnitude < (transform.position - closestFruit).magnitude){
                closestFruit = fruitsInView[i].gameObject.transform.position;
            }
            Transform fruit = fruitsInView[i].transform;
            Vector3 dirToFruit = (fruit.position - transform.position).normalized;

            if(Vector3.Angle(transform.forward, dirToFruit) < viewAngle / 2){
                float distToFruit = Vector3.Distance(transform.position, fruit.position);

                if(!Physics.Raycast(transform.position, dirToFruit, distToFruit, treeMask)){
                    stats = gameObject.GetComponent<Stats>();
                    if(stats.hungry == true){
                        isWandering = true;
                        isRotatingLeft = false;
                        isRotatingRight = false;
                        isWalking = false;

                        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(closestFruit - transform.position, Vector3.up), (rotSpeed / 4) * Time.deltaTime);
                        transform.position = Vector3.MoveTowards(transform.position, closestFruit, moveSpeed * Time.deltaTime);
                    }
                    
                    visibleFruits.Add(fruit);
                }
            }
        }
    }

    public Vector3 DirFromAngle(float angleInDegrees, bool angleIsGlobal){
        if(!angleIsGlobal){
            angleInDegrees += transform.eulerAngles.y;
        }
        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
    }

    void OnCollisionEnter(Collision collision){
        if(collision.gameObject.tag == "Fig"){
            stats = gameObject.GetComponent<Stats>();
            if(stats != null){
                if(stats.hungry == true){
                    stats.currentHunger = stats.hunger;
                    Destroy(collision.gameObject);
                    isWandering = false;
                }
            }
            
        }
    }

    void CheckIfThirsty(){

        visibleWater.Clear();
        Collider[] waterInView = Physics.OverlapSphere(transform.position, viewRadius, waterMask);

        for(int i = 0; i < waterInView.Length; i++){
            if(closestWater == null){
                closestWater = waterInView[i].gameObject.transform.position;
            }
            else if((transform.position - waterInView[i].gameObject.transform.position).magnitude < (transform.position - closestWater).magnitude){
                closestWater = waterInView[i].gameObject.transform.position;
            }
            Transform water = waterInView[i].transform;
            Vector3 dirToWater = (water.position - transform.position).normalized;

            if(Vector3.Angle(transform.forward, dirToWater) < viewAngle / 2){
                float distToWater = Vector3.Distance(transform.position, water.position);

                if(!Physics.Raycast(transform.position, dirToWater, distToWater, terrainMask)){
                    stats = gameObject.GetComponent<Stats>();
                    if(stats.thirsty == true){
                        isWandering = true;
                        isRotatingLeft = false;
                        isRotatingRight = false;
                        isWalking = false;

                        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(closestWater - transform.position, Vector3.up), (rotSpeed / 4) * Time.deltaTime);
                        transform.position = Vector3.MoveTowards(transform.position, closestWater, moveSpeed * Time.deltaTime);
                    }
                    
                    visibleFruits.Add(water);
                }
            }
        }

        // stats = gameObject.GetComponent<Stats>();

        // if(stats != null){
        //     if(stats.thirsty == true){
        //         if(waterPosition == new Vector3(0, 0, 0)){
        //             // isWandering = false;
        //         }
        //         else{
        //             isWandering = true;
        //             isRotatingLeft = false;
        //             isRotatingRight = false;
        //             isWalking = false;

        //             if(Physics.Raycast(new Vector3(transform.position.x, transform.position.y + 0.5f, transform.position.z), (transform.forward - transform.up).normalized, out hit)){
        //                 Debug.DrawRay(new Vector3(transform.position.x, transform.position.y + 0.5f, transform.position.z), (transform.forward - transform.up).normalized , Color.green);
        //                 // print(hit.collider.gameObject.tag);
        //                 if(hit.collider.gameObject.tag == "Water"){
        //                     // transform.position = transform.position;
        //                     // stats = gameObject.GetComponent<Stats>();
        //                     // if(stats != null){
        //                         if(stats.thirsty == true){
        //                             stats.currentThirst = stats.thirst;
        //                         }
        //                         // else{
        //                         //     waterPosition = transform.position;
        //                         // }
        //                     // }
        //                     isWalking = false;
        //                 }
        //                 else{
        //                     Debug.Log(new Vector3(waterPosition.x, 0, waterPosition.z) - transform.position);
        //                     transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(new Vector3(waterPosition.x, 0, waterPosition.z) - transform.position, Vector3.up), (rotSpeed / 4) * Time.deltaTime);
        //                     // transform.position = Vector3.MoveTowards(transform.position, waterPosition, moveSpeed * Time.deltaTime);
        //                 }
        //             }
        //         }
        //     }
        // }
    }
}
