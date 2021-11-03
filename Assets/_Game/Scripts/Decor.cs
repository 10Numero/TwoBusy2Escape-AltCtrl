using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Decor : MonoBehaviour
{
    private void Awake() => LookAtManager.instance.Register(this.gameObject);

    private void OnDestroy() => LookAtManager.instance.Unregister(this.gameObject);
}
