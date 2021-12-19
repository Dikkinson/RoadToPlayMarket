using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using Varia;

[CustomEditor(typeof(TreeController))]
class TreeControllerEditor : Editor
{
    private bool growthHeader = true;
    private bool branchingHeader = true;
    private bool forkHeader = true;
    private bool foliageHeader = true;
    private bool presetHeader = true;

    private SerializedObject Serialized(UnityEngine.Object o)
    {
        if (o == null) return null;
        return new SerializedObject(o);
    }

    private SerializedProperty GetRandomChanceProperty(SerializedObject o)
    {
        var conditions = o?.FindProperty(nameof(VariaBehaviour.conditionList))?.FindPropertyRelative(nameof(VariaConditionList.conditions));
        if (conditions == null) return null;
        for (var i = 0; i < conditions.arraySize; i++)
        {
            var element = conditions.GetArrayElementAtIndex(i);
            var conditionType = (VariaConditionType)element.FindPropertyRelative(nameof(VariaCondition.conditionType)).enumValueIndex;
            if (conditionType == VariaConditionType.Random)
            {
                return element.FindPropertyRelative(nameof(VariaCondition.randomChance));
            }
        }
        return null;
    }

    private SerializedProperty GetDepthFilterProperty(SerializedObject o, VariaComparison comparison = VariaComparison.LessThan)
    {
        var conditions = o?.FindProperty(nameof(VariaBehaviour.conditionList))?.FindPropertyRelative(nameof(VariaConditionList.conditions));
        if (conditions == null) return null;
        for (var i = 0; i < conditions.arraySize; i++)
        {
            var element = conditions.GetArrayElementAtIndex(i);
            var conditionType = (VariaConditionType)element.FindPropertyRelative(nameof(VariaCondition.conditionType)).enumValueIndex;
            if (conditionType == VariaConditionType.DepthFilter)
            {
                var c = (VariaComparison)element.FindPropertyRelative(nameof(VariaCondition.comparison)).enumValueIndex;

                if (c == comparison)
                {
                    return element.FindPropertyRelative(nameof(VariaCondition.depth));
                }
            }
        }
        return null;
    }

    private void MinMaxIntSlider(string label, SerializedProperty minProp, SerializedProperty maxProp, int min, int max, SerializedProperty minProp2, SerializedProperty maxProp2)
    {
        float v1 = minProp.intValue;
        float v2 = maxProp.intValue;
        EditorGUI.BeginChangeCheck();
        GUILayout.BeginHorizontal();
        EditorGUILayout.PrefixLabel(label);
        EditorGUILayout.MinMaxSlider(ref v1, ref v2, min, max);
        GUILayout.EndHorizontal();
        if (EditorGUI.EndChangeCheck())
        {
            void Set(SerializedProperty p, float v)
            {
                if (p == null) return;
                p.intValue = (int)v;
                p.serializedObject.ApplyModifiedProperties();
            }
            Set(minProp, v1);
            Set(minProp2, v1);
            Set(maxProp, v2);
            Set(maxProp2, v2);
        }
        GUILayout.BeginHorizontal();
        EditorGUILayout.PrefixLabel(" ");
        EditorGUILayout.LabelField($"{minProp.intValue} - {maxProp.intValue}");
        GUILayout.EndHorizontal();
    }

