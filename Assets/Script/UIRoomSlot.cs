using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class UIRoomSlot : MonoBehaviour
{

    public Room info;

    public Image roomImage;

    public Text idText, stateText, titleText, userCountText;

    public Color waitColor, PlayingColor;

    public void SetSlot(Room room)
    {
        info = room;

        idText.text = "No. " + info.id.ToString();
        if (info.isPlayed)
        {
            roomImage.color = PlayingColor;
            stateText.text = "Playing..";
        }
        else {
            roomImage.color = waitColor;
            stateText.text = "Wait..";
        }

        
        titleText.text = info.name;
        userCountText.text = info.currentPlayers + " / " + info.fullPlayers;
    }

    public void JoinRoom()
    {
        NetworkManager.instance.EmitJoin(info);
    }

    public void Del() {
        Destroy(this.gameObject);
    }


}
