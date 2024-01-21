using System.Collections;
using System.Collections.Generic;
using Chwan1.Utility.MediaPlayer;
using Chwan1.Utility.RootSetting;
using UnityEngine;
using System;
using UltraCombos.ControlPanel;
using Chwan1.Utility.Fragment;
using UnityAtoms.BaseAtoms;

public class MainManager : MonoBehaviour
{
    [AutoUI("Setting/Debounce 秒數")] public float debounceTime = 0.1f;
    public MediaPlayerBase mediaPlayerIdle;
    public AVProMediaPlayer mediaPlayerShow;
    public FloatVariable volumeCh3;

    KeyCode keyCodeSensor1 = KeyCode.Alpha1;
    KeyCode keyCodeSensor2 = KeyCode.Alpha2;
    KeyCode keyCodeSensor3 = KeyCode.Alpha3;
    KeyCode keyCodeSensor4 = KeyCode.Alpha4;
    KeyCode keyCodeSensor5 = KeyCode.Alpha5;
    float timerSensor1 = 0f;
    List<string> showVideoPaths = new List<string>();
    int showVideoIdx = 0;

    // Start is called before the first frame update
    IEnumerator Start()
    {
        //add files in folder to showVideoPaths
        var folder = RootSetting.GetPath("Material/Video-Show");
        var files = System.IO.Directory.GetFiles(folder);
        showVideoPaths.AddRange(files);

        yield return new WaitForSeconds(5);
        mediaPlayerIdle.Speed = 1;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(keyCodeSensor1)) OnMessage("1");

        volumeCh3.Value = timerSensor1 >= Time.deltaTime ? 1f : 0f;
    }

    public void NextVideo()
    {
        mediaPlayerShow.Path = showVideoPaths[showVideoIdx];
        showVideoIdx = (showVideoIdx + 1) % showVideoPaths.Count;
    }

    public void OnMessage(string message)
    {
        if (message.Contains("1"))
            timerSensor1 = debounceTime + Time.deltaTime;
    }





}

