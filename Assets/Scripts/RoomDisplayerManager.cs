using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoomDisplayerManager : MonoBehaviour
{
    // 아이템 정보 창 Panel Info
    [Header("ExplanationPanel")]
    public GameObject explanationPanel;
    public Text explanationPanel_itemName;
    public Text explanationPanel_itemExp;
    public RectTransform pos_TopLeft;
    public RectTransform pos_BotRight;

    // 현재 드래그 중인 아이템
    [Header("Draging Item Info")]
    public Image dragImage;

    [Header("Having Script")]
    public RoomInventoryDisplayer inventory;
    public RoomCraftingDisplayer crafting;

    // Start is called before the first frame update
    void Awake()
    {
        // 모든 Displayer의 멤버 변수를 초기화
        for(int i = 0; i < transform.childCount; i++)
            transform.GetChild(i).GetComponent<IRoomDisplayer>().Init();

    }

    private void LateUpdate()
    {
        if (RoomItemSlotScript.isDrag)
        {
            dragImage.transform.position = Input.mousePosition;
        }
    }

    public void VisibleExplanationPanel(string itemName, string itemExp, Vector2 position)
    {
        // 정보를 넣는다.
        explanationPanel_itemName.text = itemName;
        explanationPanel_itemExp.text = itemExp;

        // 위치를 지정한다.(화면 밖으로 나가지 않도록 설정)
        Vector2 resultPos = position;
        if (resultPos.x < pos_TopLeft.position.x)
            resultPos.x = pos_TopLeft.position.x;
        else if (resultPos.x > pos_BotRight.position.x)
            resultPos.x = pos_BotRight.position.x;
        if (resultPos.y > pos_TopLeft.position.y)
            resultPos.y = pos_TopLeft.position.y;
        else if (resultPos.y < pos_BotRight.position.y)
            resultPos.y = pos_BotRight.position.y;
        explanationPanel.GetComponent<RectTransform>().position = resultPos;

        // 화면에 표시
        explanationPanel.SetActive(true);
    }

    public void InVisibleExplanationPanel()
    {
        explanationPanel.SetActive(false);
        explanationPanel_itemName.text = "";
        explanationPanel_itemExp.text = "";
    }
}
