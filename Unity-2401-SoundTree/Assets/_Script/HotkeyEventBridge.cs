using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class HotkeyEventBridge : MonoBehaviour
{
    public KeyCode keyCode = KeyCode.Space;

    [Space]
    public UnityEvent keyDownEvent = new UnityEvent();

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(keyCode) == false) return;
        keyDownEvent.Invoke();
    }
}
