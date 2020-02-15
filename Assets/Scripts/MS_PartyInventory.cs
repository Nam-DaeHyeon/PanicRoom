using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MS_PartyInventory : MonoBehaviour
{
    [SerializeField] MS_PartyInventorySlot[] _slots = new MS_PartyInventorySlot[8];
    //public Item dragItem;
    public Image dragImage;
    public int dragIndex = -1;

    private void OnEnable()
    { 
        Init_Slots();
    }

    public void Init_Slots()
    {
        Item tempItem;

        for (int i = 0; i < _slots.Length; i++)
        {
            tempItem = GameManager.instance.Load_Item_DB(GameManager.instance.party_Inventory[i].itemId);

            if (tempItem != null)
            {
                _slots[i].icon.gameObject.SetActive(true);
                _slots[i].item = tempItem;
                _slots[i].icon.sprite = _slots[i].item.Get_ItemImage();
                _slots[i].count.text = GameManager.instance.party_Inventory[i].itemCount.ToString();
            }
            else
            {
                _slots[i].item = null;
                _slots[i].icon.gameObject.SetActive(false);
            }
        }
    }

    public void Control_InteratableButtons(bool setup)
    {
        for(int i = 0; i < _slots.Length; i++)
        {
            _slots[i].Interactable_Button(setup);
        }
    }

    public void Delete_Item()
    {
        GameManager.instance.Remove_Item_PartyInventory_CallIndex(dragIndex);
        Init_Slots();
    }

    public void Follow_DragImage_OnMousePointer()
    {
        StartCoroutine(IE_Follow_DragImage_OnMousePointer());
    }

    IEnumerator IE_Follow_DragImage_OnMousePointer()
    {
        while(dragIndex != -1)
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            dragImage.transform.position = mousePos;
            yield return null;
        }
    }
}
