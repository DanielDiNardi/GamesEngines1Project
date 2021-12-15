using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cuddle : MonoBehaviour
{
    public float viewRadius = 10f;
    [Range(0, 360)]
    public float viewAngle = 360f;

    public LayerMask mateMask;
    public LayerMask treeMask;

    [HideInInspector]
    public List<Transform> visibleMates = new List<Transform>();

    public Vector3 closestMate;

    Stats stats;

    public bool mated = false;

    public GameObject baby;

    public bool doneBreeding = false;

    IEnumerator MakeBaby(Collision collision){
        yield return new WaitForSeconds(6);
        if(gameObject.GetComponent<Stats>().male == false){
            GameObject newBorn = Instantiate(baby, (collision.gameObject.transform.position + gameObject.transform.position) / 2, transform.rotation);
            newBorn.GetComponent<Movement>().isWandering = false;
        }
        doneBreeding = false;
    }

    public void FindVisibleMates(){
        visibleMates.Clear();
        Collider[] matesInView = Physics.OverlapSphere(gameObject.transform.position, viewRadius, mateMask);

        Movement movement = gameObject.GetComponent<Movement>();

        if(matesInView.Length > 2){
            for(int i = 0; i < matesInView.Length; i+=2){
                if(GameObject.ReferenceEquals(gameObject,matesInView[i].gameObject) == false && matesInView[i].gameObject.GetComponent<Stats>().male != gameObject.GetComponent<Stats>().male){
                    if(closestMate == new Vector3(0, 0, 0)){
                        closestMate = matesInView[i].gameObject.transform.position;
                    }
                    else if((transform.position - matesInView[i].gameObject.transform.position).magnitude < (transform.position - closestMate).magnitude){
                        closestMate = matesInView[i].gameObject.transform.position;
                    }

                    if(matesInView[i].gameObject.GetComponent<Stats>().cuddly == true){
                        visibleMates.Add(matesInView[i].transform);
                    }
                }
            }

            if(mated == false){
                if(closestMate != new Vector3(0, 0, 0)){
                    Vector3 dirToMate = (closestMate - transform.position).normalized;

                    if(Vector3.Angle(transform.forward, dirToMate) < viewAngle / 2){
                        float distToMate = Vector3.Distance(transform.position, closestMate);

                        if(!Physics.Raycast(transform.position, dirToMate, distToMate, treeMask)){
                            stats = gameObject.GetComponent<Stats>();
                            if(stats.cuddly == true){
                                movement.isWandering = true;
                                movement.isRotatingLeft = false;
                                movement.isRotatingRight = false;
                                movement.isWalking = false;

                                gameObject.GetComponent<Rigidbody>().rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(closestMate - transform.position, Vector3.up), (movement.rotSpeed / 4) * Time.deltaTime);
                                gameObject.GetComponent<Rigidbody>().MovePosition(transform.position + transform.forward * Time.deltaTime * movement.moveSpeed);
                                closestMate = new Vector3(0, 0, 0);
                            }
                        }
                    }
                }
            }
        }
    }

    void OnCollisionEnter(Collision collision){
        if(collision.gameObject.tag == "Monkey" && collision.gameObject.GetComponent<Stats>().male != gameObject.GetComponent<Stats>().male  && collision.gameObject.GetComponent<Stats>().cuddly == true && gameObject.GetComponent<Stats>().cuddly == true){
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