using System.Collections;
using UnityEngine;
using TMPro;

[System.Serializable]
public struct EndingDialogueBlock
{
    [Tooltip("The UI Canvas to show (e.g., Boss Bubble or Narrator Panel)")]
    public GameObject DialogueCanvas;
    [Tooltip("The TMP_Text component inside that Canvas")]
    public TMP_Text DialogueTextComponent;
    [Tooltip("The lines of dialogue to type out")]
    [TextArea(3, 5)] public string[] DialogueLines;

    [Header("Cinematic Actions")]
    [Tooltip("Type an Animator Trigger here (e.g., 'Destroyed') to play it AFTER these lines finish!")]
    public string BossAnimationTrigger;

    [Tooltip("How long to wait before moving to the next block")]
    public float DelayAfterBlock;
}

public class TrueEndingSequence : MonoBehaviour
{
    [Header("Camera Settings")]
    [SerializeField] private Camera _mainCamera;
    [SerializeField] private MonoBehaviour _cameraFollowScript;
    [SerializeField] private float _panSpeed = 15f;

    [Header("Dialogue Placement")]
    [Tooltip("How high above the boss's pivot the bubble appears. Increase this if it covers their face!")]
    [SerializeField] private float _bubbleVerticalOffset = 2.5f;

    [Header("Win Panel Settings")]
    [SerializeField] private GameObject _winPanel;
    [Tooltip("Drag the text component that says 'Area Cleared' here")]
    [SerializeField] private TMP_Text _winPanelText;
    [TextArea(2, 3)]
    [SerializeField] private string _finalStatusText = "Status: Imperfect.\nStatus: Alive.";

    [Header("Fade To Black Settings")]
    [Tooltip("How long to wait on the Win Panel before starting the fade")]
    [SerializeField] private float _waitBeforeFade = 5f; // Set to 5-6 seconds!
    [Tooltip("How long the fade to black takes to complete")]
    [SerializeField] private float _fadeDuration = 3f; // A nice, slow 3-second cinematic fade
    [Tooltip("Drag your full-screen black CanvasGroup here")]
    [SerializeField] private CanvasGroup _blackFadeScreen;

    [Header("The Story Timeline")]
    [SerializeField] private float _typingSpeed = 0.05f;
    [Tooltip("Add your sequence of dialogues here!")]
    [SerializeField] private EndingDialogueBlock[] _dialogueSequence;

    private bool _isTyping = false;
    private bool _waitingForInput = false;
    private int _currentTMPPage = 1;
    private Coroutine _typingCoroutine;
    private TMP_Text _activeDialogueText;

    private void Start()
    {
        if (_mainCamera == null) _mainCamera = Camera.main;

        if (_dialogueSequence != null)
        {
            foreach (var block in _dialogueSequence)
            {
                if (block.DialogueCanvas != null) block.DialogueCanvas.SetActive(false);
            }
        }

        if (_blackFadeScreen != null)
        {
            _blackFadeScreen.alpha = 0f;
            _blackFadeScreen.gameObject.SetActive(false);
        }
    }

    public void PlaySequence(Transform deadBossTransform)
    {
        StartCoroutine(EndingRoutine(deadBossTransform));
    }

    private IEnumerator EndingRoutine(Transform deadBossTransform)
    {
        NPCDialogue.IsTalking = true;
        Time.timeScale = 0f;

        // Intro Win Panel (Level Cleared)
        if (_winPanel != null) _winPanel.SetActive(true);
        yield return new WaitForSecondsRealtime(3f);
        if (_winPanel != null) _winPanel.SetActive(false);

        if (_cameraFollowScript != null) _cameraFollowScript.enabled = false;
        Vector3 startPos = _mainCamera.transform.position;

        if (deadBossTransform != null)
        {
            Vector3 targetPos = new Vector3(deadBossTransform.position.x, deadBossTransform.position.y, startPos.z);
            while (Vector3.Distance(_mainCamera.transform.position, targetPos) > 0.01f)
            {
                _mainCamera.transform.position = Vector3.MoveTowards(_mainCamera.transform.position, targetPos, _panSpeed * Time.unscaledDeltaTime);
                yield return null;
            }
            _mainCamera.transform.position = targetPos;
        }

        yield return new WaitForSecondsRealtime(1f);

        // Play the Dialogue Sequence
        if (_dialogueSequence != null && _dialogueSequence.Length > 0)
        {
            for (int i = 0; i < _dialogueSequence.Length; i++)
            {
                EndingDialogueBlock block = _dialogueSequence[i];

                if (block.DialogueCanvas != null)
                {
                    Canvas canvasComp = block.DialogueCanvas.GetComponent<Canvas>();
                    if (canvasComp != null && canvasComp.renderMode == RenderMode.WorldSpace && deadBossTransform != null)
                    {
                        block.DialogueCanvas.transform.position = deadBossTransform.position + new Vector3(0, _bubbleVerticalOffset, 0);
                    }

                    block.DialogueCanvas.SetActive(true);
                }

                _activeDialogueText = block.DialogueTextComponent;

                if (block.DialogueLines != null)
                {
                    for (int j = 0; j < block.DialogueLines.Length; j++)
                    {
                        yield return StartCoroutine(PlayDialogueSequence(block.DialogueLines[j]));
                    }
                }

                if (block.DialogueCanvas != null) block.DialogueCanvas.SetActive(false);

                if (!string.IsNullOrEmpty(block.BossAnimationTrigger) && deadBossTransform != null)
                {
                    Animator bossAnim = deadBossTransform.GetComponent<Animator>();
                    if (bossAnim != null)
                    {
                        bossAnim.updateMode = AnimatorUpdateMode.UnscaledTime;
                        bossAnim.SetTrigger(block.BossAnimationTrigger);
                    }
                }

                if (block.DelayAfterBlock > 0f)
                {
                    yield return new WaitForSecondsRealtime(block.DelayAfterBlock);
                }
            }
        }

        // Show the Final Win Panel
        if (_winPanelText != null) _winPanelText.text = _finalStatusText;
        if (_winPanel != null) _winPanel.SetActive(true);

        // Wait 5-6 seconds while staring at the Win Panel
        yield return new WaitForSecondsRealtime(_waitBeforeFade);

        // --- THE FIXED FADE SEQUENCE ---
        if (_blackFadeScreen != null)
        {
            _blackFadeScreen.gameObject.SetActive(true);
            float timer = 0f;

            while (timer < _fadeDuration)
            {
                _blackFadeScreen.alpha = Mathf.Lerp(0f, 1f, timer / _fadeDuration);
                timer += Time.unscaledDeltaTime;
                yield return null;
            }

            _blackFadeScreen.alpha = 1f;
        }
        else
        {
            Debug.LogError("<color=red><b>FADE FAILED:</b> You forgot to drag the BlackFadeScreen CanvasGroup into the TrueEndingSequence Inspector!</color>");
        }

        // We DO NOT turn off the Win Panel anymore! 
        // It stays exactly where it is, forever buried under the black screen.
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
                if (Input.GetKeyDown(KeyCode.Space))
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