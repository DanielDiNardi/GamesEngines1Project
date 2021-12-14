using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EatFruit : MonoBehaviour
{
    public float viewRadius = 10f;
    [Range(0, 360)]
    public float viewAngle = 360f;

    public LayerMask fruitMask;
    public LayerMask treeMask;

    [HideInInspector]
    public List<Transform> visibleFruits = new List<Transform>();

    public Vector3 closestFruit;

    Stats stats;

    public bool full = false;

    public void FindVisibleFruits(){

        visibleFruits.Clear();
        Collider[] fruitsInView = Physics.OverlapSphere(gameObject.transform.position, viewRadius, fruitMask);

        Movement movement = gameObject.GetComponent<Movement>();

        if(fruitsInView.Length > 0){
            for(int i = 0; i < fruitsInView.Length; i++){
                if(closestFruit == new Vector3(0, 0, 0)){
                    closestFruit = fruitsInView[i].gameObject.transform.position;
                }
                else if((transform.position - fruitsInView[i].gameObject.transform.position).magnitude < (transform.position - closestFruit).magnitude){
                    closestFruit = fruitsInView[i].gameObject.transform.position;
                }

                visibleFruits.Add(fruitsInView[i].transform);
            }

            if(full == false){
                if(closestFruit != new Vector3(0, 0, 0)){
                    Vector3 dirToFruit = (closestFruit - transform.position).normalized;

                    if(Vector3.Angle(transform.forward, dirToFruit) < viewAngle / 2){
                        float distToFruit = Vector3.Distance(transform.position, closestFruit);

                        if(!Physics.Raycast(transform.position, dirToFruit, distToFruit, treeMask)){
                            stats = gameObject.GetComponent<Stats>();
                            if(stats.hungry == true){
                                movement.isWandering = true;
                                movement.isRotatingLeft = false;
                                movement.isRotatingRight = false;
                                movement.isWalking = false;

                                gameObject.GetComponent<Rigidbody>().rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(closestFruit - transform.position, Vector3.up), (movement.rotSpeed / 4) * Time.deltaTime);
                                gameObject.GetComponent<Rigidbody>().MovePosition(transform.position + transform.forward * Time.deltaTime * movement.moveSpeed);
                                closestFruit = new Vector3(0, 0, 0);
                            }
                        }
                    }
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
                    Movement movement = gameObject.GetComponent<Movement>();
                    movement.isWandering = false;
                    full = true;
                }
            }
            
        }
    }
}
