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
        if (cnt == 0)
        {
            board.combo = 0;
            board.chain = 0;
            board.SetUI();
        }
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
            rand = UnityEngine.Random.Range(0, 5);
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
            case 0:
                spr.color = Color.red;
                break;
            case 1:
                spr.color = Color.blue;
                break;
            case 2:
                spr.color = Color.green;
                break;
            case 3:
                spr.color = Color.yellow;
                break;
            case 4:
                spr.color = Color.cyan;
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
