using UnityEngine;
using UnityEngine.Audio;

// https://forum.unity.com/threads/changing-audio-mixer-group-volume-with-ui-slider.297884/#post-3494983

public class AudioMixerControl : MonoBehaviour
{
    public AudioMixer target;
    public string parameterName;
    public bool isDb;
    public float Value
    {
        get { target.GetFloat(parameterName, out var v); return isDb ? Mathf.Exp(v / 20) - 0.001f : v; }
        set => target.SetFloat(parameterName, isDb ? Mathf.Log(value + 0.001f) * 20 : value);
    }
}
