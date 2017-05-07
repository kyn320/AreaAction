using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class UIUserSlot : MonoBehaviour
{

    public User user;
    public Image illustImage;
    public Text nickNameText, scoreText;
    public Text rankText;
    public GameObject chatBox;
    public Text chatLog;

    public Slider hpGage;
    public Text hpText;

    bool isChatView = false;

    public Sprite[] illust;

    int oldRank = 0;

    public void SetInfo(User user)
    {
        this.user = user;
        illustImage.sprite = illust[user.characterID];
        illustImage.SetNativeSize();
        nickNameText.text = user.name;
        scoreText.text = "0";
        rankText.text = "0";
    }

    public void UpdateScore()
    {
        scoreText.text = user.score.ToString();
    }

    public void UpdateRankText()
    {
        if (oldRank != user.rank)
        {
            oldRank = user.rank;
            if (PlayerDataManager.instance.my.name == user.name)
                PlayerDataManager.instance.my.rank = oldRank;
            rankText.text = user.rank.ToString();
            SoundManager.instance.PlayUISE();
        }
    }

    public void UpdateChat(string message)
    {
        chatLog.text = message;
        if (isChatView)
            StopCoroutine("ChatAcitve");

        StartCoroutine("ChatAcitve");
    }

    /// <summary>
    /// HP를 업데이트 하는 함수
    /// </summary>
    /// <param name="hp">현재 hp</param>
    /// <param name="maxHp">최대 hp</param>
    public void UpdateHp(float hp, float maxHp)
    {
        hpText.text = hp.ToString() + " / " + maxHp.ToString();
        // 게이지 효과
        hpGage.value = hp / maxHp;
    }


    IEnumerator ChatAcitve()
    {
        isChatView = true;
        chatBox.SetActive(true);
        yield return new WaitForSeconds(4f);
        isChatView = false;
        chatBox.SetActive(false);
    }

    public void AttackParticlePlayer() {
        GameObject g = Instantiate(Player.instance.particle, transform.position + (Vector3.up * 5f), Quaternion.identity);
        g.GetComponent<TargetMove>().SetTarget(UIInGameManager.instance.hpGage.transform.position);
    }


}
