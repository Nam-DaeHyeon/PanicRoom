using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoomCraftingDisplayer : MonoBehaviour, IRoomDisplayer
{
    private RoomItemSlotScript[] slots;

    // ========================================================= public function =================================================================

    public void Init()
    {
        // 모든 slots 를 가져온다.
        slots = GetComponentsInChildren<RoomItemSlotScript>();

        // 가지고 있는 모든 slot들을 Init 시킨다.
        for (int i = 0; i < slots.Length; i++)
            slots[i].Init(i);

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

    }

    // 드래그를 시작한 slot의 아이템 1개를, 드래그를 마친 slot으로 이동
    public void SwapBetweenSlot()
    {
        string tempIndex = RoomItemSlotScript.buttonDownSlot.itemIndex;

        RoomItemSlotScript.buttonDownSlot.SubtractCount(1);
        RoomItemSlotScript.buttonUpSlot.SetSlot(tempIndex, 1);
    }

    public void OnClickCrafting()
    {
        bool[] isMaterial = { false, false, false, false };
        bool canMix = false;
        int resultIndex = -1;

        // MixList CSV 파일을 순회하며 하나씩 Mix 정보를 파싱
        List<Dictionary<string, object>> data = CSVReader.Read("CSV/MixList");
        for(int i = 0; i < data.Count; i++)
        {
            // Mix 정보와 현재 무기 slot에 올려져있는 정보가 같은 경우만 확인
            if (slots[4].itemName != data[i]["Weapon"] as string)
            {
                canMix = false;
                continue;
            }

            // 재료를 비교
            for (int j = 1; j <= 4; j++)
            {
                canMix = false;
                
                for(int k = 0; k < 4; k++)
                {
                    if(isMaterial[k] == false && data[i]["Material" + j.ToString()] as string == slots[k].itemName)
                    {
                        isMaterial[k] = true;
                        canMix = true;
                        break;
                    }
                }

                if (canMix == false)
                    break;
            }

            if (canMix == false)
                continue;

            // 무기가 올바르게 올려져 있는지 확인
            if (slots[4].itemName == data[i]["Weapon"] as string)
            {
                canMix = true;
                resultIndex = i;
                break;
            }
            else
            {
                canMix = false;
                continue;
            }
        }

        // 조합 실패
        if(!canMix)
        {
            // 무기 조합 실패
            if (slots[4].itemName != "")
            {
                // 재료들을 비운다.
                slots[0].ResetSlot();
                slots[1].ResetSlot();
                slots[2].ResetSlot();
                slots[3].ResetSlot();

                // 무기 slot에 있던 무기가 Result slot으로 이동
                string tempIndex = slots[4].itemIndex;
                slots[4].ResetSlot();
                slots[5].SetSlot(tempIndex, 1);
            }
        }
        // 조합 성공
        else
        {
            // 재료들을 비운다.
            slots[0].ResetSlot();
            slots[1].ResetSlot();
            slots[2].ResetSlot();
            slots[3].ResetSlot();
            slots[4].ResetSlot();

            // 생성되는 아이템 정보 확인
            string tempName = data[resultIndex]["T_Result"] as string;
            string tempIndex = "";
            List<Dictionary<string, object>> tempData = CSVReader.Read("CSV/ItemList");
            for(int temp = 0; temp < tempData.Count; temp++)
            {
                if(tempData[temp]["Name"] as string == tempName)
                {
                    tempIndex = tempData[temp]["Index"] as string;
                    break;
                }
            }

            // Result에 결과를 반환
            slots[5].SetSlot(tempIndex, 1);
        }
    }
}
