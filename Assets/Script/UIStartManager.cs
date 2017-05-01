using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIStartManager : MonoBehaviour {

    public static UIStartManager instance;

    public InputField nickNameInput;

    void Awake() {
        instance = this;
    }

	// Use this for initialization
	void Start () {
		
	}

    public void EnterLobby() {
        string name = "";
        if (nickNameInput.text == "")
            name = "User" + Random.Range(0, 100);
        else
            name = nickNameInput.text;

        PlayerDataManager.instance.my = new User(name);

        NetworkManager.instance.EmitLogin(name);

        SceneManager.LoadScene("Lobby");
    }

}
