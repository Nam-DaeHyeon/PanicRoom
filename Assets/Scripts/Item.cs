using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemType
{
    재료,
    상위재료,
    기본무기,
    상위무기,
    소모_상태이상,
    소모
}

public enum ItemOption
{
    NONE,
    공격력상승,
    원거리,
    데미지_단일적군,
    데미지_전체적군,
    체력회복_단일아군,
    출혈효과제거,
    방사능피폭제거_단일아군,
    방사능피폭완전제거_단일아군,
    출혈,
    혼란,
    공포,
    화상
}

public struct ItemSource
{
    string srcItemId;
    int srcCount;

    public ItemSource(string itemId, int n) : this()
    {
        srcItemId = itemId;
        srcCount = n;
    }
}

public class Item
{
    protected string _itemName;
    protected string _itemId;
    protected string _itemComment;
    protected ItemType _itemType;
    protected bool _canOverlap;
    protected int _overlapCount;

    protected ItemOption[] _options = new ItemOption[3];
    protected int[] _optionValues = new int[3];

    //조합 재료
    public List<ItemSource> itemSourceSet;

    /// <summary>
    /// 아이템 정의 : 재료, 상위재료
    /// </summary>
    /// <param name="index"></param>
    /// <param name="type">아이템 타입 : 재료, 상위재료, 기본무기, 상위무기, 소모_상태이상, 소모</param>
    /// <param name="name">아이템 이름</param>
    /// <param name="comment">아이템 설명(주석)</param>
    /// <param name="canOverlap">겹치기여부</param>
    /// <param name="overlapCount">겹치기 최대 갯수</param>
    public Item(string index, ItemType type, string name, string comment, bool canOverlap, int overlapCount)
    {
        _itemId = index;
        _itemType = type;
        _itemName = name;
        _itemComment = comment;
        _canOverlap = canOverlap;
        _overlapCount = overlapCount;
    }

    /// <summary>
    /// 아이템 정의 : 옵션 1개
    /// </summary>
    /// <param name="index"></param>
    /// <param name="type">아이템 타입 : 재료, 상위재료, 기본무기, 상위무기, 소모_상태이상, 소모</param>
    /// <param name="name">아이템 이름</param>
    /// <param name="comment">아이템 설명(주석)</param>
    /// <param name="canOverlap">겹치기여부</param>
    /// <param name="overlapCount">겹치기 최대 갯수</param>
    /// <param name="opt1">옵션1</param>
    /// <param name="optValue1">옵션1 수치</param>
    public Item(string index, ItemType type, string name, string comment, bool canOverlap, int overlapCount, ItemOption opt1, int optValue1)
    {
        _itemId = index;
        _itemType = type;
        _itemName = name;
        _itemComment = comment;
        _canOverlap = canOverlap;
        _overlapCount = overlapCount;

        _options[0] = opt1;
        _optionValues[0] = optValue1;
        _options[1] = ItemOption.NONE;
        _optionValues[1] = 0;
        _options[2] = ItemOption.NONE;
        _optionValues[2] = 0;
    }

    /// <summary>
    /// 아이템 정의 : 옵션 2개
    /// </summary>
    /// <param name="index"></param>
    /// <param name="type">아이템 타입 : 재료, 상위재료, 기본무기, 상위무기, 소모_상태이상, 소모</param>
    /// <param name="name">아이템 이름</param>
    /// <param name="comment">아이템 설명(주석)</param>
    /// <param name="canOverlap">겹치기여부</param>
    /// <param name="overlapCount">겹치기 최대 갯수</param>
    /// <param name="opt1">옵션1</param>
    /// <param name="optValue1">옵션1 수치</param>
    /// <param name="opt2">옵션2</param>
    /// <param name="optValue2">옵션2 수치</param>
    public Item(string index, ItemType type, string name, string comment, bool canOverlap, int overlapCount, ItemOption opt1, int optValue1, ItemOption opt2, int optValue2)
    {
        _itemId = index;
        _itemType = type;
        _itemName = name;
        _itemComment = comment;
        _canOverlap = canOverlap;
        _overlapCount = overlapCount;

        _options[0] = opt1;
        _optionValues[0] = optValue1;
        _options[1] = opt2;
        _optionValues[1] = optValue2;
        _options[2] = ItemOption.NONE;
        _optionValues[2] = 0;
    }

