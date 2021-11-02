using UnityEngine;

public class Displays : MonoBehaviour
{
    void Awake()
    {
        for (int i = 1; i < Display.displays.Length; i++)
        {
            Display.displays[i].Activate();
        }
    }
}
