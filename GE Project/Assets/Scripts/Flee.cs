using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flee : MonoBehaviour
{
    // Radius and angle of creature's movement.
    public float viewRadius = 10f;
    [Range(0, 360)]
    public float viewAngle = 360f;

    // Obstacle and predator detection for creature.
    public LayerMask martenMask;
    public LayerMask treeMask;

    // A list of all the predators in the viewRadius and viewAngle.
    [HideInInspector]
    public List<Transform> visibleMartens = new List<Transform>();

    public Vector3 closestMarten;

    // Child instance of Stats class to get creature's stats.
    Stats stats;

    public void FindVisibleMartens(){
        // Clear list of visible predators to remove creatures if outside
        // the viewRadius and viewAngle.
        visibleMartens.Clear();
        // Finds creatures in viewRadius and viewAngle.
        Collider[] martensInView = Physics.OverlapSphere(gameObject.transform.position, viewRadius, martenMask);

        // Gets Movement variables to allow wandering when no visible predator is found
        // or finished fleeing.
        Movement movement = gameObject.GetComponent<Movement>();

        // Checks if the creature finds predators.
        if(martensInView.Length > 0){
            // Cycles through list of visibleMartens and gets the closest predator's position.
            for(int i = 0; i < martensInView.Length; i++){
                // Checks if closestMarten is empty (in the beginning)
                // and sets the first visible predator.
                if(closestMarten == new Vector3(0, 0, 0)){
                    closestMarten = martensInView[i].gameObject.transform.position;
                }
                // Checks if current predator in view is closer than the closestMarten
                //  and sets the closestMarten to the current predator.
                else if((transform.position - martensInView[i].gameObject.transform.position).magnitude < (transform.position - closestMarten).magnitude){
                    closestMarten = martensInView[i].gameObject.transform.position;
                }

                // The predator is added to the visibleMartens list.
                visibleMartens.Add(martensInView[i].transform);
            }

            // Checks if closestMarten is not empty.
            if(closestMarten != new Vector3(0, 0, 0)){
                // Finds direction to the closest predator.
                Vector3 dirToMarten = (closestMarten - transform.position).normalized;

                // Checks if visible predator is in viewAngle.
                if(Vector3.Angle(transform.forward, dirToMarten) < viewAngle / 2){
                    float distToMarten = Vector3.Distance(transform.position, closestMarten);

                    // Checks if the view to the visible predator is not obstructed by an obstacle.
                    if(!Physics.Raycast(transform.position, dirToMarten, distToMarten, treeMask)){
                        // Removes all randomized movement.
                        movement.isWandering = true;
                        movement.isRotatingLeft = false;
                        movement.isRotatingRight = false;
                        movement.isWalking = false;

                        // Rotates away from the closestMarten and moves away from them.
                        gameObject.GetComponent<Rigidbody>().rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(transform.position - closestMarten, Vector3.up), (movement.rotSpeed / 4) * Time.deltaTime);
                        gameObject.GetComponent<Rigidbody>().MovePosition(transform.position + transform.forward * Time.deltaTime * (movement.moveSpeed * 1.5f));
                        
                        // Resets the closestFruit
                        closestMarten = new Vector3(0, 0, 0);
                    }
                }
            }
        }
    }

    // Gets the angle of the view for the Editor
    public Vector3 DirFromAngle(float angleInDegrees, bool angleIsGlobal){
        if(!angleIsGlobal){
            angleInDegrees += transform.eulerAngles.y;
        }
        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
    }
}
