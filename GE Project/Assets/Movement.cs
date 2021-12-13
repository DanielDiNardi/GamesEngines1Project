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

    public List<Transform> visibleFruits = new List<Transform>();

    Vector3 closestFruit;

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
        moveSpeed = Random.Range(10f, 10f);
    }

    // Update is called once per frame
    void Update()
    {
        if(isWandering == false){
            StartCoroutine(Wander());
        }
        if(isRotatingRight == true){
            transform.Rotate(transform.up * Time.deltaTime * rotSpeed);
        }
        if(isRotatingLeft == true){
            transform.Rotate(transform.up * Time.deltaTime * -rotSpeed);
        }
        if(isWalking == true){
            transform.position += transform.forward * Time.deltaTime * moveSpeed;
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
        
        FindVisibleTargets();
    }

    void FindVisibleTargets(){
        // visibleFruits.Clear();
        // Collider[] fruitsInView = Physics.OverlapSphere(transform.position, viewRadius, fruitMask);

        // for(int i = 0; i < fruitsInView.Length; i++){
        //     if(closestFruit == null){
        //         closestFruit = fruitsInView[i].gameObject.transform.position;
        //     }
        //     else if((transform.position - fruitsInView[i].gameObject.transform.position).magnitude < (transform.position - closestFruit).magnitude){
        //         closestFruit = fruitsInView[i].gameObject.transform.position;
        //     }
        //     Transform fruit = fruitsInView[i].transform;
        //     Vector3 dirToFruit = (fruit.position - transform.position).normalized;

        //     if(Vector3.Angle(transform.forward, dirToFruit) < viewAngle / 2){
        //         float distToFruit = Vector3.Distance(transform.position, fruit.position);

        //         if(!Physics.Raycast(transform.position, dirToFruit, distToFruit, treeMask)){
        //             isWalking = false;
        //             Debug.Log("I see it");
        //             Vector3 rot = transform.rotation.eulerAngles;
        //             rot = new Vector3(rot.x, rot.y, rot.z);
        //             transform.rotation = Quaternion.RotateTowards(Quaternion.Euler(rot), Quaternion.LookRotation(closestFruit, Vector3.up), rotSpeed * Time.deltaTime);
        //             transform.position += transform.forward * Time.deltaTime * moveSpeed;
        //             visibleFruits.Add(fruit);
        //         }
        //     }
        // }
    }

    public Vector3 DirFromAngle(float angleInDegrees, bool angleIsGlobal){
        if(!angleIsGlobal){
            angleInDegrees += transform.eulerAngles.y;
        }
        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
    }
}
