using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TouchScript.Gestures.TransformGestures;

public class KokoroControl : MonoBehaviour
{
    public Material KokoroMat;
    [Range(0, 1)]
    public float PullStrength = .5f;
    [Range(0, 20)]
    public float RecoverSpeed = 10;
    [Range(0, 1)]
    public float KokoroLife = .75f;

    ScreenTransformGesture gesture;

    static int _K = Shader.PropertyToID("_K");
    static int _Crimp = Shader.PropertyToID("_Crimp");
    static int _Size = Shader.PropertyToID("_Size");
    static int _Color = Shader.PropertyToID("_Color");

    bool taped;
    float k, crimp, size, originSize;
    Color color, originColor;

    private void Start()
    {
        gesture = GetComponent<ScreenTransformGesture>();

        gesture.TransformStarted += Gesture_TransformStarted;
        gesture.Transformed += Gesture_Transformed;
        gesture.TransformCompleted += Gesture_TransformCompleted;

        if (KokoroMat != null)
        {
            size = KokoroMat.GetFloat(_Size);
            color = KokoroMat.GetColor(_Color);
        }
        originSize = size;
        originColor = color;
    }

    private void Update()
    {
        if (!taped)
            crimp = Mathf.Lerp(crimp, 0, RecoverSpeed * Time.deltaTime);
        k = Mathf.Lerp(k, 0, RecoverSpeed * Time.deltaTime * 2);
        size = Mathf.Lerp(size, (1 - crimp) * originSize, RecoverSpeed * Time.deltaTime);
        color = Color.Lerp(color, (k * 3 + 1) * originColor, RecoverSpeed * Time.deltaTime);
        SetMat();
    }

    private void OnDestroy()
    {
        k = 0;
        crimp = 0;
        size = originSize;
        color = originColor;
        SetMat();
    }

    private void Gesture_TransformStarted(object sender, System.EventArgs e)
    {
        taped = true;
    }

    private void Gesture_Transformed(object sender, System.EventArgs e)
    {
        if (KokoroMat == null)
            return;
        crimp -= gesture.DeltaPosition.y * PullStrength * .01f;
        crimp = Mathf.Clamp01(crimp);
    }
    
    private void Gesture_TransformCompleted(object sender, System.EventArgs e)
    {
        taped = false;
        DOTween.To(() => k, x => k = x, crimp, Mathf.Pow(crimp, 1.5f) * KokoroLife).SetEase(Ease.OutBounce);
    }

    void SetMat()
    {
        KokoroMat.SetFloat(_K, k);
        KokoroMat.SetFloat(_Crimp, crimp);
        KokoroMat.SetFloat(_Size, size);
        KokoroMat.SetColor(_Color, color);
    }
}