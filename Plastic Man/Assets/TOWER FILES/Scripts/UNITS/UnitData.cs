using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "NewUnitData", menuName = "Unit Data", order = 0)]

public class UnitData : ScriptableObject
{
    [Header("UNIT STATS")]
    [SerializeField] private string _name;
    [SerializeField] private float _hp;
    [SerializeField] private float _speed;
    [SerializeField] private float _damage;
    [SerializeField] private float _attackRange;
    [SerializeField] private int _manaCost;
    [SerializeField] private int _spawnCount;
    [SerializeField] private float _spawnRate;
    [SerializeField] private UnitType _type;
    

    public Sprite unitSprite;

    public string Name => _name;
    public UnitType Type => _type;
    public float AttackRange => _attackRange;
    public float Hp { get { return _hp; } set { _hp = value; } }
    public float Damage { get => _damage; set => _damage = value; }
    public float Speed { get { return _speed; } set { _speed = value; } }
    public int ManaCost { get { return _manaCost; } set { _manaCost = value; } }
    public int SpawnCount { get { return _spawnCount; } set { _spawnCount = value; } }
    public float SpawnRate { get { return _spawnRate; } set { _spawnRate = value; } }
}
