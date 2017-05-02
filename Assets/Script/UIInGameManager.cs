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

    //rank
    public Text rank;

    //Score
    public Text scoreText;

    //HP
    public Image hpGage;
    public Text hpCurrentValue, hpMaxValue;

    //AttackPoint
    public Image attackPointGage;
    public Text attackPointCurrentValue;
    public Text damageValue;

    //popUp
    public GameObject popUp;

    //Illust
    public Image illustImage;

    //SkillButton
    public Image[] skillImages;

    //ranking
    public Text[] rankTexts;

    //chat
    public int cnt = 0;
    public Text chatLog;
    public InputField chatInput;

    //wait 
    public GameObject waitPannel;

    //end
    public GameObject endPannel;
    public Text endText;

    int oldRank = 1;

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
        timer.text = string.Format("{0} : {1:D2}", (int)(time / 60), (int)(time % 60));
    }


    public void UpdateRank(int rankNumber)
    {
        if (oldRank != rankNumber)
        {
            oldRank = rankNumber;
            rank.GetComponent<Animator>().SetTrigger("Change");
            rank.text = rankNumber.ToString();
            SoundManager.instance.PlayUISE();
        }
    }

    public void EndGame()
    {
        endPannel.SetActive(true);
        endText.text = "게임 결과!\n 순위 :" + oldRank;
    }

    /// <summary>
    /// 스코어를 업데이트 하는 함수 
    /// </summary>
    /// <param name="score">스코어</param>
    public void UpdateScore(int score)
    {
        scoreText.text = score.ToString();
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

        Destroy(g,1F);
    }

    public void UpdateWaitPannel()
    {
        waitPannel.SetActive(false);
    }

    /// <summary>
    /// Illust를 업데이트 하는 함수
    /// </summary>
    /// <param name="sprite">일러스트 이미지</param>
    public void UpdateIllust(Sprite sprite)
    {
        illustImage.sprite = sprite;
    }

    /// <summary>
    /// Skill Icon들을 업데이트 하는 함수
    /// </summary>
    /// <param name="icons">sprite[3] 의 아이콘</param>
    public void UpdateSkillIcon(Sprite[] icons)
    {
        for (int i = 0; i < 3; i++)
        {
            skillImages[i].sprite = icons[i];
        }
    }


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
            rankTexts[i].text = room.userList[i].name + " : " + room.userList[i].score;
            if (room.userList[i].name == PlayerDataManager.instance.my.name)
            {
                UpdateRank((i + 1));
            }
        }

    }

    public void ChatSend()
    {
        NetworkManager.instance.EmitChat(PlayerDataManager.instance.my.name, chatInput.text);
        chatInput.text = "";
    }

    public void UpdateChatLog(string name, string message)
    {
        if (cnt >= 2)
        {
            cnt = 0;
            chatLog.text = "";
        }
        chatLog.text += name + " : \n" + message + "\n";
        cnt++;
    }

    public void ExitGame()
    {
        NetworkManager.instance.EmitExitRoom();
    }




}