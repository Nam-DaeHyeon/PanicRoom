using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MS_PartyInventorySlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] private MS_PartyInventory _inventoryUI;
    [SerializeField] private int _currSlotIndex;

    public Item item;
    public Image icon;
    public Text count;

    Button _button;

    private void Awake()
    {
        _button = GetComponent<Button>();
    }

    public void Interactable_Button(bool setup)
    {
        if(_button == null) _button = GetComponent<Button>();
        _button.interactable = setup;
    }

    public void UI_Button_Click()
    {
        if (StageManager.instance.gameState.Equals(GameState.BATTLE))
        {
            if (StageManager.instance.readyItem == null || !StageManager.instance.readyItem.Equals(item))
            {
                if (item != null)
                {
                    StageManager.instance.readyItem = item;
                    StageManager.instance.Image_ReadyItem.sprite = item.Get_ItemImage();
                    StageManager.instance.Image_ReadyItem.gameObject.SetActive(true);
                }
            }
            else
            {
                StageManager.instance.readyItem = null;
                StageManager.instance.Image_ReadyItem.gameObject.SetActive(false);
            }
        }
    }

    void IPointerDownHandler.OnPointerDown(PointerEventData data)
    {
        _inventoryUI.dragIndex = _currSlotIndex;

        if (_inventoryUI.dragIndex != -1)
        {
            if (GameManager.instance.party_Inventory[_currSlotIndex].itemId == null || GameManager.instance.party_Inventory[_currSlotIndex].itemId.Equals("")) return;
            _inventoryUI.dragImage.sprite = GameManager.instance.Load_Item_DB(GameManager.instance.party_Inventory[_currSlotIndex].itemId).Get_ItemImage();
            _inventoryUI.dragImage.gameObject.SetActive(true);

            _inventoryUI.Follow_DragImage_OnMousePointer();
        }
    }

    void IPointerUpHandler.OnPointerUp(PointerEventData data)
    {
        //특정 영역(인벤토리창)제외하고 삭제.
        if (data.pointerEnter == null ||
            (!data.pointerEnter.name.Equals("Page_PartyInventory") && !data.pointerEnter.name.Contains("ItemButton")))
        {
            _inventoryUI.Delete_Item();

        }

        _inventoryUI.dragIndex = -1;
        _inventoryUI.dragImage.gameObject.SetActive(false);
    }

    void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
    {
        if (item == null) return;

        string tempStr = "";

        StageManager.instance.MOVER_Name_Item.text = item.Get_ItemName();
        for (int i = 0; i < item.Get_ItemOptions().Length; i++)
        {
            if (item.Get_ItemOptions()[i].Equals(ItemOption.방사능피폭완전제거_단일아군)
                || item.Get_ItemOptions()[i].Equals(ItemOption.방사능피폭제거_단일아군)
                || item.Get_ItemOptions()[i].Equals(ItemOption.체력회복_단일아군)
                || item.Get_ItemOptions()[i].Equals(ItemOption.출혈효과제거))
            {
                tempStr = "단일 아군";
                break;
            }
        }
        StageManager.instance.MOVER_OptionAlly_Item.text = tempStr;

        tempStr = "";
        for (int i = 0; i < item.Get_ItemOptions().Length; i++)
        {
            if (item.Get_ItemOptions()[i].Equals(ItemOption.데미지_단일적군))
            {
                tempStr = "단일 적군";
                break;
            }
            if (item.Get_ItemOptions()[i].Equals(ItemOption.데미지_전체적군))
            {
                tempStr = "전체 적군";
                break;
            }
        }
        StageManager.instance.MOVER_OptionEnemy_Item.text = tempStr;

        StageManager.instance.MOVER_Comment_Item.text = item.Get_ItemComment();

        StageManager.instance.MOVER_FrameUI_Item.transform.position = transform.position;
        StageManager.instance.MOVER_FrameUI_Item.SetActive(true);
    }

    void IPointerExitHandler.OnPointerExit(PointerEventData eventData)
    {
        StageManager.instance.MOVER_FrameUI_Item.SetActive(false);
    }
}
