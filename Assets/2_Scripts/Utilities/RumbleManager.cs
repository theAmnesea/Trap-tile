
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.DualShock;
using UnityEngine.InputSystem.Haptics;
using UnityEngine.InputSystem.XInput;

public class RumbleManager : MonoBehaviour
{
    public static RumbleManager Instance;

    private Gamepad pad;

    private Coroutine stopRumbleAfterTimeCoroutine;

    private bool isRumbling = false;

    [Header("LightBar Colors")]

    public Color defaultColor = Color.white;
    public Color wonColor = Color.green;
    public Color lostColor = Color.red;
    public Color damageColor = Color.red;
    public Color collectColor = Color.magenta;
    public Color jumpColor = Color.yellow;
    public Color grabThrowColor = Color.yellow;
    public Color dashColor = Color.yellow;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }

        // Get reference to our Gamepad

        pad = Gamepad.current;

#if UNITY_WEBGL || UNITY_IOS
#else
        // Set LightBar color to PS Controllers if present
        DualSenseGamepadHID.current?.SetLightBarColor(defaultColor);
#endif
    }

    #region Public Functions

    public void CollectedRumble()
    {
#if UNITY_WEBGL || UNITY_IOS
#else
        // Set LightBar color to PS Controllers if present
        DualSenseGamepadHID.current?.SetLightBarColor(collectColor);
#endif

        RumblePulseOnce(0.2f, 0.2f);
    }

    public void JumpRumble()
    {
#if UNITY_WEBGL || UNITY_IOS
#else
        // Set LightBar color to PS Controllers if present
        DualSenseGamepadHID.current?.SetLightBarColor(jumpColor);
#endif

        RumblePulseOnce(0.0f, 0.1f);
    }

    public void GrabThrowRumble()
    {
#if UNITY_WEBGL || UNITY_IOS
#else
        // Set LightBar color to PS Controllers if present
        DualSenseGamepadHID.current?.SetLightBarColor(grabThrowColor);
#endif

        RumblePulseRepeat(0.004f, 0.0f, 0.3f, 1f);
    }

    public void DashRumble()
    {
#if UNITY_WEBGL || UNITY_IOS
#else
        // Set LightBar color to PS Controllers if present
        DualSenseGamepadHID.current?.SetLightBarColor(dashColor);
#endif

        RumblePulseOnce(0.2f, 0.1f, 0.3f);
    }

    public void DamageRumble()
    {
#if UNITY_WEBGL || UNITY_IOS
#else
        // Set LightBar color to PS Controllers if present
        DualSenseGamepadHID.current?.SetLightBarColor(damageColor);
#endif
        RumblePulseOnce(0.25f, 0.3f, 1f);
    }

    public void LostRumble()
    {
#if UNITY_WEBGL || UNITY_IOS
#else
        // Set LightBar color to PS Controllers if present
        DualSenseGamepadHID.current?.SetLightBarColor(lostColor);
#endif

        defaultColor = lostColor;

        RumblePulseOnce(0.8f, 0.5f, 0.5f);
    }

    public void WonRumble()
    {
#if UNITY_WEBGL || UNITY_IOS
#else
        // Set LightBar color to PS Controllers if present
        DualSenseGamepadHID.current?.SetLightBarColor(wonColor);
#endif

        defaultColor = wonColor;

        RumblePulseOnce(0.5f, 0.8f, 0.5f);
    }

    public void ResetColor()
    {
#if UNITY_WEBGL || UNITY_IOS
#else
        // Set LightBar color to PS Controllers if present
        DualSenseGamepadHID.current?.SetLightBarColor(defaultColor);
#endif
    }

    #endregion

    #region Start Rumble

    private void RumblePulseOnce(float lowFrequency = 0.75f, float highFrequency = 0.25f, float duration = 0.2f)
    {
        // Get reference to our Gamepad
        pad ??= Gamepad.current;

        // if we have a current Gamepad
        if (pad != null)
        {
            pad.SetMotorSpeeds(lowFrequency, highFrequency);

            // stop the rumble after a certain amount of time
            stopRumbleAfterTimeCoroutine = StartCoroutine(StopRumble(duration, pad));
        }
    }

    private void RumblePulseRepeat(float lowFrequency = 0.75f, float highFrequency = 0.25f, float duration = 0.1f, float interval = 1f)
    {
        // Get reference to our Gamepad
        pad ??= Gamepad.current;

        // if we have a current Gamepad
        if (pad != null && !isRumbling)
        {
            // Only true if set to continuous rumbling
            isRumbling = true;

            StartCoroutine(RumbleRepeat(lowFrequency, highFrequency, duration, interval));
        }
    }

    private IEnumerator RumbleRepeat(float lowFrequency, float highFrequency, float duration, float interval)
    {
        while (isRumbling)
        {
            pad.SetMotorSpeeds(lowFrequency, highFrequency);
            yield return new WaitForSeconds(duration);
            pad.SetMotorSpeeds(0f, 0f);
            yield return new WaitForSeconds(interval);
        }

        yield return new WaitForSeconds(0.2f);
        ResetColor();
    }

    #endregion

    #region Stop Rumble

    public void StopRumble()
    {
        if (!isRumbling)
        {
            return;
        }

        // stop the rumble after a certain amount of time

        stopRumbleAfterTimeCoroutine = StartCoroutine(StopRumble(0f, pad));
    }

    public void StopRumbleAfterDelay(float duration)
    {
        if (!isRumbling)
        {
            return;
        }

        // stop the rumble after a certain amount of time

        stopRumbleAfterTimeCoroutine = StartCoroutine(StopRumble(duration, pad));
    }

    private IEnumerator StopRumble(float duration, Gamepad pad)
    {
        float elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime / Time.timeScale;
            yield return null;
        }

        isRumbling = false;

        // Once our duration is over
        pad.SetMotorSpeeds(0f, 0f);

        yield return new WaitForSeconds(0.4f);
        ResetColor();
    }

    #endregion
}
