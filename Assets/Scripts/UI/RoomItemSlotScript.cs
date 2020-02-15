using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[System.Serializable] public enum SlotType { NONE, PARTY, ALL,  CRAFTING_MATERIAL, CRAFTING_WEAPON, CRAFTING_RESULT, CHARACTER };

public class RoomItemSlotScript : MonoBehaviour
{
    [Header("Slot Information")]
    public SlotType slotType;
    public int slotID;
    
    [Header("Having Item Info")]
    public string itemIndex; // 가지고 있는 아이템 Index(CSV 파싱 용)
    public string itemName; // 가지고 있는 아이템의 이름
    public string itemExp; // 가지고 있는 아이템의 설명
    public int itemCount; // 가지고 있는 아이템 갯수
    public Image itemImage; // slot이 가지는 아이템 이미지 UI
    public Text itemCountText; // slot이 가지는 아이템 Count UI

    private RoomDisplayerManager manager;
    private RoomInventoryDisplayer inventory;
    private RoomCraftingDisplayer crafting;

    private List<Dictionary<string, object>> allItem;

    // slot 사이즈를 늘리고 줄이는데 사용되는 코루틴
    private Coroutine slotSizeCoroutine;
    // slot 사이즈를 늘리고 줄이는데 걸리는 시간, 크기
    private const float slotSizingSpeed = 1;
    private const float maxSlotSize = 1.2f;
    private const float minSlotSize = 1;

    [HideInInspector] public static bool isDrag;
    [HideInInspector] public static RoomItemSlotScript buttonDownSlot;
    [HideInInspector] public static RoomItemSlotScript buttonUpSlot;




