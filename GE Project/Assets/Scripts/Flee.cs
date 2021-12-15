using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flee : MonoBehaviour
{
    public float viewRadius = 10f;
    [Range(0, 360)]
    public float viewAngle = 360f;

    public LayerMask monkeyMask;
    public LayerMask treeMask;

    [HideInInspector]
    public List<Transform> visibleMartens = new List<Transform>();

    public Vector3 closestMarten;

    Stats stats;

    public bool full = false;

    public void FindVisibleMonkeys(){

        visibleMartens.Clear();
        Collider[] monkeysInView = Physics.OverlapSphere(gameObject.transform.position, viewRadius, monkeyMask);

        Movement movement = gameObject.GetComponent<Movement>();

        if(monkeysInView.Length > 0){
            for(int i = 0; i < monkeysInView.Length; i++){
                if(closestMarten == new Vector3(0, 0, 0)){
                    closestMarten = monkeysInView[i].gameObject.transform.position;
                }
                else if((transform.position - monkeysInView[i].gameObject.transform.position).magnitude < (transform.position - closestMarten).magnitude){
                    closestMarten = monkeysInView[i].gameObject.transform.position;
                }

                visibleMartens.Add(monkeysInView[i].transform);
            }

            if(full == false){
                if(closestMarten != new Vector3(0, 0, 0)){
                    Vector3 dirToMonkey = (closestMarten - transform.position).normalized;

                    if(Vector3.Angle(transform.forward, dirToMonkey) < viewAngle / 2){
                        float distToMonkey = Vector3.Distance(transform.position, closestMarten);

                        if(!Physics.Raycast(transform.position, dirToMonkey, distToMonkey, treeMask)){
                            stats = gameObject.GetComponent<Stats>();
                            if(stats.hungry == true){
                                movement.isWandering = true;
                                movement.isRotatingLeft = false;
                                movement.isRotatingRight = false;
                                movement.isWalking = false;

                                gameObject.GetComponent<Rigidbody>().rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(closestMarten - transform.position, Vector3.up), (movement.rotSpeed / 4) * Time.deltaTime);
                                gameObject.GetComponent<Rigidbody>().MovePosition(transform.position + transform.forward * Time.deltaTime * movement.moveSpeed);
                                closestMarten = new Vector3(0, 0, 0);
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
        if(collision.gameObject.tag == "Monkey"){
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
