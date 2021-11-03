using BansheeGz.BGSpline.Curve;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    public int MIN_DISTANCE_BOUND = 1; // pour les GDs
    public int MAX_DISTANCE_BOUND = 100; // pour les GDs

    public bool randomDistance = false;

    public int minimumDistance = 1;
    public int maximumDistance = 100;
    public int totalDistance = 50;

    public BGCurve path;

    [SerializeField]
    public GameObject[] pathElements;

#if UNITY_EDITOR
    public const int VAR_SPACE = 6;
    public int tab = 0;
#endif

    void Awake()
    {
        Generate();
    }
    

    public void Generate()
    {
        if (!path)
            throw new System.Exception("You forgot to select the BGCurve element from your scene");

        for (int i = 0; i < pathElements.Length; i++)
        {
            if (!pathElements[i])
                throw new System.Exception("You forgot to select a path element from your scene");
        }

        path.Clear();

        float cumul = randomDistance ? Random.Range(minimumDistance, maximumDistance + 1) : totalDistance;
        Vector3 parent = Vector3.zero;
        while(cumul > 0)
        {
            GameObject pathElement = Instantiate(pathElements[Random.Range(0, pathElements.Length)], parent, Quaternion.identity);
            parent = pathElement.transform.Find("end_pos").position;
            Vector3 zero = Vector3.zero;

            if (pathElement.name.EndsWith("_L"))
            {
                //Work
                Vector3 controlPointB = new Vector3(0, 0, .5f);

                if (path.PointsCount > 0)
                    path.Points[path.PointsCount - 1].ControlSecondLocal = controlPointB;

                Vector3 controlPointA = new Vector3(0f, 0f, 0.5f);

                BGCurvePoint _bgp = new BGCurvePoint(path, pathElement.transform.position, BGCurvePoint.ControlTypeEnum.BezierIndependant, controlPointA, zero, false);
                path.AddPoint(_bgp);
            }
            else if (pathElement.name.EndsWith("_R"))
            {
                //Work
                Vector3 controlPointB = new Vector3(0f, 0f, .5f);

                if (path.PointsCount > 0)
                    path.Points[path.PointsCount - 1].ControlSecondLocal = controlPointB;

                Vector3 controlPointA = new Vector3(0f, 0f, 0.5f);
                BGCurvePoint _bgp = new BGCurvePoint(path, pathElement.transform.position, BGCurvePoint.ControlTypeEnum.BezierIndependant, controlPointA, zero, false);
                path.AddPoint(_bgp);
            }
            else
            {
                //Work
                Vector3 controlPointB = new Vector3(0f, 0f, .5f);

                if (path.PointsCount > 0)
                    path.Points[path.PointsCount - 1].ControlSecondLocal = controlPointB;

                Vector3 controlPointA = new Vector3(0f, 0f, -0.5f);
                BGCurvePoint _bgp = new BGCurvePoint(path, pathElement.transform.position, BGCurvePoint.ControlTypeEnum.BezierIndependant, controlPointA, zero, false);
                path.AddPoint(_bgp);
            }

            cumul -= pathElement.transform.localScale.z;
        }
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(LevelGenerator))]
public class MyScriptEditor : Editor
{
    override public void OnInspectorGUI()
    {
        LevelGenerator levelGenerator = target as LevelGenerator;

        SerializedObject serializedObject = new SerializedObject(target);
        SerializedProperty pathElementsProperty = serializedObject.FindProperty("pathElements");

        levelGenerator.tab = GUILayout.Toolbar(levelGenerator.tab, new string[] { "Generator", "Settings" });
        switch (levelGenerator.tab)
        {
            case 0:
                {
                    GUILayout.Space(LevelGenerator.VAR_SPACE);
                    GUILayout.Space(LevelGenerator.VAR_SPACE);
                    levelGenerator.randomDistance = GUILayout.Toggle(levelGenerator.randomDistance, "  Random Distance");

                    if (levelGenerator.randomDistance)
                    {
                        GUILayout.Space(LevelGenerator.VAR_SPACE);
                        levelGenerator.minimumDistance = EditorGUILayout.IntSlider("Minimum Distance", levelGenerator.minimumDistance, levelGenerator.MIN_DISTANCE_BOUND, levelGenerator.maximumDistance - 1);
                        levelGenerator.maximumDistance = EditorGUILayout.IntSlider("Maximum Distance", levelGenerator.maximumDistance, levelGenerator.minimumDistance + 1, levelGenerator.MAX_DISTANCE_BOUND);
                    }
                    else
                    {
                        GUILayout.Space(LevelGenerator.VAR_SPACE);
                        levelGenerator.totalDistance = EditorGUILayout.IntSlider("Total Distance", levelGenerator.totalDistance, levelGenerator.MIN_DISTANCE_BOUND, levelGenerator.MAX_DISTANCE_BOUND);
                    }

                    GUILayout.Space(LevelGenerator.VAR_SPACE);
                    levelGenerator.path = (BGCurve)EditorGUILayout.ObjectField("BGCurve Path", levelGenerator.path, typeof(BGCurve), true);
                    if(!levelGenerator.path)
                        EditorGUILayout.HelpBox("You forgot to select the BGCurve element from your scene", MessageType.Warning);

                    serializedObject.Update();
                    EditorGUILayout.PropertyField(pathElementsProperty, true);
                    serializedObject.ApplyModifiedProperties();

                    if (levelGenerator)
                    {
                        for (int i = 0; i < levelGenerator.pathElements.Length; i++)
                        {
                            if (!levelGenerator.pathElements[i])
                            {
                                EditorGUILayout.HelpBox("You forgot to select a path element from your scene", MessageType.Warning);
                                break;
                            }
                        }
                    }
                }
                break;
            case 1:
                {
                    GUILayout.Space(LevelGenerator.VAR_SPACE);
                    GUILayout.Space(LevelGenerator.VAR_SPACE);
                    EditorGUILayout.LabelField("Random Distance Bounds", EditorStyles.boldLabel);

                    GUILayout.Space(LevelGenerator.VAR_SPACE);
                    levelGenerator.MIN_DISTANCE_BOUND = EditorGUILayout.IntSlider("   Minimum Distance Bound", levelGenerator.MIN_DISTANCE_BOUND, 1, levelGenerator.MAX_DISTANCE_BOUND - 1);
                    levelGenerator.MAX_DISTANCE_BOUND = EditorGUILayout.IntSlider("   Maximum Distance Bound", levelGenerator.MAX_DISTANCE_BOUND, levelGenerator.MIN_DISTANCE_BOUND + 1, int.MaxValue);
                }
                break;
            default:
                { levelGenerator.tab = 0; }
                break;
        }
    }
}
#endif