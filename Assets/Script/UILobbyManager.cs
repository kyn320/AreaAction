using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UILobbyManager : MonoBehaviour {

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
    public Dropdown roomMaxNumber;

    [SerializeField]
    int count;

    void Awake() {
        instance = this;
    }

	// Use this for initialization
	void Start () {
        NetworkManager.instance.EmitRoomList();

        SetPlayerName();
    }
    

    public void MakeSlots() {
        DelRoomList();

        count = NetworkManager.instance.roomList.Count;

        float height = (slotSize.y) * count + slotMarign.y * 2;

        contentView.sizeDelta = new Vector2(0, height);

        for (int i = 0; i < count; i++) {
            GameObject g =  Instantiate(slot);
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

    public void SetPlayerName() {
        playerNameText.text = PlayerDataManager.instance.my.name + "님 환영합니다!";
    }

    public void DelRoomList() {
        for (int i = 0; i < slotList.Count; i++) {
            slotList[i].Del();
        }

        slotList.Clear();
    }

    public void MakeRoom() {
        NetworkManager.instance.EmitMakeRoom(roomTitleInput.text, int.Parse(roomMaxNumber.options[roomMaxNumber.value].text));
    }


}
