using System.Collections;
using UnityEngine;
using TMPro;

public class FinalBossDialogue : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject _speechBubbleCanvas;
    [SerializeField] private TMP_Text _dialogueText;
    [SerializeField] private GameObject _interactPrompt;
    [SerializeField] private GameObject _questIndicator;

    [Header("Story Progression Lock")]
    [SerializeField] private bool _isUnlocked = false;

    [Header("Dialogue Settings")]
    [TextArea(3, 5)]
    [SerializeField] private string[] _dialogueSequences;
    [SerializeField] private float _typingSpeed = 0.05f;

    [Header("Animator Settings")]
    [SerializeField] private Animator _npcAnimator;
    [SerializeField] private string _teleportTriggerName = "TeleportOut";

    [Header("Post-Summon Visuals")]
    [SerializeField] private SpriteRenderer _npcSpriteRenderer;
    [SerializeField] private Sprite _brokenStatueSprite;

    [Header("Boss Summon Settings")]
    [SerializeField] private GameObject _bossPrefab;
    [SerializeField] private Transform _bossSpawnLocation;

    [Header("Camera Pan Settings")]
    [SerializeField] private Camera _mainCamera;
    [SerializeField] private MonoBehaviour _cameraFollowScript;
    [SerializeField] private float _panSpeed = 20f;

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

        if (_mainCamera == null) _mainCamera = Camera.main;

        if (_speechBubbleCanvas != null) _speechBubbleCanvas.SetActive(false);
        if (_interactPrompt != null) _interactPrompt.SetActive(false);
        if (_questIndicator != null) _questIndicator.SetActive(_isUnlocked);
    }

    private void Update()
    {
        if (GameManager.Instance != null && GameManager.Instance.IsPaused) return;

        if (_isDialogueActive)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                if (_isTyping) CompleteTextInstantly();
                else NextPageOrSequence();
            }
        }
        else if (!_hasTalkedBefore && _isUnlocked)
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

    public void UnlockBoss()
    {
        _isUnlocked = true;
        if (_questIndicator != null) _questIndicator.SetActive(true);
    }

    private void StartDialogueSequence()
    {
        NPCDialogue.IsTalking = true;
        Time.timeScale = 0f;

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
        if (_questIndicator != null) _questIndicator.SetActive(false);
        if (_speechBubbleCanvas != null) _speechBubbleCanvas.SetActive(true);

        _isDialogueActive = true;
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
        else if (_currentSequenceIndex < _dialogueSequences.Length - 1)
        {
            _currentSequenceIndex++;
            LoadSequence();
        }
        else
        {
            CloseDialogueAndSummon();
        }
    }

    private void CloseDialogueAndSummon()
    {
        if (_typingCoroutine != null) StopCoroutine(_typingCoroutine);
        _isTyping = false;
        _isDialogueActive = false;
        if (_speechBubbleCanvas != null) _speechBubbleCanvas.SetActive(false);
        _hasTalkedBefore = true;
        StartCoroutine(SummonSequence());
    }

    private IEnumerator SummonSequence()
    {
        if (_npcAnimator != null && !string.IsNullOrEmpty(_teleportTriggerName))
            _npcAnimator.SetTrigger(_teleportTriggerName);

        yield return new WaitForSecondsRealtime(1.5f);

        if (_cameraFollowScript != null) _cameraFollowScript.enabled = false;

        // We memorize the EXACT offset and position here!
        Vector3 startPos = _mainCamera.transform.position;

        GameObject spawnedBoss = null;
        bool needsTemporaryFlip = false;

        if (_bossSpawnLocation != null)
        {
            Vector3 targetPos = new Vector3(_bossSpawnLocation.position.x, _bossSpawnLocation.position.y, startPos.z);
            while (Vector3.Distance(_mainCamera.transform.position, targetPos) > 0.01f)
            {
                _mainCamera.transform.position = Vector3.MoveTowards(_mainCamera.transform.position, targetPos, _panSpeed * Time.unscaledDeltaTime);
                yield return null;
            }
            _mainCamera.transform.position = targetPos; // Hard snap

            yield return new WaitForSecondsRealtime(0.5f);

            if (_bossPrefab != null)
            {
                spawnedBoss = Instantiate(_bossPrefab, _bossSpawnLocation.position, Quaternion.identity);
                if (GameManager.Instance != null) GameManager.Instance.RegisterEnemy(spawnedBoss);

                if (_playerTransform != null && spawnedBoss.transform.position.x > _playerTransform.position.x)
                {
                    needsTemporaryFlip = true;
                    spawnedBoss.transform.rotation = Quaternion.Euler(0, 180, 0);
                }
            }
            yield return new WaitForSecondsRealtime(2f);
        }

        // --- THE ANTI-TWITCH FIX ---
        // Pan directly back to the original camera position, keeping all custom offsets!
        while (Vector3.Distance(_mainCamera.transform.position, startPos) > 0.01f)
        {
            _mainCamera.transform.position = Vector3.MoveTowards(_mainCamera.transform.position, startPos, _panSpeed * Time.unscaledDeltaTime);
            yield return null;
        }

        // Hard snap
        _mainCamera.transform.position = startPos;

        if (needsTemporaryFlip && spawnedBoss != null)
            spawnedBoss.transform.rotation = Quaternion.identity;

        if (_cameraFollowScript != null) _cameraFollowScript.enabled = true;

        Time.timeScale = 1f;
        NPCDialogue.IsTalking = false;

        if (_npcAnimator != null) _npcAnimator.enabled = false;
        if (_npcSpriteRenderer != null && _brokenStatueSprite != null)
            _npcSpriteRenderer.sprite = _brokenStatueSprite;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _interactRadius);
    }
}