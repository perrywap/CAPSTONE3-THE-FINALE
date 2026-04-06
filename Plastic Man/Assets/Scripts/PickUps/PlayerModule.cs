using UnityEngine;

public enum PlayerModuleType
{
    HEAD,
    BODY,
}

public class PlayerModule : MonoBehaviour
{
    [SerializeField] private PlayerModuleType moduleType;
    [SerializeField] private ModuleData moduleData;
    [SerializeField] private Sprite[] spriteDirections;
    [SerializeField] private SpriteRenderer moduleSprite;
    [SerializeField] private float increaseHealthAmount;

    public PlayerModuleType ModuleType => moduleType;
    public ModuleData ModuleData => moduleData;
    public Sprite IconSprite => moduleData != null ? moduleData.bpSprite : null;

    private void Update()
    {
        HandleSpriteDirection();
    }

    private void HandleSpriteDirection()
    {
        float angle = PlayerLookAt.Instance.angle;

        if (angle >= 337.5f || angle < 22.5f)
            moduleSprite.sprite = spriteDirections[0];
        else if (angle >= 22.5f && angle < 67.5f)
            moduleSprite.sprite = spriteDirections[1];
        else if (angle >= 67.5f && angle < 112.5f)
            moduleSprite.sprite = spriteDirections[2];
        else if (angle >= 112.5f && angle < 157.5f)
            moduleSprite.sprite = spriteDirections[3];
        else if (angle >= 157.5f && angle < 202.5f)
            moduleSprite.sprite = spriteDirections[4];
        else if (angle >= 202.5f && angle < 247.5f)
            moduleSprite.sprite = spriteDirections[5];
        else if (angle >= 247.5f && angle < 292.5f)
            moduleSprite.sprite = spriteDirections[6];
        else if (angle >= 292.5f && angle < 337.5f)
            moduleSprite.sprite = spriteDirections[7];
    }

    public void OnEquip()
    {
        if (PlayerHealth.Instance == null)
            return;

        PlayerHealth.Instance.MaxHealth += increaseHealthAmount;
        PlayerHealth.Instance.CurrentHealth += increaseHealthAmount;
    }

    public void OnUnequip()
    {
        if (PlayerHealth.Instance == null)
            return;

        PlayerHealth.Instance.MaxHealth -= increaseHealthAmount;

        if (PlayerHealth.Instance.CurrentHealth > PlayerHealth.Instance.MaxHealth)
            PlayerHealth.Instance.CurrentHealth = PlayerHealth.Instance.MaxHealth;
    }
}