using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BoardManager : MonoBehaviour
{

    // x >> + 0.5 // y >> -0.83

    public static BoardManager instance;

    public Vector2 startPos = new Vector2(-2f, 2f);
    public Vector2 Startmargin = new Vector2(0.5f, -0.82f);
    public Vector2 margin = new Vector2(-1f, -0.82f);

    public GameObject tile;

    public List<Tile> tileList;

    public List<int> selectTileList;
    public Queue<int> loopTile = new Queue<int>();

    public int combo, chain, score;

    public GameObject comboObject;
    public Text scoreText;

    public void PrintResult()
    {
        Debug.Log("총 " + (selectTileList.Count) + "개의 블럭이 선택되었습니다.");
    }

    public void AddQueue()
    {
        loopTile.Enqueue(loopTile.Count + 1);
    }

    public void RemoveQueue()
    {
        loopTile.Dequeue();
        if (loopTile.Count < 1)
        {
            PrintResult();
            ++combo;
            chain = selectTileList.Count;
            score += chain * combo;
            SetUI();
            ResetKind();
            selectTileList.Clear();
        }
    }

    public void SetUI() {
        comboObject.transform.GetChild(0).GetComponent<Text>().text = "Combo" + combo + " !";
        comboObject.transform.GetChild(1).GetComponent<Text>().text = "Chain x " + chain;
        comboObject.GetComponent<Animator>().SetTrigger("Combo");
        scoreText.text = score.ToString();
    }

    public void AddSelectList(int id)
    {
        if (FindTileID(id))
        {
            selectTileList.Add(id);
        }
    }

    public void ResetKind()
    {
        for (int i = 0; i < selectTileList.Count; i++)
        {
             tileList[selectTileList[i]-1].ChangeKind();
        }
    }

    public bool FindTileID(int id)
    {
        for (int i = 0; i < selectTileList.Count; i++)
        {
            if (id == selectTileList[i])
            {
                return false;
            }
        }
        return true;
    }

    void Awake()
    {
        //싱글톤 지정
        instance = this;
        //타일 생성 함수
        CreatHexTile();
    }

    void CreatHexTile()
    {
        //변수를 캐싱
        GameObject g;
        Transform tr;
        Tile t;
        int cnt = 0;
        //총 9 줄을 만듦.
        for (int i = 0; i < 9; i++)
        {
            //각 줄의 타일 갯수를 받습니다.
            int tilecount = SelectLineCnt(i + 1);

            if (tilecount != 0)
            {
                for (int j = 0; j < tilecount; j++)
                {
                    //타일 오브젝트 생성
                    g = Instantiate(tile);
                    //Transform 캐싱
                    tr = g.transform;
                    //부모 지정
                    tr.parent = this.transform;
                    //좌표 지정
                    tr.localPosition = (startPos + new Vector2(Startmargin.x * -(5 - tilecount), 0)) + new Vector2(margin.x * j, margin.y * i);
                    //임시 오브젝트 대입. Tile Class List 로 변경 및 Add
                    t = g.GetComponent<Tile>();
                    ++cnt;
                    t.SetTile(cnt, Random.Range(0, 5));
                    tileList.Add(t);

                }
            }
        }
        Tile.SetBoard(this);
        for (int k = 0; k < tileList.Count; k++)
        {
            tileList[k].SetNode();
        }
    }

    int SelectLineCnt(int line)
    {
        switch (line)
        {
            case 1:
                return 5;
            case 2:
                return 6;
            case 3:
                return 7;
            case 4:
                return 6;
            case 5:
                return 7;
            case 6:
                return 6;
            case 7:
                return 7;
            case 8:
                return 6;
            case 9:
                return 5;
            default:
                Debug.LogError("지정되지 않은 Line을 Return 할 수 없습니다.");
                return 0;
        }
    }

}
