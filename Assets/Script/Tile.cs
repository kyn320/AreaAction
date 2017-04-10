using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{

    public static BoardManager board;

    SpriteRenderer spr;

    public int id = 0, kind = 0;


    public List<Tile> node;

    public AudioClip[] se;

    public Sprite[] skin;

    Animator ani;
    AudioSource audio;

    public static void SetBoard(BoardManager b)
    {
        board = b;
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
        spr.sprite = skin[3];
    }

    public void SetNode()
    {
        GameObject[] max = GameObject.FindGameObjectsWithTag("Tile");
        for (int i = 0; i < max.Length; i++)
        {
            if (max[i] != this.gameObject && Vector2.Distance(transform.position, max[i].transform.position) <= 1)
            {
                node.Add(max[i].GetComponent<Tile>());
            }
        }
    }

    void RootCheckNode() {
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
        if (cnt > 0)
        {
            board.AddSelectList(id);
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
        audio.PlayOneShot(se[0],0.2f);
    }

    public void AnimationColors() {
        ColorChange(kind);
        audio.PlayOneShot(se[1],0.2f);
    }

    void ColorChange(int i)
    {
        switch (i)
        {
            //==== 스코어 블럭
            case 1:
                spr.color = Color.black;
                break;
            case 2:
                spr.color = Color.blue;
                break;
            case 3:
                spr.color = Color.cyan;
                break;
            case 4:
                spr.color = Color.gray;
                break;
            case 5:
                spr.color = Color.green;
                break;
            //==== 공격 포인트 블럭
            case 6:
                spr.color = Color.grey;
                break;
            //==== 회복 포인트 블럭
            case 7:
                spr.color = Color.magenta;
                break;
            //==== 스킬 1 포인트 블럭
            case 8:
                spr.color = Color.red;
                break;
            //==== 스킬 2 포인트 블럭
            case 9:
                spr.color = Color.white;
                break;
            //==== 스킬 3 포인트 블럭
            case 10:
                spr.color = Color.yellow;
                break;
            //==== 공격력 증가
            case 11:
                spr.color = new Color(0.1f, 0.3f, 0.1f,1f);
                break;
            //==== 공격력 감소
            case 12:
                spr.color = new Color(0.3f, 0.7f, 0.3f, 0.5f);
                break;
            //==== x초 동안 점수 +2배
            case 13:
                spr.color = new Color(1f, 0.9f, 0.5f, 1f);
                break;
            //==== 콤보 유지
            case 14:
                spr.color = new Color(0.7f, 0.7f, 1f, 1f);
                break;
            //==== 랜덤 1 종류 스코어 블럭 파괴
            case 15:
                spr.color = new Color(0.9f, 0.5f, 0.9f, 1f);
                break;
            //==== x초 동안 크리티컬 확률 100%
            case 16:
                spr.color = new Color(0.7f, 1f, 0.7f, 1f);
                break;
            //==== 골드 획득
            case 17:
                spr.color = new Color(0.5f, 0.1f, 0.5f, 0.5f);
                break;
        }
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
