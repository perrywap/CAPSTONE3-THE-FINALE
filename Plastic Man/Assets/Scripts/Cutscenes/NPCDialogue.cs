using System.Collections;
using UnityEngine;
using TMPro;

public class NPCDialogue : MonoBehaviour
{
    public static bool IsTalking = false;

    [Header("UI References")]
    [SerializeField] private GameObject _speechBubbleCanvas;
    [SerializeField] private TMP_Text _dialogueText;
    [SerializeField] private GameObject _interactPrompt;

    // NEW: The "!" GameObject
    [SerializeField] private GameObject _questIndicator;

    [Header("Dialogue Settings")]
    [TextArea(3, 5)]
    [SerializeField] private string[] _dialogueSequences;
    [SerializeField] private float _typingSpeed = 0.05f;

    [Header("Interaction Settings")]
    [SerializeField] private float _interactRadius = 3f;

    [Header("Progression Settings")]
    [SerializeField] private GameObject _invisibleBlocker;

    private int _currentSequenceIndex = 0;
    private int _currentTMPPage = 1;
    private bool _isTyping = false;
    private bool _isDialogueActive = false;
    private Coroutine _typingCoroutine;
    private Transform _playerTransform;

    private bool _hasTalkedBefore = false;

    private void Start()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            _playerTransform = player.transform;
        }

        if (_speechBubbleCanvas != null) _speechBubbleCanvas.SetActive(false);
        if (_interactPrompt != null) _interactPrompt.SetActive(false);

        // NEW: Ensure the "!" is visible when the level starts!
        if (_questIndicator != null) _questIndicator.SetActive(true);

        IsTalking = false;
    }

    private void Update()
    {
        if (_isDialogueActive)
        {
            if (_interactPrompt != null && _interactPrompt.activeSelf)
            {
                _interactPrompt.SetActive(false);
            }

            if (_hasTalkedBefore && _playerTransform != null)
            {
                float distance = Vector2.Distance(transform.position, _playerTransform.position);
                if (distance > _interactRadius)
                {
                    CloseDialogue();
                    return;
                }
            }

            if (Input.GetKeyDown(KeyCode.Space))
            {
                if (_isTyping) CompleteTextInstantly();
                else NextPageOrSequence();
            }
        }
        else
        {
            if (_playerTransform != null)
            {
                float distance = Vector2.Distance(transform.position, _playerTransform.position);

                if (distance <= _interactRadius)
                {
                    if (_interactPrompt != null && !_interactPrompt.activeSelf)
                    {
                        _interactPrompt.SetActive(true);
                    }

                    if (Input.GetKeyDown(KeyCode.E))
                    {
                        StartDialogueSequence();
                    }
                }
                else
                {
                    if (_interactPrompt != null && _interactPrompt.activeSelf)
                    {
                        _interactPrompt.SetActive(false);
                    }
                }
            }
        }
    }

    private void StartDialogueSequence()
    {
        if (!_hasTalkedBefore)
        {
            Time.timeScale = 0f;
            IsTalking = true;

            // NEW: Hide the "!" indicator forever once the conversation starts
            if (_questIndicator != null) _questIndicator.SetActive(false);
        }

        if (_interactPrompt != null) _interactPrompt.SetActive(false);
        if (_speechBubbleCanvas != null) _speechBubbleCanvas.SetActive(true);

        _isDialogueActive = true;
        _currentSequenceIndex = 0;

        LoadSequence();
    }

    private void LoadSequence()
    {
        _dialogueText.text = _dialogueSequences[_currentSequenceIndex];
        _dialogueText.ForceMeshUpdate();

        _currentTMPPage = 1;
        _typingCoroutine = StartCoroutine(TypeCurrentPage());
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
            _dialogueText.maxVisibleCharacters = i + 1;
            yield return new WaitForSecondsRealtime(_typingSpeed);
        }

        _isTyping = false;
    }

    private void CompleteTextInstantly()
    {
        if (_typingCoroutine != null) StopCoroutine(_typingCoroutine);

        int pageIndex = _currentTMPPage - 1;
        int lastChar = _dialogueText.textInfo.pageInfo[pageIndex].lastCharacterIndex;

        _dialogueText.maxVisibleCharacters = lastChar + 1;
        _isTyping = false;
    }

    private void NextPageOrSequence()
    {
        if (_currentTMPPage < _dialogueText.textInfo.pageCount)
        {
            _currentTMPPage++;
            _typingCoroutine = StartCoroutine(TypeCurrentPage());
        }
        else if (_currentSequenceIndex < _dialogueSequences.Length - 1)
        {
            _currentSequenceIndex++;
            LoadSequence();
        }
        else
        {
            CloseDialogue();
        }
    }

    private void CloseDialogue()
    {
        if (_typingCoroutine != null)
        {
            StopCoroutine(_typingCoroutine);
            _typingCoroutine = null;
        }
        _isTyping = false;

        if (_speechBubbleCanvas != null) _speechBubbleCanvas.SetActive(false);
        _isDialogueActive = false;

        if (!_hasTalkedBefore)
        {
            Time.timeScale = 1f;
            IsTalking = false;
            _hasTalkedBefore = true;

            if (_invisibleBlocker != null)
            {
                _invisibleBlocker.SetActive(false);
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, _interactRadius);
    }
}