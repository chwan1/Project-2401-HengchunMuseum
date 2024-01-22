using UnityAtoms.BaseAtoms;
using UnityEngine;

public class ValueRamp : MonoBehaviour
{
    public float Value { get; private set; }
    public FloatVariable duration;

    float valueEnd;
    float valueStart;
    float timeStampEnd;
    float timeStampStart;

    public void OnValueChanged(float value)
    {
        var t = Mathf.InverseLerp(valueStart, valueEnd, Value);
        if (valueStart == valueEnd) t = 1;

        (valueStart, valueEnd) = (Value, value);
        (timeStampStart, timeStampEnd) = (Time.time, Time.time + duration.Value * t);
    }

    void Update()
    {
        var t = Mathf.InverseLerp(timeStampStart, timeStampEnd, Time.time);
        Value = Mathf.Lerp(valueStart, valueEnd, t);
    }
}
