using System.Collections;
using UnityEngine;

public class PrinterInteractable : MonoBehaviour
{
    public static bool IsInteracting = false;

    [Header("UI References")]
    [SerializeField] private GameObject _interactPrompt;
    [Tooltip("Drag your Printer Panel Canvas/Object here")]
    [SerializeField] private GameObject _printerPanel;

    [Header("Interaction Settings")]
    [SerializeField] private float _interactRadius = 2f;

    [Header("Camera Zoom Settings")]
    [SerializeField] private Camera _mainCamera;
    [SerializeField] private MonoBehaviour _cameraFollowScript;
    [SerializeField] private float _panSpeed = 20f;
    [Tooltip("How far the camera zooms in. (Lower number = closer)")]
    [SerializeField] private float _zoomSize = 3f;
    [SerializeField] private float _zoomSpeed = 10f;

    private Transform _playerTransform;
    private bool _isPlayerInRange = false;
    private float _originalOrthoSize;
    private Vector3 _originalCameraPosition;

    private void Start()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null) _playerTransform = player.transform;

        if (_mainCamera == null) _mainCamera = Camera.main;
        if (_mainCamera != null) _originalOrthoSize = _mainCamera.orthographicSize;

        if (_interactPrompt != null) _interactPrompt.SetActive(false);
        if (_printerPanel != null) _printerPanel.SetActive(false);

        IsInteracting = false;
    }

    private void Update()
    {
        if (GameManager.Instance != null && GameManager.Instance.IsPaused) return;

        if (IsInteracting) return;

        // --- NEW: THE ENEMY LOCK ---
        // If there are still enemies alive, hide the prompt and completely ignore the rest of the code!
        if (GameManager.Instance != null && GameManager.Instance.ActiveEnemyCount > 0)
        {
            if (_interactPrompt != null && _interactPrompt.activeSelf)
            {
                _interactPrompt.SetActive(false);
            }
            return;
        }

        if (_playerTransform != null)
        {
            float distance = Vector2.Distance(transform.position, _playerTransform.position);
            _isPlayerInRange = distance <= _interactRadius;

            if (_isPlayerInRange)
            {
                if (_interactPrompt != null && !_interactPrompt.activeSelf) _interactPrompt.SetActive(true);

                if (Input.GetKeyDown(KeyCode.E))
                {
                    StartCoroutine(OpenPrinterSequence());
                }
            }
            else
            {
                if (_interactPrompt != null && _interactPrompt.activeSelf) _interactPrompt.SetActive(false);
            }
        }
    }

    private IEnumerator OpenPrinterSequence()
    {
        IsInteracting = true;

        NPCDialogue.IsTalking = true;

        Time.timeScale = 0f;

        if (_interactPrompt != null) _interactPrompt.SetActive(false);
        if (_cameraFollowScript != null) _cameraFollowScript.enabled = false;

        if (_mainCamera != null)
        {
            _originalCameraPosition = _mainCamera.transform.position;
            _originalOrthoSize = _mainCamera.orthographicSize;

            Vector3 targetPos = new Vector3(transform.position.x, transform.position.y, _originalCameraPosition.z);

            while (Vector3.Distance(_mainCamera.transform.position, targetPos) > 0.01f || Mathf.Abs(_mainCamera.orthographicSize - _zoomSize) > 0.01f)
            {
                _mainCamera.transform.position = Vector3.MoveTowards(_mainCamera.transform.position, targetPos, _panSpeed * Time.unscaledDeltaTime);
                _mainCamera.orthographicSize = Mathf.MoveTowards(_mainCamera.orthographicSize, _zoomSize, _zoomSpeed * Time.unscaledDeltaTime);
                yield return null;
            }
        }

        if (_printerPanel != null) _printerPanel.SetActive(true);
    }

    public void ClosePrinter()
    {
        if (!IsInteracting) return;
        StartCoroutine(ClosePrinterSequence());
    }

    private IEnumerator ClosePrinterSequence()
    {
        if (_printerPanel != null) _printerPanel.SetActive(false);

        if (_mainCamera != null && _playerTransform != null)
        {
            Vector3 targetPos = new Vector3(_playerTransform.position.x, _playerTransform.position.y, _originalCameraPosition.z);

            while (Vector3.Distance(_mainCamera.transform.position, targetPos) > 0.01f || Mathf.Abs(_mainCamera.orthographicSize - _originalOrthoSize) > 0.01f)
            {
                _mainCamera.transform.position = Vector3.MoveTowards(_mainCamera.transform.position, targetPos, _panSpeed * Time.unscaledDeltaTime);
                _mainCamera.orthographicSize = Mathf.MoveTowards(_mainCamera.orthographicSize, _originalOrthoSize, _zoomSpeed * Time.unscaledDeltaTime);
                yield return null;
            }
        }

        if (_cameraFollowScript != null) _cameraFollowScript.enabled = true;

        Time.timeScale = 1f;
        IsInteracting = false;

        NPCDialogue.IsTalking = false;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, _interactRadius);
    }
}