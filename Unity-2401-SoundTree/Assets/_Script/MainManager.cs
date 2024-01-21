using System.Collections;
using System.Collections.Generic;
using Chwan1.Utility.MediaPlayer;
using Chwan1.Utility.RootSetting;
using UnityEngine;
using System;
using UltraCombos.ControlPanel;
using Chwan1.Utility.Fragment;

public class MainManager : MonoBehaviour
{
    public MediaPlayerBase mediaPlayerIdle;
    public AVProMediaPlayer mediaPlayerShow;
    public List<string> showVideoPaths = new List<string>();
    int idx = 0;
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
    // void Update()
    // {
    // }

    public void NextVideo()
    {
        mediaPlayerShow.Path = showVideoPaths[idx];
        idx = (idx + 1) % showVideoPaths.Count;
    }


    [AutoUI("Setting/Debounce 秒數")] public float debounceTime = 0.1f;
    [SerializeField] MediaPlayerBase mediaPlayerFlow;
    [SerializeField] CanvasGroupControl canvasGroupCtrlFlow;

    KeyCode keyCodeSensor1 = KeyCode.Alpha1;

    float timerFlow = 0f;

    (Coroutine cIn, Coroutine cOut) stateFlow = (null, null);

    // Update is called once per frame
    void Update()
    {
        var _playFlow = Input.GetKey(keyCodeSensor1);


        if (_playFlow)
            timerFlow = debounceTime;
        else
            timerFlow -= Time.deltaTime;
        var playFlow = timerFlow >= 0;

        if (playFlow && stateFlow.cIn == null)
        {
            if (stateFlow.cOut != null)
            {
                StopCoroutine(stateFlow.cOut);
                stateFlow.cOut = null;
            }
            Func<float> func = () => 0;
            var it = FadeIn(mediaPlayerFlow, canvasGroupCtrlFlow, func);
            stateFlow.cIn = StartCoroutine(it);
        }
        else if (!playFlow && stateFlow.cIn != null && stateFlow.cOut == null)
        {
            StopCoroutine(stateFlow.cIn);
            stateFlow.cIn = null;
            Action action = () => stateFlow.cOut = null;
            var it = FadeOut(mediaPlayerFlow, canvasGroupCtrlFlow, action);
            stateFlow.cOut = StartCoroutine(it);
        }
    }


    IEnumerator FadeIn(MediaPlayerBase media, CanvasGroupControl canvasGroupControl, Func<float> getLoopStart)
    {
        Debug.Log($"{nameof(FadeIn)}: {media.name} 1");

        media.Speed = 1;
        media.CursorPosition = getLoopStart();

        Debug.Log($"{nameof(FadeIn)}: {media.name} 2");

        yield return canvasGroupControl.FadeInEnumerated();

        while (true)
        {
            if (media.CursorPosition >= 1)
            {
                media.CursorPosition = getLoopStart();
                Debug.Log($"{nameof(FadeIn)}: {media.name} Loop");
            }
            yield return null;
        }
    }

    IEnumerator FadeOut(MediaPlayerBase media, CanvasGroupControl canvasGroupControl, Action onFinish = null)
    {
        Debug.Log($"{nameof(FadeOut)}: {media.name}");
        yield return canvasGroupControl.FadeOutEnumerated();

        media.Speed = 0;
        media.CursorPosition = 0;

        onFinish?.Invoke();
        // Debug.Log($"{nameof(FadeOut)}: {media.name} Finish");
    }

    IEnumerator StopAndPlayAll()
    {
        Debug.Log($"{nameof(StopAndPlayAll)}");
        yield return StopFlowWeightEngine();
        yield return new WaitForSeconds(0.5f);
    }

    IEnumerator StopFlowWeightEngine(Action onFinish = null)
    {
        Debug.Log($"{nameof(StopFlowWeightEngine)} Start");
        if (stateFlow.cIn != null)
        {
            StopCoroutine(stateFlow.cIn);
            stateFlow.cIn = null;
        }

        var routines = new List<Coroutine>();
        routines.Add(StartCoroutine(FadeOut(mediaPlayerFlow, canvasGroupCtrlFlow)));

        foreach (var routine in routines)
            yield return routine;

        onFinish?.Invoke();
        // Debug.Log($"{nameof(StopAll)} Finish");
    }
}

