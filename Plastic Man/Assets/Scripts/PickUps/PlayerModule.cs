using UnityEngine;

public enum PlayerModuleType
{
    HEAD,
    BODY,
}

public class PlayerModule : Module
{
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
            moduleSprite.sprite = spriteDirections[0]; // Right
        else if (angle >= 22.5f && angle < 67.5f)
            moduleSprite.sprite = spriteDirections[1]; // UpRight
        else if (angle >= 67.5f && angle < 112.5f)
            moduleSprite.sprite = spriteDirections[2]; // Up
        else if (angle >= 112.5f && angle < 157.5f)
            moduleSprite.sprite = spriteDirections[3]; // UpLeft
        else if (angle >= 157.5f && angle < 202.5f)
            moduleSprite.sprite = spriteDirections[4]; // Left
        else if (angle >= 202.5f && angle < 247.5f)
            moduleSprite.sprite = spriteDirections[5]; // DownLeft
        else if (angle >= 247.5f && angle < 292.5f)
            moduleSprite.sprite = spriteDirections[6]; // Down
        else if (angle >= 292.5f && angle < 337.5f)
            moduleSprite.sprite = spriteDirections[7]; // DownRight
    }
}
