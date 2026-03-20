using Cinemachine;
using UnityEngine;

public class CameraMouseOffset : MonoBehaviour
{
    public Transform player;
    public float maxOffset = 2f;
    public float smoothSpeed = 5f;

    private CinemachineVirtualCamera vcam;
    private CinemachineFramingTransposer transposer;

    void Start()
    {
        vcam = GetComponent<CinemachineVirtualCamera>();
        transposer = vcam.GetCinemachineComponent<CinemachineFramingTransposer>();
    }

    void Update()
    {
        Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorld.z = 0;

        Vector3 direction = mouseWorld - player.position;

        Vector3 offset = direction * 0.3f; // scale influence
        offset = Vector3.ClampMagnitude(offset, maxOffset);

        transposer.m_TrackedObjectOffset = Vector3.Lerp(
            transposer.m_TrackedObjectOffset,
            offset,
            Time.deltaTime * smoothSpeed
        );
    }
}
