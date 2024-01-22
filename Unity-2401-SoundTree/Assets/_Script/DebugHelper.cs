using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UltraCombos.Chwan1.Fragment
{
    public class DebugHelper : MonoBehaviour
    {
        public string header;
        public void Log(string message) => Debug.Log($"[{header}] {message}");
        public void Log(int message) => Debug.Log($"[{header}] {message}");
        public void Log(float message) => Debug.Log($"[{header}] {message}");
        public void Log(Vector2 message) => Debug.Log($"[{header}] {message}");
        public void Log(Vector3 message) => Debug.Log($"[{header}] {message}");
        public void Log(Vector4 message) => Debug.Log($"[{header}] {message}");
    }
}