using UnityEngine;
using Cinemachine;
using System.Collections;

public class CameraShake : MonoBehaviour
{
    public static CameraShake Instance { get; private set; }

    [SerializeField] private CinemachineVirtualCamera virtualCamera;

    private CinemachineBasicMultiChannelPerlin noise;

    private void Awake()
    {
        Instance = this;
        noise = virtualCamera
            .GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
    }

    public void Shake()
    {
        StartCoroutine(ShakeCoroutine(1f, 0.2f));
    }

    public void Shake(float intensity, float time)
    {
        StartCoroutine(ShakeCoroutine(intensity, time));
    }

    private IEnumerator ShakeCoroutine(float intensity, float time)
    {
        noise.m_AmplitudeGain = intensity;

        yield return new WaitForSeconds(time);

        noise.m_AmplitudeGain = 0f;
    }
}