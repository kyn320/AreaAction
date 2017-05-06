using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class User {

    public string name;
    public string socketID;
    public int score;
    public int rank;
    public int characterID;

    public bool isDeath = false;

    public User() {
        this.name = "";
        this.score = 0;
        this.characterID = 0;
    }

    public User(string name)
    {
        this.name = name;
        this.score = 0;
        this.characterID = Random.Range(1,5);
    }

    public User(string name , string socketID)
    {
        this.name = name;
        this.score = 0;
        this.characterID = 0;
        this.socketID = socketID;
    }

    public User(string name, string socketID, int characterID)
    {
        this.name = name;
        this.score = 0;
        this.characterID = characterID;
        this.socketID = socketID;
    }

    public User(string name, int score)
    {
        this.name = name;
        this.score = score;
        this.characterID = 0;
    }

    public User(string name, int score, int characterID)
    {
        this.name = name;
        this.score = score;
        this.characterID = characterID;
    }


}
