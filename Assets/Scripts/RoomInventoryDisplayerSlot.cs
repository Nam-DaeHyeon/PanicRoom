using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;


public class RoomInventoryDisplayerSlot : MonoBehaviour
{
    [Header("Slot Information")]
    public int slotID; // Inventory Manager가 부여하는 ID
    [System.Serializable] public enum SlotType { NONE, PARTY, ALL };
    public SlotType slotType;

    [Header("Having Item Info")]
    public string itemIndex; // 가지고 있는 아이템 Index(CSV 파싱 용)
    public string itemName; // 가지고 있는 아이템의 이름
    public string itemExp; // 가지고 있는 아이템의 설명
    public int itemCount; // 가지고 있는 아이템 갯수
    public Image itemImage; // slot이 가지는 아이템 이미지 UI
    public Text itemCountText; // slot이 가지는 아이템 Count UI

    // slot 사이즈를 늘리고 줄이는데 사용되는 코루틴
    private Coroutine slotSizeCoroutine;
    // slot 사이즈를 늘리고 줄이는데 걸리는 시간, 크기
    private const float slotSizingSpeed = 1;
    private const float maxSlotSize = 1.2f;
    private const float minSlotSize = 1;

    private RoomInventoryDisplayer inventory;


    // ========================================================= public function =========================================================
    /*
    public void Init(RoomInventoryDisplayer inventory, int id)
    {
        slotID = id;

        // 인벤토리 접근을 위해 스크립트 캐싱
        this.inventory = inventory;
        itemImage = transform.Find("Image").GetComponent<Image>();
        itemCountText = transform.Find("Count").GetComponent<Text>();

        // slot 클릭 함수 정의
        EventTrigger eventTrigger = gameObject.AddComponent<EventTrigger>();

        EventTrigger.Entry entry_PointerEnter = new EventTrigger.Entry();
        entry_PointerEnter.eventID = EventTriggerType.PointerEnter;
        entry_PointerEnter.callback.AddListener((data) => { OnPointerEnter((PointerEventData)data); });
        eventTrigger.triggers.Add(entry_PointerEnter);

        EventTrigger.Entry entry_PointerExit = new EventTrigger.Entry();
        entry_PointerExit.eventID = EventTriggerType.PointerExit;
        entry_PointerExit.callback.AddListener((data) => { OnPointerExit((PointerEventData)data); });
        eventTrigger.triggers.Add(entry_PointerExit);

        EventTrigger.Entry entry_PointerDown = new EventTrigger.Entry();
        entry_PointerDown.eventID = EventTriggerType.PointerDown;
        entry_PointerDown.callback.AddListener((data) => { OnPointerDown((PointerEventData)data); });
        eventTrigger.triggers.Add(entry_PointerDown);

        EventTrigger.Entry entry_PointerUp = new EventTrigger.Entry();
        entry_PointerUp.eventID = EventTriggerType.PointerUp;
        entry_PointerUp.callback.AddListener((data) => { OnPointerUp((PointerEventData)data); });
        eventTrigger.triggers.Add(entry_PointerUp);

        // 이전에 가지고 있던 아이템 정보를 가져온다.


    }

    // 넘겨진 정보로 slot을 세팅
    public void SetSlot(string itemIndex, int itemCount)
    {
        if (itemIndex == "")
        {
            ResetSlot();
            return;
        }
        this.itemIndex = itemIndex;
        this.itemCount = itemCount;
        this.itemCountText.text = itemCount.ToString();
        this.itemCountText.enabled = true;

        List<Dictionary<string, object>> data = CSVReader.Read("CSV/ItemList");

        for (var i = 0; i < data.Count; i++)
        {
            if (data[i]["Index"] as string == itemIndex)
            {
                itemName = data[i]["Name"] as string;
                itemExp = data[i]["Exp"] as string;

                // 이미지 지정
                itemImage.sprite = Resources.Load<Sprite>("ItemImage/" + itemName);
                itemImage.enabled = true;
                break;
            }
        }
    }

    // slot을 초기화
    public void ResetSlot()
    {
        itemIndex = "";
        itemName = "";
        itemExp = "";
        itemImage.enabled = false;
        itemImage.sprite = null;
        itemCountText.enabled = false;
        itemCountText.text = "";
    }



    // 기존에 가지고 있던 아이템의 갯수를 추가
    public bool AddCount(int itemCount)
    {
        this.itemCountText.text = (itemCount + itemCount).ToString();
        return true;
    }


    // 기존에 가지고 있던 아이템의 갯수를 감산, 만약 아이템 수가
    public bool SubtractCount(int itemCount)
    {
        // 현재 slot이 가지고 있는 모든 아이템을 사용
        if (itemCount == this.itemCount)
        {
            ResetSlot();
            return true;
        }
        // 현재 slot이 가지고 있는 아이템보다 많은 아이템을 사용
        else if (itemCount > this.itemCount)
        {
            return false;
        }
        // 현재 slot이 가지고 있는 아이템보다 적은 아이템을 사용
        else
        {
            this.itemCountText.text = (itemCount - itemCount).ToString();
            return true;
        }
    }
    
   void OnPointerEnter(PointerEventData data)
    {
        inventory.buttonUpID = slotID;

        // 아이템 정보를 띄움.
        // 단, 가지고 있는 아이템이 없거나 아이템을 Drag 중이면 제외
        if (itemIndex != "" && inventory.isDrag == false)
            inventory.VisibleExplanationPanel(itemName, itemExp, GetComponent<RectTransform>().position);
    }

    void OnPointerExit(PointerEventData data)
    {
        // 아이템 정보 표시 칸을 끈다.
        inventory.InVisibleExplanationPanel();
    }

    void OnPointerDown(PointerEventData data)
    {
        // 현재 slot에 담겨있는 아이템이 있으면
        if (itemIndex != "")
        {
            // Drag 하는 아이템 이미지에 현재 slot의 아이템을 띄운다.
            inventory.dragImage.sprite = itemImage.sprite;
            inventory.dragImage.enabled = true;

            // 클릭한 slot의 정보를 Manager Script 기록
            inventory.buttonDownID = slotID;

            // Drag 시작
            inventory.isDrag = true;
        }
        
        // 아이템 정보 표시 칸을 끈다.
        inventory.InVisibleExplanationPanel();

        // 클릭한 아이템 칸의 크기를 키운다.
        if (slotSizeCoroutine != null)
            StopCoroutine(slotSizeCoroutine);
        slotSizeCoroutine = StartCoroutine(SizeUp());
    }

    void OnPointerUp(PointerEventData data)
    {
        if (inventory.isDrag)
        {
            // Drag 종료
            inventory.isDrag = false;

            // 두 slot간 정보 교환
            inventory.SwapBetweenSlot(inventory.buttonDownID, inventory.buttonUpID);

            // Drag 하는 아이템 이미지
            inventory.dragImage.sprite = null;
            inventory.dragImage.enabled = false;
            // 클릭한 slot의 정보를 Reset
            inventory.buttonDownID = -1;
        }

        // 클릭한 아이템 칸의 크기를 줄인다.
        if (slotSizeCoroutine != null)
            StopCoroutine(slotSizeCoroutine);
        slotSizeCoroutine = StartCoroutine(SizeDown());
    }


    // ========================================================= private function =========================================================

    IEnumerator SizeUp()
    {
        RectTransform rectTransform = GetComponent<RectTransform>();
        float temp = 0;
        while (true)
        {
            if (minSlotSize + temp >= maxSlotSize)
            {
                rectTransform.localScale = new Vector2(maxSlotSize, maxSlotSize);
                break;
            }
            else
            {
                rectTransform.localScale = new Vector2(minSlotSize + temp, minSlotSize + temp);
            }

            temp += Time.deltaTime * slotSizingSpeed;
            yield return null;
        }
    }

    IEnumerator SizeDown()
    {
        RectTransform rectTransform = GetComponent<RectTransform>();
        float temp = 0;
        float conSize = rectTransform.localScale.x;
        while (true)
        {
            if (conSize - temp <= minSlotSize)
            {
                rectTransform.localScale = new Vector2(minSlotSize, minSlotSize);
                break;
            }
            else
            {
                rectTransform.localScale = new Vector2(conSize - temp, conSize - temp);
            }

            temp += Time.deltaTime * slotSizingSpeed;
            yield return null;
        }
    }
    */
}
