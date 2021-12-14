using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(EatFruit))]
public class FOV : Editor
{
    void OnSceneGUI(){
        EatFruit fow = (EatFruit) target;
        Handles.color = Color.white;
        Handles.DrawWireArc(fow.transform.position, Vector3.up, Vector3.forward, 360, fow.viewRadius);

        Vector3 viewAngleA = fow.DirFromAngle (-fow.viewAngle / 2, false);
        Vector3 viewAngleB = fow.DirFromAngle (fow.viewAngle / 2, false);

        Handles.DrawLine(fow.transform.position, fow.transform.position + viewAngleA * fow.viewRadius);
        Handles.DrawLine(fow.transform.position, fow.transform.position + viewAngleB * fow.viewRadius);

        Handles.color = Color.red;
        if(fow.closestFruit != new Vector3(0, 0, 0)){
            Handles.DrawLine(fow.transform.position, fow.closestFruit);
        }
    }
}
