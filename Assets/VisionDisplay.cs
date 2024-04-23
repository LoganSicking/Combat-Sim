using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(VisionScript))]
public class VisionDisplay : Editor
{
    private void OnSceneGUI()
    {
        
        VisionScript vs = (VisionScript)target;
        if (vs.coneDensity <= 0)
        {
            vs.coneDensity = 1;
        }
        Handles.color = vs.coneColor;
        for(float i = 0;i < 180;i += vs.coneDensity)
        {
            Handles.DrawSolidArc(vs.transform.position, Quaternion.AngleAxis(i,vs.transform.forward) * Vector3.up, vs.transform.forward, vs.angle, vs.distance);
            Handles.DrawSolidArc(vs.transform.position, Vector3.down, vs.transform.forward, vs.angle, vs.distance);
        }
        
    }
}
