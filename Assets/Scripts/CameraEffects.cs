using UnityEngine;
using System.Collections;
using UnityEngine.UI;
public class CameraEffects : MonoBehaviour
{
    public static CameraEffects instance;

	// Components
	public Transform mainCamera;

	// Camera Shake
    public static bool IsShaking { get; private set; } = false;
    private float ShakeAmplitude = 1.2f;
    private float ShakeElapsedTime = 0f;

	// Fade out/in
	public Image fadeOutObject;
    private Color fadeColor = Color.black;



    void Awake()
	{
        if (instance == null)
            instance = this;
    }

	void Start ()
	{
		fadeOutObject.gameObject.SetActive(true);
	}

    void Update()
	{
        // If Camera Shake effect is still playing
        if (ShakeElapsedTime > 0)
		{
            // Update Shake Timer
            ShakeElapsedTime -= Time.deltaTime;

            // Generate perlin noise values for smooth camera shake
            float x = Random.Range(-1f, 1f) * ShakeAmplitude;
            float y = Random.Range(-1f, 1f) * ShakeAmplitude;

            // Apply the noise to the camera's position
            mainCamera.transform.localPosition = new Vector3(x, y, mainCamera.transform.localPosition.z);
        }
        else
		{
            // If Camera Shake effect is over, reset variables
            ShakeElapsedTime = 0f;
            IsShaking = false;

            // Reset the camera position
            mainCamera.transform.localPosition = new Vector3(0f, 0f, mainCamera.transform.localPosition.z);
        }
    }

    // SCREENSHAKE
    public void DoScreenShake(float _time, float _intensity = 1.2f)
	{
        ShakeElapsedTime = _time;
        ShakeAmplitude = _intensity;
        IsShaking = true;
    }

	// FADE OUT
    public void DoFadeOut(float fadeDuration)
	{
        StartCoroutine(FadeOut(fadeDuration));
    }

    // FADE IN
    public void DoFadeIn(float fadeDuration)
	{
        StartCoroutine(FadeIn(fadeDuration));
    }

	// FLASH SCREEN
	public void DoFlashScreen (Color color, float duration)
	{
		StartCoroutine (FlashScreenRoutine(color, duration));
	}


#region Coroutines
	// Fade Out
    private IEnumerator FadeOut(float fadeDuration)
	{
        float elapsedTime = 0f;
        while (elapsedTime < fadeDuration) {
            fadeColor.a = Mathf.Lerp(0f, 1f, elapsedTime / fadeDuration);
			fadeOutObject.color = fadeColor;
            elapsedTime += Time.deltaTime;
            yield return null;
        }
    }
    // Fade In
    private IEnumerator FadeIn(float fadeDuration)
	{
        float elapsedTime = 0f;
        while (elapsedTime < fadeDuration)
		{
            fadeColor.a = Mathf.Lerp(1f, 0f, elapsedTime / fadeDuration);
			fadeOutObject.color = fadeColor;
            elapsedTime += Time.deltaTime;
            yield return null;
        }
    }

	// Flash Screen
	private IEnumerator FlashScreenRoutine (Color flashColor, float duration)
	{
        float elapsedTime = 0f;
        while (elapsedTime < duration)
		{
            flashColor.a = Mathf.Lerp(1f, 0f, elapsedTime / duration);
			fadeOutObject.color = flashColor;
            elapsedTime += Time.deltaTime;
            yield return null;
        }
	}
#endregion
}
