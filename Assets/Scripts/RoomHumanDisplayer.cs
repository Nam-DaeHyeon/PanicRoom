using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoomHumanDisplayer : MonoBehaviour, IRoomDisplayer
{
    // slot들이 들어갈 content
    public Transform content;
    // 살아있는 사람들 수 표시 Text
    public Text livingCount;

    // 최대 캐릭터들이 등록되었을 때 표시할 Displayer
    public GameObject tooMuchManDisplayer;

    // 살아있는 캐릭터들의 정보
    private LivingPlayerData livingPlayerData;

    // 생성할 slot prefab
    private RoomHumanDisplayerSlot slotPrefab;
    private List<RoomHumanDisplayerSlot> slots = new List<RoomHumanDisplayerSlot>();

    [Header("other UI")]
    public RoomInventoryDisplayer inventoryDisplayer;
    


    // ============================================================== public functuin ====================================================================

    // IRoomDisplayer 의 OnVisible 메서드 구현
    public void OnVisible()
    {
        if (gameObject.activeSelf)
        {
            gameObject.SetActive(false);
            return;
        }

        ResetDisplayer();

        gameObject.SetActive(true);
    }

    // IRoomDisplayer 의 OnInvisible 메서드 구현
    public void OnInvisible()
    {
        gameObject.SetActive(false);
    }


    // IRoomDisplayer 의 Init 메서드 구현
    public void Init()
    {
        // 현재 살아있는 캐릭터 정보 캐싱
        livingPlayerData = Resources.Load<LivingPlayerData>("LivingPlayerData");
        livingCount.text = "생 존 자   명 단 ( " + livingPlayerData.playerList.Count + " 명 )";

        // 프리팹 정보 캐싱
        slotPrefab = Resources.Load<RoomHumanDisplayerSlot>("NewCharacterrSlot");

        RoomHumanDisplayerSlot tempSlot;
        foreach (LivingPlayer temp in livingPlayerData.playerList)
        {
            //Instantiate<RoomHumanDisplayerSlot>(slotPrefab, content).Init(temp.characterName, temp.exposure, temp.maxHp, temp.attack);
            tempSlot = Instantiate(slotPrefab, content);
            tempSlot.Init(temp.characterName, temp.exposure, temp.str, temp.dex, temp.con, temp.res);
            slots.Add(tempSlot);
        }
    }

    // IRoomDisplayer 의 ResetDisplayer 메서드 구현
    public void ResetDisplayer()
    {
        if (inventoryDisplayer.gameObject.activeSelf) inventoryDisplayer.gameObject.SetActive(false);
    }

    public void Update_SlotData()
    {
        for(int i = 0; i < slots.Count; i++)
        {
            LivingPlayer tempPlayer = livingPlayerData.playerList[i];
            slots[i].Init(tempPlayer.characterName, tempPlayer.exposure, tempPlayer.str, tempPlayer.dex, tempPlayer.con, tempPlayer.res);
        }
    }

    // ============================================================== private functuin ====================================================================

}
