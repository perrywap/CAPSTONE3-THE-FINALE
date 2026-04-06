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

    [SerializeField] public AudioClip printerFinishedPrintingSfx;
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

        // Tells your rotation script to freeze (just like The Maker!)
        NPCDialogue.IsTalking = true;
        Time.timeScale = 0f;

        // Cache the player's exact spot to lock them down
        Vector3 lockedPlayerPos = Vector3.zero;
        Rigidbody2D playerRb = null;

        if (_playerTransform != null)
        {
            lockedPlayerPos = _playerTransform.position;
            playerRb = _playerTransform.GetComponent<Rigidbody2D>();
        }

        if (_interactPrompt != null) _interactPrompt.SetActive(false);
        if (_cameraFollowScript != null) _cameraFollowScript.enabled = false;

        if (_mainCamera != null)
        {
            _originalCameraPosition = _mainCamera.transform.position;
            _originalOrthoSize = _mainCamera.orthographicSize;

            Vector3 targetPos = new Vector3(transform.position.x, transform.position.y, _originalCameraPosition.z);

            float timeout = 0f;
            while ((Vector3.Distance(_mainCamera.transform.position, targetPos) > 0.01f || Mathf.Abs(_mainCamera.orthographicSize - _zoomSize) > 0.01f) && timeout < 1.5f)
            {
                // --- THE ULTIMATE PHYSICAL FREEZE ---
                // Forces the player to stay glued to their exact coordinates during the entire zoom
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

        // Camera finishes, UI opens, and your mouse can interact with it safely!
        SfxManager.instance.PlaySFX(printerFinishedPrintingSfx, 0.5f); 
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

        // Cache the player's spot so they can't run away while the camera is zooming OUT
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
                // --- THE ULTIMATE PHYSICAL FREEZE (ZOOM OUT) ---
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