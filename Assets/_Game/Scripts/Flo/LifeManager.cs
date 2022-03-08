using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LifeManager : MonoBehaviour
{
    private int life = 3;

    [System.Serializable]
    public class Life
    {
        public Transform[] lifes;
    }

    public List<Life> lifes = new List<Life>();

    private void Start()
    {
        EventManager.instance.OnLostOneLife.AddListener(_LostOneLife);
    }

    void _LostOneLife()
    {
        life--;
        Debug.Log("Lose one life");
        _UpdateUI();
    }

    void _UpdateUI()
    {
        foreach(Life l in lifes)
        {
            for(int i = 0; i < l.lifes.Length; i++)
            {
                l.lifes[i].gameObject.SetActive(i < life);
            }
        }
    }

    private void Update()
    {
        if (life <= 0)
        {
            Debug.Log("Loose");
            UnityEngine.SceneManagement.SceneManager.LoadScene("LoseScene");
        }

    }
}
