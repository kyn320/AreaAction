using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 인게임을 관리하는 매니저 입니다.
/// </summary>
public class GameManager : MonoBehaviour {

    public static GameManager instance;

    BoardManager boardManager;
    UIInGameManager uiManager;
    Player player;


    public float time = 90;
    public int baseScore = 100;

    public int score = 0;

    void Awake() {
        instance = this;
    }

    void Start() {
        uiManager = UIInGameManager.instance;
        player = Player.instance;
        boardManager = BoardManager.instance;
    }

    void Update() {
        Timer();
    }

    /// <summary>
    /// 타이머 함수
    /// </summary>
    void Timer() {
        time -= Time.deltaTime;
        uiManager.UpdateTimer(time);
        if (time < 0)
            time = 200;
    }

    /// <summary>
    /// 스코어를 획득하는 함수
    /// </summary>
    /// <param name="chain">체인 수</param>
    /// <param name="combo">콤보 수</param>
    /// <param name="multiple">타일의 배수</param>
    public void AddScore(int chain, int combo, int multiple) {
        score += chain * baseScore * combo * multiple;
        uiManager.UpdateScore(score);
    }


}
