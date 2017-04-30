using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BoardManager : MonoBehaviour
{

    public static BoardManager instance;

    UIInGameManager uiManager;
    GameManager gameManager;
    Player player;

    public Vector2 startPos = new Vector2(-2f, 2f);
    public Vector2 Startmargin = new Vector2(0.5f, -0.82f);
    public Vector2 margin = new Vector2(-1f, -0.82f);

    public GameObject tile;

    public List<Tile> tileList;

    public List<int> selectTileList;
    public Queue<int> loopTile = new Queue<int>();

    public float hp = 9999, maxHp = 9999;
    public float atkPoint = 0, atkMaxPoint = 10;
    public int chain;


    void Awake()
    {
        //싱글톤 지정
        instance = this;
        //타일 생성 함수
    }

    void Start()
    {
        uiManager = UIInGameManager.instance;
        player = Player.instance;
        gameManager = GameManager.instance;

        CreatHexTile();

        StartCoroutine("ReadyForSecondes");
    }

    IEnumerator ReadyForSecondes() {
        yield return new WaitForSeconds(0.1f);
        NetworkManager.instance.EmitReady();
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
                    t.SetTile(cnt, KindRandomBalance());
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

    public int KindRandomBalance()
    {
        int rand = Random.Range(0, 101);

        if (0 <= rand && rand <= 100)
        {
            int scoreRand = Random.Range(1, 6);
            return scoreRand;
        }
        else if (80 <= rand && rand < 90)
        {
            int actionRand = Random.Range(0,20);
            if (player.info.job == Job.Attack)
            {
                if (0 <= actionRand && actionRand <= 12)
                    return 6;
                else
                    return 7;
            }
            else if (player.info.job == Job.Save)
            {
                if (0 <= actionRand && actionRand <= 12)
                    return 7;
                else
                    return 6;
            }
            else {
                if (0 <= actionRand && actionRand <= 10)
                    return 6;
                else
                    return 7;
            }
        }
        else if (90 <= rand && rand < 99)
        {
            int SkillRand = Random.Range(0, 16);
            if (0 <= SkillRand && SkillRand < 8)
                return 8;
            else if (8 <= SkillRand && SkillRand <= 13)
                return 9;
            else
                return 10;
        }
        else {
            return Random.Range(11, 18);
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

    public void PrintResult()
    {
        if(selectTileList.Count > 0)
        Debug.Log("총 " + (selectTileList.Count) + "개의 "+tileList[selectTileList[0] - 1].kind+" 블럭이 선택되었습니다.");
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
            if (selectTileList.Count > 0)
            {
                PrintResult();
                player.combo++;
                chain = selectTileList.Count;
                SoundManager.instance.PlayComboVoice(player.combo);
                uiManager.UpdateCombo(player.combo);
                uiManager.UpdateChain(chain);
                KindWork(tileList[selectTileList[0] - 1].kind, selectTileList.Count);
                ResetKind();
                selectTileList.Clear();
            }
            else {
                player.combo = 0;
            }
        }
    }

    public void KindWork(int kind, int value)
    {
        switch (kind)
        {
            //==== 스코어 블럭
            case 1:
                gameManager.AddScore(chain, player.combo, 1);
                break;
            case 2:
                gameManager.AddScore(chain, player.combo, 2);
                break;
            case 3:
                gameManager.AddScore(chain, player.combo, 3);
                break;
            case 4:
                gameManager.AddScore(chain, player.combo, 4);
                break;
            case 5:
                gameManager.AddScore(chain, player.combo, 5);
                break;
            //==== 공격 포인트 블럭
            case 6:
                player.AttackManager(chain);
                break;
            //==== 회복 포인트 블럭
            case 7:
                player.RecoverHP(chain);
                break;
            //==== 스킬 1 포인트 블럭
            case 8:
                break;
            //==== 스킬 2 포인트 블럭
            case 9:
                break;
            //==== 스킬 3 포인트 블럭
            case 10:
                break;
            //==== 공격력 증가
            case 11:
                player.SetAddDamage(1);
                break;
            //==== 공격력 감소
            case 12:
                player.SetAddDamage(-1);
                break;
            //==== x초 동안 점수 +2배
            case 13:
                break;
            //==== 콤보 유지
            case 14:
                break;
            //==== 랜덤 1 종류 스코어 블럭 파괴
            case 15:
                break;
            //==== x초 동안 크리티컬 확률 100%
            case 16:
                break;
            //==== 골드 획득
            case 17:
                break;
        }
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
            tileList[selectTileList[i] - 1].ChangeKind();
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

    public void EndGame() {
        for (int i = 0; i < tileList.Count; i++) {
            tileList[i].Del();
        }
    }

}
