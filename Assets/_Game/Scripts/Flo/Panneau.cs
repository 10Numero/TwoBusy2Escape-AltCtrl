using UnityEngine;

public class Panneau : MonoBehaviour
{
    public void ChangeToLeftDirection() => transform.localEulerAngles = Vector3.zero;


    public void ChangeToRightDirection() => transform.localEulerAngles = new Vector3(0, 180, 0);

    //private void Awake() => DirectionManager.instance.RegisterPanneau(gameObject);

    //private void OnDestroy() => DirectionManager.instance.UnegisterPanneau(gameObject);
}
