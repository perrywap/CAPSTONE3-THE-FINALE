using System.Collections;
using UnityEngine;
using TMPro;

public class AegisDialogue : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject _speechBubbleCanvas;
    [SerializeField] private TMP_Text _dialogueText;
    [SerializeField] private GameObject _interactPrompt;

    [Header("Dialogue Settings")]
    [TextArea(3, 5)]
    [SerializeField] private string[] _dialogueSequences;
    [SerializeField] private float _typingSpeed = 0.05f;

    [Header("Post-Dialogue Settings")]
    [TextArea(1, 3)]
    [SerializeField] private string _postDialogueText = ".....";

    [Tooltip("The Animator attached to Aegis. Leave empty if none.")]
    [SerializeField] private Animator _npcAnimator;
    [Tooltip("The trigger name in the Animator to play the shutdown animation.")]
    [SerializeField] private string _shutdownTriggerName = "Shutdown";

    [Header("Interaction Settings")]
    [SerializeField] private float _interactRadius = 3f;

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
        if (player != null) _playerTransform = player.transform;

        if (_speechBubbleCanvas != null) _speechBubbleCanvas.SetActive(false);
        if (_interactPrompt != null) _interactPrompt.SetActive(false);
    }

    private void Update()
    {
        if (GameManager.Instance != null && GameManager.Instance.IsPaused) return;

        if (_isDialogueActive)
        {
            if (_interactPrompt != null && _interactPrompt.activeSelf) _interactPrompt.SetActive(false);

            // Allow the player to walk away from the "....." post-dialogue text
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
                    if (_interactPrompt != null && !_interactPrompt.activeSelf) _interactPrompt.SetActive(true);
                    if (Input.GetKeyDown(KeyCode.E)) StartDialogueSequence();
                }
                else
                {
                    if (_interactPrompt != null && _interactPrompt.activeSelf) _interactPrompt.SetActive(false);
                }
            }
        }
    }

    private void StartDialogueSequence()
    {
        // --- FIX: Only freeze the player if it's the FIRST conversation! ---
        if (!_hasTalkedBefore)
        {
            NPCDialogue.IsTalking = true;
            Time.timeScale = 0f;
        }

        if (_interactPrompt != null) _interactPrompt.SetActive(false);
        if (_speechBubbleCanvas != null) _speechBubbleCanvas.SetActive(true);

        _isDialogueActive = true;
        _currentSequenceIndex = 0;

        LoadSequence();
    }

    private void LoadSequence()
    {
        if (_hasTalkedBefore) _dialogueText.text = _postDialogueText;
        else _dialogueText.text = _dialogueSequences[_currentSequenceIndex];

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
            while (GameManager.Instance != null && GameManager.Instance.IsPaused) yield return null;

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

    private void NextPageOrSequence()
    {
        if (_currentTMPPage < _dialogueText.textInfo.pageCount)
        {
            _currentTMPPage++;
            _typingCoroutine = StartCoroutine(TypeCurrentPage());
        }
        else if (!_hasTalkedBefore && _currentSequenceIndex < _dialogueSequences.Length - 1)
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
        _isDialogueActive = false;
        if (_speechBubbleCanvas != null) _speechBubbleCanvas.SetActive(false);

        // It is perfectly safe to set this to false every time we close dialogue
        NPCDialogue.IsTalking = false;

        if (!_hasTalkedBefore)
        {
            Time.timeScale = 1f;
            _hasTalkedBefore = true;

            if (_npcAnimator != null && !string.IsNullOrEmpty(_shutdownTriggerName))
            {
                _npcAnimator.SetTrigger(_shutdownTriggerName);
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, _interactRadius);
    }
}