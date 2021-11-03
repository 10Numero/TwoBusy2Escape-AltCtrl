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

    private class Split
    {
        public int start, end;
        public Split(int start) { this.start = start; end = -1; } // end va etre instancié apres
    }
    List<Split> pathSplits = new List<Split>();

#if UNITY_EDITOR
    public const int VAR_SPACE = 6;
    public int tab = 0;
#endif

    public static LevelGenerator _instance; 
    void Awake()
    {
        if (_instance == null)
        {
            _instance = this;

            Generate();
        }
        else Destroy(this.gameObject);
    }

    #region Generate
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
        pathSplits.Clear();

        #region SpawnParents
        GameObject temp = new GameObject();

        Transform levelParent = Instantiate(temp, Vector3.zero, Quaternion.identity).transform;
        levelParent.name = "_LEVEL_";

        Transform pathParent = Instantiate(temp, Vector3.zero, Quaternion.identity).transform;
        pathParent.parent = levelParent;
        pathParent.name = "Path";

        Transform obstaclesParent = Instantiate(temp, Vector3.zero, Quaternion.identity).transform;
        obstaclesParent.parent = levelParent;
        obstaclesParent.name = "Obstacles";

        Destroy(temp);
        #endregion

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
                    {
                        InstantiateSimpleStraightPath(straightPath, ref pathElement, ref parent, ref cumul, ref ptIndex);
                        pathElement.transform.parent = pathParent;
                    }
                    else
                    {
                        leftSplitElement = Instantiate(leftPath, parent, Quaternion.identity);
                        leftSplitElement.transform.parent = pathParent;
                        rightSplitElement = Instantiate(rightPath, parent, Quaternion.identity);
                        rightSplitElement.transform.parent = pathParent;

                        isObstacleLeftSide = Random.value < 0.5;    // equiprobable
                        isObstacleSpawned = false;

                        pathSplits.Add(new Split(ptIndex));

                        AddBGCPoint(isObstacleLeftSide ? leftSplitElement.transform.position : rightSplitElement.transform.position, false, ref ptIndex);

                        cumul -= leftSplitElement.transform.localScale.z; // assuming equal lengths between left and right

                        isSplit = true;
                    }
                }
                else
                {
                    InstantiateSimpleStraightPath(straightPath, ref pathElement, ref parent, ref cumul, ref ptIndex);
                    pathElement.transform.parent = pathParent;
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
                        leftSplitElement.transform.parent = pathParent;
                        rightSplitElement.transform.parent = pathParent;

                        if (!isObstacleSpawned)
                        {
                            bool spawnObstacleHere = Random.value < 0.5; // equiprobable
                            if (spawnObstacleHere)
                            {
                                Transform obs = Instantiate(obstacles[Random.Range(0, obstacles.Length)], isObstacleLeftSide ? leftSplitElement.transform.position : rightSplitElement.transform.position, Quaternion.identity).transform;
                                obs.parent = obstaclesParent;
                                
                                isObstacleSpawned = true;
                            }
                        }
                    }
                    else
                    {
                        if (!isObstacleSpawned)
                        {
                            Transform obs = Instantiate(obstacles[Random.Range(0, obstacles.Length)], isObstacleLeftSide ? leftSplitElement.transform.position : rightSplitElement.transform.position, Quaternion.identity).transform;
                            obs.parent = obstaclesParent;
                        }

                        pathSplits[pathSplits.Count - 1].end = ptIndex;

                        InstantiateSplitPath(rightPath, leftPath, isObstacleLeftSide, ref leftSplitElement, ref rightSplitElement, ref cumul, ref ptIndex); // right pour left, et left pour right -> rencontre du split
                        leftSplitElement.transform.parent = pathParent;
                        rightSplitElement.transform.parent = pathParent;

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
                    leftSplitElement.transform.parent = pathParent;
                    rightSplitElement.transform.parent = pathParent;
                    isAtLeastOneStraightOnSplit = true;
                }
            }
        }
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
    #endregion

    public void SwitchLane(bool left)
    {
        int currentIndex = path.GetComponent<BGCcMath>().CalcSectionIndexByDistance(path.GetComponent<BGCcCursor>().Distance);
        Split nextSplit = null;
        for (int i = 0; i < pathSplits.Count; i++)
        {
            if (currentIndex >= pathSplits[i].start && currentIndex <= pathSplits[i].end)   // on est sur un split actuellement
                return;
            if (pathSplits[i].start > currentIndex)
            {
                nextSplit = pathSplits[i];
                break;
            }
        }

        if (nextSplit != null)
        {
            int sign = left ? -1 : 1;
            int cur = nextSplit.start + 1;
            while (cur <= nextSplit.end)
            {
                path.Points[cur].PositionWorld = new Vector3(Mathf.Abs(path.Points[cur].PositionWorld.x) * sign, path.Points[cur].PositionWorld.y, path.Points[cur].PositionWorld.z);
                cur++;
            }
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