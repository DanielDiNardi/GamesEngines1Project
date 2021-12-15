# Games Engines 1 Project Proposal

## Description
For my Games Engines 1 project, I plan on creating an organism that can move, search, and interact with others of its species. The organism will live in a confined space that has everything it needs to live (water, food, and breeding mates). The organism will have a FOV so that it can search for the resources it needs to survive. Each organism will have a necessity bar that will influence its movement. The world in which the organisms will inhabit, will be procedurally generated. I plan on creating a limited 3x3 grid that will have randomly chosen tiles (acrisols soil and water) to create the flat terrain. Each grass tile will be further divided into a 3x3 grid that will have a chance of spawning one of three assets (preditor (yellow-throated marten), prey (long-tailed macaque), or fig trees that drop the fig fruit that the prey eats) per grid position.

## Requirements
* Organism object.
* Limited world.
* Procedural Terrain.
* Needs list.
* Randomised lerped movement.
* Organism FOV.
* Organism drinking, eating, and breeding mechanics.

## Tools
* Unity.
* Visual Studio Code.
* MagicaVoxel (Object 3D Modelling).

# Games Engines 1 Project Report - (South East Asian Ecosystem Simulation)

Name: Daniel Di Nardi
Student Number: C18487682
Class Group: TU 858

## Description
This project is a simulation on a South East Asian (SEA) ecosystem. The simulation contains a prey, a preditor, a procedurally generated terrain, lakes, trees found in the area of SEA, and the fruits of the tree.

## How it works
A terrain is generated using perlin noise (Perlin.cs) and the water plane is placed inside the terrain to simulate water. 

```cs
  using UnityEngine;

public class Perlin : MonoBehaviour
{
    public int depth = 10;

    public int width = 256;
    public int height = 256;

    public float scale = 20f;

    public float offsetX = 100f;
    public float offsetY = 100f;

    void Start(){
        offsetX = Random.Range(0f, 9999f);
        offsetY = Random.Range(0f, 9999f);
        Terrain terrain = GetComponent<Terrain>();
        terrain.terrainData = GenerateTerrain(terrain.terrainData);
    }

    TerrainData GenerateTerrain(TerrainData terrainData){
        terrainData.heightmapResolution = width + 1;

        terrainData.size = new Vector3 (width, depth, height);

        terrainData.SetHeights(0, 0, GenerateHeights());

        return terrainData;
    }
    
    float[,] GenerateHeights(){
        float[,] heights = new float[width, height];
        for(int x = 0; x < width; x++){
            for(int y = 0; y < height; y++){
                heights[x, y] = CalculateHeight(x, y);
            }
        }

        return heights;
    }

    float CalculateHeight(int x, int y){
        float xCoord = (float)x / width * scale + offsetX;
        float yCoord = (float)y / height * scale + offsetY;

        return Mathf.PerlinNoise(xCoord, yCoord);
    }
}
```

A spawner generates random points and spawns tree, marten and monkey objects (Spawner.cs); if the point is above water, the object does not spawn. 

```cs
  using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{

    GameObject[] objects;
    public GameObject monkey;
    public GameObject marten;
    public GameObject tree;
    public int[] populations = new int[] {100, 10, 50};
    public float spawnWidth = 246f;
    public float spawnHeight = 246f;

    System.Collections.IEnumerator Spawn()
    {
        yield return new WaitForSeconds(1);
        RaycastHit hit;

        for(int i = 0; i < objects.Length; i++){
            for(int j = 0; j < populations[i]; j++){
                Vector3 spawnPoint = new Vector3(Random.Range(10f, spawnWidth), transform.position.y, Random.Range(10f, spawnHeight));
                if(Physics.Raycast(spawnPoint, transform.TransformDirection(Vector3.down), out hit)){
                    if(hit.distance < 4.43f){
                        Instantiate(objects[i], spawnPoint, transform.rotation);
                    }
                }
            }
        }

    }

    void CallCoroutine()
    {
        StartCoroutine(Spawn());
    }

    void Start(){

        objects = new GameObject[] {monkey, marten, tree};

        CallCoroutine();

    }
}

```

Once the trees have spawned, a fruit will spawn within it's range after 5 to 30 seconds (DropFruit.cs). 

