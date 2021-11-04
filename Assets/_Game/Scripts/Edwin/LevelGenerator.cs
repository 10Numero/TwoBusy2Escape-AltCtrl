using BansheeGz.BGSpline.Curve;
using BansheeGz.BGSpline.Components;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    public int MIN_DISTANCE_BOUND = 1; // pour les GDs
    public int MAX_DISTANCE_BOUND = 3000; // pour les GDs

    public bool randomDistance = false;

    public int minimumDistance = 1;
    public int maximumDistance = 100;
    public int totalDistance = 50;

    public float straightProbability = 0.9f;
    public float splitStraightProbability = 0.8f;

    public int minimumSimpleStraight = 2;
    public int minimumSplitStraight = 2;

    public BGCurve path;

    public float curvature = 0.5f;

    public GameObject simpleStraightPath;
    public GameObject splitStraightLeftPath;
    public GameObject splitStraightRightPath;
    public GameObject leftPath;
    public GameObject rightPath;

    [SerializeField]
    public GameObject[] obstacles;

    private class Split
    {
        public int start, end;
        public bool shouldWarn, isWarned;

        public Split(int start) { this.start = start; end = -1; shouldWarn = false; isWarned = false; } // end et isWarned vont etre instancié après

    }
    List<Split> pathSplits = new List<Split>();

#if UNITY_EDITOR
    public const int VAR_SPACE = 6;
    public int tab = 0;
