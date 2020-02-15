using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class GameManager : MonoBehaviour
{
    private const char 재료 = 'a';
    private const char 상위재료 = 'b';
    private const char 기본무기 = 'c';
    private const char 상위무기 = 'd';
    private const char 소모_상태이상 = 'e';
    private const char 소모 = 'f';

    //아이템 종류에 따른 확률
    [Header("Reward Rate of Item' Type")]
    [SerializeField] int rw_type_a = 60;        //재료
    [SerializeField] int rw_type_b = 5;         //상위재료
    [SerializeField] int rw_type_c = 4;         //기본무기
    [SerializeField] int rw_type_d = 1;         //상위무기
    [SerializeField] int rw_type_e = 10;        //소모(상태이상)
    [SerializeField] int rw_type_f = 20;        //소모

    [Header("ItemBox Rate of Item' Type")]
    [SerializeField] int itb_type_a = 60;        //재료
    [SerializeField] int itb_type_b = 5;         //상위재료
    [SerializeField] int itb_type_c = 4;         //기본무기
    [SerializeField] int itb_type_d = 1;         //상위무기
    [SerializeField] int itb_type_e = 10;        //소모(상태이상)
    [SerializeField] int itb_type_f = 20;        //소모

    public Item Load_Item_DB(string itemId)
    {
        Item getItem = null;
        if (itemId == null || itemId.Equals("")) return getItem;

        List<Dictionary<string, object>> ItemList = CSVReader.Read("CSV/ItemList");

        ItemOption[] tempOpt = null;
        int[] tempValue = new int[3];

        //아이템 데이터베이스 탐색
        for (int k = 0; k < ItemList.Count; k++)
        {
            if (itemId.Equals((string)ItemList[k]["Index"]))
            {
                switch (itemId.ToCharArray()[0])
                {
                    case 재료:
                        getItem = new Item(itemId, ItemType.재료, (string)ItemList[k]["Name"], (string)ItemList[k]["Exp"], true, (int)ItemList[k]["OverlapCount"]);
                        break;
                    case 상위재료:
                        getItem = new Item(itemId, ItemType.상위재료, (string)ItemList[k]["Name"], (string)ItemList[k]["Exp"], true, (int)ItemList[k]["OverlapCount"]);
                        break;
                    case 기본무기:
                        tempOpt = new ItemOption[] { Get_ItemOption((string)ItemList[k]["Effect1"]), Get_ItemOption((string)ItemList[k]["Effect2"]), Get_ItemOption((string)ItemList[k]["Effect3"]) };
                        if (ItemList[k]["Effect1Value"] is int) { tempValue[0] = (int)ItemList[k]["Effect1Value"]; }
                        else { tempValue[0] = Get_ItemValue((string)ItemList[k]["Effect1Value"]); }
                        if (ItemList[k]["Effect2Value"] is int) { tempValue[1] = (int)ItemList[k]["Effect2Value"]; }
                        else { tempValue[1] = Get_ItemValue((string)ItemList[k]["Effect2Value"]); }
                        if (ItemList[k]["Effect3Value"] is int) { tempValue[2] = (int)ItemList[k]["Effect3Value"]; }
                        else { tempValue[2] = Get_ItemValue((string)ItemList[k]["Effect3Value"]); }

                        getItem = new Item(itemId, ItemType.기본무기, (string)ItemList[k]["Name"], (string)ItemList[k]["Exp"], false, 1,
                                           tempOpt[0], tempValue[0], tempOpt[1], tempValue[1], tempOpt[2], tempValue[2] );
                        break;
                    case 상위무기:
                        tempOpt = new ItemOption[]{ Get_ItemOption((string)ItemList[k]["Effect1"]), Get_ItemOption((string)ItemList[k]["Effect2"]), Get_ItemOption((string)ItemList[k]["Effect3"]) };
                        if (ItemList[k]["Effect1Value"] is int) { tempValue[0] = (int)ItemList[k]["Effect1Value"]; }
                        else { tempValue[0] = Get_ItemValue((string)ItemList[k]["Effect1Value"]); }
                        if (ItemList[k]["Effect2Value"] is int) { tempValue[1] = (int)ItemList[k]["Effect2Value"]; }
                        else { tempValue[1] = Get_ItemValue((string)ItemList[k]["Effect2Value"]); }
                        if (ItemList[k]["Effect3Value"] is int) { tempValue[2] = (int)ItemList[k]["Effect3Value"]; }
                        else { tempValue[2] = Get_ItemValue((string)ItemList[k]["Effect3Value"]); }

                        getItem = new Item(itemId, ItemType.상위무기, (string)ItemList[k]["Name"], (string)ItemList[k]["Exp"], false, 1,
                                           tempOpt[0], tempValue[0], tempOpt[1], tempValue[1], tempOpt[2], tempValue[2]);
                        break;
                    case 소모_상태이상:
                        tempOpt = new ItemOption[] { Get_ItemOption((string)ItemList[k]["Effect1"]), Get_ItemOption((string)ItemList[k]["Effect2"]), Get_ItemOption((string)ItemList[k]["Effect3"]) };
                        if (ItemList[k]["Effect1Value"] is int) { tempValue[0] = (int)ItemList[k]["Effect1Value"]; }
                        else { tempValue[0] = Get_ItemValue((string)ItemList[k]["Effect1Value"]); }
                        if (ItemList[k]["Effect2Value"] is int) { tempValue[1] = (int)ItemList[k]["Effect2Value"]; }
                        else { tempValue[1] = Get_ItemValue((string)ItemList[k]["Effect2Value"]); }
                        if (ItemList[k]["Effect3Value"] is int) { tempValue[2] = (int)ItemList[k]["Effect3Value"]; }
                        else { tempValue[2] = Get_ItemValue((string)ItemList[k]["Effect3Value"]); }

                        getItem = new Item(itemId, ItemType.상위무기, (string)ItemList[k]["Name"], (string)ItemList[k]["Exp"], true, (int)ItemList[k]["OverlapCount"],
                                           tempOpt[0], tempValue[0], tempOpt[1], tempValue[1], tempOpt[2], tempValue[2]);
                        break;
                    case 소모:
                        tempOpt = new ItemOption[] { Get_ItemOption((string)ItemList[k]["Effect1"]), Get_ItemOption((string)ItemList[k]["Effect2"]), Get_ItemOption((string)ItemList[k]["Effect3"]) };
                        if (ItemList[k]["Effect1Value"] is int) { tempValue[0] = (int)ItemList[k]["Effect1Value"]; }
                        else { tempValue[0] = Get_ItemValue((string)ItemList[k]["Effect1Value"]); }
                        if (ItemList[k]["Effect2Value"] is int) { tempValue[1] = (int)ItemList[k]["Effect2Value"]; }
                        else { tempValue[1] = Get_ItemValue((string)ItemList[k]["Effect2Value"]); }
                        if (ItemList[k]["Effect3Value"] is int) { tempValue[2] = (int)ItemList[k]["Effect3Value"]; }
                        else { tempValue[2] = Get_ItemValue((string)ItemList[k]["Effect3Value"]); }

                        getItem = new Item(itemId, ItemType.상위무기, (string)ItemList[k]["Name"], (string)ItemList[k]["Exp"], false, 1,
                                           tempOpt[0], tempValue[0], tempOpt[1], tempValue[1], tempOpt[2], tempValue[2]);
                        break;
                }

                break;
            }
        }

        if (getItem != null) getItem.Debug_ItemInformation();
        return getItem;
    }

    ItemOption Get_ItemOption(string optKey)
    {
        ItemOption getOpt = ItemOption.NONE;

        switch (optKey)
        {
            case "공격력 상승":
                getOpt = ItemOption.공격력상승;
                break;
            case "원거리":
                getOpt = ItemOption.원거리;
                break;
            case "데미지 (적 지정)":
                getOpt = ItemOption.데미지_단일적군;
                break;
            case "데미지 (적 전체)":
                getOpt = ItemOption.데미지_전체적군;
                break;
            case "체력회복 (아군 지정)":
                getOpt = ItemOption.체력회복_단일아군;
                break;
            case "출혈 효과 제거":
                getOpt = ItemOption.출혈효과제거;
                break;
            case "방사능 피폭 제거 (아군 지정)":
                getOpt = ItemOption.방사능피폭제거_단일아군;
                break;
            case "방사능 피폭률 완전 제거 (아군 지정)":
                getOpt = ItemOption.방사능피폭완전제거_단일아군;
                break;
            case "둔화":
                getOpt = ItemOption.혼란;
                break;
            case "공포":
                getOpt = ItemOption.공포;
                break;
            case "출혈":
                getOpt = ItemOption.출혈;
                break;
            case "화상":
                getOpt = ItemOption.화상;
                break;
            default:
                getOpt = ItemOption.NONE;
                break;
        }

        return getOpt;
    }

    int Get_ItemValue(string valueKey)
    {
        //최소~최대로 입력된 경우
        if (valueKey.Contains("~"))
        {
            string[] tempNumStr = valueKey.Split('~');
            int minNum = 0;
            int maxNum = 0;
            System.Int32.TryParse(tempNumStr[0], out minNum);

            string strTmp = System.Text.RegularExpressions.Regex.Replace(tempNumStr[1], @"\D", "");
            maxNum = int.Parse(strTmp);

            //int getValue = Random.Range(minNum, maxNum + 1);
            int getValue = (int)((minNum + maxNum) * 0.5f); //임시(190526) 중간값 보정.
            if (getValue == maxNum + 1) getValue = maxNum;  //최대값 보정.

            return getValue;
        }
        else
        {
            //현재(2018.05.18) 개발단계에서 상태이상도 값을 0으로 입력한다.
            //문자로만 입력된 경우
            return 0;
        }
    }

    public string Get_Random_ItemId()
    {
        List<Dictionary<string, object>> ItemList = CSVReader.Read("CSV/ItemList");

        int randIndex = Random.Range(0, ItemList.Count);
        if (randIndex == ItemList.Count) randIndex -= 1;

        return ItemList[randIndex]["Index"] as string;
    }

    public Item Get_Random_Item_ExceptRareWeapon()
    {
        List<Dictionary<string, object>> ItemList = CSVReader.Read("CSV/ItemList");

        int randIndex = 0;
        string itemId = "";
        while (true)
        {
            randIndex = Random.Range(0, ItemList.Count);
            if (randIndex == ItemList.Count) randIndex -= 1;

            itemId = ItemList[randIndex]["Index"] as string;
            if (itemId.ToCharArray()[0] != 'd') break;
        }

        return Load_Item_DB(itemId);
    }
    
    public Item Get_Random_Item_RewardSpecialRate()
    {
        List<Dictionary<string, object>> ItemList = CSVReader.Read("CSV/ItemList");
        string itemId = null;

        int tempCount = 0;
        string tempId = null;
        int randomKey = Random.Range(0, 100);

        //아이템 선정.
        tempCount = 0;
        if (randomKey <= rw_type_a) tempId = "재료";
        else
        {
            tempCount += rw_type_a;
            if (randomKey <= rw_type_b + tempCount) tempId = "상위재료";
            else
            {
                tempCount += rw_type_b;
                if (randomKey <= rw_type_c + tempCount) tempId = "기본무기";
                else
                {
                    tempCount += rw_type_c;
                    if (randomKey <= rw_type_d + tempCount) tempId = "상위무기";
                    else
                    {
                        tempCount += rw_type_d;
                        if (randomKey <= rw_type_e + tempCount) tempId = "소모(상태이상)";
                        else
                        {
                            tempCount += rw_type_e;
                            if (randomKey <= rw_type_f + tempCount) tempId = "소모";
                        }
                    }
                }
            }
        }

        //아이템 생성.
        //1. 해당 타입 시작, 종료 인덱스 탐색.
        int startIdx = -1;
        int endIdx = -1;
        char tempChar = 'a';
        switch (tempId)
        {
            case "재료": tempChar = 재료; break;
            case "상위재료": tempChar = 상위재료; break;
            case "기본무기": tempChar = 기본무기; break;
            case "상위무기": tempChar = 상위무기; break;
            case "소모(상태이상)": tempChar = 소모_상태이상; break;
            case "소모": tempChar = 소모; break;
        }

        for(int i = 0; i < ItemList.Count; i++)
        {
            itemId = ItemList[i]["Index"] as string;
            //시작 인덱스 저장.
            if (itemId.ToCharArray()[0] == tempChar && startIdx == -1)
            {
                startIdx = i;
            }
            //종료 인덱스 저장
            if (itemId.ToCharArray()[0] != tempChar && startIdx != -1)
            {
                endIdx = i;
            }
            
            if (endIdx != -1) break;
        }
        if (endIdx == -1) endIdx = ItemList.Count;

        //2. 영역내 랜덤 아이템 생성.
        int randIndex = Random.Range(startIdx, endIdx);
        if (randIndex == endIdx) randIndex = endIdx - 1;    //최댓값 보정. (오버플로우 방지)

        itemId = ItemList[randIndex]["Index"] as string;

        //아이템 반환.
        return Load_Item_DB(itemId);
    }

    public Item Get_Random_Item_ItemBoxSpecialRate()
    {
        List<Dictionary<string, object>> ItemList = CSVReader.Read("CSV/ItemList");
        string itemId = null;

        int tempCount = 0;
        string tempId = null;
        int randomKey = Random.Range(0, 100);

        //아이템 선정.
        tempCount = 0;
        if (randomKey <= itb_type_a) tempId = "재료";
        else
        {
            tempCount += itb_type_a;
            if (randomKey <= itb_type_b + tempCount) tempId = "상위재료";
            else
            {
                tempCount += itb_type_b;
                if (randomKey <= itb_type_c + tempCount) tempId = "기본무기";
                else
                {
                    tempCount += itb_type_c;
                    if (randomKey <= itb_type_d + tempCount) tempId = "상위무기";
                    else
                    {
                        tempCount += itb_type_d;
                        if (randomKey <= itb_type_e + tempCount) tempId = "소모(상태이상)";
                        else
                        {
                            tempCount += itb_type_e;
                            if (randomKey <= itb_type_f + tempCount) tempId = "소모";
                        }
                    }
                }
            }
        }

        //아이템 생성.
        //1. 해당 타입 시작, 종료 인덱스 탐색.
        int startIdx = -1;
        int endIdx = -1;
        char tempChar = 'a';
        switch (tempId)
        {
            case "재료": tempChar = 재료; break;
            case "상위재료": tempChar = 상위재료; break;
            case "기본무기": tempChar = 기본무기; break;
            case "상위무기": tempChar = 상위무기; break;
            case "소모(상태이상)": tempChar = 소모_상태이상; break;
            case "소모": tempChar = 소모; break;
        }

        for (int i = 0; i < ItemList.Count; i++)
        {
            itemId = ItemList[i]["Index"] as string;
            //시작 인덱스 저장.
            if (itemId.ToCharArray()[0] == tempChar && startIdx == -1)
            {
                startIdx = i;
            }
            //종료 인덱스 저장
            if (itemId.ToCharArray()[0] != tempChar && startIdx != -1)
            {
                endIdx = i;
            }

            if (endIdx != -1) break;
        }
        if (endIdx == -1) endIdx = ItemList.Count;

        //2. 영역내 랜덤 아이템 생성.
        int randIndex = Random.Range(startIdx, endIdx);
        if (randIndex == endIdx) randIndex = endIdx - 1;    //최댓값 보정. (오버플로우 방지)

        itemId = ItemList[randIndex]["Index"] as string;

        //아이템 반환.
        return Load_Item_DB(itemId);
    }

    void Test_InventorySet()
    {
        Add_Item_TotalInventory("a0001", 998);
        Add_Item_TotalInventory("a0002", 1);
        Add_Item_TotalInventory("a0003", 3);
        Add_Item_TotalInventory("a0001", 10);
        Add_Item_TotalInventory("c0001", 2);
        Add_Item_TotalInventory("c0001", 1);

        Add_Item_TotalInventory("e0001", 3);
        Add_Item_TotalInventory("e0002", 3);
        Add_Item_TotalInventory("e0003", 1);
        Add_Item_TotalInventory("f0001", 3);

        Add_Item_TotalInventory("a0001", 900);
    }
}
