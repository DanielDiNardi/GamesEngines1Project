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

A spawner generates random points and spawns tree, marten and monkey objects (Spawner.cs); if the point is above water, the object does not spawn. 

Once the trees have spawned, a fruit will spawn within it's range after 5 to 30 seconds (DropFruit.cs). 

When the marten and the monkeys have spawned, their needs are set after 5 seconds and a death age is set (Stats.cs). If the thirst or hunger needs reach 0, they will die. The needs can be reset by either eating the fruit dropped by the tree, drink from the water, or reproduce. The age increases per 5 seconds, and once the age reaches the death age, they die.

Monkeys and martens wander when they are searching for food, water and not in pursuit/being pursuited (Movement.cs).

Only monkeys eat fruit (Eatfruit.cs) and martens only eat monkeys (Hunt.cs). Monkeys will search for the closest fruit in their field of view. When they find a fruit, they will stop wandering, move towards the fruit and eat it. If a marten sees a monkey they will stop wandering and try to eat the closest monkey. 

The monkeys will flee if a marten is spotted in their field of view (Flee.cs); the speed of the monkey is increased while it sees the marten. If a monkey or marten fall into the water, they will drown (DespawnOnContact.cs).

Both monkeys and martens can reproduce if they spot another one of its kind (Cuddle.cs), its the opposite gender and if the other wants to cuddle; reproduction means instantiating a baby in between the mother and father positions.



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

### Monkey

### Fig Tree

### Fig
