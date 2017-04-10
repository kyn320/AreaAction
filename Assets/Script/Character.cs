using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 캐릭터 정보 클래스
/// </summary>
[System.Serializable]
public class Character {

    public string Name;
    public int id;
    [TextArea]
    public string Context;

    public Job job;

    public float HP;
    public float Damage;
    public float Recover;

    public int RequireAttackPoint;

    public Skill[] Skills;

    public CharacterOption options;

}

/// <summary>
/// 캐릭터 옵션
/// </summary>
[System.Serializable]
public class CharacterOption {
    public float AddDamage;
    public float AddRecover;
    public float SaveDamagePercent;
    public float CriticalPercent;
    public float SaveAttackPercent;
    public float AddGold;
    public float AddScore;
}

[System.Serializable]
public enum Job {
    Attack,
    Save,
    Balance
}
