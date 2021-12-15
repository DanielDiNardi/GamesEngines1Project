using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flee : MonoBehaviour
{
    public float viewRadius = 10f;
    [Range(0, 360)]
    public float viewAngle = 360f;

    public LayerMask martenMask;
    public LayerMask treeMask;

    [HideInInspector]
    public List<Transform> visibleMartens = new List<Transform>();

    public Vector3 closestMarten;

    Stats stats;

    public void FindVisibleMartens(){

        visibleMartens.Clear();
        Collider[] martensInView = Physics.OverlapSphere(gameObject.transform.position, viewRadius, martenMask);

        Movement movement = gameObject.GetComponent<Movement>();

        if(martensInView.Length > 0){
            for(int i = 0; i < martensInView.Length; i++){
                if(closestMarten == new Vector3(0, 0, 0)){
                    closestMarten = martensInView[i].gameObject.transform.position;
                }
                else if((transform.position - martensInView[i].gameObject.transform.position).magnitude < (transform.position - closestMarten).magnitude){
                    closestMarten = martensInView[i].gameObject.transform.position;
                }

                visibleMartens.Add(martensInView[i].transform);
            }

            if(closestMarten != new Vector3(0, 0, 0)){
                Vector3 dirToMarten = (closestMarten - transform.position).normalized;

                if(Vector3.Angle(transform.forward, dirToMarten) < viewAngle / 2){
                    float distToMarten = Vector3.Distance(transform.position, closestMarten);


                    if(!Physics.Raycast(transform.position, dirToMarten, distToMarten, treeMask)){
                        movement.isWandering = true;
                        movement.isRotatingLeft = false;
                        movement.isRotatingRight = false;
                        movement.isWalking = false;

                        gameObject.GetComponent<Rigidbody>().rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(transform.position - closestMarten, Vector3.up), (movement.rotSpeed / 4) * Time.deltaTime);
                        gameObject.GetComponent<Rigidbody>().MovePosition(transform.position + transform.forward * Time.deltaTime * (movement.moveSpeed * 1.5f));
                        closestMarten = new Vector3(0, 0, 0);
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
}
