using UnityEngine;

public class RotationMovement : MonoBehaviour
{
    [SerializeField] private float _rotationSpeed;
    void Update()
    {
        transform.Rotate(0f, 0f, _rotationSpeed * Time.deltaTime);
    }
}
