//using UnityEngine;
//using Cinemachine;
//using System.Collections;

//public class CameraShake : MonoBehaviour
//{
//    public static CameraShake Instance { get; private set; }

//    [SerializeField] private CinemachineVirtualCamera virtualCamera;

//    private CinemachineBasicMultiChannelPerlin noise;

//    private void Awake()
//    {
//        Instance = this;
//        noise = virtualCamera
//            .GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
//    }

//    public void Shake()
//    {
//        StartCoroutine(ShakeCoroutine(1f, 0.2f));
//    }

//    public void Shake(float intensity, float time)
//    {
//        StartCoroutine(ShakeCoroutine(intensity, time));
//    }

//    private IEnumerator ShakeCoroutine(float intensity, float time)
//    {
//        noise.m_AmplitudeGain = intensity;

//        yield return new WaitForSeconds(time);

//        noise.m_AmplitudeGain = 0f;
//    }
//}

using UnityEngine;
using Cinemachine;
using System.Collections;

public class CameraShake : MonoBehaviour
{
    public static CameraShake Instance { get; private set; }

    [SerializeField] private CinemachineVirtualCamera virtualCamera;

    private CinemachineBasicMultiChannelPerlin noise;
    private Coroutine shakeCoroutine;

    private void Awake()
    {
        // Singleton
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        if (virtualCamera == null)
        {
            Debug.LogError("CameraShake: Virtual Camera not assigned!");
            return;
        }

        noise = virtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();

        if (noise == null)
        {
            Debug.LogError("CameraShake: CinemachineBasicMultiChannelPerlin missing!");
        }
    }

    // GENERAL SHAKE FUNCTION
    public void Shake(float intensity, float time)
    {
        if (noise == null) return;

        if (shakeCoroutine != null)
            StopCoroutine(shakeCoroutine);

        shakeCoroutine = StartCoroutine(ShakeCoroutine(intensity, time));
    }

    private IEnumerator ShakeCoroutine(float intensity, float time)
    {
        noise.m_AmplitudeGain = intensity;

        yield return new WaitForSeconds(time);

        noise.m_AmplitudeGain = 0f;
        shakeCoroutine = null;
    }

    // WEAPON SHAKE PRESETS

    public void PistolShake()
    {
        Shake(0.4f, 0.08f);
    }

    public void RifleShake()
    {
        Shake(0.7f, 0.12f);
    }

    public void GrenadeShake()
    {
        Shake(2.5f, 0.35f);
    }

    // OPTIONAL: Distance-based explosion shake
    public void ExplosionShake(Vector3 explosionPos, Transform player, float maxDistance = 10f)
    {
        float distance = Vector3.Distance(player.position, explosionPos);

        float intensity = Mathf.Lerp(2.5f, 0f, distance / maxDistance);

        Shake(intensity, 0.35f);
    }
}