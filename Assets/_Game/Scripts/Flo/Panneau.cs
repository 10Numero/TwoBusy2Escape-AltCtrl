using UnityEngine;

public class Panneau : MonoBehaviour
{
    private void Awake() => DirectionManager.instance.RegisterPanneau(gameObject);

    private void OnDestroy() => DirectionManager.instance.UnegisterPanneau(gameObject);
}
