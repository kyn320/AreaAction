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

    void Awake() {
        DontDestroyOnLoad(this.gameObject);
    }

    // Use this for initialization
    void Start()
    {

        instance = this;
        socket = GetComponent<SocketIOComponent>();

        socket.On("userList", OnUserList);
        socket.On("enterRoom", OnEnterRoom);
        socket.On("join", OnJoin);
        socket.On("start", OnStart);
        socket.On("chat", OnChat);
        socket.On("score", OnScore);
        socket.On("attack", OnAttack);
        socket.On("roomList", OnRoomList);
        socket.On("roomFinish", OnUpdateLobby);
        socket.On("test", OnTest);

        StartCoroutine("WaitForLobby");
    }

    public void OnTest(SocketIOEvent e) {
        JSONObject json = e.data;

        print(json.GetField("test").str);
    }

    public void urlChange() {
        socket.url = "ws://"+editor.text+":8080/socket.io/?EIO=3&transport=websocket";
    }

    public void LoginTest()
    {
        //urlChange();
        //socket.Connect();

        StartCoroutine("WaitForConnet");
    }

    IEnumerator WaitForConnet() {
        yield return new WaitForSeconds(0.1f);
        Test();
    }

    IEnumerator WaitForLobby() {
        yield return new WaitForSeconds(0.1f);
        JSONObject json = new JSONObject(JSONObject.Type.OBJECT);
        json.AddField("a",2);
        socket.Emit("roomList",json);
    }

    public void Test() {
        EmitLogin(PlayerDataManager.instance.my.name);
        //EmitJoin(TestRoom);
    }

    public void EmitLogin(string name)
    {
        JSONObject json = new JSONObject(JSONObject.Type.OBJECT);
        json.AddField("name", name);

        socket.Emit("login", json);

        print("Login is called");
    }

    public void OnLogin(SocketIOEvent e) {
        JSONObject json = e.data;

        PlayerDataManager.instance.my.socketID = json.GetField("socketID").str;

    }

    public void OnUserList(SocketIOEvent e)
    {
        print("clear?");
        enterRoom.userList.Clear();

        print("update userList");
        
        LitJson.JsonData json = LitJson.JsonMapper.ToObject(e.data.ToString());

        print(e.data);

        for (int i = 0; i < json["userList"].Count; ++i)
        {
            print("make user");
            User user = new User();

            user.socketID = json["userList"][i]["socketID"].ToString();
            user.name = json["userList"][i]["name"].ToString();
            user.characterID = int.Parse(json["userList"][i]["character"].ToString());

            print(user.name  + " | " + user.socketID);

            enterRoom.userList.Add(user);
        }
    }

    public void OnEnterRoom(SocketIOEvent e) {
        JSONObject json = e.data;
        string roomName = json.GetField("roomName").str;
        for (int i = 0; i < roomList.Count; i++) {
            if (roomName == roomList[i].id + roomList[i].name)
            {
                enterRoom = roomList[i];
                break;
            }
        }

        UnityEngine.SceneManagement.SceneManager.LoadScene("InGameUI");
    }

    public void OnJoin(SocketIOEvent e)
    {
        JSONObject json = e.data;

        print("asdqwe");

        string name = json.GetField("name").str;
        string socketID = json.GetField("socketID").str;
        int characterID = (int)(json.GetField("character").f);

        enterRoom.userList.Add(new User(name, socketID, characterID));

        print(name + " 님이 접속 |  " + characterID);
    }

    public void EmitJoin(Room room)
    {
        JSONObject json = new JSONObject(JSONObject.Type.OBJECT);
        json.AddField("roomName", room.id + room.name);
        json.AddField("socketID", PlayerDataManager.instance.my.socketID);
        json.AddField("name", PlayerDataManager.instance.my.name);
        json.AddField("character", 3);

        socket.Emit("join", json);
    }


    public void EmitReady() {
        JSONObject json = new JSONObject(JSONObject.Type.OBJECT);
        json.AddField("socketID", PlayerDataManager.instance.my.socketID);
        json.AddField("name", PlayerDataManager.instance.my.name);
        json.AddField("character", 3);

        socket.Emit("ready", json);
    }

    public void OnStart(SocketIOEvent e) {
        print("gogo");
        GameManager.instance.isplayed = true;
        UIInGameManager.instance.UpdateWaitPannel();
    }

    public void OnChat(SocketIOEvent e)
    {
        JSONObject json = e.data;

        string name = json.GetField("name").str;
        string message = json.GetField("message").str;

        print(name + " 의 메세지 " + message);
        UIInGameManager.instance.UpdateChatLog(name,message);
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

        print(name + "님께서 공격 = " + damage);
    }

    public void EmitAttack(string name, int damage, string other)
    {
        JSONObject json = new JSONObject(JSONObject.Type.OBJECT);
        json.AddField("name", name);
        json.AddField("damage", damage);
        json.AddField("other", other);

        socket.Emit("attack", json);
    }

    public void OnRoomList(SocketIOEvent e)
    {
        roomList.Clear();

        print("json is work");
        print(e.data.ToString());
        LitJson.JsonData json = LitJson.JsonMapper.ToObject(e.data.ToString());
        for (int i = 0;  i < json["roomLists"].Count; ++i) {
            Room room = new Room();

            room.id = int.Parse(json["roomLists"][i]["id"].ToString());
            room.name = json["roomLists"][i]["name"].ToString();
            room.currentPlayers = int.Parse(json["roomLists"][i]["readyPlayers"].ToString());
            room.fullPlayers = int.Parse(json["roomLists"][i]["fullPlayers"].ToString());
            room.isPlayed = bool.Parse(json["roomLists"][i]["isPlayed"].ToString());

            roomList.Add(room);
        }
    }

    public void OnUpdateLobby(SocketIOEvent e) {
        UILobbyManager.instance.MakeSlots();
    }

    public void EmitMakeRoom(string name, int max) {
        JSONObject json = new JSONObject(JSONObject.Type.OBJECT);

        json.AddField("roomName", name);
        json.AddField("fullPlayers",max);
        
        socket.Emit("make", json);


    }

}
