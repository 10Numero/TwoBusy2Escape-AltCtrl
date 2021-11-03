using BansheeGz.BGSpline.Curve;
using BansheeGz.BGSpline.Components;
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

    public float straightProbability = 0.9f;
    public float splitStraightProbability = 0.8f;

    public BGCurve path;

    public float curvature = 0.5f;

    public GameObject straightPath;
    public GameObject leftPath;
    public GameObject rightPath;

    [SerializeField]
    public GameObject[] obstacles;

#if UNITY_EDITOR
    public const int VAR_SPACE = 6;
    public int tab = 0;
#endif

    

    public static LevelGenerator _instance; 
    void Awake()
    {
        _instance = this;

        Generate();
    }

    private void Update()
    {
        SwitchLane();
    }

    public void Generate()
    {
        if (!path)
            throw new System.Exception("You forgot to select the BGCurve element");

        if (!straightPath || !leftPath || !rightPath)
            throw new System.Exception("You forgot to select some path elements");

        if(obstacles.Length == 0)
            throw new System.Exception("Obstacles array cannot be empty");

        foreach (GameObject obstacle in obstacles)
        {
            if(!obstacle)
                throw new System.Exception("You forgot to select some obstacles elements");
        }    

        path.Clear();

        float cumul = randomDistance ? Random.Range(minimumDistance, maximumDistance + 1) : totalDistance;
        Vector3 parent = Vector3.zero;
        GameObject pathElement = null, leftSplitElement = null, rightSplitElement = null;

        bool isSplit = false, isAtLeastOneStraightOnSplit = false, isAtLeastOneStraigntAfterSplit = false, isObstacleLeftSide = false, isObstacleSpawned = false;

        int ptIndex = 0;

        while (cumul > 0)
        {
            if (!isSplit)
            {
                if (isAtLeastOneStraigntAfterSplit)
                {
                    bool isStraight = Random.value <= straightProbability;  // proba normale

                    if (isStraight)
                        InstantiateSimpleStraightPath(straightPath, ref pathElement, ref parent, ref cumul, ref ptIndex);
                    else
                    {
                        leftSplitElement = Instantiate(leftPath, parent, Quaternion.identity);
                        rightSplitElement = Instantiate(rightPath, parent, Quaternion.identity);

                        isObstacleLeftSide = Random.value < 0.5;    // equiprobable
                        isObstacleSpawned = false;

                        AddBGCPoint(isObstacleLeftSide ? leftSplitElement.transform.position : rightSplitElement.transform.position, false, ref ptIndex);

                        cumul -= leftSplitElement.transform.localScale.z; // assuming equal lengths between left and right

                        isSplit = true;
                    }
                }
                else
                {
                    InstantiateSimpleStraightPath(straightPath, ref pathElement, ref parent, ref cumul, ref ptIndex);
                    isAtLeastOneStraigntAfterSplit = true;
                }
            }
            else
            {
                if (isAtLeastOneStraightOnSplit)
                {
                    bool isStraight = Random.value <= splitStraightProbability; // proba si split

                    if (isStraight)
                    {
                        InstantiateSplitPath(straightPath, straightPath, isObstacleLeftSide, ref leftSplitElement, ref rightSplitElement, ref cumul, ref ptIndex);

                        if (!isObstacleSpawned)
                        {
                            bool spawnObstacleHere = Random.value < 0.5; // equiprobable
                            if (spawnObstacleHere)
                            {
                                Instantiate(obstacles[Random.Range(0, obstacles.Length)], isObstacleLeftSide ? leftSplitElement.transform.position : rightSplitElement.transform.position, Quaternion.identity);
                                isObstacleSpawned = true;
                            }
                        }
                    }
                    else
                    {
                        if (!isObstacleSpawned)
                            Instantiate(obstacles[Random.Range(0, obstacles.Length)], isObstacleLeftSide ? leftSplitElement.transform.position : rightSplitElement.transform.position, Quaternion.identity);

                        InstantiateSplitPath(rightPath, leftPath, isObstacleLeftSide, ref leftSplitElement, ref rightSplitElement, ref cumul, ref ptIndex); // right pour left, et left pour right -> rencontre du split

                        parent = isObstacleLeftSide ? leftSplitElement.transform.Find("end_pos").position : rightSplitElement.transform.Find("end_pos").position;
                        pathElement = isObstacleLeftSide ? leftSplitElement : rightSplitElement;

                        isAtLeastOneStraigntAfterSplit = false;
                        isAtLeastOneStraightOnSplit = false;
                        isObstacleSpawned = false;
                        isSplit = false;
                    }
                }
                else
                {
                    InstantiateSplitPath(straightPath, straightPath, isObstacleLeftSide, ref leftSplitElement, ref rightSplitElement, ref cumul, ref ptIndex);
                    isAtLeastOneStraightOnSplit = true;
                }
            }
        }
    }

    public void SwitchLane()
    {
        float dist = path.GetComponent<BGCcCursor>().Distance;
        int closestPoint = path.GetComponent<BGCcMath>().CalcSectionIndexByDistance(dist);
        Debug.Log(closestPoint);
    }

    private void AddBGCPoint(Vector3 position, bool isStraight, ref int ptIndex)
    {
        int c = isStraight ? -1 : 1;
        if (path.PointsCount > 0)
            path.Points[path.PointsCount - 1].ControlSecondLocal = new Vector3(0f, 0f, -curvature) * c;

        path.AddPoint(new BGCurvePoint(path, position, BGCurvePoint.ControlTypeEnum.BezierIndependant, new Vector3(0f, 0f, curvature) * c, Vector3.zero, false));

        ptIndex++;
    }
    
    private void InstantiateSimpleStraightPath(GameObject prefab, ref GameObject pathElement, ref Vector3 parent, ref float cumul, ref int ptIndex)
    {
        pathElement = Instantiate(prefab, parent, Quaternion.identity);
        parent = pathElement.transform.Find("end_pos").position;
        AddBGCPoint(pathElement.transform.position, true, ref ptIndex);
        cumul -= pathElement.transform.localScale.z;
    }

    private void InstantiateSplitPath(GameObject leftPrefab, GameObject rightPrefab, bool isObstacleLeftSide, ref GameObject leftSplitElement, ref GameObject rightSplitElement, ref float cumul, ref int ptIndex)
    {
        Vector3 leftParent = leftSplitElement.transform.Find("end_pos").position;
        leftSplitElement = Instantiate(leftPrefab, leftParent, Quaternion.identity);
        Vector3 rightParent = rightSplitElement.transform.Find("end_pos").position;
        rightSplitElement = Instantiate(rightPrefab, rightParent, Quaternion.identity);

        AddBGCPoint(isObstacleLeftSide ? leftSplitElement.transform.position : rightSplitElement.transform.position, leftPrefab == rightPrefab, ref ptIndex);

        cumul -= leftSplitElement.transform.localScale.z; // assuming equal lengths between left and right
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
        SerializedProperty obstaclesPproperty = serializedObject.FindProperty("obstacles");

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
                    levelGenerator.straightProbability = EditorGUILayout.Slider("Straight Path Probability", levelGenerator.straightProbability, 0f, 1f);

                    GUILayout.Space(LevelGenerator.VAR_SPACE);
                    levelGenerator.splitStraightProbability = EditorGUILayout.Slider("Split Path Straight Probability", levelGenerator.splitStraightProbability, 0f, 1f);

                    GUILayout.Space(LevelGenerator.VAR_SPACE);
                    levelGenerator.path = (BGCurve)EditorGUILayout.ObjectField("BGCurve Path", levelGenerator.path, typeof(BGCurve), true);
                    if(!levelGenerator.path)
                        EditorGUILayout.HelpBox("You forgot to select the BGCurve element", MessageType.Warning);

                    GUILayout.Space(LevelGenerator.VAR_SPACE);
                    levelGenerator.curvature = EditorGUILayout.Slider("Curvature", levelGenerator.curvature, 0f, 1f);

                    GUILayout.Space(LevelGenerator.VAR_SPACE);
                    levelGenerator.straightPath = (GameObject)EditorGUILayout.ObjectField("Straight Path", levelGenerator.straightPath, typeof(GameObject), true);
                    levelGenerator.leftPath = (GameObject)EditorGUILayout.ObjectField("Left Path", levelGenerator.leftPath, typeof(GameObject), true);
                    levelGenerator.rightPath = (GameObject)EditorGUILayout.ObjectField("Right Path", levelGenerator.rightPath, typeof(GameObject), true);

                    if (!levelGenerator.straightPath || !levelGenerator.leftPath || !levelGenerator.rightPath)
                        EditorGUILayout.HelpBox("You forgot to select some path elements ", MessageType.Warning);

                    serializedObject.Update();
                    EditorGUILayout.PropertyField(obstaclesPproperty, true);
                    serializedObject.ApplyModifiedProperties();

                    if (!levelGenerator || levelGenerator.obstacles.Length == 0)
                        EditorGUILayout.HelpBox("You need to have at least one obstacle", MessageType.Warning);
                    else if(levelGenerator)
                    {
                        foreach (GameObject obstacle in levelGenerator.obstacles)
                        {
                            if (!obstacle)
                            {
                                EditorGUILayout.HelpBox("You forgot to select some obstacles elements", MessageType.Warning);
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