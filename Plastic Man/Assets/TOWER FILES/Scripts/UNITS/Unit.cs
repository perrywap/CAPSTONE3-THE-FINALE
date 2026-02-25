using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum UnitType
{
    Normal,
    Tank,
    Runner,
    Flying
}

public enum UnitState
{
    WALKING,
    SEEKING,
    ATTACKING,
    DEAD
}

public class Unit : MonoBehaviour
{
    #region VARIABLES

    [Header("UNIT STATS")]
    [SerializeField] private string _name;
    [SerializeField] private float _hp;
    [SerializeField] private float _speed;
    [SerializeField] private float _damage;
    [SerializeField] private int _manaCost;
    [SerializeField] private int _spawnCount;
    [SerializeField] private UnitType _type;
    [SerializeField] private UnitState state;
    [SerializeField] private UnitData unitData;
    private float _attackRange;
    private UnitCombat combat;
    public AudioClip AttackSound;
    public bool isDead;


    #endregion

    #region GETTERS AND SETTERS

    public string Name => _name;
    public float Hp => _hp;
    public float Speed { get { return _speed; } set { _speed = value; } }
    public float AttackRange => _attackRange; 
    public int ManaCost => _manaCost;
    public int SpawnCount => _spawnCount;
    public UnitType Type => _type;
    public float Damage { get => _damage; set => _damage = value; }
    public UnitState State { get { return state; } set { state = value; } }
    public UnitData Data => unitData;

    #endregion

    #region UNITY METHODS

    private void Start()
    {
        combat = GetComponent<UnitCombat>();

        _name = unitData.Name;
        _hp = unitData.Hp;
        _speed = unitData.Speed;
        _damage = unitData.Damage;
        _attackRange = unitData.AttackRange;
        _manaCost = unitData.ManaCost;
        _spawnCount = unitData.SpawnCount;
        _type = unitData.Type;

        isDead = false;
    }

    #endregion

    #region METHODS
    
    public void OnPathComplete()
    {
        Debug.Log($"{gameObject.name} reached the end!");
        Destroy(gameObject);
    }

    public virtual void TakeDamage(float damage)
    {
        _hp -= damage;
        this.gameObject.GetComponentInChildren<HpBar>().PopHpBar();
        if (_hp <= 0)
        {
            _hp = 0;
            isDead = true;
            state = UnitState.DEAD;
        }
    }

    public virtual void Die()
    {
        if (!isDead)
            return;

        GameManager.Instance.unitsOnField.Remove(this.gameObject);
        Destroy(gameObject);
    }

    #endregion
}
