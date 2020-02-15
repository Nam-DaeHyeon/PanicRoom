using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class RoomCraftingDisplayerSlot : MonoBehaviour
{
    [Header("Slot Information")]
    public int slotID; // Inventory Manager가 부여하는 ID
    [System.Serializable] public enum SlotType {NONE, CRAFTING_MATERIAL, CRAFTING_WEAPON, CRAFTING_RESULT};
    public SlotType slotType;
    [HideInInspector] public bool canDrop;

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
    private const float maxSlotSize = 1.1f;
    private const float minSlotSize = 1;

    private Coroutine itemInfoCoroutine;
    private const float infoVisibleDelay = 0.5f;

    private RoomCraftingDisplayer crafting;


    // ========================================================= public function =========================================================
    /*
    public void Init(RoomCraftingDisplayer crafting, int id)
    {
        slotID = id;

        // 크래프팅 접근을 위해 스크립트 캐싱
        this.crafting = crafting;
        itemImage = transform.Find("Image").GetComponent<Image>();

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

    public void SetItem(string itemIndex)
    {
        if (itemIndex == "")
        {
            ResetSlot();
            return;
        }
        this.itemIndex = itemIndex;

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

    public void ResetSlot()
    {
        itemIndex = "";
        itemName = "";
        itemExp = "";
        itemImage.enabled = false;
        itemImage.sprite = null;
    }

    // 이 Slot의 크기를 키운다.
    public void SlotSizeUp()
    {
        if (slotSizeCoroutine != null)
            StopCoroutine(slotSizeCoroutine);
        slotSizeCoroutine = StartCoroutine(SizeUp());
    }

    public void SlotSizeDown()
    {
        if (slotSizeCoroutine != null)
            StopCoroutine(slotSizeCoroutine);
        slotSizeCoroutine = StartCoroutine(SizeDown());
    }

    void OnPointerEnter(PointerEventData data)
    {
        crafting.buttonUpID = slotID;

        // 일정 시간이 지나면 아이템 정보 표시 칸을 킨다.
        if (itemInfoCoroutine != null)
            StopCoroutine(itemInfoCoroutine);
        itemInfoCoroutine = StartCoroutine(ItemInformation());
    }

    void OnPointerExit(PointerEventData data)
    {
        // 아이템 정보 표시 칸을 끈다.
        if (itemInfoCoroutine != null)
            StopCoroutine(itemInfoCoroutine);
        crafting.InVisibleExplanationPanel();
    }

    void OnPointerDown(PointerEventData data)
    {
        // 현재 slot에 담겨있는 아이템이 있으면
        if (itemIndex != "")
        {
            // Drag 하는 아이템 이미지에 현재 slot의 아이템을 띄운다.
            crafting.dragImage.sprite = itemImage.sprite;
            crafting.dragImage.enabled = true;

            // 클릭한 slot의 정보를 Manager Script 기록
            crafting.buttonDownID = slotID;

            // Drag 시작
            crafting.isDrag = true;
        }

        // 아이템 정보 표시 칸을 끈다.
        if (itemInfoCoroutine != null)
            StopCoroutine(itemInfoCoroutine);
        crafting.InVisibleExplanationPanel();

        // 클릭한 아이템 칸의 크기를 늘린다.
        if (slotSizeCoroutine != null)
            StopCoroutine(slotSizeCoroutine);
        slotSizeCoroutine = StartCoroutine(SizeUp());

        // 클릭한 slot의 Type에 Drag 가능한 slot들의 크기를 늘린다.
        crafting.SlotsSizeUp(crafting.DragTypeConverter(slotType));

    }

    void OnPointerUp(PointerEventData data)
    {

        Debug.Log("111");
        if (crafting.isDrag)
        {
            // Drag 종료
            crafting.isDrag = false;

            // Drag Drop 가능 확인
            if (crafting.slots[crafting.buttonUpID].canDrop)
            {
                Debug.Log("111");
            }

            // Drag 하는 아이템 이미지
            crafting.dragImage.sprite = null;
            crafting.dragImage.enabled = false;
            // 클릭한 slot의 정보를 Reset
            crafting.buttonDownID = -1;

        }

        // 클릭한 아이템 칸의 크기를 줄인다.
        if (slotSizeCoroutine != null)
            StopCoroutine(slotSizeCoroutine);
        slotSizeCoroutine = StartCoroutine(SizeDown());

        // 클릭한 slot의 Type에 Drag 가능한 slot들의 크기를 줄인다.
        crafting.SlotsSizeDown(crafting.DragTypeConverter(slotType));
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

    IEnumerator ItemInformation()
    {
        // 일정 시간 대기
        yield return new WaitForSeconds(infoVisibleDelay);

        // 아이템 정보를 띄움.
        // 단, 가지고 있는 아이템이 없거나 아이템을 Drag 중이면 제외
        if (itemIndex != "" && crafting.isDrag == false)
        {
            crafting.VisibleExplanationPanel(itemName, itemExp, GetComponent<RectTransform>().position);
        }
    }
    */
}