    private void PropertyField(SerializedProperty p, string label, SerializedProperty p2 = null, float? min = null, float? max = null)
    {
        EditorGUI.BeginChangeCheck();
        var guiContent = new GUIContent(label);
        if (min != null && max != null)
        {
            if(p.propertyType == SerializedPropertyType.Integer)
            {
                EditorGUILayout.IntSlider(p, (int)min.Value, (int)max.Value, guiContent);
            }
            else
            {
                EditorGUILayout.Slider(p, (int)min.Value, (int)max.Value, guiContent);
            }
        }
        else
        {
            EditorGUILayout.PropertyField(p, guiContent);
        }
        if (EditorGUI.EndChangeCheck())
        {
            if (p.propertyType == SerializedPropertyType.Integer)
            {
                if (min.HasValue && p.intValue < min) p.intValue = (int)min.Value;
                if (max.HasValue && p.intValue > max) p.intValue = (int)max.Value;
                p.serializedObject.ApplyModifiedProperties();
                if (p2 != null)
                {
                    p2.intValue = p.intValue;
                    p2.serializedObject.ApplyModifiedProperties();
                }
            }
            else if (p.propertyType == SerializedPropertyType.Float)
            {
                if (min.HasValue && p.floatValue < min) p.floatValue = min.Value;
                if (max.HasValue && p.floatValue > max) p.floatValue = max.Value;
                p.serializedObject.ApplyModifiedProperties();
                if (p2 != null)
                {
                    p2.floatValue = p.floatValue;
                    p2.serializedObject.ApplyModifiedProperties();
                }
            }
            else
            {
                p.serializedObject.ApplyModifiedProperties();
            }
        }
    }

    private void WithCheck(SerializedObject o, Action action)
    {
        EditorGUI.BeginChangeCheck();
        action();
        if (EditorGUI.EndChangeCheck())
        {
            o.ApplyModifiedProperties();
        }
    }

