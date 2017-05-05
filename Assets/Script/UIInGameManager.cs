using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 인게임 UI 요소를 모두 관리하는 클래스 입니다.
/// </summary>
public class UIInGameManager : MonoBehaviour
{

    public Camera cam;

    //싱글톤 
    public static UIInGameManager instance;

    public GameObject canvas;

    //Timer
    public Text timer;

    //userSlot
    public UIUserSlot[] userSlots;

    //HP
    public Image hpGage;
    public Text hpCurrentValue, hpMaxValue;

    //AttackPoint
    public Image attackPointGage;
    public Text attackPointCurrentValue;
    public Text damageValue;

    //popUp
    public GameObject popUp;

    //SkillButton
    public Image skillImage;

    //chat
    public InputField chatInput;

    //wait 
    public GameObject waitPannel;

    //end
    public GameObject endPannel;
    public Text endText;

    //Notice
    public GameObject noticeObj;
    public Text noticeText;

    bool isNotice = false;

    void Awake()
    {
        //싱글톤 생성
        instance = this;
    }


    /// <summary>
    /// 타이머를 업데이트 하는 함수
    /// </summary>
    /// <param name="time">시간 초</param>
    public void UpdateTimer(float time)
    {
        timer.text = string.Format("{0:D2} : {1:D2}", (int)(time / 60), (int)(time % 60));
    }

    /// <summary>
    /// 최종 결과를 표시
    /// </summary>
    public void EndGame()
    {
        endPannel.SetActive(true);
        endText.text = "게임 결과!\n 순위 :" + PlayerDataManager.instance.my.rank;
    }

    /// <summary>
    /// HP를 업데이트 하는 함수
    /// </summary>
    /// <param name="hp">현재 hp</param>
    /// <param name="maxHp">최대 hp</param>
    public void UpdateHp(float hp, float maxHp)
    {
        hpCurrentValue.text = hp.ToString();
        hpMaxValue.text = maxHp.ToString();
        // 게이지 효과
        hpGage.fillAmount = hp / maxHp;
        NetworkManager.instance.EmitHpChange(PlayerDataManager.instance.my.name, (int)hp, (int)maxHp);
    }

    /// <summary>
    /// Damage를 업데이트 하는 함수
    /// </summary>
    /// <param name="damage">데미지</param>
    public void UpdateDamage(int damage)
    {
        damageValue.text = damage.ToString();
    }

    /// <summary>
    /// attackPoint를 업데이트 하는 함수
    /// </summary>
    /// <param name="attackPoint">현재 point</param>
    /// <param name="maxAttackPoint">최대 point</param>
    public void UpdateAttackPoint(float attackPoint, float maxAttackPoint)
    {
        attackPointCurrentValue.text = attackPoint.ToString() + "/" + maxAttackPoint.ToString();
        //게이지 효과
        attackPointGage.fillAmount = attackPoint / maxAttackPoint;

        if (attackPoint >= maxAttackPoint)
            attackPointGage.GetComponent<Button>().interactable = true;
        else
            attackPointGage.GetComponent<Button>().interactable = false;

    }

    /// <summary>
    /// Combo 와 Chain을 역동적으로 표현
    /// </summary>
    /// <param name="combo">Combo</param>
    /// <param name="chain">Chain</param>
    public void MakePopUp(int combo, int chain, Transform pos)
    {
        GameObject g = Instantiate(popUp);
        RectTransform tr = g.GetComponent<RectTransform>();

        Vector3 mainPos = Camera.main.WorldToScreenPoint(pos.position);
        Vector3 uiPos = cam.ScreenToWorldPoint(mainPos);

        tr.SetParent(canvas.transform, false);
        tr.position = uiPos;
        tr.GetChild(0).GetChild(0).GetComponent<Text>().text = "Combo x " + combo + "\nChain x " + chain;

        Destroy(g, 1F);
    }

    /// <summary>
    /// 대기 상태를 헤제합니다.
    /// </summary>
    public void UpdateWaitPannel()
    {
        waitPannel.SetActive(false);
    }

    /// <summary>
    /// Skill Icon을 업데이트 하는 함수
    /// </summary>
    /// <param name="icons">sprite[3] 의 아이콘</param>
    public void UpdateSkillIcon(Sprite icon)
    {

    }

    /// <summary>
    /// 유저의 순위를 업데이트 합니다.
    /// </summary>
    /// <param name="room">방의 정보</param>
    public void UpdateUserRanking(Room room)
    {
        room.userList.Sort(delegate (User a, User b)
        {
            if (a.score > b.score) return -1;
            else if (a.score < b.score) return 1;
            return 0;
        });

        for (int i = 0; i < room.userList.Count; i++)
        {
            room.userList[i].rank = (i + 1);
            for (int j = 0; j < userSlots.Length; j++)
            {
                if (room.userList[i] == userSlots[j].user)
                {
                    userSlots[j].UpdateRankText();
                    userSlots[j].UpdateScore();
                    break;
                }
            }
        }
    }

    /// <summary>
    /// 채팅을 보냅니다.
    /// </summary>
    public void ChatSend()
    {
        if (chatInput.text != "")
        {
            NetworkManager.instance.EmitChat(PlayerDataManager.instance.my.name, chatInput.text);
            chatInput.text = "";
        }
    }

    /// <summary>
    /// 채팅을 띄웁니다.
    /// </summary>
    /// <param name="name"></param>
    /// <param name="message"></param>
    public void UpdateChatLog(int count, string name, string message)
    {
        for (int i = 0; i < count; i++)
        {
            if (name == userSlots[i].user.name)
            {
                userSlots[i].UpdateChat(message);
                break;
            }
        }
    }

    public void UpdateUserSlotHpChange(int count, string name, int hp, int maxHp)
    {
        for (int i = 0; i < count; i++)
        {
            if (name == userSlots[i].user.name)
            {
                userSlots[i].UpdateHp(hp, maxHp);
                break;
            }
        }
    }

    /// <summary>
    /// 게임에서 나갑니다.
    /// </summary>
    public void ExitGame()
    {
        NetworkManager.instance.EmitExitRoom();
    }

    public void UpdateNotice(string message) {
        if (isNotice)
            StopCoroutine("WaitNotice");

        noticeObj.SetActive(true);
        noticeText.text = message;
        StartCoroutine("WaitNotice");
    }

    IEnumerator WaitNotice() {
        isNotice = true;
        yield return new WaitForSeconds(1f);
        isNotice = false;
        noticeText.text = "";
        noticeObj.SetActive(false);
    }


}