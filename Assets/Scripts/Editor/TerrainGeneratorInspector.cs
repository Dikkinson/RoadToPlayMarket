using System.Collections;
using System.Collections.Generic;
using System.Reflection.Emit;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(TerrainGenerator))]
public class TerrainGeneratorInspector : Editor
{
   public override void OnInspectorGUI()
   {
      TerrainGenerator terrain = (TerrainGenerator) target;
      if (GUILayout.Button("Generate"))
      {
         terrain.Generate();
      }
      GUILayout.BeginHorizontal();
      if (GUILayout.Button("Delete All"))
      {
         terrain.DestroyAll();
      }
      if (GUILayout.Button("Delete Land"))
      {
         terrain.DestroyLand();
      }
      if (GUILayout.Button("Delete Props"))
      {
         terrain.DestroyProps();
      }
      GUILayout.EndHorizontal();
      base.OnInspectorGUI();
   }
}
