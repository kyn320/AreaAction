﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 인게임을 관리하는 매니저 입니다.
/// </summary>
public class GameManager : MonoBehaviour
{

    public static GameManager instance;

    BoardManager boardManager;
    UIInGameManager uiManager;
    Player player;

    public bool isplayed = false;
    public bool isDeath = false;

    public float time = 90;
    public int baseScore = 100;

    public int score = 0;
    public int isAlive = 10;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        uiManager = UIInGameManager.instance;
        player = Player.instance;
        boardManager = BoardManager.instance;
    }

    void Update()
    {
        if (isplayed)
            Timer();
    }

    /// <summary>
    /// 타이머 함수
    /// </summary>
    void Timer()
    {
        time -= Time.deltaTime;
        uiManager.UpdateTimer(time);
        if (time < 0)
        {
            time = 10;
            GameOver();
            GameEnd();
        }
    }

    public void GameOver()
    {
        if (!isDeath)
        {
            BoardManager.instance.EndGame();
            isDeath = true;
        }
    }

    public void GameEnd()
    {
        isplayed = false;
        uiManager.EndGame();
    }

    /// <summary>
    /// 스코어를 획득하는 함수
    /// </summary>
    /// <param name="chain">체인 수</param>
    /// <param name="combo">콤보 수</param>
    /// <param name="multiple">타일의 배수</param>
    public void AddScore(int chain, int combo, int multiple)
    {
        score += chain * baseScore * combo * multiple;
        NetworkManager.instance.EmitScore(PlayerDataManager.instance.my.name, score);
    }

    public void DownIsAlive()
    {
        --isAlive;

        if (isAlive < 2)
        {
            if (isplayed)
                GameOver();
            
            GameEnd();
        }
    }
}