    public override void OnInspectorGUI()
    {
        //base.OnInspectorGUI();

        GUILayout.Label("This script is just a convienced for setting properties on the Varia components directly.\n"
            +"It demonstrates how the behaviour of the tree can be controlled.");

        var treeController = target as TreeController;
        var previewer = new SerializedObject(treeController.previewer.GetComponent<VariaPreviewer>());

        var branch1 = Serialized(treeController.branchSegment.transform.Find("Branch1").GetComponent<VariaInstantiate>());
        var branch2 = Serialized(treeController.branchSegment.transform.Find("Branch2").GetComponent<VariaInstantiate>());
        var growthScale = Serialized(treeController.branchSegment.transform.Find("Next").GetComponent<VariaRandomScale>());
        var growthInstantiate = Serialized(treeController.branchSegment.transform.Find("Next").GetComponent<VariaInstantiate>());
        var growthRotate = Serialized(treeController.branchSegment.transform.Find("Next").GetComponent<VariaRandomRotation>());
        var branchSegmentInstantiate = Serialized(treeController.branchSegment.GetComponent<VariaInstantiate>());
        var foliageTransform = Serialized(treeController.branchSegment.transform.Find("foliage").transform);
        var foliageScale = Serialized(treeController.branchSegment.transform.Find("foliage").GetComponent<VariaRandomScale>());
        var foliageKeep = Serialized(treeController.branchSegment.transform.Find("foliage").GetComponent<VariaKeep>());
        var foliageInstantiate = Serialized(treeController.branchSegment.transform.Find("foliage").GetComponent<VariaInstantiate>());

        var branch1Chance = GetRandomChanceProperty(branch1);
        var branch2Chance = GetRandomChanceProperty(branch2);

        var branch1MinDepth = GetDepthFilterProperty(branch1, VariaComparison.GreaterThanOrEquals);
        var branch2MinDepth = GetDepthFilterProperty(branch2, VariaComparison.GreaterThanOrEquals);
        var branch1MaxDepth = GetDepthFilterProperty(branch1, VariaComparison.LessThanOrEquals);
        var branch2MaxDepth = GetDepthFilterProperty(branch2, VariaComparison.LessThanOrEquals);

        var growthMaxDepth = GetDepthFilterProperty(growthInstantiate);
        var growthMaxAngle = growthRotate.FindProperty(nameof(VariaRandomRotation.dispersionMax));

        var forkChance = GetRandomChanceProperty(branchSegmentInstantiate);
        var forkMinDepth = GetDepthFilterProperty(branchSegmentInstantiate, VariaComparison.GreaterThanOrEquals);
        var forkMaxDepth = GetDepthFilterProperty(branchSegmentInstantiate, VariaComparison.LessThanOrEquals);

        var foliageChance = GetRandomChanceProperty(foliageKeep);
        var foliageMinDepth = GetDepthFilterProperty(foliageKeep, VariaComparison.GreaterThanOrEquals);
        var foliageYScale = foliageTransform.FindProperty("m_LocalScale.y");
        var foliagePalmTop = foliageInstantiate.FindProperty("m_Enabled");

        PropertyField(previewer.FindProperty(nameof(VariaPreviewer.seed)), "Random Seed");

        if (growthHeader = EditorGUILayout.BeginFoldoutHeaderGroup(growthHeader, "Growth"))
        {
            PropertyField(growthMaxDepth, "Growth Max Depth");
            PropertyField(growthMaxAngle, "Growth Max Angle");
            WithCheck(growthScale, () => Varia.EditorUtility.NiceRange(growthScale, "minX", "maxX", 0, 1, "Growth Scaling", "Min", "Max"));
        }
        EditorGUILayout.EndFoldoutHeaderGroup();

        if (branchingHeader = EditorGUILayout.BeginFoldoutHeaderGroup(branchingHeader, "Branching"))
        {
            PropertyField(branch1Chance, "Branching Chance", branch2Chance);
            MinMaxIntSlider("Branching Depth", branch1MinDepth, branch1MaxDepth, 0, 6, branch2MinDepth, branch2MaxDepth);
        }
        EditorGUILayout.EndFoldoutHeaderGroup();


        if (forkHeader = EditorGUILayout.BeginFoldoutHeaderGroup(forkHeader, "Forking"))
        {
            PropertyField(forkChance, "Forking Chance");
            MinMaxIntSlider("Forking Depth", forkMinDepth, forkMaxDepth, 0, 10, null, null);
        }
        EditorGUILayout.EndFoldoutHeaderGroup();

        if (foliageHeader = EditorGUILayout.BeginFoldoutHeaderGroup(foliageHeader, "Foliage"))
        {
            PropertyField(foliageChance, "Foliage Chance");
            PropertyField(foliageMinDepth, "Foliage Min Depth");
            WithCheck(foliageScale, () => Varia.EditorUtility.NiceRange(foliageScale, "minX", "maxX", 0, 1, "Foliage Scaling", "Min", "Max"));
            PropertyField(foliageYScale, "Foliage Y Scale");
            PropertyField(foliagePalmTop, "Use Palm Leaves");
        }
        EditorGUILayout.EndFoldoutHeaderGroup();

        if (presetHeader = EditorGUILayout.BeginFoldoutHeaderGroup(presetHeader, "Presets"))
        {
            bool save = false;
            if (GUILayout.Button("Default"))
            {
                growthMaxDepth.intValue = 10;
                growthMaxAngle.floatValue = 20;
                growthScale.FindProperty("minX").floatValue = 0.8f;
                growthScale.FindProperty("maxX").floatValue = 0.9f;
                branch1Chance.floatValue = branch2Chance.floatValue = 0.4f;
                branch1MinDepth.intValue = branch2MinDepth.intValue = 1;
                branch1MaxDepth.intValue = branch2MaxDepth.intValue = 10;
                forkChance.floatValue = 0f;
                forkMinDepth.intValue = 2;
                forkMaxDepth.intValue = 10;
                foliageChance.floatValue = 0.8f;
                foliageMinDepth.intValue = 4;
                foliageScale.FindProperty("minX").floatValue = 3f;
                foliageScale.FindProperty("maxX").floatValue = 8f;
                foliageYScale.floatValue = 0.5f;
                foliagePalmTop.boolValue = false;
                save = true;
            }
            if (GUILayout.Button("Oak"))
            {
                growthMaxDepth.intValue = 10;
                growthMaxAngle.floatValue = 20;
                growthScale.FindProperty("minX").floatValue = 0.6f;
                growthScale.FindProperty("maxX").floatValue = 0.9f;
                branch1Chance.floatValue = branch2Chance.floatValue = 1f;
                branch1MinDepth.intValue = branch2MinDepth.intValue = 2;
                branch1MaxDepth.intValue = branch2MaxDepth.intValue = 4;
                forkChance.floatValue = 0f;
                forkMinDepth.intValue = 2;
                forkMaxDepth.intValue = 10;
                foliageChance.floatValue = 1f;
                foliageMinDepth.intValue = 8;
                foliageScale.FindProperty("minX").floatValue = 15f;
                foliageScale.FindProperty("maxX").floatValue = 15f;
                foliageYScale.floatValue = 0.5f;
                foliagePalmTop.boolValue = false;
                save = true;
            }
            if (GUILayout.Button("Fir"))
            {
                growthMaxDepth.intValue = 10;
                growthMaxAngle.floatValue = 0;
                growthScale.FindProperty("minX").floatValue = 0.8f;
                growthScale.FindProperty("maxX").floatValue = 0.8f;
                branch1Chance.floatValue = branch2Chance.floatValue = 0.0f;
                branch1MinDepth.intValue = branch2MinDepth.intValue = 1;
                branch1MaxDepth.intValue = branch2MaxDepth.intValue = 10;
                forkChance.floatValue = 0f;
                forkMinDepth.intValue = 2;
                forkMaxDepth.intValue = 10;
                foliageChance.floatValue = 1f;
                foliageMinDepth.intValue = 1;
                foliageScale.FindProperty("minX").floatValue = 5f;
                foliageScale.FindProperty("maxX").floatValue = 5f;
                foliageYScale.floatValue = 0.35f;
                foliagePalmTop.boolValue = false;
                save = true;
            }
            if (GUILayout.Button("Gnarled"))
            {
                growthMaxDepth.intValue = 10;
                growthMaxAngle.floatValue = 20;
                growthScale.FindProperty("minX").floatValue = 0.6f;
                growthScale.FindProperty("maxX").floatValue = 0.9f;
                branch1Chance.floatValue = branch2Chance.floatValue = 0.2f;
                branch1MinDepth.intValue = branch2MinDepth.intValue = 3;
                branch1MaxDepth.intValue = branch2MaxDepth.intValue = 10;
                forkChance.floatValue = 0.6f;
                forkMinDepth.intValue = 2;
                forkMaxDepth.intValue = 4;
                foliageChance.floatValue = 0.0f;
                foliageMinDepth.intValue = 4;
                foliageScale.FindProperty("minX").floatValue = 3f;
                foliageScale.FindProperty("maxX").floatValue = 15f;
                foliageYScale.floatValue = 0.5f;
                foliagePalmTop.boolValue = false;
                save = true;
            }
            if (GUILayout.Button("Palm"))
            {
                growthMaxDepth.intValue = 5;
                growthMaxAngle.floatValue = 10;
                growthScale.FindProperty("minX").floatValue = 1f;
                growthScale.FindProperty("maxX").floatValue = 1f;
                branch1Chance.floatValue = branch2Chance.floatValue = 0.0f;
                branch1MinDepth.intValue = branch2MinDepth.intValue = 3;
                branch1MaxDepth.intValue = branch2MaxDepth.intValue = 10;
                forkChance.floatValue = 0.0f;
                forkMinDepth.intValue = 2;
                forkMaxDepth.intValue = 4;
                foliageChance.floatValue = 1.0f;
                foliageMinDepth.intValue = 5;
                foliageScale.FindProperty("minX").floatValue = 3f;
                foliageScale.FindProperty("maxX").floatValue = 3f;
                foliageYScale.floatValue = 0.5f;
                foliagePalmTop.boolValue = true;
                save = true;
            }
            if (save)
            {
                branch1.ApplyModifiedProperties();
                branch2.ApplyModifiedProperties();
                growthInstantiate.ApplyModifiedProperties();
                growthRotate.ApplyModifiedProperties();
                growthScale.ApplyModifiedProperties();
                branchSegmentInstantiate.ApplyModifiedProperties();
                foliageScale.ApplyModifiedProperties();
                foliageKeep.ApplyModifiedProperties();
                foliageTransform.ApplyModifiedProperties();
                foliageInstantiate.ApplyModifiedProperties();
            }
        }
        EditorGUILayout.EndFoldoutHeaderGroup();
    }
}
