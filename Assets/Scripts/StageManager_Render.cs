using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class StageManager : MonoBehaviour
{
    int tempRenderOrder = 0;

    public void Set_PoolParentNull()
    {
        for (int i = 0; i < PlayerPool.Count; i++)
        {
            if (PlayerPool[i].Get_currentHP() <= 0) continue;
            PlayerPool[i].transform.parent = null;
        }

        for (int i = 0; i < MonsterPool.Count; i++)
        {
            MonsterPool[i].transform.parent = null;
        }

        Set_RenderSet();
    }

    public void Set_RenderSet()
    {
        //횡렬 상위 정렬 : 0번 인덱스를 가장 앞으로
        for(int i = 0; i < PlayerPool.Count; i++)
        {
            if (PlayerPool[i].Get_currentHP() <= 0) continue;
            PlayerPool[i].transform.SetAsLastSibling();
            PlayerPool[i].transform.GetChild(1).GetComponent<SpriteRenderer>().sortingOrder = PlayerPool.Count - i;
        }

        for (int i = 0; i < MonsterPool.Count; i++)
        {
            if (MonsterPool[i].Get_currentHP() <= 0) continue;
            MonsterPool[i].transform.SetAsLastSibling();
            MonsterPool[i].GetComponent<SpriteRenderer>().sortingOrder = MonsterPool.Count - i;
        }

        //조력자NPC(비합류자)는 무조건 후위 정렬
        if (SupporterPrefabs.gameObject.activeSelf)
        {
            if (!PlayerPool.Contains(SupporterPrefabs))
                SupporterPrefabs.transform.GetChild(1).GetComponent<SpriteRenderer>().sortingOrder = -1;
        }
    }

    public void Set_RenderSet_MyTurnStart(Unit turnUnit)
    {
        //turnUnit.transform.SetAsLastSibling();

        if (turnUnit is Player)
        {
            tempRenderOrder = turnUnit.transform.GetChild(1).GetComponent<SpriteRenderer>().sortingOrder;
            turnUnit.transform.GetChild(1).GetComponent<SpriteRenderer>().sortingOrder = 10;
        }
        if (turnUnit is Monster)
        {
            tempRenderOrder = turnUnit.GetComponent<SpriteRenderer>().sortingOrder;
            turnUnit.GetComponent<SpriteRenderer>().sortingOrder = 10;
        }
    }

    public void Set_RenderSet_MyTurnEnd(Unit turnUnit)
    {
        if (turnUnit is Player)
        {
            turnUnit.transform.GetChild(1).GetComponent<SpriteRenderer>().sortingOrder = tempRenderOrder;
        }
        if (turnUnit is Monster)
        {
            turnUnit.GetComponent<SpriteRenderer>().sortingOrder = tempRenderOrder;
        }

        //Set_RenderSet();
    }
}
