using System.Collections.Generic;
using System.Collections;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditorInternal;
#endif

public class BloodSpawner : MonoBehaviour
{
    public int areas = 5;

    public int DirtAmount = 4;

    public ToSpawnData[] data = new ToSpawnData[100];

    GameObject m_SplatDecalPool;

    private LayerMask m_Enviroment = 1 << 8;

    bool test = false;

    float tt = 0;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log(m_Enviroment.value);
        m_SplatDecalPool = GameObject.FindGameObjectWithTag("SpaltDecalPool");

        StartCoroutine(WaitToSpawn());
    }

    IEnumerator WaitToSpawn()
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

                Vector3 lmaovector = new Vector3(x, 20, z);

                RaycastHit l_Hit;

                Ray ray = new Ray(lmaovector, -transform.up);
                if (Physics.Raycast(ray, out l_Hit, 200f, m_Enviroment))
                {
                    Debug.Log(l_Hit.collider.gameObject.layer);
                    m_SplatDecalPool.GetComponent<ParticleDecalPool>().SetParticalDataDirect(l_Hit.point + (Random.Range(0.01f, 0.05f) * Vector3.up), -transform.up);
                    test = false;
                    tt = -10000;
                }


            }

        }
        yield return new WaitForEndOfFrame();
        yield return new WaitForFixedUpdate();

    }
}


#if UNITY_EDITOR
    [CustomEditor(typeof(BloodSpawner))]
    public class BloodSpawnerEditor : Editor
    {
        protected virtual void OnSceneGUI()
        {
            EditorGUILayout.LabelField("Custom editor:");
            var serializedObject = new SerializedObject(target);
            serializedObject.Update();
            serializedObject.ApplyModifiedProperties();

            BloodSpawner example = (BloodSpawner)target;

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

            BloodSpawner mp = (BloodSpawner)target;

            int oldareas = mp.areas;
            mp.areas = EditorGUILayout.IntSlider("Number of Spawn Areas", mp.areas, 0, 100);

            //mp.m_Enviroment = EditorGUILayout.MaskField(mp.m_Enviroment, InternalEditorUtility.layers);
            //Debug.Log(mp.m_Enviroment.value);

            EditorGUILayout.LabelField("");
            for (int i = 0; i < mp.areas; i++)
            {
                EditorGUILayout.LabelField("Area" + (i + 1));
                mp.data[i].seed = EditorGUILayout.TextField("Seed", mp.data[i].seed);
                int tempdirt = EditorGUILayout.IntField("Blood To Spawn", mp.data[i].dirtToSpawn);
                mp.data[i].xMin.x = EditorGUILayout.FloatField("X Min", mp.data[i].xMin.x);
                mp.data[i].xMax.x = EditorGUILayout.FloatField("X Max", mp.data[i].xMax.x);
                mp.data[i].zMin.z = EditorGUILayout.FloatField("Z Min", mp.data[i].zMin.z);
                mp.data[i].zMax.z = EditorGUILayout.FloatField("Z Max", mp.data[i].zMax.z);
                EditorGUILayout.LabelField("");

            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(mp, "Change Look At Target Position");
                mp.data[i].dirtToSpawn = tempdirt;
            }

        }

            serializedObject.Update();
            serializedObject.ApplyModifiedProperties();
        }
    }
#endif