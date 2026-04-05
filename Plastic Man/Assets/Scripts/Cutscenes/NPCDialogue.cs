using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.Events;

public class NPCDialogue : MonoBehaviour
{
    public static bool IsTalking = false;
    public static bool HasStartedFirstConversation = false;

    public static bool TutorialWeaponEquippedDuringDialogue = false;
    public static UnityEvent PendingDoorEvent;

    [Header("UI References")]
    [SerializeField] private GameObject _speechBubbleCanvas;
    [SerializeField] private TMP_Text _dialogueText;
    [SerializeField] private GameObject _interactPrompt;
    [SerializeField] private GameObject _questIndicator;

    [Header("Dialogue Settings")]
    [TextArea(3, 5)]
    [SerializeField] private string[] _dialogueSequences;
    [SerializeField] private float _typingSpeed = 0.05f;

    [Header("Post-Dialogue Visuals (Optional)")]
    [SerializeField] private Animator _npcAnimator;
    [SerializeField] private SpriteRenderer _npcSpriteRenderer;
    [SerializeField] private Sprite _postDialogueSprite;
    [TextArea(1, 3)]
    [SerializeField] private string _postDialogueText = ".....";

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
        if (_questIndicator != null) _questIndicator.SetActive(true);

        IsTalking = false;
        HasStartedFirstConversation = false;
        TutorialWeaponEquippedDuringDialogue = false;
        PendingDoorEvent = null;
    }

    private void Update()
    {
        if (GameManager.Instance != null && GameManager.Instance.IsPaused) return;

        if (_isDialogueActive)
        {
            if (_interactPrompt != null && _interactPrompt.activeSelf) _interactPrompt.SetActive(false);

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
        if (!_hasTalkedBefore)
        {
            IsTalking = true;
            Time.timeScale = 0f;
            if (_questIndicator != null) _questIndicator.SetActive(false);

            HasStartedFirstConversation = true;
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

        IsTalking = false;

        if (TutorialWeaponEquippedDuringDialogue && PendingDoorEvent != null)
        {
            PendingDoorEvent.Invoke();
            PendingDoorEvent = null;
            TutorialWeaponEquippedDuringDialogue = false;
        }

        if (!_hasTalkedBefore)
        {
            Time.timeScale = 1f;
            _hasTalkedBefore = true;

            if (_postDialogueSprite != null && _npcSpriteRenderer != null)
            {
                if (_npcAnimator != null)
                {
                    _npcAnimator.enabled = false;
                    _npcAnimator.runtimeAnimatorController = null;
                    Destroy(_npcAnimator);
                }

                _npcSpriteRenderer.sprite = _postDialogueSprite;
                StartCoroutine(DyingLightEffect());
            }
        }
    }

    private IEnumerator DyingLightEffect()
    {
        if (_npcSpriteRenderer == null) yield break;

        Color originalColor = _npcSpriteRenderer.color;
        Color dimmedColor = originalColor;
        dimmedColor.a = 0.2f;

        while (true)
        {
            _npcSpriteRenderer.color = dimmedColor;
            yield return new WaitForSecondsRealtime(Random.Range(0.05f, 0.15f));
            _npcSpriteRenderer.color = originalColor;
            yield return new WaitForSecondsRealtime(Random.Range(0.1f, 0.6f));
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, _interactRadius);
    }
}