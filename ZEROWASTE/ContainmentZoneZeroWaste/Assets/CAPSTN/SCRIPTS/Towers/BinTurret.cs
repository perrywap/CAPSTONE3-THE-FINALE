using UnityEngine;

public class BinTurret : TowerBase
{
    // The Update and Shoot logic is handled by TowerBase.
    // We only need to add specific Bin logic here if you want special effects later
    // (e.g., Recycle bonus, slowing enemies).

    // This method runs when you attach this script in the Inspector.
    // It sets default values matching your request: 
    // Fast Speed, Low Damage, Moderate Range.
    private void Reset()
    {
        // High Fire Rate (3 bullets per second)
        bps = 3f;           
        
        // Low Damage
        damage = 3f;        
        
        // Moderate Range
        attackRange = 4f;   
        
        rotationSpeed = 300f; 
    }
    
    // Optional: If you want the Bin Turret to have a specific sound or 
    // particle effect when shooting, you can override Shoot:
    /*
    public override void Shoot()
    {
        base.Shoot();
        // PlaySound("BinBang");
        // SpawnParticles();
    }
    */
}