using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Room {

    public int id;
    public string name;

    public int currentPlayers;
    public int fullPlayers;

    public List<User> userList = new List<User>();

    public bool isPlayed;
}
