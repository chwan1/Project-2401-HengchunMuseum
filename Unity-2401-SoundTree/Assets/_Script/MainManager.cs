using System.Collections;
using System.Collections.Generic;
using Chwan1.Utility.MediaPlayer;
using Chwan1.Utility.RootSetting;
using UnityEngine;
using UnityEngine.Audio;

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
    void Update()
    {
    }

    public void NextVideo()
    {
        mediaPlayerShow.Path = showVideoPaths[idx];
        idx = (idx + 1) % showVideoPaths.Count;
    }
}

/*
using System;
using System.Collections;
using System.Collections.Generic;
using UltraCombos.Chwan1.Fragment;
using UltraCombos.Chwan1.MediaPlayer;
using UltraCombos.ControlPanel;
using UnityEngine;
using UnityEngine.UI;

public class MainManager : MonoBehaviour
{
    [AutoUI("Setting/Loop 起始：風"), Range(0, 1)] public float loopStartFlow;
    [AutoUI("Setting/Loop 起始：重量"), Range(0, 1)] public float loopStartWeight;
    [AutoUI("Setting/偵測邊框粗細")] public float padding;
    [AutoUI("Setting/偵測邊框顏色")] public Color paddingColor = Color.black;
    [AutoUI("Setting/Debounce 秒數")] public float debounceTime = 0.1f;
    [SerializeField] MediaPlayerBase mediaPlayerFlow;
    [SerializeField] MediaPlayerBase mediaPlayerEngine;
    [SerializeField] MediaPlayerBase mediaPlayer30PSEngine;
    [SerializeField] MediaPlayerBase mediaPlayerWeight;
    [SerializeField] MediaPlayerBase mediaPlayer30PSAll;
    [SerializeField] CanvasGroupControl canvasGroupCtrlFlow;
    [SerializeField] CanvasGroupControl canvasGroupCtrlEngine;
    [SerializeField] CanvasGroupControl canvasGroupCtrl30PSEngine;
    [SerializeField] CanvasGroupControl canvasGroupCtrlWeight;
    [SerializeField] CanvasGroupControl canvasGroupCtrl30PSAll;
    [SerializeField] CanvasGroupControl canvasGroupCtrlLogo;
    [SerializeField] AnimationCurveFader animationCurveFader;
    [SerializeField] ContourProcess contourProcess;
    [SerializeField] List<RectTransform> rtBarRows;
    [SerializeField] List<RectTransform> rtBarCols;
    [SerializeField] List<RawImage> rawImagesBars;

    KeyCode keyCodeFlow = KeyCode.Alpha1;
    KeyCode keyCodeEngine = KeyCode.Alpha2;
    KeyCode keyCodeWeight = KeyCode.Alpha3;
    KeyCode keyCodeAll = KeyCode.Alpha4;

    float timerFlow = 0f;
    float timerEngine = 0f;
    float timerWeight = 0f;
    float timerAll = 0f;


    (Coroutine cIn, Coroutine cOut) stateFlow = (null, null);
    (Coroutine cIn, Coroutine cOut) stateEngine = (null, null);
    (Coroutine cIn, Coroutine cOut) stateWeight = (null, null);
    (Coroutine cIn, Coroutine cOut) stateAll = (null, null);
    (Coroutine cIn, Coroutine cOut) stateLight = (null, null);

    // Update is called once per frame
    void Update()
    {
        foreach (var rt in rtBarRows)
            rt.sizeDelta = new Vector2(padding, rt.sizeDelta.y);

        foreach (var rt in rtBarCols)
            rt.sizeDelta = new Vector2(rt.sizeDelta.x, padding);

        foreach (var ri in rawImagesBars)
            ri.color = paddingColor;

        var blobs = contourProcess.Blobs;

        // foreach (var blob in blobs)
        // {
        //     Debug.Log($"{blob.locationNormalized.x} {blob.locationNormalized.y}");
        // }

        // Debug.Log($"{FindBlob(blobs, 0.00f, 0.25f)} {FindBlob(blobs, 0.25f, 0.50f)} {FindBlob(blobs, 0.50f, 0.75f)} {FindBlob(blobs, 0.75f, 1.00f)}");

        var _playAll = Input.GetKey(keyCodeAll) || FindBlob(blobs, 0.75f, 1.00f);

        var _playFlow = (Input.GetKey(keyCodeFlow) || FindBlob(blobs, 0.00f, 0.25f));
        var _playEngine = (Input.GetKey(keyCodeEngine) || FindBlob(blobs, 0.25f, 0.50f));
        var _playWeight = (Input.GetKey(keyCodeWeight) || FindBlob(blobs, 0.50f, 0.75f));

        if (_playAll)
            timerAll = debounceTime;
        else
            timerAll -= Time.deltaTime;
        var playAll = timerAll >= 0;

        if (_playFlow)
            timerFlow = debounceTime;
        else
            timerFlow -= Time.deltaTime;
        var playFlow = timerFlow >= 0 && !playAll;

        if (_playEngine)
            timerEngine = debounceTime;
        else
            timerEngine -= Time.deltaTime;
        var playEngine = timerEngine >= 0 && !playAll;

        if (_playWeight)
            timerWeight = debounceTime;
        else
            timerWeight -= Time.deltaTime;
        var playWeight = timerWeight >= 0 && !playAll;

        // Debug.Log($"{playFlow} {playEngine} {playWeight} {playAll}..");

        if (playAll && stateAll.cIn == null)
            stateAll.cIn = StartCoroutine(StopAndPlayAll());
        else if (!playAll && stateAll.cIn != null && stateAll.cOut == null)
        {
            StopCoroutine(stateAll.cIn);
            stateAll.cIn = null;
            Action action = () => stateAll.cOut = null;
            var it = StopFlowWeightEngine(action);
            stateAll.cOut = StartCoroutine(it);
        }

        if (playFlow && stateFlow.cIn == null)
        {
            if (stateFlow.cOut != null)
            {
                StopCoroutine(stateFlow.cOut);
                stateFlow.cOut = null;
            }
            Func<float> func = () => loopStartFlow;
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

        if (playEngine && stateEngine.cIn == null)
        {
            if (stateEngine.cOut != null)
            {
                StopCoroutine(stateEngine.cOut);
                stateEngine.cOut = null;
            }
            stateEngine.cIn = StartCoroutine(FadeInEngine());
        }
        else if (!playEngine && stateEngine.cIn != null && stateEngine.cOut == null)
        {
            StopCoroutine(stateEngine.cIn);
            stateEngine.cIn = null;
            Action action = () => stateEngine.cOut = null;
            var it = FadeOutEngine(action);
            stateEngine.cOut = StartCoroutine(it);
        }

        if (playWeight && stateWeight.cIn == null)
        {
            if (stateWeight.cOut != null)
            {
                StopCoroutine(stateWeight.cOut);
                stateWeight.cOut = null;
            }
            Func<float> func = () => loopStartWeight;
            var it = FadeIn(mediaPlayerWeight, canvasGroupCtrlWeight, func);
            stateWeight.cIn = StartCoroutine(it);
        }
        else if (!playWeight && stateWeight.cIn != null && stateWeight.cOut == null)
        {
            StopCoroutine(stateWeight.cIn);
            stateWeight.cIn = null;
            Action action = () => stateWeight.cOut = null;
            var it = FadeOut(mediaPlayerWeight, canvasGroupCtrlWeight, action);
            stateWeight.cOut = StartCoroutine(it);
        }


        var allPlayingFinished =
            stateFlow.cIn == null &&
            stateFlow.cOut == null &&
            stateEngine.cIn == null &&
            stateEngine.cOut == null &&
            stateWeight.cIn == null &&
            stateWeight.cOut == null &&
            stateAll.cIn == null &&
            stateAll.cOut == null;

        // Debug.Log($"{allPlayingFinished} {stateLight.cIn == null}");

        if (allPlayingFinished && stateLight.cIn == null)
        {
            if (stateLight.cOut != null)
            {
                StopCoroutine(stateLight.cOut);
                stateLight.cOut = null;
            }
            stateLight.cIn = StartCoroutine(FadeInLight());
        }
    }


    bool FindBlob(List<ContourProcess.Blob> blobs, float xMin, float xMax)
    {
        // return false;
        return blobs.Exists(b => b.locationNormalized.x >= xMin && b.locationNormalized.x < xMax);
    }

    IEnumerator FadeInLight()
    {
        Debug.Log($"{nameof(FadeInLight)}");
        yield return animationCurveFader.FadeInEnumerated();
        // stateLight.cIn = null;
        Debug.Log($"{nameof(FadeInLight)} Finish");
    }

    IEnumerator FadeOutLight()
    {
        Debug.Log($"{nameof(FadeOutLight)}");
        yield return animationCurveFader.FadeOutEnumerated();
        stateLight.cOut = null;
        Debug.Log($"{nameof(FadeOutLight)} Finish");
    }

    IEnumerator FadeInEngine()
    {
        Func<float> func = () => 0;
        var c1 = StartCoroutine(FadeIn(mediaPlayerEngine, canvasGroupCtrlEngine, func));
        var c2 = StartCoroutine(FadeIn(mediaPlayer30PSEngine, canvasGroupCtrl30PSEngine, func));
        yield return c1;
        yield return c2;
    }

    IEnumerator FadeOutEngine(Action onFinish = null)
    {
        var c1 = StartCoroutine(FadeOut(mediaPlayerEngine, canvasGroupCtrlEngine));
        var c2 = StartCoroutine(FadeOut(mediaPlayer30PSEngine, canvasGroupCtrl30PSEngine));
        yield return c1;
        yield return c2;

        onFinish?.Invoke();
    }

    IEnumerator FadeIn(MediaPlayerBase media, CanvasGroupControl canvasGroupControl, Func<float> getLoopStart)
    {
        Debug.Log($"{nameof(FadeIn)}: {media.name}");
        if (stateLight.cIn != null)
        {
            StopCoroutine(stateLight.cIn);
            stateLight.cIn = null;
        }
        if (stateLight.cOut == null)
            stateLight.cOut = StartCoroutine(FadeOutLight());

        while (LightInProgress)
            yield return null;

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

    bool LightInProgress => stateLight.cIn != null || stateLight.cOut != null;

    IEnumerator FadeInLogo()
    {
        Debug.Log($"{nameof(FadeInLogo)}");
        if (stateLight.cIn != null)
        {
            StopCoroutine(stateLight.cIn);
            stateLight.cIn = null;
        }
        if (stateLight.cOut == null)
            stateLight.cOut = StartCoroutine(FadeOutLight());

        while (LightInProgress)
            yield return null;

        yield return canvasGroupCtrlLogo.FadeInEnumerated();
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

        Func<float> startFromZero = () => 0;

        var routines = new List<Coroutine>();
        // routines.Add(StartCoroutine(FadeIn(mediaPlayerWeight, canvasGroupCtrlWeight, startFromZero)));
        routines.Add(StartCoroutine(FadeIn(mediaPlayerEngine, canvasGroupCtrlEngine, startFromZero)));
        // routines.Add(StartCoroutine(FadeIn(mediaPlayerFlow, canvasGroupCtrlFlow, startFromZero)));
        routines.Add(StartCoroutine(FadeIn(mediaPlayer30PSAll, canvasGroupCtrl30PSAll, startFromZero)));
        routines.Add(StartCoroutine(FadeInLogo()));

        foreach (var routine in routines)
            yield return routine;

        // Debug.Log($"{nameof(StopAllAndPlayAll)} Finish");
    }

    IEnumerator StopFlowWeightEngine(Action onFinish = null)
    {
        Debug.Log($"{nameof(StopFlowWeightEngine)} Start");
        if (stateFlow.cIn != null)
        {
            StopCoroutine(stateFlow.cIn);
            stateFlow.cIn = null;
        }
        if (stateWeight.cIn != null)
        {
            StopCoroutine(stateWeight.cIn);
            stateWeight.cIn = null;
        }
        if (stateEngine.cIn != null)
        {
            StopCoroutine(stateEngine.cIn);
            stateEngine.cIn = null;
        }

        var routines = new List<Coroutine>();
        routines.Add(StartCoroutine(FadeOut(mediaPlayerFlow, canvasGroupCtrlFlow)));
        // routines.Add(StartCoroutine(FadeOut(mediaPlayerEngine, canvasGroupCtrlEngine)));
        routines.Add(StartCoroutine(FadeOutEngine()));
        routines.Add(StartCoroutine(FadeOut(mediaPlayerWeight, canvasGroupCtrlWeight)));
        routines.Add(StartCoroutine(FadeOut(mediaPlayer30PSAll, canvasGroupCtrl30PSAll)));
        routines.Add(StartCoroutine(canvasGroupCtrlLogo.FadeOutEnumerated()));

        foreach (var routine in routines)
            yield return routine;

        onFinish?.Invoke();
        // Debug.Log($"{nameof(StopAll)} Finish");
    }
}

*/