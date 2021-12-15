using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EatFruit : MonoBehaviour
{
    // Radius and angle of creature's movement.
    public float viewRadius = 10f;
    [Range(0, 360)]
    public float viewAngle = 360f;

    // Obstacle and fruit detection for creature.
    public LayerMask fruitMask;
    public LayerMask treeMask;

    // A list of all the fruits in the viewRadius and viewAngle.
    [HideInInspector]
    public List<Transform> visibleFruits = new List<Transform>();
    // Holds position of closest fruit when detected.
    public Vector3 closestFruit;

    // Child instance of Stats class to get creature's stats.
    Stats stats;

    // Checks if the creature ate one fruit already.
    public bool full = false;

    public void FindVisibleFruits(){
        // Clear list of visible fruits to remove creatures if outside
        // the viewRadius and viewAngle.
        visibleFruits.Clear();
        // Finds fruits in viewRadius and viewAngle.
        Collider[] fruitsInView = Physics.OverlapSphere(gameObject.transform.position, viewRadius, fruitMask);

        // Gets Movement variables to allow wandering when no visible fruit is found
        // or finished eating.
        Movement movement = gameObject.GetComponent<Movement>();

        // Checks if the creature finds fruit.
        if(fruitsInView.Length > 0){
            // Cycles through list of visibleFruits and gets the closest fruit's position.
            for(int i = 0; i < fruitsInView.Length; i++){
                // Checks if closestFruit is empty (in the beginning)
                // and sets the first visible fruit.
                if(closestFruit == new Vector3(0, 0, 0)){
                    closestFruit = fruitsInView[i].gameObject.transform.position;
                }
                // Checks if current fruit in view is closer than the closestFruit
                //  and sets the closestFruit to the current fruit.
                else if((transform.position - fruitsInView[i].gameObject.transform.position).magnitude < (transform.position - closestFruit).magnitude){
                    closestFruit = fruitsInView[i].gameObject.transform.position;
                }

                // The fruits are added to the visibleFruits list.
                visibleFruits.Add(fruitsInView[i].transform);
            }

            // Checks if the creature has not eaten a fruit.
            if(full == false){
                // Checks if closestFruit is not empty.
                if(closestFruit != new Vector3(0, 0, 0)){
                    // Finds direction to the closest fruit.
                    Vector3 dirToFruit = (closestFruit - transform.position).normalized;
                    
                    // Checks if visible fruit is in viewAngle.
                    if(Vector3.Angle(transform.forward, dirToFruit) < viewAngle / 2){
                        // Finds distance to the closest fruit.
                        float distToFruit = Vector3.Distance(transform.position, closestFruit);

                        // Checks if the view to the visible fruit is not obstructed by an obstacle.
                        if(!Physics.Raycast(transform.position, dirToFruit, distToFruit, treeMask)){
                            // Gets stats variables
                            stats = gameObject.GetComponent<Stats>();

                            if(stats.hungry == true){
                                // Removes all randomized movement.
                                movement.isWandering = true;
                                movement.isRotatingLeft = false;
                                movement.isRotatingRight = false;
                                movement.isWalking = false;

                                // Rotates towards the closestFruit and moves towards them.
                                gameObject.GetComponent<Rigidbody>().rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(closestFruit - transform.position, Vector3.up), (movement.rotSpeed / 4) * Time.deltaTime);
                                gameObject.GetComponent<Rigidbody>().MovePosition(transform.position + transform.forward * Time.deltaTime * movement.moveSpeed);
                                // Resets the closestFruit
                                closestFruit = new Vector3(0, 0, 0);
                            }
                        }
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

    void OnCollisionEnter(Collision collision){
        // Checks if the creature collides with a fruit.
        if(collision.gameObject.tag == "Fig"){
            // Gets stats variables
            stats = gameObject.GetComponent<Stats>();

            // Check for stats as it may have null instance
            if(stats != null){
                if(stats.hungry == true){
                    // Resets hunger stat
                    stats.currentHunger = stats.hunger;
                    
                    // Destroys the fruit as it's eaten
                    Destroy(collision.gameObject);
                    
                    // Gets movement stats and resets randomized movement.
                    Movement movement = gameObject.GetComponent<Movement>();
                    movement.isWandering = false;
                    
                    // Creature is full as it just ate fruit.
                    full = true;
                }
            }
            
        }
    }
}
