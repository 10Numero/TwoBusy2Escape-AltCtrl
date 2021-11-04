using UnityEngine;

public class MultipleScreensDisplay : MonoBehaviour
{
    public Camera[] cameras;
    private int lastDisplayLength;

    void Awake()
    {
        for (int i = 1; i < Display.displays.Length; i++)
            Display.displays[i].Activate();
    }

    private void Start()
    {
        EventManager.instance.OnDisplaysLengthChanged.Invoke(Display.displays.Length);
    }

    private void Update()
    {
        lastDisplayLength = Display.displays.Length;

        for (int i = 0; i < cameras.Length; i++)
        {
            int index = System.Array.IndexOf(cameras, cameras[i]);

            if (index < Display.displays.Length)
                cameras[i].enabled = true;
            else
                cameras[i].enabled = false;
        }
    }

    private void LateUpdate()
    {
        if (lastDisplayLength != Display.displays.Length)
        {
            EventManager.instance.OnDisplaysLengthChanged.Invoke(Display.displays.Length);

            for (int i = 1; i < Display.displays.Length; i++)
                Display.displays[i].Activate();
        }
    }
}
