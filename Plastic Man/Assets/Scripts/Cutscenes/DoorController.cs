using UnityEngine;

public class DoorController : MonoBehaviour
{
    [Header("Door Components")]
    [Tooltip("If checked, it will turn off ALL colliders on this object and its children")]
    [SerializeField] private bool _disableAllChildColliders = true;
    [SerializeField] private Animator _animator;
    [Tooltip("Type the exact trigger name from your Animator (e.g., 'OpenGate' or 'Open')")]
    [SerializeField] private string _animationTrigger = "Open";

    [Header("Audio")]
    [SerializeField] private AudioClip _openSfx;
    [SerializeField] private float _sfxVolume = 1f;

    public void OpenDoor()
    {
        // 1. Play the animation
        if (_animator != null)
        {
            _animator.SetTrigger(_animationTrigger);
        }
        else
        {
            this.gameObject.SetActive(false);
        }

        // 2. Handle Collisions
        if (_disableAllChildColliders)
        {
            Collider2D[] allColliders = GetComponentsInChildren<Collider2D>();
            foreach (Collider2D col in allColliders) col.enabled = false;
        }

        // 3. Handle NavMesh for Enemies
        UnityEngine.AI.NavMeshObstacle navObstacle = GetComponentInChildren<UnityEngine.AI.NavMeshObstacle>();
        if (navObstacle != null) navObstacle.enabled = false;

        // 4. Play the sound effect
        if (_openSfx != null && SfxManager.instance != null)
        {
            SfxManager.instance.PlaySFX(_openSfx, _sfxVolume);
        }
    }
}