using UnityEngine;

public class MultipleScreensDisplay : MonoBehaviour
{
    public Camera[] cameras;

    void Awake()
    {
        for (int i = 1; i < Display.displays.Length; i++)
            Display.displays[i].Activate();
    }

    private void Update()
    {
        for(int i = 0; i < cameras.Length; i++)
        {
            int index = System.Array.IndexOf(cameras, cameras[i]);

            if (index < Display.displays.Length)
                cameras[i].enabled = true;
            else
                cameras[i].enabled = false;
        }
    }
}
