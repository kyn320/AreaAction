using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UILobbyManager : MonoBehaviour
{

    public static UILobbyManager instance;

    //Header
    public Text playerNameText;

    //ScrollView
    public RectTransform contentView;

    public Vector2 slotStartPos, slotSize, slotMarign;

    public GameObject slot;

    public List<UIRoomSlot> slotList;

    //MakeRoom
    public InputField roomTitleInput;
    public Text maxNumberSetButtonText;
    public int maxNumberSet = 2;

    //chatLog
    public InputField chatInput;
    public Text chatOneLine;
    public Text chatLog;
    int chatCount;

    [SerializeField]
    int count;

    void Awake()
    {
        instance = this;
    }

    // Use this for initialization
    void Start()
    {
        NetworkManager.instance.EmitRoomList();

        SetPlayerName();
        PlayerDataManager.instance.where = 1;
    }


    public void MakeSlots()
    {
        DelRoomList();

        count = NetworkManager.instance.roomList.Count;

        float height = (slotSize.y) * count + slotMarign.y * 2;

        contentView.sizeDelta = new Vector2(0, height);

        for (int i = 0; i < count; i++)
        {
            GameObject g = Instantiate(slot);
            RectTransform gTr = g.GetComponent<RectTransform>();
            UIRoomSlot data = g.GetComponent<UIRoomSlot>();
            slotList.Add(data);
            gTr.SetParent(contentView);
            gTr.localPosition = new Vector2(0, slotStartPos.y + -i * (slotSize.y) - slotMarign.y);
            gTr.localScale = new Vector3(1, 1, 1);
            gTr.sizeDelta = new Vector2(-10, 60);
            data.SetSlot(NetworkManager.instance.roomList[i]);
        }
    }

    public void SetPlayerName()
    {
        playerNameText.text = PlayerDataManager.instance.my.name + "님 환영합니다!";
    }



    public void UpdateChat(string name, string message)
    {
        if (chatCount > 33)
        {
            chatLog.text = "";
            chatCount = 0;
        }

        chatOneLine.text = name + " : " + message;
        chatLog.text += name + " : " + message + "\n";
        chatCount++;
    }

    public void DelRoomList()
    {
        for (int i = 0; i < slotList.Count; i++)
        {
            slotList[i].Del();
        }

        slotList.Clear();
    }

    public void MakeRoom()
    {
        NetworkManager.instance.EmitMakeRoom(roomTitleInput.text, maxNumberSet);
    }

    public void SendChat()
    {
        if (chatInput.text != "")
        {
            NetworkManager.instance.EmitChat(PlayerDataManager.instance.my.name, chatInput.text);
            chatInput.text = "";
        }
    }

    public void SetMaxNumber(int num) {
        maxNumberSet = num;
        maxNumberSetButtonText.text = num + "명";
    }

}
