using System.Collections;
using UnityEngine;
using TMPro;

public class CutsceneTrigger : MonoBehaviour
{
    [Header("Camera Pan Settings")]
    [SerializeField] private Camera _mainCamera;
    [Tooltip("Drag the script that usually makes your camera follow the player in here so we can turn it off temporarily!")]
    [SerializeField] private MonoBehaviour _cameraFollowScript;

    [Tooltip("Add as many targets as you want! The camera will visit them in order.")]
    [SerializeField] private Transform[] _targetLocations;

    [SerializeField] private float _initialDelay = 0.5f;
    [SerializeField] private float _panSpeed = 15f;
    [SerializeField] private float _viewWaitTime = 3f;

    [Header("Dialogue UI")]
    [SerializeField] private GameObject _dialogueCanvas;
    [SerializeField] private TMP_Text _dialogueText;

    [Header("Dialogue Text")]
    [TextArea(3, 5)]
    [SerializeField] private string[] _dialogueSequences;
    [SerializeField] private float _typingSpeed = 0.05f;

    private bool _hasTriggered = false;
    private bool _isTyping = false;
    private bool _waitingForInput = false;
    private int _currentSequenceIndex = 0;
    private int _currentTMPPage = 1;
    private Coroutine _typingCoroutine;

    private void Start()
    {
        if (_dialogueCanvas != null) _dialogueCanvas.SetActive(false);
        if (_mainCamera == null) _mainCamera = Camera.main;
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

        if (_targetLocations != null && _targetLocations.Length > 0)
        {
            for (int i = 0; i < _targetLocations.Length; i++)
            {
                Transform currentTarget = _targetLocations[i];
                if (currentTarget == null) continue;

                Vector3 targetPos = new Vector3(currentTarget.position.x, currentTarget.position.y, startPos.z);

                while (Vector3.Distance(_mainCamera.transform.position, targetPos) > 0.1f)
                {
                    _mainCamera.transform.position = Vector3.MoveTowards(_mainCamera.transform.position, targetPos, _panSpeed * Time.unscaledDeltaTime);
                    yield return null;
                }
                _mainCamera.transform.position = targetPos;

                yield return new WaitForSecondsRealtime(_viewWaitTime);
            }
        }

        if (_dialogueSequences != null && _dialogueSequences.Length > 0)
        {
            if (_dialogueCanvas != null) _dialogueCanvas.SetActive(true);
            _currentSequenceIndex = 0;

            while (_currentSequenceIndex < _dialogueSequences.Length)
            {
                yield return StartCoroutine(PlayDialogueSequence());
                _currentSequenceIndex++;
            }

            if (_dialogueCanvas != null) _dialogueCanvas.SetActive(false);
        }

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

    private IEnumerator PlayDialogueSequence()
    {
        _dialogueText.text = _dialogueSequences[_currentSequenceIndex];
        _dialogueText.ForceMeshUpdate();
        _currentTMPPage = 1;

        bool sequenceFinished = false;

        while (!sequenceFinished)
        {
            _typingCoroutine = StartCoroutine(TypeCurrentPage());
            _waitingForInput = true;

            while (_waitingForInput)
            {
                // NEW: Only accept the Spacebar if the game is NOT paused
                if (Input.GetKeyDown(KeyCode.Space) && (GameManager.Instance == null || !GameManager.Instance.IsPaused))
                {
                    if (_isTyping)
                    {
                        CompleteTextInstantly();
                    }
                    else
                    {
                        if (_currentTMPPage < _dialogueText.textInfo.pageCount)
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
        _dialogueText.pageToDisplay = _currentTMPPage;

        int pageIndex = _currentTMPPage - 1;
        int firstChar = _dialogueText.textInfo.pageInfo[pageIndex].firstCharacterIndex;
        int lastChar = _dialogueText.textInfo.pageInfo[pageIndex].lastCharacterIndex;

        _dialogueText.maxVisibleCharacters = firstChar;

        for (int i = firstChar; i <= lastChar; i++)
        {
            // NEW: If the game gets paused mid-sentence, wait right here!
            while (GameManager.Instance != null && GameManager.Instance.IsPaused)
            {
                yield return null;
            }

            _dialogueText.maxVisibleCharacters = i + 1;
            yield return new WaitForSecondsRealtime(_typingSpeed);
        }

        _isTyping = false;
    }

    private void CompleteTextInstantly()
    {
        if (_typingCoroutine != null) StopCoroutine(_typingCoroutine);
        int pageIndex = _currentTMPPage - 1;
        _dialogueText.maxVisibleCharacters = _dialogueText.textInfo.pageInfo[pageIndex].lastCharacterIndex + 1;
        _isTyping = false;
    }
}