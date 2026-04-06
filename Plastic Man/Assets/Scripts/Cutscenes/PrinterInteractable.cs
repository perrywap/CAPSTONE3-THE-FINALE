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
    [Tooltip("CRITICAL: You must assign your Camera Follow Script here so it stops following the player!")]
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

        // --- NEW SAFETY WARNING ---
        if (_cameraFollowScript == null)
        {
            Debug.LogError($"<color=red>WARNING:</color> The Camera Follow Script is missing on the printer named <b>{gameObject.name}</b>! The camera will get stuck on the player during zoom.");
        }

        IsInteracting = false;
    }

    private void Update()
    {
        if (GameManager.Instance != null && GameManager.Instance.IsPaused) return;

        if (IsInteracting) return;

        bool isLocked = false;

        if (GameManager.Instance != null)
        {
            if (GameManager.Instance.ActiveEnemyCount > 0) isLocked = true;
            if (!GameManager.Instance.IsGameCleared) isLocked = true;
            if (GameManager.Instance.IsWinPanelActive) isLocked = true;
        }

        if (NPCDialogue.IsTalking) isLocked = true;

        if (isLocked)
        {
            if (_interactPrompt != null && _interactPrompt.activeSelf) _interactPrompt.SetActive(false);
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

        Vector3 lockedPlayerPos = Vector3.zero;
        Rigidbody2D playerRb = null;

        if (_playerTransform != null)
        {
            lockedPlayerPos = _playerTransform.position;
            playerRb = _playerTransform.GetComponent<Rigidbody2D>();
        }

        if (_interactPrompt != null) _interactPrompt.SetActive(false);

        // This is where the magic happens (if the slot is filled!)
        if (_cameraFollowScript != null) _cameraFollowScript.enabled = false;

        if (_mainCamera != null)
        {
            _originalCameraPosition = _mainCamera.transform.position;
            _originalOrthoSize = _mainCamera.orthographicSize;

            Vector3 targetPos = new Vector3(transform.position.x, transform.position.y, _originalCameraPosition.z);

            float timeout = 0f;
            while ((Vector3.Distance(_mainCamera.transform.position, targetPos) > 0.01f || Mathf.Abs(_mainCamera.orthographicSize - _zoomSize) > 0.01f) && timeout < 1.5f)
            {
                if (_playerTransform != null)
                {
                    _playerTransform.position = lockedPlayerPos;
                    if (playerRb != null)
                    {
                        playerRb.linearVelocity = Vector2.zero;
                        playerRb.angularVelocity = 0f;
                    }
                }

                _mainCamera.transform.position = Vector3.MoveTowards(_mainCamera.transform.position, targetPos, _panSpeed * Time.unscaledDeltaTime);
                _mainCamera.orthographicSize = Mathf.MoveTowards(_mainCamera.orthographicSize, _zoomSize, _zoomSpeed * Time.unscaledDeltaTime);
                timeout += Time.unscaledDeltaTime;
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

        Vector3 lockedPlayerPos = Vector3.zero;
        Rigidbody2D playerRb = null;

        if (_playerTransform != null)
        {
            lockedPlayerPos = _playerTransform.position;
            playerRb = _playerTransform.GetComponent<Rigidbody2D>();
        }

        if (_mainCamera != null && _playerTransform != null)
        {
            Vector3 targetPos = new Vector3(_playerTransform.position.x, _playerTransform.position.y, _originalCameraPosition.z);

            float timeout = 0f;
            while ((Vector3.Distance(_mainCamera.transform.position, targetPos) > 0.01f || Mathf.Abs(_mainCamera.orthographicSize - _originalOrthoSize) > 0.01f) && timeout < 1.5f)
            {
                if (_playerTransform != null)
                {
                    _playerTransform.position = lockedPlayerPos;
                    if (playerRb != null)
                    {
                        playerRb.linearVelocity = Vector2.zero;
                        playerRb.angularVelocity = 0f;
                    }
                }

                _mainCamera.transform.position = Vector3.MoveTowards(_mainCamera.transform.position, targetPos, _panSpeed * Time.unscaledDeltaTime);
                _mainCamera.orthographicSize = Mathf.MoveTowards(_mainCamera.orthographicSize, _originalOrthoSize, _zoomSpeed * Time.unscaledDeltaTime);
                timeout += Time.unscaledDeltaTime;
                yield return null;
            }
        }

        // Turns the follow script back on!
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