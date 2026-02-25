using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Card : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerUpHandler, IPointerDownHandler, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    [Header("CARD UI REFERENCES")]
    [SerializeField] private Text nameText;
    [SerializeField] private Text manaCostText;
    [SerializeField] private Text healthText;
    [SerializeField] private Text damageText;
    [SerializeField] private Image portrait;
    [SerializeField] private Image TypeIcon;
    [SerializeField] private Sprite[] TypeIcons;
    [SerializeField] private GameObject unitPrefab;

    [SerializeField] private int popValue;

    [Header("SFX")]
    [SerializeField] private AudioClip pointerDownSound;
    [SerializeField] private AudioClip pointerExitSound;

    [Header("VFX")]
    [SerializeField] private GameObject burnVfx;
    [SerializeField] private RectTransform vfxSpawnPos;

    private float yPos;
    private Vector3 originalPos;
    private CanvasGroup canvasGroup;

    public bool spawned;

    public GameObject UnitPrefab { get { return unitPrefab; } set { unitPrefab = value; } }

    private void Start()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        originalPos = transform.position;

        if (unitPrefab != null)
        {
            // nameText.text = unitPrefab.GetComponent<Unit>().Data.Name;
            manaCostText.text = unitPrefab.GetComponent<Unit>().Data.ManaCost.ToString();
            damageText.text = unitPrefab.GetComponent<Unit>().Data.Damage.ToString();
            healthText.text = unitPrefab.GetComponent<Unit>().Data.Hp.ToString();
            portrait.sprite = unitPrefab.GetComponent<SpriteRenderer>().sprite;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (spawned)
            return;

        yPos = transform.position.y;
        transform.position = new Vector3(transform.position.x, transform.position.y + popValue, transform.position.z);

        if (pointerDownSound != null)
        {
            var audioController = FindObjectOfType<AudioController>();
            audioController?.PlayAudio(null, pointerDownSound);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (spawned)
            return;

        transform.position = new Vector3(transform.position.x, yPos, transform.position.z);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        CameraController.Instance.canMoveCam = false;
        originalPos = new Vector3(transform.position.x, yPos, transform.position.z);
    }

    public void OnDrag(PointerEventData eventData)
    {
        CameraController.Instance.canMoveCam = false;
        transform.position = Input.mousePosition;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = false;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = true;

    }

    public void OnPointerUp(PointerEventData eventData)
    {
        
        CameraController.Instance.canMoveCam = true;

        if (!spawned)
            transform.position = originalPos;

        if (pointerExitSound != null)
        {
            var audioController = FindObjectOfType<AudioController>();
            audioController?.PlayAudio(null, pointerExitSound);
        }

    }

    public void OnCardDrop()
    {
        Destroy(this.gameObject);
    }
}
