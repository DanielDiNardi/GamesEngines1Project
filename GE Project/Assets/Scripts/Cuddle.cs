using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cuddle : MonoBehaviour
{
    // Radius and angle of creature's movement.
    public float viewRadius = 10f;
    [Range(0, 360)]
    public float viewAngle = 360f;

    // Obstacle and mate detection for creature.
    public LayerMask mateMask;
    public LayerMask treeMask;

    // A list of all the mates in the viewRadius and viewAngle.
    [HideInInspector]
    public List<Transform> visibleMates = new List<Transform>();
    // Holds position of closest mate when detected.
    public Vector3 closestMate;

    // Child instance of Stats class to get creature's stats.
    Stats stats;

    // Checks if mating is allowed.
    public bool mated = false;
    // Checks if baby is instantiated.
    public bool doneBreeding = false;
    // Stores new GameObject instance of the baby.
    public GameObject baby;


    IEnumerator MakeBaby(Collision collision){
        // Get mother and father position to instantiate the new instance
        // of the baby in the average position.
        Vector3 fatherPos = collision.gameObject.transform.position;
        Vector3 motherPos = gameObject.transform.position;

        // Gestation period.
        yield return new WaitForSeconds(6);

        // Checks if gameObject is male or female to allow instantiation of baby.
        if(gameObject.GetComponent<Stats>().male == false){

            GameObject newBorn = Instantiate(baby, (fatherPos + motherPos) / 2, transform.rotation);

            // Sets newBorn to walk.
            newBorn.GetComponent<Movement>().isWandering = false;
        }
        // Stops breeding process to allow only one instantiation of baby.
        doneBreeding = false;
    }

    public void FindVisibleMates(){
        // Clear list of visible mates to remove creatures if outside
        // the viewRadius and viewAngle.
        visibleMates.Clear();
        // Finds creatures in viewRadius and viewAngle.
        Collider[] matesInView = Physics.OverlapSphere(gameObject.transform.position, viewRadius, mateMask);

        // Gets Movement variables to allow wandering when no visible mate is found
        // or finished mating.
        Movement movement = gameObject.GetComponent<Movement>();

        // matesInView includes its own colliders so it only mates when it finds
        // more than just its own colliders.
        if(matesInView.Length > 2){
            // Cycles through list of visibleMates and gets the closest mate's position.
            for(int i = 0; i < matesInView.Length; i+=2){
                // Checks if the GameObject of the mate in question is the same as itself
                // and checks if the creature and mate are not the same gender.
                if(GameObject.ReferenceEquals(gameObject,matesInView[i].gameObject) == false && matesInView[i].gameObject.GetComponent<Stats>().male != gameObject.GetComponent<Stats>().male){
                    // Checks if closestMate is empty (in the beginning)
                    // and sets the first visible mate.
                    if(closestMate == new Vector3(0, 0, 0)){
                        closestMate = matesInView[i].gameObject.transform.position;
                    }
                    // Checks if current mate in view is closer than the closestMate
                    //  and sets the closestMate to the current mate.
                    else if((transform.position - matesInView[i].gameObject.transform.position).magnitude < (transform.position - closestMate).magnitude){
                        closestMate = matesInView[i].gameObject.transform.position;
                    }

                    // Checks if mate in view is also cuddly,
                    // it is added to the visibleMates list.
                    if(matesInView[i].gameObject.GetComponent<Stats>().cuddly == true){
                        visibleMates.Add(matesInView[i].transform);
                    }
                }
            }

            // Checks if creature has just mated.
            if(mated == false){
                // Checks if closestMate is not empty.
                if(closestMate != new Vector3(0, 0, 0)){
                    // Finds direction to the closest mate.
                    Vector3 dirToMate = (closestMate - transform.position).normalized;

                    // Checks if visible mate is in viewAngle.
                    if(Vector3.Angle(transform.forward, dirToMate) < viewAngle / 2){
                        // Finds distance to the closest mate.
                        float distToMate = Vector3.Distance(transform.position, closestMate);

                        // Checks if the view to the visible mate is not obstructed by an obstacle.
                        if(!Physics.Raycast(transform.position, dirToMate, distToMate, treeMask)){
                            // Gets stats variables
                            stats = gameObject.GetComponent<Stats>();
                            
                            if(stats.cuddly == true){
                                // Removes all randomized movement.
                                movement.isWandering = true;
                                movement.isRotatingLeft = false;
                                movement.isRotatingRight = false;
                                movement.isWalking = false;

                                // Rotates towards the closestMate and moves towards them.
                                gameObject.GetComponent<Rigidbody>().rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(closestMate - transform.position, Vector3.up), (movement.rotSpeed / 4) * Time.deltaTime);
                                gameObject.GetComponent<Rigidbody>().MovePosition(transform.position + transform.forward * Time.deltaTime * movement.moveSpeed);
                                
                                // Resets the closestMate
                                closestMate = new Vector3(0, 0, 0);
                            }
                        }
                    }
                }
            }
        }
    }

    void OnCollisionEnter(Collision collision){
        // Checks if the creature is a monkey, checks collided creature's gender is the opposite,
        // and if they're both cuddly.
        if(collision.gameObject.tag == "Monkey" && collision.gameObject.GetComponent<Stats>().male != gameObject.GetComponent<Stats>().male  && collision.gameObject.GetComponent<Stats>().cuddly == true && gameObject.GetComponent<Stats>().cuddly == true){
            
            stats = gameObject.GetComponent<Stats>();
            // Resets reproductionNeed
            stats.currentReproductionNeed = stats.reproductionNeed;

            // Starts randomized movement
            Movement movement = gameObject.GetComponent<Movement>();
            movement.isWandering = false;

            // Disallows mating.
            mated = true;

            // Checks if the creature is done breeding
            // and starts gestation period only once.
            if(doneBreeding == false){
                doneBreeding = true;
                StartCoroutine(MakeBaby(collision));
            }
        }
        // Checks if the creature is a marten
        else if(collision.gameObject.tag == "Marten" && collision.gameObject.GetComponent<Stats>().male != gameObject.GetComponent<Stats>().male  && collision.gameObject.GetComponent<Stats>().cuddly == true && gameObject.GetComponent<Stats>().cuddly == true){
            
            stats = gameObject.GetComponent<Stats>();
            stats.currentReproductionNeed = stats.reproductionNeed;
            
            Movement movement = gameObject.GetComponent<Movement>();
            movement.isWandering = false;
            
            mated = true;

            if(doneBreeding == false){
                doneBreeding = true;
                StartCoroutine(MakeBaby(collision));
            }
        }
    }
}