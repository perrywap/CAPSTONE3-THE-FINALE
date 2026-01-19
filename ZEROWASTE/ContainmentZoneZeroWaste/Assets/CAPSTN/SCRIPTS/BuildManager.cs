using UnityEngine;

public class BuildManager : MonoBehaviour
{
    public static BuildManager Instance;
    [Header("References")]
    [SerializeField] private GameObject[] towerPrefabs;

    private int selectedTower = 0;
}
