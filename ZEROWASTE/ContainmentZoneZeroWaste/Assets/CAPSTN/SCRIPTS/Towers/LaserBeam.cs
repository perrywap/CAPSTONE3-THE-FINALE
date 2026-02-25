using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(LineRenderer))]
public class LaserBeam : MonoBehaviour
{
    [SerializeField] private float duration = 0.25f; // How long the beam stays visible
    private LineRenderer lineRenderer;

    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
    }

    public void Fire(Vector2 startPos, Vector2 direction, float maxDistance, float width, float damage, LayerMask hitLayer)
    {
        // 1. VISUALS
        // Set the width of the laser line
        lineRenderer.startWidth = width;
        lineRenderer.endWidth = width;

        // Calculate end position (The beam goes full distance!)
        Vector2 endPos = startPos + (direction * maxDistance);
        
        lineRenderer.SetPosition(0, startPos);
        lineRenderer.SetPosition(1, endPos);

        // 2. HIT DETECTION (Shredding)
        // We use CircleCastAll to simulate a thick beam ("Capsule shape") hitting multiple enemies
        // Radius = width / 2
        RaycastHit2D[] hits = Physics2D.CircleCastAll(startPos, width / 2f, direction, maxDistance, hitLayer);

        // Use a Set to ensure we don't damage the same enemy twice (if it has multiple colliders)
        HashSet<GameObject> hitEnemies = new HashSet<GameObject>();

        foreach (RaycastHit2D hit in hits)
        {
            // Try to get EnemyBase (could be on parent or self)
            EnemyBase enemy = hit.collider.GetComponent<EnemyBase>();
            if (enemy == null) enemy = hit.collider.GetComponentInParent<EnemyBase>();

            if (enemy != null && !hitEnemies.Contains(enemy.gameObject))
            {
                hitEnemies.Add(enemy.gameObject);

                // Find HP Bar and Deal Damage
                HpBarComponent hpBar = enemy.GetComponentInChildren<HpBarComponent>(true);
                if (hpBar != null)
                {
                    hpBar.TakeDamage(damage);
                }
            }
        }

        // 3. CLEANUP
        StartCoroutine(FadeAndDestroy());
    }

    private IEnumerator FadeAndDestroy()
    {
        // Wait for the duration, then destroy the laser effect
        yield return new WaitForSeconds(duration);
        Destroy(gameObject);
    }
}