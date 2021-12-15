using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Cuddle))]
public class MatingFOV : Editor
{
    void OnSceneGUI(){
        Cuddle fow = (Cuddle) target;

        Handles.color = Color.green;
        if(fow.closestMate != new Vector3(0, 0, 0)){
            Handles.DrawLine(fow.transform.position, fow.closestMate);
        }
    }
}