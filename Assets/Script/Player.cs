using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 플레이어 정보가 저장됨.
/// </summary>
public class Player : MonoBehaviour {

    public static Player instance;
    public Character info;
    public float damage;
    public float addDamage;
    public float hp;
    public int attackPoint;
    public int combo;
    public int[] skillPoints = new int[3] {0,0,0};
    UIInGameManager ui;

    void Awake() {
        instance = this;
    }

	// Use this for initialization
	void Start () {
        ui = UIInGameManager.instance;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    /// <summary>
    /// 플레이어의 체력을 감소합니다.
    /// </summary>
    /// <param name="damage">데미지</param>
    public void DamageHP(int damage)
    {
        hp -= damage * info.Options.SaveDamagePercent;

        hp = Mathf.Clamp(hp,-info.HP, info.HP);

        ui.UpdateHp(hp,info.HP);
    }

    /// <summary>
    /// 플레이어의 체력을 증가합니다.
    /// </summary>
    /// <param name="chain">체인 수</param>
    public void RecoverHP(int chain)
    {
        hp += info.Recover * chain * info.Options.AddRecover;

        hp = Mathf.Clamp(hp, -info.HP, info.HP);

        ui.UpdateHp(hp, info.HP);
    }

    /// <summary>
    /// 공격포인트를 관리합니다.
    /// </summary>
    /// <param name="chain">체인 수</param>
    public void AttackManager(int chain)
    {
        attackPoint += chain;

        if (attackPoint - info.RequireAttackPoint > 0) {
            damage += (int)(chain * info.Damage * 0.25f);
        }

        ui.UpdateAttackPoint(attackPoint,info.RequireAttackPoint);
        ui.UpdateDamage(DamageWork());
    }

    public void SetAddDamage(int v) {
        addDamage += (int)(DamageWork() * v * 0.2f);
    }

    public void Attack() {
        int totalDamage = DamageWork();
        print("공격 = "+damage);
    }

    public int DamageWork() {
        return (int)((info.Damage + damage) + ((info.Damage + damage) * (addDamage + info.Options.AddDamage)));
    }

}
