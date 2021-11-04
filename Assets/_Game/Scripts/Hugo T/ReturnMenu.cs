using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReturnMenu : MonoBehaviour
{
    public float timeToReturn;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(ReturnToMenu());
    }

    IEnumerator ReturnToMenu()
    {
        yield return new WaitForSeconds(timeToReturn);

        UnityEngine.SceneManagement.SceneManager.LoadScene(0);
    }
}
