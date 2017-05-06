using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 플레이어 정보가 저장됨.
/// </summary>
public class Player : MonoBehaviour
{

    public static Player instance;
    public Character info;
    public float totalDamage;
    public float hp;
    public int attackPoint;
    public int combo;
    public int[] skillPoints = new int[3] { 0, 0, 0 };
    UIInGameManager ui;

    public GameObject particle;

    void Awake()
    {
        instance = this;
    }

    // Use this for initialization
    void Start()
    {
        ui = UIInGameManager.instance;

        RecoverHP(0);
        AttackManager(0);

    }

    // Update is called once per frame
    void Update()
    {

    }

    /// <summary>
    /// 플레이어의 체력을 감소합니다.
    /// </summary>
    /// <param name="damage">데미지</param>
    public void DamageHP(int damage)
    {
        hp -= damage;
        SoundManager.instance.PlayAttackSE();

        hp = Mathf.Clamp(hp, 0, info.hp);

        if (!GameManager.instance.isDeath && hp <= 0) {
            print("Death");
            GameManager.instance.GameOver();
            NetworkManager.instance.EmitDeath(PlayerDataManager.instance.my.name);
        }

        ui.UpdateHp(hp, info.hp);
    }

    /// <summary>
    /// 플레이어의 체력을 증가합니다.
    /// </summary>
    /// <param name="chain">체인 수</param>
    public void RecoverHP(int chain)
    {
        hp += info.recover * chain;
        SoundManager.instance.PlayHealSE();

        hp = Mathf.Clamp(hp, 0, info.hp);

        ui.UpdateHp(hp, info.hp);
    }

    /// <summary>
    /// 공격포인트를 관리합니다.
    /// </summary>
    /// <param name="chain">체인 수</param>
    public void AttackManager(int chain)
    {
        attackPoint += chain;

        if (attackPoint - info.requireAttackPoint > 0)
        {
            totalDamage += (int)(chain * info.damage * 0.25f);
        }

        ui.UpdateAttackPoint(attackPoint, info.requireAttackPoint);
        ui.UpdateDamage((int)totalDamage);
    }

    public void SetAddDamage(int v)
    {
        totalDamage += (int)(totalDamage * v * 0.2f);
    }

    public void Attack()
    {
        print("공격 = " + (int)totalDamage);
        SoundManager.instance.PlayAttackSE();
        string name = NetworkManager.instance.EnterRoomGetRandomUser();
        NetworkManager.instance.EmitAttack(PlayerDataManager.instance.my.name, (int)totalDamage, name);
        attackPoint -= info.requireAttackPoint;
        AttackParticle(name);
        ui.UpdateAttackPoint(attackPoint, info.requireAttackPoint);
        ui.UpdateDamage((int)totalDamage);


    }

    void AttackParticle(string other) {
        for (int i = 0; i < NetworkManager.instance.enterRoom.userList.Count; i++)
        {
            if (other == ui.userSlots[i].user.name)
            {
                Transform tr = ui.userSlots[i].transform;
                GameObject g = Instantiate(particle,UIInGameManager.instance.attackPointGage.transform.position,Quaternion.identity);
                g.GetComponent<TargetMove>().SetTarget(tr);
                break;
            }
        }
    }


}
