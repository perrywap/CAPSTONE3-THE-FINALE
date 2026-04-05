using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class DragIconManager : MonoBehaviour
{
    public static DragIconManager Instance { get; private set; }

    [SerializeField] private Image dragIcon;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        if (dragIcon != null)
            dragIcon.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (dragIcon != null && dragIcon.gameObject.activeSelf && Mouse.current != null)
        {
            dragIcon.transform.position = Mouse.current.position.ReadValue();
        }
    }

    public void Show(Sprite sprite)
    {
        if (dragIcon == null)
            return;

        dragIcon.sprite = sprite;
        dragIcon.gameObject.SetActive(true);
    }

    public void Hide()
    {
        if (dragIcon == null)
            return;

        dragIcon.gameObject.SetActive(false);
        dragIcon.sprite = null;
    }
}