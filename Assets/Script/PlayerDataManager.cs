using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDataManager : MonoBehaviour {

    public static PlayerDataManager instance;

    public User my;

    public int where;

    void Awake() {
        instance = this;
        DontDestroyOnLoad(this.gameObject);
    }



}
