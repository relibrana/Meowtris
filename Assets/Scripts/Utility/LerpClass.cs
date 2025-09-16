using UnityEngine;

/// <summary>
/// Usage: Create an instance of this class whenever you need to lerp between two values,
/// initialize its lerpTime (0.5f by default). Always Call ActivateLerp to reset lerp values
/// and to set it to ready whenever you call LerpValues in an Update, checking if IsLerping == true.
/// Have a dedicated function to set your own values and to reset the lerping values.
/// 
/// Note: for full accurate values, use different instances of this class (one for each Lerp call)
/// </summary>
[System.Serializable]
public class LerpClass {

    public float LerpTime { get; set; } = 0.5f;
    public float LerpCurrentTime { get; private set; } = 0f;
    public bool IsLerping { get; private set; } = false;
    public float Perc { get; private set; } = 0f;

    public AnimationCurve smoothingCurve = AnimationCurve.Constant(0f, 1f, 1f);

    public LerpClass(float _lerpTime) {
        LerpTime = _lerpTime;
    }
    public LerpClass() {
    }

    public void ActivateLerp() {
        ResetLerpValues();
        IsLerping = true;
    }
    public void ResetLerpValues() {
        LerpCurrentTime = 0f;
        Perc = 0f;
        IsLerping = false;
    }

    public Color LerpValues(Color _valueFrom, Color _valueTo) {
        LerpCurrentTime += Time.deltaTime;
        Perc = LerpCurrentTime / LerpTime;
        Perc *= smoothingCurve.Evaluate(LerpCurrentTime/LerpTime);
        Perc = Mathf.Clamp01(Perc);

        if (Perc == 1f) {
            IsLerping = false;
            return _valueTo;
        }
        return Color.Lerp(_valueFrom, _valueTo, Perc);
    }
    public float LerpValues(float _valueFrom, float _valueTo) {
        LerpCurrentTime += Time.deltaTime;
        Perc = LerpCurrentTime / LerpTime;
        Perc *= smoothingCurve.Evaluate(LerpCurrentTime/LerpTime);
        Perc = Mathf.Clamp01(Perc);

        if (Perc == 1f) {
            IsLerping = false;
            return _valueTo;
        }
        return Mathf.Lerp(_valueFrom, _valueTo, Perc);
    }
    public Vector3 LerpValues(Vector3 _valueFrom, Vector3 _valueTo) {
        LerpCurrentTime += Time.deltaTime;
        Perc = LerpCurrentTime / LerpTime;
        Perc *= smoothingCurve.Evaluate(LerpCurrentTime/LerpTime);
        Perc = Mathf.Clamp01(Perc);

        if (Perc == 1f) {
            IsLerping = false;
            return _valueTo;
        }
        return Vector3.Lerp(_valueFrom, _valueTo, Perc);
    }
}
