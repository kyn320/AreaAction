using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SocketIO;

public class NetworkManager : MonoBehaviour
{
    public static NetworkManager instance;

    SocketIOComponent socket;

    public List<Room> roomList;

    public InputField editor;

    public Room enterRoom;

    int index;

    void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }

    // Use this for initialization
    void Start()
    {

        instance = this;
        socket = GetComponent<SocketIOComponent>();

        socket.On("userList", OnUserList);
        socket.On("enterRoom", OnEnterRoom);
        socket.On("login", OnLogin);
        socket.On("join", OnJoin);
        socket.On("start", OnStart);
        socket.On("chat", OnChat);
        socket.On("score", OnScore);
        socket.On("attack", OnAttack);
        socket.On("hpchange", OnHpChange);
        socket.On("death", OnDeath);
        socket.On("roomList", OnRoomList);
        socket.On("roomFinish", OnUpdateLobby);
        socket.On("out", OnExitRoom);

    }


    public void EmitRoomList()
    {
        JSONObject json = new JSONObject(JSONObject.Type.OBJECT);
        json.AddField("a", 2);
        socket.Emit("roomList", json);
    }


    public void EmitLogin(string name)
    {
        JSONObject json = new JSONObject(JSONObject.Type.OBJECT);
        json.AddField("name", name);

        socket.Emit("login", json);

    }

    public void OnLogin(SocketIOEvent e)
    {
        JSONObject json = e.data;

        PlayerDataManager.instance.my.socketID = json.GetField("socketID").str;
    }

    public void OnUserList(SocketIOEvent e)
    {
        enterRoom.userList.Clear();

        LitJson.JsonData json = LitJson.JsonMapper.ToObject(e.data.ToString());

        for (int i = 0; i < json["userList"].Count; ++i)
        {
            User user = new User();

            user.socketID = json["userList"][i]["socketID"].ToString();
            user.name = json["userList"][i]["name"].ToString();
            user.characterID = int.Parse(json["userList"][i]["character"].ToString());


            enterRoom.userList.Add(user);
            UIInGameManager.instance.userSlots[enterRoom.userList.Count - 1].gameObject.SetActive(true);
            UIInGameManager.instance.userSlots[enterRoom.userList.Count - 1].SetInfo(user);
        }
    }


    public void OnEnterRoom(SocketIOEvent e)
    {
        JSONObject json = e.data;
        string roomName = json.GetField("roomName").str;
        for (int i = 0; i < roomList.Count; i++)
        {
            if (roomName == roomList[i].id + roomList[i].name)
            {
                enterRoom = roomList[i];
                break;
            }
        }
        PlayerDataManager.instance.where = 2;

        UnityEngine.SceneManagement.SceneManager.LoadScene("InGameUI");
    }

    public void OnJoin(SocketIOEvent e)
    {
        JSONObject json = e.data;

        string name = json.GetField("name").str;
        string socketID = json.GetField("socketID").str;
        int characterID = (int)(json.GetField("character").f);
        User user = new User(name, socketID, characterID);
        enterRoom.userList.Add(user);
        UIInGameManager.instance.userSlots[enterRoom.userList.Count - 1].gameObject.SetActive(true);
        UIInGameManager.instance.userSlots[enterRoom.userList.Count - 1].SetInfo(user);
        UIInGameManager.instance.UpdateNotice(user.name + "님께서 참여하셨습니다.");
    }

    public void EmitJoin(Room room)
    {
        JSONObject json = new JSONObject(JSONObject.Type.OBJECT);
        json.AddField("roomName", room.id + room.name);
        json.AddField("socketID", PlayerDataManager.instance.my.socketID);
        json.AddField("name", PlayerDataManager.instance.my.name);
        json.AddField("character", PlayerDataManager.instance.my.characterID);

        socket.Emit("join", json);
    }


    public void EmitReady()
    {
        JSONObject json = new JSONObject(JSONObject.Type.OBJECT);
        json.AddField("socketID", PlayerDataManager.instance.my.socketID);
        json.AddField("name", PlayerDataManager.instance.my.name);
        json.AddField("character", PlayerDataManager.instance.my.characterID);

        socket.Emit("ready", json);
    }

    public void OnStart(SocketIOEvent e)
    {
        GameManager.instance.isAlive = enterRoom.userList.Count;
        GameManager.instance.isplayed = true;
        BoardManager.instance.CreatHexTile();
        UIInGameManager.instance.UpdateWaitPannel();
        UIInGameManager.instance.UpdateNotice("게임 시작!");
    }

    public void OnChat(SocketIOEvent e)
    {
        JSONObject json = e.data;

        string name = json.GetField("name").str;
        string message = json.GetField("message").str;

        switch (PlayerDataManager.instance.where)
        {
            case 0: break;
            case 1: UILobbyManager.instance.UpdateChat(name, message); break;
            case 2: UIInGameManager.instance.UpdateChatLog(enterRoom.userList.Count, name, message); break;
        }
    }

    public void EmitChat(string name, string message)
    {
        JSONObject json = new JSONObject(JSONObject.Type.OBJECT);
        json.AddField("name", name);
        json.AddField("message", message);

        socket.Emit("chat", json);
    }

    public void OnScore(SocketIOEvent e)
    {
        JSONObject json = e.data;

        string name = json.GetField("name").str;
        int score = (int)(json.GetField("score").f);

        for (int i = 0; i < enterRoom.userList.Count; i++)
        {
            if (enterRoom.userList[i].name == name)
            {
                enterRoom.userList[i].score = score;
                UIInGameManager.instance.UpdateUserRanking(enterRoom);
                break;
            }
        }

        print(name + " 의 점수 획득 = " + score);
    }

    public void EmitScore(string name, int score)
    {
        JSONObject json = new JSONObject(JSONObject.Type.OBJECT);
        json.AddField("name", name);
        json.AddField("score", score);

        socket.Emit("score", json);
    }

    public void OnAttack(SocketIOEvent e)
    {
        JSONObject json = e.data;

        string name = json.GetField("name").str;
        int damage = (int)(json.GetField("damage").f);
        string other = json.GetField("other").str;

        print(name + "님께서 공격 = " + damage + " >> " + other);
        UIInGameManager.instance.UpdateNotice(name + "님이 " + other + "님에게 " + damage + "의 데미지를 입혔습니다.");
        if (other == PlayerDataManager.instance.my.name)
        {
            Player.instance.DamageHP(damage);
        }
    }

    public void EmitAttack(string name, int damage, string other)
    {
        JSONObject json = new JSONObject(JSONObject.Type.OBJECT);
        json.AddField("name", name);
        json.AddField("damage", damage);
        json.AddField("other", other);

        socket.Emit("attack", json);
    }

    public void OnHpChange(SocketIOEvent e)
    {
        JSONObject json = e.data;

        string name = json.GetField("name").str;
        int hp = (int)(json.GetField("hp").f);
        int maxhp = (int)(json.GetField("maxhp").f);

        print(name + "님의 HP : " + hp + " / " + maxhp);

        UIInGameManager.instance.UpdateUserSlotHpChange(enterRoom.userList.Count, name, hp, maxhp);
    }

    public void EmitHpChange(string name, int hp, int maxHp)
    {
        JSONObject json = new JSONObject(JSONObject.Type.OBJECT);
        json.AddField("name", name);
        json.AddField("hp", hp);
        json.AddField("maxhp", maxHp);

        socket.Emit("hpchange", json);
    }


    public void OnDeath(SocketIOEvent e)
    {
        JSONObject json = e.data;

        string name = json.GetField("name").str;

        print(name + " is dead");
        UIInGameManager.instance.UpdateNotice(name + "님이 사망하였습니다.");
        for (int i = 0; i < enterRoom.userList.Count; i++)
        {
            if (name == enterRoom.userList[i].name)
            {
                enterRoom.userList[i].isDeath = true;
            }
        }
        GameManager.instance.DownIsAlive();
    }

    public void EmitDeath(string name)
    {
        JSONObject json = new JSONObject(JSONObject.Type.OBJECT);
        json.AddField("name", name);

        socket.Emit("death", json);
    }

    public void OnRoomList(SocketIOEvent e)
    {
        roomList.Clear();

        LitJson.JsonData json = LitJson.JsonMapper.ToObject(e.data.ToString());
        for (int i = 0; i < json["roomLists"].Count; ++i)
        {
            Room room = new Room();

            room.id = int.Parse(json["roomLists"][i]["id"].ToString());
            room.name = json["roomLists"][i]["name"].ToString();
            room.currentPlayers = int.Parse(json["roomLists"][i]["readyPlayers"].ToString());
            room.fullPlayers = int.Parse(json["roomLists"][i]["fullPlayers"].ToString());
            room.isPlayed = bool.Parse(json["roomLists"][i]["isPlayed"].ToString());

            roomList.Add(room);
        }
    }

    public void OnUpdateLobby(SocketIOEvent e)
    {
        UILobbyManager.instance.MakeSlots();
    }

    public void EmitMakeRoom(string name, int max)
    {
        JSONObject json = new JSONObject(JSONObject.Type.OBJECT);

        json.AddField("roomName", name);
        json.AddField("fullPlayers", max);

        socket.Emit("make", json);


    }

    public void EmitExitRoom()
    {
        JSONObject json = new JSONObject(JSONObject.Type.OBJECT);

        json.AddField("a", 1);

        socket.Emit("out", json);
    }

    public void OnExitRoom(SocketIOEvent e)
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("Lobby");
    }



    public string EnterRoomGetRandomUser()
    {
        string name = "";
        bool death = false;

        do
        {
            User user = enterRoom.userList[Random.Range(0, enterRoom.userList.Count)];
            name = user.name;
            death = user.isDeath;

        } while (death || name == PlayerDataManager.instance.my.name);

        return name;

    }

}