    /// <summary>
    /// 아이템 정의 : 옵션 3개
    /// </summary>
    /// <param name="index"></param>
    /// <param name="type">아이템 타입 : 재료, 상위재료, 기본무기, 상위무기, 소모_상태이상, 소모</param>
    /// <param name="name">아이템 이름</param>
    /// <param name="comment">아이템 설명(주석)</param>
    /// <param name="canOverlap">겹치기여부</param>
    /// <param name="overlapCount">겹치기 최대 갯수</param>
    /// <param name="opt1">옵션1</param>
    /// <param name="optValue1">옵션1 수치</param>
    /// <param name="opt2">옵션2</param>
    /// <param name="optValue2">옵션2 수치</param>
    /// <param name="opt3">옵션3</param>
    /// <param name="optValue3">옵션3 수치</param>
    public Item(string index, ItemType type, string name, string comment, bool canOverlap, int overlapCount, ItemOption opt1, int optValue1, ItemOption opt2, int optValue2, ItemOption opt3, int optValue3)
    {
        _itemId = index;
        _itemType = type;
        _itemName = name;
        _itemComment = comment;
        _canOverlap = canOverlap;
        _overlapCount = overlapCount;

        _options[0] = opt1;
        _optionValues[0] = optValue1;
        _options[1] = opt2;
        _optionValues[1] = optValue2;
        _options[2] = opt3;
        _optionValues[2] = optValue3;
    }

    public string Get_ItemName() { return _itemName; }
    public string Get_ItemId() { return _itemId; }
    public string Get_ItemComment() { return _itemComment; }
    public ItemType Get_ItemType() { return _itemType; }
    public bool Get_CanOverLap() { return _canOverlap; }
    public int Get_OverLapCount() { return _overlapCount; }
    public Sprite Get_ItemImage() { return Resources.Load<Sprite>("ItemImage/" + _itemName); }
    public ItemOption[] Get_ItemOptions() { return _options; }
    public bool Check_AllTarget()
    {
        for (int i = 0; i < _options.Length; i++)
        {
            switch (_options[i])
            {
                case ItemOption.데미지_전체적군: return true;
            }
        }

        return false;
    }

    public int Get_AttackPower()
    {
        int getPower = 0;
        for(int i = 0; i < _options.Length; i++)
        {
            if (_options[i].Equals(ItemOption.공격력상승)) getPower += _optionValues[i];
        }

        return getPower;
    }

    public bool Get_CanLongDistance()
    {
        for(int i = 0; i < _options.Length; i++)
        {
            if (_options[i].Equals(ItemOption.원거리)) return true;
        }
        return false;
    }

    //아이템 사용
    public void Use_Item(Unit target)
    {
        for(int i = 0; i < _options.Length; i++)
        {
            switch (_options[i])
            {
                case ItemOption.데미지_단일적군:
                case ItemOption.데미지_전체적군:
                    target.TakeDamage(_optionValues[i]);
                    break;
                case ItemOption.방사능피폭완전제거_단일아군:
                case ItemOption.방사능피폭제거_단일아군:
                    (target as Player).Take_RP_Heal(_optionValues[i]);
                    break;
                case ItemOption.체력회복_단일아군:
                    target.Take_HP_Heal(_optionValues[i]);
                    break;
                case ItemOption.출혈효과제거:
                    target.Remove_Debuff(DEBUFF.BLOOD);
                    break;
                case ItemOption.공포:
                    target.TakeDebuff(DEBUFF.HORROR, 0, 1);
                    break;
                case ItemOption.혼란:
                    target.TakeDebuff(DEBUFF.CONFUSE, 20, 2);
                    break;
                case ItemOption.출혈:
                    target.TakeDebuff(DEBUFF.BLOOD, 1, 2);
                    break;
                case ItemOption.화상:
                    target.TakeDebuff(DEBUFF.BURN, 2, 3);
                    break;
            }
        }

    }

    public void Debug_ItemInformation()
    {
        string _debugStr = "ItemId : " + _itemId + "\r\n" +
                           "ItemName : " + _itemName + "\r\n" +
                           "ItemComment : " + _itemComment + "\r\n" +
                           "CanOverlap : " + _canOverlap + "\r\n" +
                           "OverLapCount : " + _overlapCount + "\r\n";

        if (!_options[0].Equals(ItemOption.NONE)) _debugStr += "Option 1 : " + _options[0].ToString() + " => " + _optionValues[0] + "\r\n";
        if (!_options[1].Equals(ItemOption.NONE)) _debugStr += "Option 2 : " + _options[1].ToString() + " => " + _optionValues[1] + "\r\n";
        if (!_options[2].Equals(ItemOption.NONE)) _debugStr += "Option 3 : " + _options[2].ToString() + " => " + _optionValues[2] + "\r\n";

        //Debug.Log(_debugStr);   
    }
}
