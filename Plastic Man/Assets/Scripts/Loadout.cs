using System.Runtime.InteropServices;
using UnityEngine;

public class Loadout : MonoBehaviour
{
    public static Loadout Instance { get; private set; }

    #region VARIABLES
    [Header("PLAYER MODULES")]
    [SerializeField] private GameObject headModule;
    [SerializeField] private GameObject bodyModule;
    #endregion


    private void Awake()
    {
        Instance = this;
    }

    public void InitializeLoadout(GameObject _headModule, GameObject _bodyModule)
    {
        if (headModule != null)
            Destroy(headModule);
        if (bodyModule != null)
            Destroy(headModule);

        headModule = _headModule;
        bodyModule = _bodyModule;

    }
}