    public void Init(int count)
    {
        slotID = count;

        itemImage = transform.Find("Image").GetComponent<Image>();
        itemCountText = transform.Find("Count").GetComponent<Text>();

        manager = FindObjectOfType<RoomDisplayerManager>();
        inventory = manager.inventory;
        crafting = manager.crafting;

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


        allItem = CSVReader.Read("CSV/ItemList");

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
        
        for (var i = 0; i < allItem.Count; i++)
        {
            if (allItem[i]["Index"] as string == itemIndex)
            {
                itemName = allItem[i]["Name"] as string;
                itemExp = allItem[i]["Exp"] as string;

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
    public bool AddCount(int value)
    {
        itemCount += value;

        itemCountText.text = itemCount.ToString();
        return true;
    }


    // 기존에 가지고 있던 아이템의 갯수를 감산, 만약 아이템 수가
    public bool SubtractCount(int value)
    {
        // 현재 slot이 가지고 있는 모든 아이템을 사용
        if (value == this.itemCount)
        {
            ResetSlot();
            return true;
        }
        // 현재 slot이 가지고 있는 아이템보다 많은 아이템을 사용
        else if (value > this.itemCount)
        {
            return false;
        }
        // 현재 slot이 가지고 있는 아이템보다 적은 아이템을 사용
        else
        {
            itemCount -= value;

            itemCountText.text = itemCount.ToString();
            return true;
        }
    }

    void OnPointerEnter(PointerEventData data)
    {
        buttonUpSlot = this;

        // 아이템 정보를 띄움.
        // 단, 가지고 있는 아이템이 없거나 아이템을 Drag 중이면 제외
        if (itemIndex != "" && isDrag == false)
            manager.VisibleExplanationPanel(itemName, itemExp, GetComponent<RectTransform>().position);
    }

    void OnPointerExit(PointerEventData data)
    {
        buttonUpSlot = null;

        // 아이템 정보 표시 칸을 끈다.
        manager.InVisibleExplanationPanel();
    }

    void OnPointerDown(PointerEventData data)
    {
        // 현재 slot에 담겨있는 아이템이 있으면
        if (itemIndex != "")
        {
            // Drag 하는 아이템 이미지에 현재 slot의 아이템을 띄운다.
            manager.dragImage.sprite = itemImage.sprite;
            manager.dragImage.enabled = true;

            // 클릭한 slot의 정보를 기록
            buttonDownSlot = this;

            // Drag 시작
            isDrag = true;
        }

        // 아이템 정보 표시 칸을 끈다.
        manager.InVisibleExplanationPanel();

        // 클릭한 아이템 칸의 크기를 키운다.
        if (slotSizeCoroutine != null)
            StopCoroutine(slotSizeCoroutine);
        slotSizeCoroutine = StartCoroutine(SizeUp());
    }

    void OnPointerUp(PointerEventData data)
    {
        if (isDrag)
        {
            // Drag 종료
            isDrag = false;

            // 두 slot간 정보 교환
            SwapItem();

            // Drag 하는 아이템 이미지
            manager.dragImage.sprite = null;
            manager.dragImage.enabled = false;

            // 클릭한 slot의 정보를 Reset
            buttonDownSlot = null;
            buttonUpSlot = null;
        }

        // 클릭한 아이템 칸의 크기를 줄인다.
        if (slotSizeCoroutine != null)
            StopCoroutine(slotSizeCoroutine);
        slotSizeCoroutine = StartCoroutine(SizeDown());
    }


    // ========================================================= private function =========================================================

    private void SwapItem()
    {
        if (buttonUpSlot == null)
            return;

        if (buttonUpSlot == buttonDownSlot)
            return;

        switch (buttonDownSlot.slotType)
        {
            case SlotType.ALL:
            case SlotType.PARTY:
                switch (buttonUpSlot.slotType)
                {
                    // 인벤토리 -> 인벤토리
                    case SlotType.ALL:
                        inventory.SwapBetweenSlot();
                        break;
                    case SlotType.PARTY:
                        if (itemIndex.ToCharArray()[0] == 'e' || itemIndex.ToCharArray()[0] == 'f')
                        {
                            inventory.SwapBetweenSlot();
                        }
                        break;


                    // 인벤토리 -> 크래프팅(재료 칸)
                    case SlotType.CRAFTING_MATERIAL:
                        if (buttonUpSlot.itemIndex == "")
                        {
                            for (var i = 0; i < allItem.Count; i++)
                            {
                                if (allItem[i]["Index"] as string == buttonDownSlot.itemIndex)
                                {
                                    string type = allItem[i]["Type"] as string;
                                    if (type == "재료" || type == "상위재료")
                                        crafting.SwapBetweenSlot();
                                    break;
                                }
                            }
                        }
                        break;


                    // 인벤토리 -> 크래프팅(무기 칸)
                    case SlotType.CRAFTING_WEAPON:

                        if (buttonUpSlot.itemIndex == "")
                        {
                            for (var i = 0; i < allItem.Count; i++)
                            {
                                if (allItem[i]["Index"] as string == buttonDownSlot.itemIndex)
                                {
                                    string type = allItem[i]["Type"] as string;
                                    if (type == "기본무기" || type == "상위무기")
                                        crafting.SwapBetweenSlot();
                                    break;
                                }
                            }
                        }
                        break;


                    // 인벤토리 -> 캐릭터 무기
                    case SlotType.CHARACTER:

                        for (var i = 0; i < allItem.Count; i++)
                        {
                            if (allItem[i]["Index"] as string == buttonDownSlot.itemIndex)
                            {
                                string type = allItem[i]["Type"] as string;
                                if (type == "기본무기" || type == "상위무기")
                                {
                                    inventory.SwapBetweenSlot();
                                }
                                break;
                            }
                        }

                        if (buttonUpSlot.itemIndex == "")
                            buttonUpSlot.transform.Find("Text").GetComponent<Text>().enabled = true;
                        else
                            buttonUpSlot.transform.Find("Text").GetComponent<Text>().enabled = false;


                        buttonUpSlot.transform.parent.GetComponent<RoomHumanDisplayerSlot>().ItemRegistration();

                        break;
                }
                break;


            case SlotType.CRAFTING_MATERIAL:
                switch (buttonUpSlot.slotType)
                {
                    // 크래프팅(재료 칸) -> 크래프팅(재료 칸)
                    case SlotType.CRAFTING_MATERIAL:
                        crafting.SwapBetweenSlot();
                        break;


                    // 크래프팅(재료 칸) -> 인벤토리(전체)
                    case SlotType.ALL:
                        // 이동시키려는 slot이 비어있으면 그대로 이동
                        if (buttonUpSlot.itemIndex == "")
                            crafting.SwapBetweenSlot();
                        // 이동시키려는 slot에 같은 재료가 있는경우
                        else if (buttonUpSlot.itemIndex == buttonDownSlot.itemIndex)
                        {
                            buttonUpSlot.AddCount(1);
                            buttonDownSlot.ResetSlot();
                        }
                        // 이동시키려는 slot이 다른 재료가 있는경우
                        else
                        {
                            // 전체 인벤토리의 비어있는 칸으로 이동
                            // 만약 전체 인벤토리가 꽉찬경우는 현재 무시
                            inventory.GetItem(-1, buttonDownSlot.itemIndex, 1);
                            buttonDownSlot.ResetSlot();
                        }
                        break;
                }
                break;


            case SlotType.CRAFTING_WEAPON:
                switch (buttonUpSlot.slotType)
                {
                    // 크래프팅(무기 칸) -> 인벤토리(전체)
                    case SlotType.ALL:
                        // 이동시키려는 slot이 비어있으면 그대로 이동
                        if (buttonUpSlot.itemIndex == "")
                            crafting.SwapBetweenSlot();
                        // 이동시키려는 slot이 다른 아이템이 있는경우
                        else
                        {
                            // 전체 인벤토리의 비어있는 칸으로 이동
                            // 만약 전체 인벤토리가 꽉찬경우는 현재 무시
                            inventory.GetItem(-1, buttonDownSlot.itemIndex, 1);
                            buttonDownSlot.ResetSlot();
                        }
                        break;
                }
                break;


            case SlotType.CRAFTING_RESULT:
                switch (buttonUpSlot.slotType)
                {
                    // 크래프팅(결과 칸) -> 인벤토리(전체)
                    case SlotType.ALL:
                        // 이동시키려는 slot이 비어있으면 그대로 이동
                        if (buttonUpSlot.itemIndex == "")
                            crafting.SwapBetweenSlot();
                        // 이동시키려는 slot이 다른 아이템이 있는경우
                        else
                        {
                            // 전체 인벤토리의 비어있는 칸으로 이동
                            // 만약 전체 인벤토리가 꽉찬경우는 현재 무시
                            inventory.GetItem(-1, buttonDownSlot.itemIndex, 1);
                            buttonDownSlot.ResetSlot();
                        }
                        break;
                }
                break;

            case SlotType.CHARACTER:

                switch(buttonUpSlot.slotType)
                {
                    // 캐릭터 무기 -> 인벤토리
                    case SlotType.ALL:

                        if (buttonDownSlot.itemIndex != "")
                        {
                            if(buttonUpSlot.itemIndex == "")
                            {
                                crafting.SwapBetweenSlot();
                            }
                            else
                            {
                                inventory.GetItem(-1, buttonDownSlot.itemIndex, 1);
                                buttonDownSlot.ResetSlot();
                            }

                            buttonDownSlot.transform.Find("Text").GetComponent<Text>().enabled = true;

                            buttonDownSlot.transform.parent.GetComponent<RoomHumanDisplayerSlot>().ItemRegistration();
                        }
                        break;
                }
                break;
        }
    }
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
}
