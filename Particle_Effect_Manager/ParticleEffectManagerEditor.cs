using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ParticleEffectManager))]
public class ParticleEffectManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        ParticleEffectManager thisManager = (ParticleEffectManager)target;
        if (GUILayout.Button("Generate Test Effect"))
        {
            thisManager.TestEffect();
        }
    }
}
