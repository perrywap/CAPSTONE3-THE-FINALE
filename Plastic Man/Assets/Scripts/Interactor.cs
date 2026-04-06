using UnityEngine;

public class Interactor : MonoBehaviour
{
    [SerializeField] private GameObject interactIcon;
    [SerializeField] private GameObject printerPanel;

    private void Start()
    {
        printerPanel = GameObject.FindGameObjectWithTag("PrinterPanel");
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Player player = collision.gameObject.GetComponent<Player>();

        if (player != null)
        {
            interactIcon.SetActive(true);

            if (Input.GetKeyDown(KeyCode.E))
            {
                interactIcon.SetActive(false);

                if (printerPanel != null)
                    printerPanel.SetActive(true);
                else
                    Debug.LogWarning("No printer panel assigned on the interactor");
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        Player player = collision.gameObject.GetComponent<Player>();

        if (player != null)
        {
            interactIcon.SetActive(false);
        }
    }
}
