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
    [SerializeField] private GameObject _questIndicator;

    [Header("Dialogue Settings")]
    [TextArea(3, 5)]
    [SerializeField] private string[] _dialogueSequences;
    [SerializeField] private float _typingSpeed = 0.05f;

    [Header("Post-Dialogue Visuals (Optional)")]
    [Tooltip("If assigned, the NPC's animator will turn off and this sprite will be applied after the first conversation.")]
    [SerializeField] private Animator _npcAnimator;
    [SerializeField] private SpriteRenderer _npcSpriteRenderer;
    [SerializeField] private Sprite _postDialogueSprite;
    [Tooltip("What the NPC says if you talk to them again after the first conversation.")]
    [TextArea(1, 3)]
    [SerializeField] private string _postDialogueText = ".....";

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
        if (_questIndicator != null) _questIndicator.SetActive(true);

        IsTalking = false;
    }

    private void Update()
    {
        // Stop doing anything if the game is paused!
        if (GameManager.Instance != null && GameManager.Instance.IsPaused) return;

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
        // NEW: ONLY freeze time and lock controls if it is the very first conversation!
        if (!_hasTalkedBefore)
        {
            Time.timeScale = 0f;
            IsTalking = true;
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
        // NEW: Decide whether to play the main story, or the "dead" text
        if (_hasTalkedBefore)
        {
            _dialogueText.text = _postDialogueText;
        }
        else
        {
            _dialogueText.text = _dialogueSequences[_currentSequenceIndex];
        }

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
            // Wait if the game gets paused mid-sentence
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

        if (_speechBubbleCanvas != null) _speechBubbleCanvas.SetActive(false);
        _isDialogueActive = false;

        // NEW: Only unfreeze time and trigger death effects if this was the first conversation
        if (!_hasTalkedBefore)
        {
            Time.timeScale = 1f;
            IsTalking = false;
            _hasTalkedBefore = true;

            // --- POST-DIALOGUE VISUAL SWAP ---
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

            // --- BULLETPROOF ANIMATED BLOCKER LOGIC ---
            if (_invisibleBlocker != null)
            {
                Animator blockerAnim = _invisibleBlocker.GetComponent<Animator>();

                if (blockerAnim != null)
                {
                    blockerAnim.SetTrigger("OpenGate");

                    Collider2D[] allColliders = _invisibleBlocker.GetComponentsInChildren<Collider2D>();
                    foreach (Collider2D col in allColliders)
                    {
                        col.enabled = false;
                    }

                    UnityEngine.AI.NavMeshObstacle navObstacle = _invisibleBlocker.GetComponentInChildren<UnityEngine.AI.NavMeshObstacle>();
                    if (navObstacle != null)
                    {
                        navObstacle.enabled = false;
                    }
                }
                else
                {
                    _invisibleBlocker.SetActive(false);
                }
            }
            // -----------------------------------------------
        }
    }

    // --- NEW: DYING LIGHT COROUTINE (NO VANISHING) ---
    private IEnumerator DyingLightEffect()
    {
        // Make sure we actually have a Sprite Renderer before starting
        if (_npcSpriteRenderer == null) yield break;

        // 1. Remember your exact custom color and transparency (Alpha 170)
        Color originalColor = _npcSpriteRenderer.color;

        // 2. Create a "dim" version of that color (Drops the alpha to make it faint)
        Color dimmedColor = originalColor;
        dimmedColor.a = 0.2f; // 0.2f is roughly Alpha 50. It stays visible, just faint!

        // Loop this forever once he dies
        while (true)
        {
            // Dim the hologram
            _npcSpriteRenderer.color = dimmedColor;

            // Wait for a tiny, random fraction of a second
            yield return new WaitForSecondsRealtime(Random.Range(0.05f, 0.15f));

            // Restore it to full strength (Original Color & Alpha 170)
            _npcSpriteRenderer.color = originalColor;

            // Wait a little bit longer before the next flicker
            yield return new WaitForSecondsRealtime(Random.Range(0.1f, 0.6f));
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, _interactRadius);
    }
}