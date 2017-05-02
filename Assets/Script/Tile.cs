using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileInfo
{
    public enum TileKind
    {
        Score
        , Attack
        , Recover
        , Skill
        , Item
    }

    public TileKind kind;

    public enum ScoreMultiply {
        Zero
        , One
        , Two
        , Three
        , Four
        , Five
    }

    public ScoreMultiply scoreMultiply;

    public enum SkillKind {
        Zero
        , One
        , Two
        , Three
    }

    public SkillKind skillKind;

    public enum ItemKind {
        AddAttackNumber
        , AddAttackPercent
        , AddRecoverNumber
        , AddRecoverPercent
        , ComboProtect
        , BlockDestory
        , AddScorePercentForSeconds
        , AddCriticalPercentForSeconds
        , AddGold
    }

    public ItemKind[] itemKind;

}



public class Tile : MonoBehaviour
{

    public static BoardManager board;

    SpriteRenderer spr;

    public int id = 0, kind = 0;

    public List<Tile> node;

    public AudioClip[] se;

    public Sprite[] skin;

    public GameObject particles;

    Animator ani;
    AudioSource audio;

    Color color = new Color();

    public static void SetBoard(BoardManager b)
    {
        board = b;
    }

    public void Del() {
        Destroy(this.gameObject);
    }

    void Awake()
    {
        ani = GetComponent<Animator>();
        spr = GetComponent<SpriteRenderer>();
        audio = GetComponent<AudioSource>();
    }

    public void SetTile(int _id, int _kind)
    {
        id = _id;
        kind = _kind;
        ColorChange(kind);
        spr.sprite = skin[0];
    }

    public void SetNode()
    {
        GameObject[] max = GameObject.FindGameObjectsWithTag("Tile");
        for (int i = 0; i < max.Length; i++)
        {
            if (max[i] != this.gameObject && Vector2.Distance(transform.position, max[i].transform.position) <= 1.12f)
            {
                node.Add(max[i].GetComponent<Tile>());
            }
        }
    }

    void RootCheckNode()
    {
        int cnt = 0;
        board.AddQueue();
        if (kind < 8)
        {
            for (int i = 0; i < node.Count; i++)
            {
                if (kind == node[i].kind && board.FindTileID(node[i].id))
                {
                    cnt++;
                    node[i].CheckNode();
                }
            }
            if (cnt > 0)
            {
                board.AddSelectList(id);
                board.rootTransform = this.transform;
            }
        }
        else {
            board.AddSelectList(id);
            board.rootTransform = this.transform;
        }

        board.RemoveQueue();
    }

    //root 진입
    void CheckNode()
    {
        board.AddSelectList(id);
        int cnt = 0;
        board.AddQueue();
        for (int i = 0; i < node.Count; i++)
        {
            if (kind == node[i].kind && board.FindTileID(node[i].id))
            {
                cnt++;
                node[i].CheckNode();
            }
        }
        board.RemoveQueue();
    }


    public void ChangeKind()
    {
        int rand = -1;
        do
        {
            rand = board.KindRandomBalance();
        } while (kind == rand);
        kind = rand;

        ani.SetTrigger("Create");

        GameObject temp = Instantiate(particles, transform.position + Vector3.back, Quaternion.identity);


        for (int i = 0; i < temp.transform.childCount; i++)
        {
            temp.transform.GetChild(i).GetComponent<SpriteRenderer>().color = color;
        }

        Destroy(temp, 1f);

        audio.PlayOneShot(se[0], 0.2f);
    }

    public void AnimationColors()
    {
        ColorChange(kind);
        audio.PlayOneShot(se[1], 0.2f);
    }

    void ColorChange(int i)
    {

        switch (i)
        {
            //==== 스코어 블럭
            case 1:
                ColorUtility.TryParseHtmlString("#FF4646", out color);
                break;
            case 2:
                ColorUtility.TryParseHtmlString("#ff7f00", out color);
                break;
            case 3:
                ColorUtility.TryParseHtmlString("#80E12A", out color);
                break;
            case 4:
                ColorUtility.TryParseHtmlString("#00A5FF", out color);
                break;
            case 5:
                ColorUtility.TryParseHtmlString("#7D00FF", out color);
                break;
            //==== 공격 포인트 블럭
            case 6:
                ColorUtility.TryParseHtmlString("#703800", out color);
                break;
            //==== 회복 포인트 블럭
            case 7:
                ColorUtility.TryParseHtmlString("#FF00C3", out color);
                break;
            //==== 스킬 1 포인트 블럭
            case 8:
            //==== 스킬 2 포인트 블럭
            case 9:
            //==== 스킬 3 포인트 블럭
            case 10:
                ColorUtility.TryParseHtmlString("#FFFFFF", out color);
                break;
            //==== 공격력 증가
            case 11:
            //==== 공격력 감소
            case 12:
            //==== x초 동안 점수 +2배
            case 13:
            //==== 콤보 유지
            case 14:
            //==== 랜덤 1 종류 스코어 블럭 파괴
            case 15:
            //==== x초 동안 크리티컬 확률 100%
            case 16:
                ColorUtility.TryParseHtmlString("#454647", out color);
                break;
            //==== 골드 획득
            case 17:
                ColorUtility.TryParseHtmlString("#FFC31E", out color);
                break;
        }

        spr.color = color;

    }

    public void DebugColor()
    {
        spr.color = Color.black;
    }

    public void OnMouseDown()
    {
        RootCheckNode();
    }
}