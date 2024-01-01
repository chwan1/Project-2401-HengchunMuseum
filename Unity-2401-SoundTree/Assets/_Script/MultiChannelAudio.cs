using System.Collections;
using System.Collections.Generic;
using UltraCombos.ControlPanel;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class MultiChannelAudio : MonoBehaviour
{
    [AutoUI, Range(0, 7)]
    public int startChannel = 2;
    // Start is called before the first frame update
    void Start()
    {
        running = true;
    }

    // Update is called once per frame
    void Update()
    {

    }


    private bool running = false;

    void OnAudioFilterRead(float[] data, int channels)
    {
        if (!running) return;
        if (channels <= 2) return;

        for (var sample = 0; sample < data.Length / channels; sample++)
        {
            var offset = sample * channels;
            data[offset + startChannel] = data[offset + 0];
            data[offset + startChannel + 1] = data[offset + 1];
            data[offset + 0] = 0.0F;
            data[offset + 1] = 0.0F;
        }
    }
}
