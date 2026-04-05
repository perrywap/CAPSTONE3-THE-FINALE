using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.Events;

[System.Serializable]
public struct CutsceneStop
{
    public Transform TargetLocation;
    public float CameraSettleTime;
    public UnityEvent OnTargetReached;
    public float ViewWaitTime;
    public GameObject TargetNameLabel;
    public GameObject LocalDialogueCanvas;
    public TMP_Text LocalDialogueText;
    [TextArea(3, 5)] public string[] DialogueLines;
    public UnityEvent OnDialogueFinished;
    public float WaitAfterEvent;
}

public class CutsceneTrigger : MonoBehaviour
{
    [Header("Camera Pan Settings")]
    [SerializeField] private Camera _mainCamera;
    [SerializeField] private MonoBehaviour _cameraFollowScript;
    [SerializeField] private float _initialDelay = 0.5f;
    [SerializeField] private float _panSpeed = 15f;

    [Header("The Cutscene Timeline")]
    [SerializeField] private CutsceneStop[] _cutsceneStops;

    [Header("Dialogue Settings")]
    [SerializeField] private float _typingSpeed = 0.05f;

    [Header("Chain Cutscenes (Optional)")]
    [Tooltip("Drag another cutscene here to play it instantly after this one finishes!")]
    [SerializeField] private CutsceneTrigger _nextCutscene;

    private bool _hasTriggered = false;
    private bool _isTyping = false;
    private bool _waitingForInput = false;
    private int _currentTMPPage = 1;
    private Coroutine _typingCoroutine;
    private TMP_Text _activeDialogueText;

    private void Start()
    {
        if (_mainCamera == null) _mainCamera = Camera.main;

        if (_cutsceneStops != null)
        {
            foreach (CutsceneStop stop in _cutsceneStops)
            {
                if (stop.TargetNameLabel != null) stop.TargetNameLabel.SetActive(false);
                if (stop.LocalDialogueCanvas != null) stop.LocalDialogueCanvas.SetActive(false);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!_hasTriggered && collision.CompareTag("Player"))
        {
            _hasTriggered = true;
            Rigidbody2D playerRb = collision.GetComponent<Rigidbody2D>();
            if (playerRb != null)
            {
                playerRb.linearVelocity = Vector2.zero;
                playerRb.angularVelocity = 0f;
            }
            StartCoroutine(PlayCutscene(collision.transform));
        }
    }

    public void PlayFromGameManager()
    {
        if (!_hasTriggered)
        {
            _hasTriggered = true;
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                Rigidbody2D playerRb = player.GetComponent<Rigidbody2D>();
                if (playerRb != null)
                {
                    playerRb.linearVelocity = Vector2.zero;
                    playerRb.angularVelocity = 0f;
                }
                StartCoroutine(PlayCutscene(player.transform));
            }
        }
    }

