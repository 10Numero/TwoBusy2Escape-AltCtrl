using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RestartManager : MonoBehaviour
{
    public string SceneToLoad = "Main";

    public List<string> objectsToExclude;

    public static RestartManager instance;
    public bool cleanReset;

    private void Awake()
    {
        instance = this;
    }

    public void RestartScene()
    {
        if (cleanReset)
        {
            List<Transform> _roots = new List<Transform>();
            foreach (Transform transform in FindObjectsOfType<Transform>())
            {
                if (!_roots.Contains(transform.root)) _roots.Add(transform.root);
            }

            foreach (Transform transform in _roots)
            {
                if (!objectsToExclude.Contains(transform.name))
                    Destroy(transform.gameObject);
            }
        }

        SceneManager.LoadScene(SceneToLoad);
    }
}
