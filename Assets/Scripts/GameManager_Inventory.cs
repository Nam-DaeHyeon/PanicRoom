using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using UnityEngine.JSONSerializeModule;

[Serializable]
public class ItemSlot
{
    public string itemId;
    public int itemCount;

    public ItemSlot() { }
    public ItemSlot(string id, int count)
    {
        itemId = id;
        itemCount = count;
    }
}

public partial class GameManager : MonoBehaviour
{
    [Header("Inventory")]
    public List<ItemSlot> total_Inventory = new List<ItemSlot>();
    public ItemSlot[] party_Inventory = new ItemSlot[8];

    #region 아이템 추가
    public void Add_Item_TotalInventory(string itemId, int amount)
    {
        ItemSlot item = new ItemSlot();
        string toJson = "";
        int tempAmount = amount;
        Item getItem = Load_Item_DB(itemId);

        for (int i = 0; i < total_Inventory.Count; i++)
        {
            if (total_Inventory[i].itemId.Equals(itemId))
            {
                if (getItem.Get_CanOverLap())
                {
                    //인벤토리에 있을 경우 (겹치기 가능)
                    item.itemId = itemId;
                    item.itemCount = total_Inventory[i].itemCount + amount;

                    //최대허용량 초과
                    if (item.itemCount > getItem.Get_OverLapCount())
                    {
                        //초과량
                        int subCount = item.itemCount - getItem.Get_OverLapCount();
                        //최대량
                        item.itemCount = getItem.Get_OverLapCount();

                        total_Inventory[i] = item;
                        total_Inventory.Add(new ItemSlot(itemId, subCount));
                    }
                    else
                    {
                        total_Inventory[i] = item;  //Replace
                    }

                    toJson = JsonHelper.ToJson(total_Inventory.ToArray(), prettyPrint: true);
                    File.WriteAllText(Application.dataPath + "/Resources/Inventory.json", toJson);
                    return;
                }
                else break;
            }
        }
        
        //인벤토리에 아이템이 없을 경우 (겹치기 가능)
        if (getItem.Get_CanOverLap())
        {
            item.itemId = itemId;
            item.itemCount = amount;
            total_Inventory.Add(item);  //Replace
        }
        //인벤토리에 있을 경우 (겹치기 불가능)
        //인벤토리에 아이템이 없을 경우
        else
        {
            for (int i = 0; i < amount; i++)
            {
                item.itemId = itemId;
                item.itemCount = 1;
                total_Inventory.Add(item);
            }
        }

        toJson = JsonHelper.ToJson(total_Inventory.ToArray(), prettyPrint: true);
        File.WriteAllText(Application.dataPath + "/Resources/Inventory.json", toJson);
    }

    public void Add_Item_PartyInventory(string itemId, int amount)
    {
        ItemSlot item = new ItemSlot();
        int startIdx = 0;

        for (int i = 0; i < party_Inventory.Length; i++)
        {
            if(party_Inventory[i].itemId == null || party_Inventory[i].itemId.Equals(""))
            {
                startIdx = i;
                break;
            }

            if (party_Inventory[i].itemId.Equals(itemId))
            {
                Item getItem = Load_Item_DB(itemId);
                //파티 인벤토리에 있을 경우 (겹치기 가능)
                if (getItem.Get_CanOverLap())
                {
                    item.itemId = itemId;
                    item.itemCount = party_Inventory[i].itemCount + amount;

                    //최대허용량 초과
                    if (item.itemCount > getItem.Get_OverLapCount())
                    {
                        //초과량
                        int subCount = item.itemCount - getItem.Get_OverLapCount();
                        //최대량
                        item.itemCount = getItem.Get_OverLapCount();

                        party_Inventory[i] = item;

                        //빈 공간 또는 같은 아이템 공간 검색
                        int putIdx = Get_Index_EmptyORSameSlot_PartyInventory(itemId, subCount);
                        party_Inventory[putIdx].itemId = itemId;
                        party_Inventory[putIdx].itemCount += subCount;
                    }
                    else
                    {
                        party_Inventory[i] = item;  //Replace
                    }

                    return;
                }
                else break;
            }
        }

        //파티 인벤토리에 있을 경우 (겹치기 불가능)
        //파티 인벤토리에 아이템이 없을 경우
        for (int i = 0; i < amount; i++)
        {
            if (startIdx + i >= party_Inventory.Length) break;

            item.itemId = itemId;
            item.itemCount = 1;
            party_Inventory[startIdx + i] = item;
        }
    }
    #endregion

