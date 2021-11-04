using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;
using BansheeGz.BGSpline.Components;

public class CanvasManager : MonoBehaviour
{
    public List<Slider> actualUsedSliders = new List<Slider>();
    public GameObject warning;
    public GameObject shoot;
    public List<GameObject> monitorCanvasList = new List<GameObject>();

    public BGCcCursor curveCursor;

    [System.Serializable]
    public class MonitorCanvas
    {
        [Header("Monitor's Canvas")]
        public List<CanvasComponents> canvasScreen = new List<CanvasComponents>();

    }

    [System.Serializable]
    public class CanvasComponents
    {
        public Slider slider;
        public GameObject warning;
        public GameObject shoot;
    }

    public List<MonitorCanvas> monitorCanvas = new List<MonitorCanvas>();

    public static CanvasManager instance;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        EventManager.instance.OnDisplaysLengthChanged += UpdateActualUiComponents;
    }

    private void Update()
    {
        _UpdateUI();
    }

    void _UpdateUI()
    {
        foreach(Slider s in actualUsedSliders)
        {
            s.value = curveCursor.DistanceRatio;
        }
    }

    public void UpdateActualUiComponents(int monitorsCount)
    {
        CanvasActiveState(monitorsCount);

        actualUsedSliders.Clear();

        switch (monitorsCount)
        {
            case 1:

                for(int i = 0; i < monitorCanvas[0].canvasScreen.Count; i++)
                {
                    if (monitorCanvas[0].canvasScreen[i].slider != null)
                        actualUsedSliders.Add(monitorCanvas[0].canvasScreen[i].slider);

                    if (monitorCanvas[0].canvasScreen[i].warning != null)
                        warning = monitorCanvas[0].canvasScreen[i].warning;

                    if (monitorCanvas[0].canvasScreen[i].shoot != null)
                        shoot = monitorCanvas[0].canvasScreen[i].shoot;
                }


                break;
            case 2:

                for (int i = 0; i < monitorCanvas[1].canvasScreen.Count; i++)
                {
                    if(monitorCanvas[1].canvasScreen[i].slider != null)
                        actualUsedSliders.Add(monitorCanvas[1].canvasScreen[i].slider);

                    if (monitorCanvas[1].canvasScreen[i].warning != null)
                        warning = monitorCanvas[1].canvasScreen[i].warning;

                    if (monitorCanvas[1].canvasScreen[i].shoot != null)
                        shoot = monitorCanvas[1].canvasScreen[i].shoot;
                }

                break;
            case 3:

                actualUsedSliders.Add(monitorCanvas[2].canvasScreen[2].slider);

                for (int i = 0; i < monitorCanvas[2].canvasScreen.Count; i++)
                {
                    if (monitorCanvas[2].canvasScreen[i].warning != null)
                        warning = monitorCanvas[2].canvasScreen[i].warning;

                    if (monitorCanvas[2].canvasScreen[i].shoot != null)
                        shoot = monitorCanvas[2].canvasScreen[i].shoot;
                }

                break;
        }
    }

    public void CanvasActiveState(int monitorsCount)
    {
        for(int i = 0; i < monitorCanvasList.Count; i++)
            monitorCanvasList[i].gameObject.SetActive(i == monitorsCount - 1);
    }
}