```cs
  using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropFruit : MonoBehaviour
{
    public GameObject fig;

    System.Collections.IEnumerator Drop()
    {
        // Continuously drops fruits in between 5 to 30 seconds
        // in the range of the tree's position.
        while(true)
        {
            yield return new WaitForSeconds(Random.Range(5, 30));
            Instantiate(fig, new Vector3(transform.position.x + Random.Range(-2f, 2f), transform.position.y + 4, transform.position.z + Random.Range(-2f, 2f)), transform.rotation);
        }
    }

    public void OnEnable()
    {
        StartCoroutine(Drop());
    }
}
```

When the marten and the monkeys have spawned, their needs are set after 5 seconds and a death age is set (Stats.cs). If the thirst or hunger needs reach 0, they will die. The needs can be reset by either eating the fruit dropped by the tree, drink from the water, or reproduce. The age increases per 5 seconds, and once the age reaches the death age, they die.

```cs
  using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stats : MonoBehaviour
{
    public float thirst = 0;
    public float hunger = 0;
    public float reproductionNeed = 0;

    public float thirstPercent = 0;
    public float hungerPercent = 0;
    public float reproductionNeedPercent = 0;

    public bool thirsty = false;
    public bool hungry = false;
    public bool cuddly = false;

    public float currentThirst = 0;
    public float currentHunger = 0;
    public float currentReproductionNeed = 0;

    public bool male;

    bool reduceStats = true;

    public int age = 0;

    bool birthday = false;

    public int deathAge;

    EatFruit eatFruit;

    Hunt hunt;

    Cuddle cuddle;

    System.Collections.IEnumerator NeedReduction()
    {
        yield return new WaitForSeconds(5);
        currentThirst--;
        currentHunger--;
        currentReproductionNeed--;

        thirstPercent = (currentThirst / thirst) * 100f;
        hungerPercent = (currentHunger / hunger) * 100f;
        reproductionNeedPercent = (currentReproductionNeed / reproductionNeed) * 100f;

        if(thirstPercent < hungerPercent && thirstPercent < reproductionNeedPercent){
            thirsty = true;
            hungry = false;
            cuddly = false;
        }
        else if(hungerPercent < thirstPercent && hungerPercent < reproductionNeedPercent){
            thirsty = false;
            hungry = true;
            cuddly = false;

            if(gameObject.tag == "Monkey"){
                eatFruit = gameObject.GetComponent<EatFruit>();
                eatFruit.full = false;
            }
            else if(gameObject.tag == "Marten"){
                hunt = gameObject.GetComponent<Hunt>();
                hunt.full = false;
            }
        }
        else if(reproductionNeedPercent < thirstPercent && reproductionNeedPercent < hungerPercent){
            thirsty = false;
            hungry = false;
            cuddly = true;

            cuddle = gameObject.GetComponent<Cuddle>();
            cuddle.mated = false;
        }
        
        if(thirstPercent == hungerPercent || thirstPercent == reproductionNeedPercent){
            currentThirst--;
        }
        else if(hungerPercent == reproductionNeedPercent){
            currentHunger--;
        }

        if(currentThirst <= 0 || currentHunger <= 0){
            Destroy(gameObject);
        }

        reduceStats = true;
    }

    IEnumerator Aging(){
        yield return new WaitForSeconds(5);
        age++;

        if(age >= deathAge){
            Destroy(gameObject);
        }

        birthday = false;

    }

    void CallCoroutine()
    {
        StartCoroutine(NeedReduction());
    }

    void CallAgingCoroutine(){
        StartCoroutine(Aging());
    }

    void Start(){
        thirst = Random.Range(10, 30);
        hunger = Random.Range(10, 50);
        reproductionNeed = Random.Range(10, 100);

        deathAge = Random.Range(10, 30);

        male = Random.Range(0, 2) == 0;

        currentThirst = thirst;
        currentHunger = hunger;
        currentReproductionNeed = reproductionNeed;
    }

    void Update(){
        if(reduceStats){
            reduceStats = false;   
            CallCoroutine();
        }

        if(birthday == false){
            birthday = true;   
            CallAgingCoroutine();
        }
    }
}

```

Monkeys and martens wander when they are searching for food, water and not in pursuit/being pursuited (Movement.cs).

