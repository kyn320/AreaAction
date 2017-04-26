using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class User {

    public string name;
    public int score;
    public int characterID;

    public User() {
        this.name = "User" + System.DateTime.Now;
        this.score = 0;
        this.characterID = 0;
    }

    public User(string name)
    {
        this.name = name;
        this.score = 0;
        this.characterID = 0;
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
