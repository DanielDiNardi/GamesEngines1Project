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

    EatFruit eatFruit;

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

            eatFruit = gameObject.GetComponent<EatFruit>();
            eatFruit.full = false;
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

        male = Random.Range(0, 2) == 0;

        currentThirst = thirst;
        currentHunger = hunger;
        currentReproductionNeed = reproductionNeed * 0.1f;
    }

    void Update(){
        if(reduceStats){
            reduceStats = false;   
            CallCoroutine();
        }
    }
}
