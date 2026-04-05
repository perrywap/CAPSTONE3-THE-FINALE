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

        // --- THE INTEGRATED LOCKS ---
        bool isLocked = false;

        if (GameManager.Instance != null)
        {
            if (GameManager.Instance.ActiveEnemyCount > 0) isLocked = true; // 1. Enemies still alive
            if (!GameManager.Instance.IsGameCleared) isLocked = true;       // 2. Generators still alive
            if (GameManager.Instance.IsWinPanelActive) isLocked = true;     // 3. Win Panel is currently showing
        }

        if (NPCDialogue.IsTalking) isLocked = true; // 4. Cutscene is currently playing/panning

        // If ANY of the 4 rules above are true, completely lock the printer and hide the prompt
        if (isLocked)
        {
            if (_interactPrompt != null && _interactPrompt.activeSelf) _interactPrompt.SetActive(false);
            return;
        }

        // --- NORMAL INTERACTION ---
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

        // --- THE TRICK: Borrow the NPC Dialogue Lock to freeze player rotation! ---
        NPCDialogue.IsTalking = true;

        Time.timeScale = 0f;

        // Stop physical sliding
        if (_playerTransform != null)
        {
            Rigidbody2D rb = _playerTransform.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.linearVelocity = Vector2.zero;
                rb.angularVelocity = 0f;
            }
        }

        if (_interactPrompt != null) _interactPrompt.SetActive(false);
        if (_cameraFollowScript != null) _cameraFollowScript.enabled = false;

        if (_mainCamera != null)
        {
            _originalCameraPosition = _mainCamera.transform.position;
            _originalOrthoSize = _mainCamera.orthographicSize;

            Vector3 targetPos = new Vector3(transform.position.x, transform.position.y, _originalCameraPosition.z);

            // Safety timeout to prevent infinite camera loops
            float timeout = 0f;
            while ((Vector3.Distance(_mainCamera.transform.position, targetPos) > 0.01f || Mathf.Abs(_mainCamera.orthographicSize - _zoomSize) > 0.01f) && timeout < 1.5f)
            {
                _mainCamera.transform.position = Vector3.MoveTowards(_mainCamera.transform.position, targetPos, _panSpeed * Time.unscaledDeltaTime);
                _mainCamera.orthographicSize = Mathf.MoveTowards(_mainCamera.orthographicSize, _zoomSize, _zoomSpeed * Time.unscaledDeltaTime);
                timeout += Time.unscaledDeltaTime;
                yield return null;
            }
        }

        // Camera is done, show the panel (and now your mouse can actually click it!)
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

            // Safety timeout to prevent infinite camera loops
            float timeout = 0f;
            while ((Vector3.Distance(_mainCamera.transform.position, targetPos) > 0.01f || Mathf.Abs(_mainCamera.orthographicSize - _originalOrthoSize) > 0.01f) && timeout < 1.5f)
            {
                _mainCamera.transform.position = Vector3.MoveTowards(_mainCamera.transform.position, targetPos, _panSpeed * Time.unscaledDeltaTime);
                _mainCamera.orthographicSize = Mathf.MoveTowards(_mainCamera.orthographicSize, _originalOrthoSize, _zoomSpeed * Time.unscaledDeltaTime);
                timeout += Time.unscaledDeltaTime;
                yield return null;
            }
        }

        if (_cameraFollowScript != null) _cameraFollowScript.enabled = true;

        Time.timeScale = 1f;
        IsInteracting = false;

        // --- Release the lock so you can move and rotate again! ---
        NPCDialogue.IsTalking = false;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, _interactRadius);
    }
}