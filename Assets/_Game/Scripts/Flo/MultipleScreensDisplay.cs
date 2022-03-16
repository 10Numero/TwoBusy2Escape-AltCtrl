using UnityEngine;

public class MultipleScreensDisplay : MonoBehaviour
{
    public Camera[] cameras;
    private int lastDisplayLength;
    private int currentDisplayLength;

    void Awake()
    {
        currentDisplayLength = 1;
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

        if (Input.GetKeyDown(KeyCode.KeypadPlus))
        {
            currentDisplayLength++;

            if (Display.displays.Length > currentDisplayLength)
            {
                Debug.Log("Current : " + currentDisplayLength);
                for (int i = 1; i < currentDisplayLength; i++)
                {
                    Display.displays[i].Activate();
                }
            }
            else
            {
                currentDisplayLength--;
                Debug.Log("Tried to add screen.");
            }
        }
        
        if (Input.GetKeyDown(KeyCode.KeypadMinus))
        {
            currentDisplayLength--;
            
            if (currentDisplayLength <= 0)
            {
                Debug.Log("Display can't be equal to 0. Set to 1.");
                currentDisplayLength = 1;
            }


            for (int i = 1; i < currentDisplayLength; i++)
            {
                Display.displays[i].Activate();
            }
 
        }
    }

    // private void LateUpdate()
    // {
    //     if (lastDisplayLength != Display.displays.Length)
    //     {
    //         EventManager.instance.OnDisplaysLengthChanged.Invoke(Display.displays.Length);
    //
    //         for (int i = 1; i < Display.displays.Length; i++)
    //             Display.displays[i].Activate();
    //     }
    // }
}
