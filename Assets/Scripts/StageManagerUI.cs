using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class StageManager : MonoBehaviour
{
    public void UI_OpenPartyInventory()
    {
        if (page_PartyInventory.activeSelf) page_PartyInventory.SetActive(false);
        else page_PartyInventory.SetActive(true);
    }

    public void UI_BackHome()
    {
        StartCoroutine(IE_FadeOut(true));
    }

    public void UI_RewardUI_OFF()
    {
        //인벤토리 여유공간 확인
        for (int i = 0; i < RW_Items.Count; i++)
        {
            if (RW_Items[i].Get_Clicked())
            {
                if (GameManager.instance.Get_Index_EmptyORSameSlot_PartyInventory(RW_Items[i].Get_Checked_RewardItemId(), 1) == -1)
                {
                    RW_Msg.text = "여유공간이 없습니다.";
                    if (!page_PartyInventory.activeSelf) page_PartyInventory.SetActive(true);
                    return;
                }
            }
        }

        //파티 인벤토리 저장.
        for(int i = 0; i < RW_Items.Count; i++)
        {
            if (RW_Items[i].Get_Clicked())
                GameManager.instance.Add_Item_PartyInventory(RW_Items[i].Get_Checked_RewardItemId(), 1);
        }

        //파티 인벤토리 UI 업데이트
        Update_PartyInventoryDisplayer();

        //드랍 아이템 풀 삭제.
        for (int i = 0; i < RW_Items.Count; i++)
        {
            RW_Items[i].Reset_RewardItem();
        }

        //UI 비활성화
        RW_UI.SetActive(false);
    }

    public void UI_PopUpUI_OFF()
    {
        bool prevBeforeNarr = true;
        if (MS_EventManager.instance.isEnd_idleBefore) prevBeforeNarr = false;

        string tempStr = MS_EventManager.instance.Get_PopMessage_Index0(prevBeforeNarr);
        if (tempStr == null)
        {
            //UI 비활성화
            _popupObj.SetActive(false);
        }
        else
        {
            _popupText.text = tempStr;
            MS_EventManager.instance.Remove_PopMessage_Index0(prevBeforeNarr);
        }
    }

    public void Update_MouseOverPlayerData(Player player)
    {
        StartCoroutine(IE_Update_MouseOverPlayerData(player));
    }

    IEnumerator IE_Update_MouseOverPlayerData(Player player)
    {
        //아이템 유무
        if (player.privateItem != null)
        {
            MOVER_EquipIcon.sprite = player.privateItem.Get_ItemImage();
            MOVER_EquipIcon.gameObject.SetActive(true);

            MOVER_EquipNameText.text = player.privateItem.Get_ItemName();
            MOVER_EquipComment.text = player.privateItem.Get_ItemComment();

            MOVER_EquipUI.SetActive(true);
        }
        else
        {
            MOVER_EquipIcon.gameObject.SetActive(false);
            MOVER_EquipUI.SetActive(false);
        }

        //플레이어 갱신
        MOVER_NameText.text = player.Get_NickName();
        MOVER_HPText.text = ((int)player.Get_currentHP()).ToString();
        MOVER_RPText.text = ((int)player.GetCurrInfectedGauge()).ToString();
        MOVER_ATKText.text = ((int)player.Get_AttackPower()).ToString();
        MOVER_DEFText.text = ((int)player.Get_DefencePower()).ToString();
        MOVER_CRIText.text = (int)player.Get_CriticalRate() + "%";

        MOVER_FrameUI.transform.position = player.transform.position;
        MOVER_FrameUI.SetActive(true);

        //지속적인 업데이트가 필요한 것들.
        while (MOVER_FrameUI.activeSelf)
        {
            MOVER_HPText.text = ((int)player.Get_currentHP()).ToString();
            MOVER_RPText.text = ((int)player.GetCurrInfectedGauge()).ToString();

            yield return null;
        }
    }

    IEnumerator IE_Interact_UI()
    {
        yield return new WaitUntil(() => gameState.Equals(GameState.IDLE));

        while(gameState.Equals(GameState.IDLE) || gameState.Equals(GameState.BATTLE))
        {
            if(Input.GetKeyDown(KeyCode.I))
            {
                if (page_PartyInventory.activeSelf) page_PartyInventory.SetActive(false);
                else page_PartyInventory.SetActive(true);
            }

            yield return null;
        }

        if (page_PartyInventory.activeSelf) page_PartyInventory.SetActive(false);
    }

    /// <summary>
    /// 플레이어 및 몬스터 상단에 표시되는 데미지 또는 힐량 텍스트 로그를 호출합니다.
    /// </summary>
    /// <param name="message">텍스트 메세지</param>
    /// <param name="pos">활성화 위치</param>
    /// <param name="colorName">텍스트 색상 : WHILE | YELLOW | GREEN</param>
    public void Get_HitLog(string message, Vector3 pos, string colorName)
    {
        bool allEnable = true;
        for(int i = 0; i < HitLogs.Count; i++)
        {
            if(!HitLogs[i].transform.parent.gameObject.activeSelf)
            {
                currHitLogIdx = i;
                HitLogs[i].text = message;
                allEnable = false;
                break;
            }
        }

        //전부 활성화되있다면 가장 오래된 히트로그 비활성 후 재활용.
        if(allEnable)
        {
            currHitLogIdx++;
            if (currHitLogIdx == HitLogs.Count) currHitLogIdx = 0;
            HitLogs[currHitLogIdx].transform.parent.gameObject.SetActive(false);
            HitLogs[currHitLogIdx].text = message;
            StopCoroutine(IE_Get_HitLog(currHitLogIdx));
        }

        //색상 조정.
        Color tempColor;
        switch(colorName)
        {
            case "YELLOW":
                tempColor = _hitColorYellow;
                break;
            case "GREEN":
                tempColor = _hitColorGreen;
                break;
            case "RED":
                tempColor = _hitColorRed;
                break;
            default:
                tempColor = _hitColorWhite;
                break;
        }
        HitLogs[currHitLogIdx].color = tempColor;

        //높이 보정.
        HitLogs[currHitLogIdx].transform.parent.position = pos;
        StartCoroutine(IE_Get_HitLog(currHitLogIdx));
    }

    IEnumerator IE_Get_HitLog(int logIdx)
    {
        HitLogs[logIdx].transform.parent.gameObject.SetActive(true);

        yield return new WaitForSeconds(1);

        HitLogs[logIdx].transform.parent.gameObject.SetActive(false);
    }
}
