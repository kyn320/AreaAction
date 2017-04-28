using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class UIRoomSlot : MonoBehaviour {

    public Room info;

    public Text idText, stateText, titleText, userCountText;

    public void SetSlot(Room room) {
        info = room;

        idText.text = info.id.ToString();
        stateText.text = info.isPlayed ? "Playing.." : "Wait..";
        titleText.text = info.name;
        userCountText.text = info.userList.Count + " / "+ info.fullPlayers;
    }

    public void JoinRoom() {
        print("join");
    }


}