```cs
  using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    Stats stats;

    public float moveSpeed = 0f;
    public float rotSpeed = 100f;

    public bool isWandering = false;
    public bool isRotatingLeft = false;
    public bool isRotatingRight = false;
    public bool isWalking = false;

    RaycastHit hit;

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
        
        if(gameObject.tag == "Monkey"){
            EatFruit eat = gameObject.GetComponent<EatFruit>();
            if(eat != null){
                eat.FindVisibleFruits();
            }

            Flee flee = gameObject.GetComponent<Flee>();
            if(flee != null){
                flee.FindVisibleMartens();
            }
        }
        else if(gameObject.tag == "Marten"){
            Hunt eat = gameObject.GetComponent<Hunt>();
            if(eat != null){
                eat.FindVisibleMonkeys();
            }
        }

        Cuddle cuddle = gameObject.GetComponent<Cuddle>();
        if(cuddle != null){
            cuddle.FindVisibleMates();
        }
    }
}

```

Only monkeys eat fruit (Eatfruit.cs) and martens only eat monkeys (Hunt.cs). Monkeys will search for the closest fruit in their field of view. When they find a fruit, they will stop wandering, move towards the fruit and eat it. If a marten sees a monkey they will stop wandering and try to eat the closest monkey. 

```cs
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

```

The monkeys will flee if a marten is spotted in their field of view (Flee.cs); the speed of the monkey is increased while it sees the marten. If a monkey or marten fall into the water, they will drown (DespawnOnContact.cs).

```cs
  using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DespawnOnContact : MonoBehaviour
{
    // When a creature touches the water, it drowns.
    private void OnCollisionEnter(Collision col){
        Destroy(col.gameObject);
    }
}

```

Both monkeys and martens can reproduce if they spot another one of its kind (Cuddle.cs), its the opposite gender and if the other wants to cuddle; reproduction means instantiating a baby in between the mother and father positions.

```cs
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
```



| Class / Assets  | Source  |
|---|---|
| Cuddle.cs  | Modfied from [reference](https://www.youtube.com/watch?v=rQG9aUWarwE)  |
| DecayFruits.cs  | Self written  |
| DespawnOnContact.cs  | Self written  |
| DropFruit.cs  | Self written  |
| EatFruit.cs  | Modified from [reference](https://www.youtube.com/watch?v=rQG9aUWarwE)  |
| Flee.cs  | Modified from [reference](https://www.youtube.com/watch?v=rQG9aUWarwE)  |
| Hunt.cs  | Modified from [reference](https://www.youtube.com/watch?v=rQG9aUWarwE)  |
| Movement.cs  | Modified from [reference](https://www.youtube.com/watch?v=aEPSuGlcTUQ&t=1146s)  |
| Perlin.cs  | From [reference](https://www.youtube.com/watch?v=vFvwyu_ZKfU)  |
| Spawner.cs  | Self written  |
| Stats.cs  | Self written  |
| EatFruitFOV.cs  | From [reference](https://www.youtube.com/watch?v=rQG9aUWarwE)  |
| HuntFOV.cs  | From [reference](https://www.youtube.com/watch?v=rQG9aUWarwE)  |
| MatingFOV.cs  | Modified from [reference](https://www.youtube.com/watch?v=rQG9aUWarwE)  |

## References
Field of view visualisation (E01) by Sebastian Lague 
  https://www.youtube.com/watch?v=rQG9aUWarwE

How To Make Easy Wander AI In Unity! by TRGameDev
 https://www.youtube.com/watch?v=aEPSuGlcTUQ&t=1146s
 
GENERATING TERRAIN in Unity - Procedural Generation Tutorial by Brackeys
  https://www.youtube.com/watch?v=vFvwyu_ZKfU
  
## What I am most proud of in the assignment
I am proud of the whole project, especially the object interaction scripts such as Cuddle, Hunt, Movement, Flee, EatFruit.

## Screenshots
### Terrain
![Terrain](https://user-images.githubusercontent.com/55494739/144599147-e98266fb-5bdc-47a6-a6ee-a3f17ceb7e32.png)

### Marten
![Marten](/Images/Marten.png)

### Monkey
![Monkey](/Images/Monke.png)

### Fig Tree
![FigTree](/Images/FigTree.png)

### Fig
![Fig](/Images/Fig.png)