    #region 아이템 제거
    public void Remove_Item_TotalInventory(string itemId, bool isAll = false)
    {
        ItemSlot item = new ItemSlot();
        string toJson = "";

        for (int i = 0; i < total_Inventory.Count; i++)
        {
            if (total_Inventory[i].itemId.Equals(itemId))
            {
                //인벤토리에 있을 경우 (겹치기 가능)
                if (Load_Item_DB(itemId).Get_CanOverLap())
                {
                    if (!isAll)
                    {
                        //일부(1개)만 제거한다.
                        item.itemId = itemId;
                        item.itemCount = total_Inventory[i].itemCount - 1;
                        total_Inventory[i] = item;  //Replace
                        if (total_Inventory[i].itemCount <= 0) total_Inventory.RemoveAt(i);
                    }
                    else
                    {
                        //전부 제거한다.
                        total_Inventory.RemoveAt(i);
                    }
                    break;
                }
                //(겹치기 불가능)
                else
                {
                    total_Inventory.RemoveAt(i);
                    break;
                }
            }
        }

        toJson = JsonHelper.ToJson(total_Inventory.ToArray(), prettyPrint: true);
        File.WriteAllText(Application.dataPath + "/Resources/Inventory.json", toJson);
    }

    public void Remove_Item_PartyInventory(string itemId, bool isAll = false)
    {
        ItemSlot item = new ItemSlot();
       
        for (int i = 0; i < party_Inventory.Length; i++)
        {
            string tempStr = party_Inventory[i].itemId;
            if (tempStr == null) continue;
            tempStr = tempStr.Trim();
            if (tempStr.Equals("")) continue;
            
            if (tempStr.Equals(itemId))
            {
                //파티 인벤토리에 있을 경우 (겹치기 가능)
                if (Load_Item_DB(itemId).Get_CanOverLap())
                {
                    if (!isAll)
                    {
                        //일부(1개)만 제거한다.
                        item.itemId = itemId;
                        item.itemCount = party_Inventory[i].itemCount - 1;
                        party_Inventory[i] = item;
                        if (party_Inventory[i].itemCount <= 0)
                        {
                            party_Inventory[i].itemId = null;
                            party_Inventory[i].itemCount = 0;
                        }
                    }
                    else
                    {
                        //전부 제거한다.
                        party_Inventory[i].itemId = null;
                        party_Inventory[i].itemCount = 0;
                    }
                }
                //(겹치기 불가능)
                else
                {
                    party_Inventory[i].itemId = null;
                    party_Inventory[i].itemCount = 0;
                }
            }

            return;
        }
    }

    public void Remove_Item_PartyInventory_CallIndex(int index)
    {
        if (index >= party_Inventory.Length) return;
        party_Inventory[index].itemId = null;
        party_Inventory[index].itemCount = 0;
    }
    #endregion

   public int Get_Index_EmptyORSameSlot_PartyInventory(string id, int count)
    {
        //같은 아이템일 때
        for (int i = 0; i < party_Inventory.Length; i++)
        {
            if (id.Equals(party_Inventory[i].itemId))
            {
                //여유공간이 있을 때
                int leftCount = Load_Item_DB(id).Get_OverLapCount() - party_Inventory[i].itemCount;
                if (leftCount > count)
                {
                    return i;
                }
            }

        }

        //빈 슬롯일때
        for (int i = 0; i < party_Inventory.Length; i++)
            if (party_Inventory[i].itemId == null) return i;
            else if (party_Inventory[i].itemId.Equals("")) return i;

        return -1;
    }

    void Debug_Items_TotalInventory()
    {
        string jsonString = File.ReadAllText(Application.dataPath + "/Resources/Inventory.json");
        var data = JsonHelper.FromJson<ItemSlot>(jsonString);
        
        foreach (var item in data)
        {
            Debug.Log(item.itemId + " (" + item.itemCount + ")");
        }
    }
}
