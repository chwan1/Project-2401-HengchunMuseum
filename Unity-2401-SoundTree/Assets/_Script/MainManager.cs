using System.Collections;
using System.Collections.Generic;
using Chwan1.Utility.MediaPlayer;
using Chwan1.Utility.RootSetting;
using UnityEngine;
using System;
using UltraCombos.ControlPanel;
using Chwan1.Utility.Fragment;
using UnityAtoms.BaseAtoms;
using System.Linq;

public class MainManager : MonoBehaviour
{
    [AutoUI("Setting/Debounce 秒數")] public float debounceTime = 0.1f;
    public MediaPlayerBase mediaPlayerIdle;
    public AVProMediaPlayer mediaPlayerShow;
    public FloatVariable sensor1;
    public FloatVariable sensor2;
    public FloatVariable sensor3;
    public FloatVariable sensor4;
    public FloatVariable sensor5;
    public FloatVariable sensorVocalVirtual;

    public List<ValueRamp> valueRamps = new List<ValueRamp>();

    public string SensorStatus { get; private set; }
    public string SensorAudioRate { get; private set; }

    KeyCode keyCodeSensor1 = KeyCode.Alpha1;
    KeyCode keyCodeSensor2 = KeyCode.Alpha2;
    KeyCode keyCodeSensor3 = KeyCode.Alpha3;
    KeyCode keyCodeSensor4 = KeyCode.Alpha4;
    KeyCode keyCodeSensor5 = KeyCode.Alpha5;
    float timerSensor1 = 0f;
    float timerSensor2 = 0f;
    float timerSensor3 = 0f;
    float timerSensor4 = 0f;
    float timerSensor5 = 0f;

    List<string> showVideoPaths = new List<string>();
    int showVideoIdx = 0;

    // Start is called before the first frame update
    IEnumerator Start()
    {
        yield return new WaitForEndOfFrame();//wait for AvProMediaPlayer ready
        yield return new WaitForEndOfFrame();

        var folder = RootSetting.GetPath("Material/Video-Show");
        var files = System.IO.Directory.GetFiles(folder);
        showVideoPaths.AddRange(files);

        NextVideo();
        showVideoIdx = 0;

        // yield return mediaPlayerShow.Speed = 0;

        yield return new WaitForSeconds(5);
        mediaPlayerIdle.Speed = 1;
        mediaPlayerIdle.Loop = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(keyCodeSensor1)) OnMessage("1");
        if (Input.GetKey(keyCodeSensor2)) OnMessage("2");
        if (Input.GetKey(keyCodeSensor3)) OnMessage("3");
        if (Input.GetKey(keyCodeSensor4)) OnMessage("4");
        if (Input.GetKey(keyCodeSensor5)) OnMessage("5");

        sensor1.Value = timerSensor1 >= Time.time ? 1f : 0f;
        sensor2.Value = timerSensor2 >= Time.time ? 1f : 0f;
        sensor3.Value = timerSensor3 >= Time.time ? 1f : 0f;
        sensor4.Value = timerSensor4 >= Time.time ? 1f : 0f;
        sensor5.Value = timerSensor5 >= Time.time ? 1f : 0f;

        SensorStatus = $"{sensor1.Value} {sensor2.Value} {sensor3.Value} {sensor4.Value} {sensor5.Value}";

        var arr = new float[] { sensor1.Value, sensor2.Value, sensor3.Value, sensor4.Value, sensor5.Value };
        sensorVocalVirtual.Value = arr.Any(v => v == 1) ? 1f : 0f;

        SensorAudioRate = valueRamps
            .Aggregate("", (cur, nxt) => $"{cur}{nxt.Value}\n");
    }

    public void NextVideo()
    {
        mediaPlayerShow.Path = showVideoPaths[showVideoIdx];
        showVideoIdx = (showVideoIdx + 1) % showVideoPaths.Count;
    }

    public void OnMessage(string message)
    {
        if (message.Contains("1"))
            timerSensor1 = debounceTime + Time.time;
        if (message.Contains("2"))
            timerSensor2 = debounceTime + Time.time;
        if (message.Contains("3"))
            timerSensor3 = debounceTime + Time.time;
        if (message.Contains("4"))
            timerSensor4 = debounceTime + Time.time;
        if (message.Contains("5"))
            timerSensor5 = debounceTime + Time.time;
    }
}

