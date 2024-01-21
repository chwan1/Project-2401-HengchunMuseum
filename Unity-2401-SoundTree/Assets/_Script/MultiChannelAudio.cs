using System.Collections;
using System.Collections.Generic;
using UltraCombos.ControlPanel;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class MultiChannelAudio : MonoBehaviour
{
    [Range(0, 7)]
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
            var _data = data[offset + 3];

            data[offset + 0] = 0.0F;
            data[offset + 1] = 0.0F;
            data[offset + 3] = 0.0F;
            data[offset + startChannel] = _data;
        }
    }
}
