using UnityEngine;

public class Interactor : MonoBehaviour
{
    [SerializeField] private GameObject interactIcon;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Player player = collision.gameObject.GetComponent<Player>();

        if (player != null)
        {
            interactIcon.SetActive(true);
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
