using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MS_RewardItem : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] Image _colorFrame;
    [SerializeField] Image _itemIcon;
    Item _item;

    private void OnEnable()
    {
        if (_item == null) _colorFrame.enabled = false;
        StartCoroutine(IE_Twinkle());
    }

    public void Reset_RewardItem()
    {
        _item = null;

        _itemIcon.gameObject.SetActive(false);
    }

    public void Set_RewardItem(Item item)
    {
        //_item = GameManager.instance.Load_Item_DB(itemId);
        _item = item;

        _itemIcon.sprite = _item.Get_ItemImage();
        _itemIcon.gameObject.SetActive(true);
    }

    public string Get_RewardItemId()
    {
        if (_item == null) return null;
        return _item.Get_ItemId();
    }

    public string Get_Checked_RewardItemId()
    {
        if(_colorFrame.enabled) return _item.Get_ItemId();
        return null;
    }

    public bool Get_Clicked() { return _colorFrame.enabled; }

    public void UI_Select_Reward()
    {
        if (!_colorFrame.enabled)
        {
            _colorFrame.enabled = true;
            StartCoroutine(IE_Twinkle());
        }
        else
        {
            _colorFrame.enabled = false;
        }
    }

    IEnumerator IE_Twinkle()
    {
        float twinkleSpeed = 0.6f;
        Color tempColor = new Color(_colorFrame.color.r, _colorFrame.color.g, _colorFrame.color.b, _colorFrame.color.a);
        while(_colorFrame.enabled)
        {
            tempColor.g -= twinkleSpeed * Time.deltaTime;
            if (tempColor.g < 0.8f) twinkleSpeed *= -1;
            if (tempColor.g > 1f) twinkleSpeed *= -1;
            _colorFrame.color = tempColor;
            yield return null;
        }
    }

    void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
    {
        if (_item == null) return;

        string tempStr = "";

        StageManager.instance.MOVER_Name_Item.text = _item.Get_ItemName();
        for (int i = 0; i < _item.Get_ItemOptions().Length; i++)
        {
            if (_item.Get_ItemOptions()[i].Equals(ItemOption.방사능피폭완전제거_단일아군)
                || _item.Get_ItemOptions()[i].Equals(ItemOption.방사능피폭제거_단일아군)
                || _item.Get_ItemOptions()[i].Equals(ItemOption.체력회복_단일아군)
                || _item.Get_ItemOptions()[i].Equals(ItemOption.출혈효과제거))
            {
                tempStr = "단일 아군";
                break;
            }
        }
        StageManager.instance.MOVER_OptionAlly_Item.text = tempStr;

        tempStr = "";
        for (int i = 0; i < _item.Get_ItemOptions().Length; i++)
        {
            if (_item.Get_ItemOptions()[i].Equals(ItemOption.데미지_단일적군))
            {
                tempStr = "단일 적군";
                break;
            }
            if (_item.Get_ItemOptions()[i].Equals(ItemOption.데미지_전체적군))
            {
                tempStr = "전체 적군";
                break;
            }
        }
        StageManager.instance.MOVER_OptionEnemy_Item.text = tempStr;

        StageManager.instance.MOVER_Comment_Item.text = _item.Get_ItemComment();

        StageManager.instance.MOVER_FrameUI_Item.transform.position = transform.position;
        StageManager.instance.MOVER_FrameUI_Item.SetActive(true);
    }

    void IPointerExitHandler.OnPointerExit(PointerEventData eventData)
    {
        StageManager.instance.MOVER_FrameUI_Item.SetActive(false);
    }
}
