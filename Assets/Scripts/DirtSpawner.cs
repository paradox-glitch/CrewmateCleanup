using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using sys = System;

public class DirtSpawner : MonoBehaviour
{
    public int areas = 5;

    public int DirtAmount = 4;

    public ToSpawnData[] data = new ToSpawnData[100];

    public GameObject[] m_DirtObjects;

    public List<ToSpawnData> list = new List<ToSpawnData>();

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < areas; i++)
        {
            var hash = 0;
            for (int c = 0; c < data[i].seed.Length; c++)
            {
                var hi = data[i].seed.ToCharArray()[c];
                hash = ((hash << 5) - hash) + hi;
                hash = hash & hash;
            }
            Random.InitState(hash);
            for (int b = 0; b < data[i].dirtToSpawn; b++)
            {
                float x = (float)(data[i].xMin.x + ((data[i].xMax.x - data[i].xMin.x) * (float)Random.value));
                float z = (float)(data[i].zMin.z + ((data[i].zMax.z - data[i].zMin.z) * (float)Random.value));

                Vector3 lmaovector = new Vector3(x, 2, z);

                Debug.DrawRay(lmaovector, Vector3.down * 6, Color.red, 10f);

                int l_ObjectToSpawn = Random.Range(0, m_DirtObjects.Length);

                Instantiate(m_DirtObjects[l_ObjectToSpawn], lmaovector, transform.rotation);
            }
        }
    }

}

[CustomEditor(typeof(DirtSpawner))]
public class DirtSpawnerEditor : Editor
{
    protected virtual void OnSceneGUI()
    {
        EditorGUILayout.LabelField("Custom editor:");
        var serializedObject = new SerializedObject(target);
        serializedObject.Update();
        serializedObject.ApplyModifiedProperties();

        DirtSpawner example = (DirtSpawner)target;

        float snap = 0.1f;

        for (int i = 0; i < example.areas; i++)
        {

            EditorGUI.BeginChangeCheck();
            Vector3 xMaxNew = Handles.Slider(example.data[i].xMax, example.transform.right, 0.1f, Handles.DotHandleCap, snap);
            Vector3 xMinNew = Handles.Slider(example.data[i].xMin, -example.transform.right, 0.1f, Handles.DotHandleCap, snap);
            Vector3 zMaxNew = Handles.Slider(example.data[i].zMax, example.transform.forward, 0.1f, Handles.DotHandleCap, snap);
            Vector3 zMinNew = Handles.Slider(example.data[i].zMin, -example.transform.forward, 0.1f, Handles.DotHandleCap, snap);

            xMaxNew = new Vector3(xMaxNew.x, 0, (zMinNew.z + ((zMaxNew.z - zMinNew.z) / 2f)));
            xMinNew = new Vector3(xMinNew.x, 0, (zMinNew.z + ((zMaxNew.z - zMinNew.z) / 2f)));
            zMaxNew = new Vector3((xMinNew.x + ((xMaxNew.x - xMinNew.x) / 2f)), 0, zMaxNew.z);
            zMinNew = new Vector3((xMinNew.x + ((xMaxNew.x - xMinNew.x) / 2f)), 0, zMinNew.z);

            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(example, "Change Look At Target Position");
                example.data[i].xMax = xMaxNew;
                example.data[i].xMin = xMinNew;
                example.data[i].zMax = zMaxNew;
                example.data[i].zMin = zMinNew;
            }

            Vector3 xMinZmin = new Vector3(xMinNew.x, 0, zMinNew.z);
            Vector3 xMinZmax = new Vector3(xMinNew.x, 0, zMaxNew.z);
            Vector3 xMaxZMin = new Vector3(xMaxNew.x, 0, zMinNew.z);
            Vector3 xMaxZMax = new Vector3(xMaxNew.x, 0, zMaxNew.z);

            Handles.DrawDottedLine(xMinZmin, xMinZmax, 6);
            Handles.DrawDottedLine(xMaxZMax, xMinZmax, 6);
            Handles.DrawDottedLine(xMaxZMax, xMaxZMin, 6);
            Handles.DrawDottedLine(xMinZmin, xMaxZMin, 6);
        }
        serializedObject.Update();
        serializedObject.ApplyModifiedProperties();
    }

    public override void OnInspectorGUI()
    {
        var serializedObject = new SerializedObject(target);
        serializedObject.Update();
        serializedObject.ApplyModifiedProperties();

        DirtSpawner mp = (DirtSpawner)target;

        int oldareas = mp.areas;
        mp.areas = EditorGUILayout.IntSlider("Number of Spawn Areas", mp.areas, 0, 100);

        var property = serializedObject.FindProperty("m_DirtObjects");
        serializedObject.Update();
        EditorGUILayout.PropertyField(property, true);
        serializedObject.ApplyModifiedProperties();

        EditorGUILayout.LabelField("");
        for (int i = 0; i < mp.areas; i++)
        {
            EditorGUILayout.LabelField("Area" + (i + 1));
            mp.data[i].seed = EditorGUILayout.TextField("Seed", mp.data[i].seed);
            mp.data[i].dirtToSpawn = EditorGUILayout.IntField("Dirt To Spawn", mp.data[i].dirtToSpawn);
            mp.data[i].xMin.x = EditorGUILayout.FloatField("X Min", mp.data[i].xMin.x);
            mp.data[i].xMax.x = EditorGUILayout.FloatField("X Max", mp.data[i].xMax.x);
            mp.data[i].zMin.z = EditorGUILayout.FloatField("Z Min", mp.data[i].zMin.z);
            mp.data[i].zMax.z = EditorGUILayout.FloatField("Z Max", mp.data[i].zMax.z);
            EditorGUILayout.LabelField("");
        }

        serializedObject.Update();
        serializedObject.ApplyModifiedProperties();
    }
}
