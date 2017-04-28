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

    public Room TestRoom;

    // Use this for initialization
    void Start()
    {

        instance = this;
        socket = GetComponent<SocketIOComponent>();

        

        socket.On("userList", OnUserList);
        socket.On("join", OnJoin);
        socket.On("start", OnStart);
        socket.On("chat", OnChat);
        socket.On("score", OnScore);
        socket.On("attack", OnAttack);
        socket.On("roomList", OnRoomList);

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

    public void Test() {
        EmitLogin(PlayerDataManager.instance.my.name);
        EmitJoin(TestRoom);
    }

    public void EmitLogin(string name)
    {
        JSONObject json = new JSONObject(JSONObject.Type.OBJECT);
        json.AddField("name", name);

        socket.Emit("login", json);

        print("Login is called");
    }

    public void OnUserList(SocketIOEvent e)
    {
        TestRoom.userList.Clear();

        JSONObject json = e.data;

        string[] users = json.GetField("userList").ToString().Replace("[", "").Replace("]", "").Replace("\"","").Split(',');

        for (int i = 0; i < users.Length; i++) {
            TestRoom.userList.Add(new User(users[i]));
        }
    }

    public void OnJoin(SocketIOEvent e)
    {
        JSONObject json = e.data;

        string name = json.GetField("name").str;
        int characterID = (int)(json.GetField("character").f);

        TestRoom.userList.Add(new User(name, 0, characterID));

        print(name + " 님이 접속 |  " + characterID);
    }

    public void EmitJoin(Room room)
    {
        JSONObject json = new JSONObject(JSONObject.Type.OBJECT);
        json.AddField("roomName", room.id + room.name);
        json.AddField("character", 1);

        socket.Emit("join", json);
    }

    public void OnStart(SocketIOEvent e) {
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

        for (int i = 0; i < TestRoom.userList.Count; i++)
        {
            if (TestRoom.userList[i].name == name)
            {
                TestRoom.userList[i].score = score;
                UIInGameManager.instance.UpdateUserRanking(TestRoom);
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



    public void OnRoomList(SocketIOEvent e) {
        JSONObject json = e.data;

    }

}
