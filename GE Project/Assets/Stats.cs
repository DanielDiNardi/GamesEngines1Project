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

    bool reduceStats = true;

    System.Collections.IEnumerator NeedReduction()
    {
        yield return new WaitForSeconds(1);
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
        }
        else if(reproductionNeedPercent < thirstPercent && reproductionNeedPercent < hungerPercent){
            thirsty = false;
            hungry = false;
            cuddly = true;
        }
        else{
            thirsty = false;
            hungry = false;
            cuddly = false;
        }

        reduceStats = true;
    }

    void CallCoroutine()
    {
        StartCoroutine(NeedReduction());
    }

    void Start(){
        thirst = Random.Range(10, 30);
        hunger = Random.Range(10, 50);
        reproductionNeed = Random.Range(10, 100);

        currentThirst = thirst;
        currentHunger = hunger;
        currentReproductionNeed = reproductionNeed;
    }

    void Update(){
        if(reduceStats){
            reduceStats = false;   
            CallCoroutine();
        }
    }
}