#endif

    public static LevelGenerator _instance; 
    void Awake()
    {
        if (_instance == null) _instance = this;
        else Destroy(this.gameObject);

        Generate();
    }

    void Update()
    {
        SplitDetectAndWarning();
    }

    #region Generate
    public void Generate()
    {
        if (!path)
            throw new System.Exception("You forgot to select the BGCurve element");

        if (!simpleStraightPath || !splitStraightLeftPath || !splitStraightRightPath || !leftPath || !rightPath)
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
        totalDistance = (int)cumul; // pour récupérer le random au cas ou
        Vector3 parent = Vector3.zero;
        GameObject pathElement = null, leftSplitElement = null, rightSplitElement = null;

        bool isSplit = false, isAtLeastSomeStraightOnSplit = false, isAtLeastSomeStraigntAfterSplit = false, isObstacleLeftSide = false, isObstacleSpawned = false;
        int atLeastStraightAfterSplit = minimumSimpleStraight, atLeastStraightOnSplit = minimumSplitStraight;

        int ptIndex = 0;

        while (cumul > 0)
        {
            if (!isSplit)
            {
                if (isAtLeastSomeStraigntAfterSplit)
                {
                    bool isStraight = Random.value <= straightProbability;  // proba normale

                    if (isStraight)
                    {
                        InstantiateSimpleStraightPath(simpleStraightPath, ref pathElement, ref parent, ref cumul, ref ptIndex);
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

                        cumul -= leftSplitElement.transform.Find("_Sand").localScale.z; // assuming equal lengths between left and right

                        isSplit = true;
                    }
                }
                else
                {
                    InstantiateSimpleStraightPath(simpleStraightPath, ref pathElement, ref parent, ref cumul, ref ptIndex);
                    pathElement.transform.parent = pathParent;

                    atLeastStraightAfterSplit--;
                    if (atLeastStraightAfterSplit == 0)
                    {
                        atLeastStraightAfterSplit = minimumSimpleStraight;
                        isAtLeastSomeStraigntAfterSplit = true;
                    }
                }
            }
            else
            {
                if (isAtLeastSomeStraightOnSplit)
                {
                    bool isStraight = Random.value <= splitStraightProbability; // proba si split

                    if (isStraight)
                    {
                        InstantiateSplitPath(splitStraightLeftPath, splitStraightRightPath, isObstacleLeftSide, ref leftSplitElement, ref rightSplitElement, ref cumul, ref ptIndex);
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

                        isAtLeastSomeStraigntAfterSplit = false;
                        isAtLeastSomeStraightOnSplit = false;
                        isObstacleSpawned = false;
                        isSplit = false;
                    }
                }
                else
                {
                    InstantiateSplitPath(splitStraightLeftPath, splitStraightRightPath, isObstacleLeftSide, ref leftSplitElement, ref rightSplitElement, ref cumul, ref ptIndex);
                    leftSplitElement.transform.parent = pathParent;
                    rightSplitElement.transform.parent = pathParent;

                    atLeastStraightOnSplit--;
                    if (atLeastStraightOnSplit == 0)
                    {
                        atLeastStraightOnSplit = minimumSplitStraight;
                        isAtLeastSomeStraightOnSplit = true;
                    }
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
        cumul -= pathElement.transform.transform.Find("_Sand").localScale.z;
    }

    private void InstantiateSplitPath(GameObject leftPrefab, GameObject rightPrefab, bool isObstacleLeftSide, ref GameObject leftSplitElement, ref GameObject rightSplitElement, ref float cumul, ref int ptIndex)
    {
        Vector3 leftParent = leftSplitElement.transform.Find("end_pos").position;
        leftSplitElement = Instantiate(leftPrefab, leftParent, Quaternion.identity);
        Vector3 rightParent = rightSplitElement.transform.Find("end_pos").position;
        rightSplitElement = Instantiate(rightPrefab, rightParent, Quaternion.identity);

        AddBGCPoint(isObstacleLeftSide ? leftSplitElement.transform.position : rightSplitElement.transform.position, leftPrefab == rightPrefab, ref ptIndex);

        cumul -= leftSplitElement.transform.transform.Find("_Sand").localScale.z; // assuming equal lengths between left and right
    }
    #endregion

    #region Switch Lanes
    public void SwitchLane(bool left)
    {
        Split nextSplit = NextSplit();

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
    #endregion

    #region Split Warning
    private void SplitDetectAndWarning()
    {
        Split nextSplit = NextSplit(), currentSplit = CurrentSplit();

        if (nextSplit != null && Vector3.Distance(path.GetComponent<BGCcCursor>().CalculatePosition(), path.Points[nextSplit.start].PositionWorld) <= (minimumSimpleStraight * simpleStraightPath.transform.Find("_Sand").localScale.z))
        {
            if (!nextSplit.shouldWarn)
            {
                //EventManager.instance.OnWarningStart.Invoke();
                Debug.Log(nextSplit.start + " Should Warn");
                nextSplit.shouldWarn = true;
            }
        }

        if (currentSplit != null && currentSplit.shouldWarn && !currentSplit.isWarned)
        {
            //EventManager.instance.OnWarningStop.Invoke();
            Debug.Log(currentSplit.start + " Is Warned");
            currentSplit.isWarned = true;
        }
    }

    private Split CurrentSplit()
    {
        int currentIndex = path.GetComponent<BGCcMath>().CalcSectionIndexByDistance(path.GetComponent<BGCcCursor>().Distance);
        Split previousSplit = pathSplits[0];
        for (int i = 1; i <= pathSplits.Count; i++)
        {
            int end = previousSplit.end != -1 ? previousSplit.end : path.PointsCount - 1;
            if (i != 0 && currentIndex >= previousSplit.start && currentIndex <= end)   // on est sur un split actuellement
                return previousSplit;
            if(i < pathSplits.Count)
                previousSplit = pathSplits[i];
        }
        return null;
    }
    #endregion

    #region Detect Next Split
    private Split NextSplit()
    {
        int currentIndex = path.GetComponent<BGCcMath>().CalcSectionIndexByDistance(path.GetComponent<BGCcCursor>().Distance);
        for (int i = 0; i < pathSplits.Count; i++)
        {
            if (pathSplits[i].start > currentIndex)
                return pathSplits[i];
        }
        return null;
    }
    #endregion
}

#region Custom Editor
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
                    levelGenerator.minimumSimpleStraight = EditorGUILayout.IntSlider("Minimum Simple Straight Paths", levelGenerator.minimumSimpleStraight, 1, 10);
                    GUILayout.Space(LevelGenerator.VAR_SPACE);
                    levelGenerator.minimumSplitStraight = EditorGUILayout.IntSlider("Minimum Split Straight Paths", levelGenerator.minimumSplitStraight, 1, 10);

                    GUILayout.Space(LevelGenerator.VAR_SPACE);
                    levelGenerator.path = (BGCurve)EditorGUILayout.ObjectField("BGCurve Path", levelGenerator.path, typeof(BGCurve), true);
                    if (!levelGenerator.path)
                        EditorGUILayout.HelpBox("You forgot to select the BGCurve element", MessageType.Warning);

                    GUILayout.Space(LevelGenerator.VAR_SPACE);
                    levelGenerator.curvature = EditorGUILayout.Slider("Curvature", levelGenerator.curvature, -5f, 5f);

                    GUILayout.Space(LevelGenerator.VAR_SPACE);
                    levelGenerator.simpleStraightPath = (GameObject)EditorGUILayout.ObjectField("Simple Straight Path", levelGenerator.simpleStraightPath, typeof(GameObject), true);
                    levelGenerator.splitStraightLeftPath = (GameObject)EditorGUILayout.ObjectField("Split Straight Left Path", levelGenerator.splitStraightLeftPath, typeof(GameObject), true);
                    levelGenerator.splitStraightRightPath = (GameObject)EditorGUILayout.ObjectField("Split Straight Right Path", levelGenerator.splitStraightRightPath, typeof(GameObject), true);
                    levelGenerator.leftPath = (GameObject)EditorGUILayout.ObjectField("Left Path", levelGenerator.leftPath, typeof(GameObject), true);
                    levelGenerator.rightPath = (GameObject)EditorGUILayout.ObjectField("Right Path", levelGenerator.rightPath, typeof(GameObject), true);

                    if (!levelGenerator.simpleStraightPath || !levelGenerator.splitStraightLeftPath || !levelGenerator.splitStraightRightPath || !levelGenerator.leftPath || !levelGenerator.rightPath)
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
#endregion