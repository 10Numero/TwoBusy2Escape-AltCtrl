using UnityEngine;

public class MultipleScreensDisplay : MonoBehaviour
{
    public Camera[] cameras;
    private int lastDisplayLength;
    private int currentDisplayLength;

    void Awake()
    {
        currentDisplayLength = Display.displays.Length;
    }

    private void Start()
    {
        EventManager.instance.OnDisplaysLengthChanged.Invoke(Display.displays.Length);
    }

    private void Update()
    {
        //Debug.Log("Display length : " + Display.displays.Length);
        lastDisplayLength = Display.displays.Length;

        for (int i = 0; i < cameras.Length; i++)
        {
            int index = System.Array.IndexOf(cameras, cameras[i]);

            if (index < Display.displays.Length)
                cameras[i].enabled = true;
            else
                cameras[i].enabled = false;
        }

        if (Input.GetKeyDown(KeyCode.KeypadPlus) || Input.GetKeyDown(KeyCode.P))
        {
            
            if (Display.displays.Length >= currentDisplayLength)
            {
                currentDisplayLength++;
                Debug.Log("Current : " + currentDisplayLength);
                for (int i = 1; i <currentDisplayLength ; i++)
                {
                    Debug.Log("active : " + i);
                    Display.displays[i].Activate();
                }
            }
            else
            {
                currentDisplayLength--;
                Debug.Log("Tried to add screen.");
            }
        }
        
        if (Input.GetKeyDown(KeyCode.KeypadMinus) || Input.GetKeyDown(KeyCode.M))
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