    private IEnumerator PlayCutscene(Transform playerTransform)
    {
        NPCDialogue.IsTalking = true;
        Time.timeScale = 0f;

        if (_cameraFollowScript != null) _cameraFollowScript.enabled = false;

        yield return new WaitForSecondsRealtime(_initialDelay);
        Vector3 startPos = _mainCamera.transform.position;

        if (_cutsceneStops != null && _cutsceneStops.Length > 0)
        {
            for (int i = 0; i < _cutsceneStops.Length; i++)
            {
                CutsceneStop currentStop = _cutsceneStops[i];

                if (currentStop.TargetLocation != null)
                {
                    Vector3 targetPos = new Vector3(currentStop.TargetLocation.position.x, currentStop.TargetLocation.position.y, startPos.z);
                    while (Vector3.Distance(_mainCamera.transform.position, targetPos) > 0.1f)
                    {
                        _mainCamera.transform.position = Vector3.MoveTowards(_mainCamera.transform.position, targetPos, _panSpeed * Time.unscaledDeltaTime);
                        yield return null;
                    }
                    _mainCamera.transform.position = targetPos;
                }

                if (currentStop.TargetNameLabel != null) currentStop.TargetNameLabel.SetActive(true);

                if (currentStop.CameraSettleTime > 0f) yield return new WaitForSecondsRealtime(currentStop.CameraSettleTime);

                currentStop.OnTargetReached?.Invoke();

                yield return new WaitForSecondsRealtime(currentStop.ViewWaitTime);

                if (currentStop.DialogueLines != null && currentStop.DialogueLines.Length > 0 && currentStop.LocalDialogueText != null)
                {
                    _activeDialogueText = currentStop.LocalDialogueText;
                    if (currentStop.LocalDialogueCanvas != null) currentStop.LocalDialogueCanvas.SetActive(true);

                    for (int j = 0; j < currentStop.DialogueLines.Length; j++)
                    {
                        yield return StartCoroutine(PlayDialogueSequence(currentStop.DialogueLines[j]));
                    }

                    if (currentStop.LocalDialogueCanvas != null) currentStop.LocalDialogueCanvas.SetActive(false);
                }

                if (currentStop.TargetNameLabel != null) currentStop.TargetNameLabel.SetActive(false);

                currentStop.OnDialogueFinished?.Invoke();

                if (currentStop.WaitAfterEvent > 0f) yield return new WaitForSecondsRealtime(currentStop.WaitAfterEvent);
            }
        }

        // --- NEW: THE RELAY HANDOFF ---
        // If there is another cutscene chained, play it and completely STOP this script!
        if (_nextCutscene != null)
        {
            _nextCutscene.PlayFromGameManager();
            Destroy(gameObject);
            yield break;
        }

        // Return to player (Only happens if this is the absolute final cutscene)
        Vector3 playerPos = new Vector3(playerTransform.position.x, playerTransform.position.y, startPos.z);
        while (Vector3.Distance(_mainCamera.transform.position, playerPos) > 0.1f)
        {
            _mainCamera.transform.position = Vector3.MoveTowards(_mainCamera.transform.position, playerPos, _panSpeed * Time.unscaledDeltaTime);
            yield return null;
        }

        if (_cameraFollowScript != null) _cameraFollowScript.enabled = true;
        Time.timeScale = 1f;
        NPCDialogue.IsTalking = false;

        Destroy(gameObject);
    }

    private IEnumerator PlayDialogueSequence(string dialogueToPlay)
    {
        _activeDialogueText.text = dialogueToPlay;
        _activeDialogueText.ForceMeshUpdate();
        _currentTMPPage = 1;

        bool sequenceFinished = false;
        while (!sequenceFinished)
        {
            _typingCoroutine = StartCoroutine(TypeCurrentPage());
            _waitingForInput = true;

            while (_waitingForInput)
            {
                if (Input.GetKeyDown(KeyCode.Space) && (GameManager.Instance == null || !GameManager.Instance.IsPaused))
                {
                    if (_isTyping) CompleteTextInstantly();
                    else
                    {
                        if (_currentTMPPage < _activeDialogueText.textInfo.pageCount)
                        {
                            _currentTMPPage++;
                            _typingCoroutine = StartCoroutine(TypeCurrentPage());
                        }
                        else
                        {
                            _waitingForInput = false;
                            sequenceFinished = true;
                        }
                    }
                }
                yield return null;
            }
        }
    }

    private IEnumerator TypeCurrentPage()
    {
        _isTyping = true;
        _activeDialogueText.pageToDisplay = _currentTMPPage;
        int pageIndex = _currentTMPPage - 1;
        int firstChar = _activeDialogueText.textInfo.pageInfo[pageIndex].firstCharacterIndex;
        int lastChar = _activeDialogueText.textInfo.pageInfo[pageIndex].lastCharacterIndex;
        _activeDialogueText.maxVisibleCharacters = firstChar;

        for (int i = firstChar; i <= lastChar; i++)
        {
            while (GameManager.Instance != null && GameManager.Instance.IsPaused) yield return null;
            _activeDialogueText.maxVisibleCharacters = i + 1;
            yield return new WaitForSecondsRealtime(_typingSpeed);
        }
        _isTyping = false;
    }

    private void CompleteTextInstantly()
    {
        if (_typingCoroutine != null) StopCoroutine(_typingCoroutine);
        int pageIndex = _currentTMPPage - 1;
        _activeDialogueText.maxVisibleCharacters = _activeDialogueText.textInfo.pageInfo[pageIndex].lastCharacterIndex + 1;
        _isTyping = false;
    }
}