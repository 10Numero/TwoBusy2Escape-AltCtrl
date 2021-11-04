using UnityEngine;

public class Decor : MonoBehaviour
{
    private void Start() => LookAtManager.instance.Register(this.gameObject);

    private void OnDestroy() => LookAtManager.instance.Unregister(this.gameObject);
}
