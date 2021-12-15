using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hunt : MonoBehaviour
{
    // Radius and angle of creature's movement.
    public float viewRadius = 10f;
    [Range(0, 360)]
    public float viewAngle = 360f;

    // Obstacle and monkey detection for creature.
    public LayerMask monkeyMask;
    public LayerMask treeMask;

    // A list of all the monkeys in the viewRadius and viewAngle.
    [HideInInspector]
    public List<Transform> visibleMonkeys = new List<Transform>();

    public Vector3 closestMonkey;

    // Child instance of Stats class to get creature's stats.
    Stats stats;

    // Checks if the creature ate one monkey already.
    public bool full = false;

    public void FindVisibleMonkeys(){
        // Clear list of visible monkeys to remove creatures if outside
        // the viewRadius and viewAngle.
        visibleMonkeys.Clear();
        // Finds monkeys in viewRadius and viewAngle.
        Collider[] monkeysInView = Physics.OverlapSphere(gameObject.transform.position, viewRadius, monkeyMask);

        // Gets Movement variables to allow wandering when no visible monkey is found
        // or finished eating.
        Movement movement = gameObject.GetComponent<Movement>();

        // Checks if the creature finds monkeys.
        if(monkeysInView.Length > 0){
            // Cycles through list of visibleMonkey and gets the closest monkey's position.
            for(int i = 0; i < monkeysInView.Length; i++){
                // Checks if closestMonkey is empty (in the beginning)
                // and sets the first visible monkey.
                if(closestMonkey == new Vector3(0, 0, 0)){
                    closestMonkey = monkeysInView[i].gameObject.transform.position;
                }
                // Checks if current monkey in view is closer than the closestMonkey
                //  and sets the closestMonkey to the current monkey.
                else if((transform.position - monkeysInView[i].gameObject.transform.position).magnitude < (transform.position - closestMonkey).magnitude){
                    closestMonkey = monkeysInView[i].gameObject.transform.position;
                }

                // The fruits are added to the visibleMonkeys list.
                visibleMonkeys.Add(monkeysInView[i].transform);
            }

            // Checks if the creature has not eaten a monkey.
            if(full == false){
                // Checks if closestMonkey is not empty.
                if(closestMonkey != new Vector3(0, 0, 0)){
                    // Finds direction to the closest monkey.
                    Vector3 dirToMonkey = (closestMonkey - transform.position).normalized;

                    // Checks if visible monkey is in viewAngle.
                    if(Vector3.Angle(transform.forward, dirToMonkey) < viewAngle / 2){
                        // Finds distance to the closest monkey.
                        float distToMonkey = Vector3.Distance(transform.position, closestMonkey);

                        // Checks if the view to the visible monkey is not obstructed by an obstacle.
                        if(!Physics.Raycast(transform.position, dirToMonkey, distToMonkey, treeMask)){
                            // Gets stats variables
                            stats = gameObject.GetComponent<Stats>();
                            
                            
                            if(stats.hungry == true){
                                // Removes all randomized movement.
                                movement.isWandering = true;
                                movement.isRotatingLeft = false;
                                movement.isRotatingRight = false;
                                movement.isWalking = false;

                                // Rotates towards the closestMonkey and moves towards them.
                                gameObject.GetComponent<Rigidbody>().rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(closestMonkey - transform.position, Vector3.up), (movement.rotSpeed / 4) * Time.deltaTime);
                                gameObject.GetComponent<Rigidbody>().MovePosition(transform.position + transform.forward * Time.deltaTime * movement.moveSpeed);
                                // Resets the closestMonkey
                                closestMonkey = new Vector3(0, 0, 0);
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
        // Checks if the creature collides with a monkey.
        if(collision.gameObject.tag == "Monkey"){
            // Gets stats variables
            stats = gameObject.GetComponent<Stats>();

            // Check for stats as it may have null instance
            if(stats != null){
                if(stats.hungry == true){
                    // Resets hunger stat
                    stats.currentHunger = stats.hunger;

                    // Destroys the monkey as it's eaten
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
