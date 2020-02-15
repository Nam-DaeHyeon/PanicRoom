using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MS_ItemBox : MonoBehaviour
{
    [SerializeField] GameObject _highLightObj;

    Item[] _getItems = new Item[3];

    [SerializeField] int _firstRate = 70;
    [SerializeField] int _secondRate = 25;
    [SerializeField] int _thirdRate = 5;

    private void Start()
    {
        //if (_createRate < Random.Range(0, 100)) gameObject.SetActive(false);
        
        if (StageManager.instance.isIgnored) gameObject.SetActive(false);
        if (GameManager.instance.Get_CurrentStage_Rate("ITEMBOX") == 0) gameObject.SetActive(false);
        if (GameManager.instance.Get_CurrentStage_Rate("ITEMBOX") < Random.Range(0, 100)) gameObject.SetActive(false);
        SetInit_GetItem();
    }

    private void SetInit_GetItem()
    {
        //맵에서 아이템 항목이 고정되있는지 확인.
        string tutorial_ItemId = GameManager.instance.Get_CurrentStage_OptFixedItemBoxList();
        if (tutorial_ItemId != null)
        {
            if (!tutorial_ItemId.Equals(""))
            {
                //복귀했다가 다시 들어온거라면 비활성화
                if(MS_EventManager.instance.IsPlayed_StoryLine())
                {
                    gameObject.SetActive(false);
                }

                _highLightObj.SetActive(true);
                
                if (!tutorial_ItemId.Equals("<random>"))
                {
                    //if (tutorial_ItemId.Equals("<empty>")) return;
                    _getItems[0] = GameManager.instance.Load_Item_DB(tutorial_ItemId);
                    return;
                }
            }
        }

        //드랍 아이템 갯수 결정
        int maxCount = 0;
        int randomKey = Random.Range(0, 100);
        if (_firstRate >= randomKey) maxCount++;
        if (_secondRate >= randomKey) maxCount++;
        if (_thirdRate >= randomKey) maxCount++;

        for (int i = 0; i < maxCount; i++)
        {
            //_getItem = GameManager.instance.Load_Item_DB(getList[getIdx]);
            _getItems[i] = GameManager.instance.Get_Random_Item_ItemBoxSpecialRate();
        }
        //_getItemId = getList[getIdx];
    }

    public Item[] Get_Items()
    {
        return _getItems;
    }

    public void Open_Box()
    {
        gameObject.SetActive(false);
    }
}
