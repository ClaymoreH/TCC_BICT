using UnityEngine;
using Cinemachine;

public class CameraShake : MonoBehaviour
{
    public CinemachineVirtualCamera virtualCamera;
    public float shakeDuration = 0.5f;
    public float shakeAmplitude = 2f;
    public float shakeFrequency = 2f;

    private float shakeTimer;

    void Start()
    {
        if (virtualCamera == null)
        {
            Debug.LogError("Virtual Camera is not assigned!");
        }
    }

    public void ShakeCamera(float duration)
    {
        var noise = virtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        noise.m_AmplitudeGain = shakeAmplitude;
        noise.m_FrequencyGain = shakeFrequency;
        shakeTimer = duration;
    }

    void Update()
    {
        if (shakeTimer > 0)
        {
            shakeTimer -= Time.deltaTime;
            if (shakeTimer <= 0)
            {
                var noise = virtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
                noise.m_AmplitudeGain = 0;
                noise.m_FrequencyGain = 0;
            }
        }
    }
}
