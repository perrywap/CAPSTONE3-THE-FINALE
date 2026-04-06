using UnityEngine;

public class PlayerModule : MonoBehaviour
{
    [Header("Armor Stats")]
    [SerializeField] private float _bonusHealth = 15f;
    public float BonusHealth => _bonusHealth;

    [SerializeField] private Sprite[] spriteDirections;
    [SerializeField] private SpriteRenderer moduleSprite;

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
}