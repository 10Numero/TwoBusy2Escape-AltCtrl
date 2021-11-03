using UnityEngine;

public class DecorSpawner : MonoBehaviour
{
    public Transform[] spawnPoints;
    public GameObject[] decors;

    [Range(0, 100)]
    public float spawnProb;

    private void Awake()
    {
        foreach(Transform t in spawnPoints)
        {
            if (Random.Range(0,100) < spawnProb)
                Instantiate(decors[Random.Range(0, decors.Length)], t);
        }
    }
}
