using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoomInventoryDisplayer : MonoBehaviour, IRoomDisplayer
{
    private RoomItemSlotScript[] slots;

    public GameObject all;
    public GameObject party;

    // ========================================================= public function =================================================================

    public void Init()
    {
        // 모든 slots 를 가져온다.
        slots = GetComponentsInChildren<RoomItemSlotScript>();

        // 가지고 있는 모든 slot들을 Init 시킨다.
        for (int i = 0; i < slots.Length; i++)
            slots[i].Init(i);
        
        for(int i = 0; i < GameManager.instance.total_Inventory.Count; i++)
        {
            GetItem(-1, GameManager.instance.total_Inventory[i].itemId, GameManager.instance.total_Inventory[i].itemCount);
        }

        GetItem(20, "a0002", 50);
        GetItem(21, "a0004", 50);
        GetItem(22, "a0006", 50);
        GetItem(23, "a0005", 50);
    }

    public void OnInvisible()
    {
        gameObject.SetActive(false);
    }

    public void OnVisible()
    {
        ResetDisplayer();

        gameObject.SetActive(true);
    }

    public void ResetDisplayer()
    {
        all.SetActive(true);
        party.SetActive(true);
    }

    public void OnVisibleAll()
    {
        all.SetActive(true);
        party.SetActive(false);

        gameObject.SetActive(true);
    }

    public void OnInvisibleAll()
    {
        all.SetActive(false);

        if(party.activeSelf == false)
            gameObject.SetActive(false);
    }

    public void OnInvisibleParty()
    {
        party.SetActive(false);

        if (all.activeSelf == false)
            gameObject.SetActive(false);
    }




    public void GetItem(int slotIndex, string itemIndex, int itemCount)
    {
        // 빈 slot을 찾아서 아이템을 넣음
        if(slotIndex == -1)
        {
            // 전체 인벤토리 중 넣는 아이템과 같은 아이템이 있는 slot의 count를 증가
            for(int i = 0; i < slots.Length; i++)
            {
                if(slots[i].slotType.Equals(SlotType.ALL))
                {
                    if(slots[i].itemIndex.Equals(itemIndex))
                    {
                        Item loadedItem = GameManager.instance.Load_Item_DB(itemIndex);
                        //겹치기 불가능이라면 프로세스 건너뛰기.
                        if (!loadedItem.Get_CanOverLap()) break;

                        slots[i].AddCount(itemCount);
                        return;
                    }
                }
            }

            // 기존에 가지고 있는 아이템이 아니므로 비어있는 slot을 찾아서 아이템을 새로 추가
            for (int i = 0; i < slots.Length; i++)
            {
                if (slots[i].slotType.Equals(SlotType.ALL))
                {
                    if (slots[i].itemIndex == "")
                    {
                        slots[i].SetSlot(itemIndex, itemCount);
                        return;
                    }
                }
            }

        }
        // 특정 slot에 아이템을 넣음
        else
        {
            slots[slotIndex].SetSlot(itemIndex, itemCount);
        }
    }

    // 두 slot의 아이템을 정보를 교환한다.
    public void SwapBetweenSlot()
    {
        // 두 slot의 물건이 같다면..
        if(RoomItemSlotScript.buttonDownSlot.itemIndex == RoomItemSlotScript.buttonUpSlot.itemIndex)
        {
            // 겹칠 수 있을 때..
            Item loadedItem = GameManager.instance.Load_Item_DB(RoomItemSlotScript.buttonUpSlot.itemIndex);
            if (loadedItem.Get_CanOverLap())
            {
                // 두 아이템 수를 합친다.
                RoomItemSlotScript.buttonUpSlot.AddCount(RoomItemSlotScript.buttonDownSlot.itemCount);
                RoomItemSlotScript.buttonDownSlot.ResetSlot();

                return;
            }
        }


        string tempIndex = RoomItemSlotScript.buttonDownSlot.itemIndex;
        int tempCount = RoomItemSlotScript.buttonDownSlot.itemCount;

        RoomItemSlotScript.buttonDownSlot.SetSlot(RoomItemSlotScript.buttonUpSlot.itemIndex, RoomItemSlotScript.buttonUpSlot.itemCount);
        RoomItemSlotScript.buttonUpSlot.SetSlot(tempIndex, tempCount);


        SavePartyInventory();
    }

    public void SavePartyInventory()
    {
        if (!RoomItemSlotScript.buttonUpSlot.slotType.Equals(SlotType.PARTY)) return;

        // 파티 인벤토리의 slot 수는 8개
        for(int i = 0; i < 8; i++)
        {
            GameManager.instance.party_Inventory[i].itemId = slots[24 + i].itemIndex;
            GameManager.instance.party_Inventory[i].itemCount = slots[24 + i].itemCount;
        }
    }
}
