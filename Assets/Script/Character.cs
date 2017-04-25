using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 캐릭터 정보 클래스
/// </summary>
[System.Serializable]
public class Character {

    public string name;
    public int id;
    [TextArea]
    public string context;

    public Job job;

    public float hp;
    public float damage;
    public float recover;

    public int requireAttackPoint;

    public Skill[] skills = new Skill[3];

    public CharacterOption options;

    public Sprite illust;
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
