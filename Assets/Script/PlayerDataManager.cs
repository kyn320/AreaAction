using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDataManager : MonoBehaviour {

    public static PlayerDataManager instance;

    public User my;

    void Awake() {
        instance = this;
        my = new User("User" + Random.Range(0, 100));
    }



}
