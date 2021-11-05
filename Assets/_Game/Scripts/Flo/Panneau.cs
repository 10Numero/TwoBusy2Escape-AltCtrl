using UnityEngine;

public class Panneau : MonoBehaviour
{
    public void ChangeToLeftDirection() => transform.localEulerAngles = new Vector3(0, 180, 0);


    public void ChangeToRightDirection() => transform.localEulerAngles = Vector3.zero;

    //private void Awake() => DirectionManager.instance.RegisterPanneau(gameObject);

    //private void OnDestroy() => DirectionManager.instance.UnegisterPanneau(gameObject);
}
