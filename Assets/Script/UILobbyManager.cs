using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UILobbyManager : MonoBehaviour {

    public static UILobbyManager instance;

    //Header
    public Text playerName;

    //ScrollView
    public RectTransform contentView;

    public Vector2 slotStartPos, slotSize, slotMarign;

    public GameObject slot;

    [SerializeField]
    int count;

    void Awake() {
        instance = this;
    }

	// Use this for initialization
	void Start () {
        MakeSlots();
	}

    void MakeSlots() {
        float height = (slotSize.y) * count + slotMarign.y * 2;

        contentView.sizeDelta = new Vector2(0, height);

        for (int i = 0; i < count; i++) {
            GameObject g =  Instantiate(slot);
            RectTransform gTr = g.GetComponent<RectTransform>();
            gTr.parent = contentView;
            print(slotStartPos.y + -i * (slotSize.y + slotMarign.y));
            gTr.localPosition = new Vector2(0, slotStartPos.y + -i * (slotSize.y) - slotMarign.y);
            gTr.localScale = new Vector3(1, 1, 1);
        }
    }

    public void MakeRoom() {
        print("asdqwe");
    }
}
