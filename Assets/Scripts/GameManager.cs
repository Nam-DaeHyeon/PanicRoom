using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum MapType
{
    NONE,
    LOBBY,
}

public partial class GameManager : MonoBehaviour
{
    public int StageCount = 0;      //스테이지 카운터

    public int DungeonCount = -1;    //던전을 몇번 지나갔는가.
    public bool isFirst = true;
    public List<Player> prevPlayerPool;

    //public List<MapType> maps;

    [Header("Resource - AssetDataBase")]
    public LivingPlayerData livingPlayerData;
    public InfectedPlayerData infectedPlayerData;
    public List<StageData> stageList;
    public StageMapData m_stageMapData;
    public StageMapData m_tutorialMapData;

    List<LivingPlayer> DeletePlayerList = new List<LivingPlayer>();

    [Header("Day Parameters")]
    public int DayCount = 1;
    [SerializeField] int _dayFoodAmount = 10;
    [SerializeField] int _dayHealHPAmount = 5;
    [SerializeField] int _dayHealRPAmount = 10;

    [HideInInspector] public string mainStage_Mission = null;

    // 얻은 이벤트리스트 목록
    // 방공호에서 순차적으로 꺼내 관련 팝업을 띄운다.
    public static Queue<string> eventList = new Queue<string>();

    public static GameManager s_instance;
    public static GameManager instance
    {
        get
        {
            if (!s_instance)
            {
                s_instance = GameObject.FindObjectOfType(typeof(GameManager)) as GameManager;
                if (!s_instance)
                {
                    Debug.LogError("GameManager s_instance null");
                    return null;
                }
            }

            //if (s_instance == null)
            //{
            //    s_instance = new GameManager();
            //}

            return s_instance;
        }
    }

    [Header("Selected Player")]
    // 여정에 떠나는 캐릭터들(최대 3)
    public List<LivingPlayer> goPlayers = new List<LivingPlayer>();
    // 최초로 여정에 떠나는 플레이어 수
    public int goPlayerCount;
    // 여정에 떠난 캐릭터 중 죽은 플레이어의 수
    public int diePlayerCount;

    // 임시로 지금은 최대값 100을 전달
    public static int foodCount = 100;

    void Awake()
    {
        if (s_instance == null)
        {
            s_instance = this;

            if (livingPlayerData == null) livingPlayerData = Resources.Load<LivingPlayerData>("LivingPlayerData");
            if (infectedPlayerData == null) infectedPlayerData = Resources.Load<InfectedPlayerData>("InfectedPlayerData");

            SoundManager.instance.Play_BGM_Title();

            DontDestroyOnLoad(this);

        }
        else if (this != s_instance)
        {
            Destroy(gameObject);
        }
        
        Test_InventorySet();
    }

    public int Get_CurrentStageIndex()
    {
        return DungeonCount;
    }

    public float Get_Current_MapPosition()
    {
        int tempStage = StageCount;
        int tempMap = stageList[tempStage].mapList.Count - 1;
        if (StageCount >= stageList.Count) tempStage = stageList.Count - 1;
        if (DungeonCount >= stageList[tempStage].mapList.Count) tempMap = stageList[tempStage].mapList.Count - 1;

        return ((float)DungeonCount) / ((float)tempMap);
    }

    public bool Is_MiddleLine()
    {
        //튜토리얼 중일 경우 안 뜨도록
        if ((PlayerPrefs.GetString("PlayTutorial", "F") == "F")) return false;

        int tempStage = StageCount;
        int tempMap = stageList[tempStage].mapList.Count - 1;
        if (StageCount >= stageList.Count) tempStage = stageList.Count - 1;
        if (DungeonCount >= stageList[tempStage].mapList.Count) tempMap = stageList[tempStage].mapList.Count - 1;
        
        if (DungeonCount == (int)(tempMap * 0.5f))
        {
            //시작 지점일 경우 안 뜨도록
            if (DungeonCount == 0) return false;
            //최종 지점일 경우 안 뜨도록
            if (DungeonCount == tempMap) return false;

            return true;
        }
        return false;
    }

    public bool Is_EndLine()
    {
        int tempStage = StageCount;
        int tempMap = stageList[tempStage].mapList.Count - 1;
        if (StageCount >= stageList.Count) tempStage = stageList.Count - 1;
        if (DungeonCount >= stageList[tempStage].mapList.Count) tempMap = stageList[tempStage].mapList.Count - 1;

        if (DungeonCount == tempMap) return true;
        return false;
    }

    /// <summary>
    /// 각 스테이지에 있는 개별 맵에 몬스터/아이템박스 출현 확률 또는 아무것도 등장하지 않을 확률(우선순위)을 반환합니다.
    /// </summary>
    /// <param name="keyCode">MONSTER | ITEMBOX</param>
    /// <returns></returns>
    public int Get_CurrentStage_Rate(string keyCode)
    {
        int tempStage = StageCount;
        int tempMap = (DungeonCount < 0)? 0 : DungeonCount;
        if (StageCount >= stageList.Count) tempStage = stageList.Count - 1;
        if (DungeonCount >= stageList[tempStage].mapList.Count) tempMap = stageList[tempStage].mapList.Count - 1;

        switch(keyCode)
        {
            case "MONSTER":
                return stageList[tempStage].mapList[tempMap].MonsterRate;
            case "ITEMBOX":
                return stageList[tempStage].mapList[tempMap].ItemBoxRate;
        }
        return 0;
    }

    public string Get_CurrentStage_OptFixedItemBoxList()
    {
        int tempStage = StageCount;
        int tempMap = (DungeonCount < 0) ? 0 : DungeonCount;
        if (StageCount >= stageList.Count) tempStage = stageList.Count - 1;
        if (DungeonCount >= stageList[tempStage].mapList.Count) tempMap = stageList[tempStage].mapList.Count - 1;

        return stageList[tempStage].mapList[tempMap].fixedItemBox_Id.Trim();
    }
    
    public bool Ignore_CreateMonsterAndItemBox()
    {
        int tempStage = StageCount;
        int tempMap = (DungeonCount < 0) ? 0 : DungeonCount;
        if (StageCount >= stageList.Count) tempStage = stageList.Count - 1;
        if (DungeonCount >= stageList[tempStage].mapList.Count) tempMap = stageList[tempStage].mapList.Count - 1;

        if (Random.Range(0, 100) < stageList[tempStage].mapList[tempMap].IgnoreRate) return true;
        return false;
    }

    public void Pass_theDay()
    {
        //경과일 누적.
        DayCount++;

        //식량 소모
        foodCount -= _dayFoodAmount;

        //던전 인덱스 초기화
        DungeonCount = -1;
        isFirst = true;

        //거주자 회복 (체력 & 방사능 수치)
        int maxHp = 0;
        for (int i = 0; i < livingPlayerData.playerList.Count; i++)
        {
            livingPlayerData.playerList[i].conHP += _dayHealHPAmount;
            livingPlayerData.playerList[i].exposure -= _dayHealRPAmount;

            //최대 체력 제한.
            maxHp = livingPlayerData.playerList[i].str + livingPlayerData.playerList[i].con * 2 + 10;
            if (livingPlayerData.playerList[i].conHP > maxHp) livingPlayerData.playerList[i].conHP = maxHp;

            //최소 피폭량 제한.
            if (livingPlayerData.playerList[i].exposure < 0) livingPlayerData.playerList[i].exposure = 0;
        }

        GameObject.Find("Displayers").transform.GetChild(1).GetComponent<RoomHumanDisplayer>().Update_SlotData();
    }

    public void Go_PanicRoom()
    {
        diePlayerCount = 0;
        goPlayerCount = goPlayers.Count;

        //경과일 누적
        DayCount++;

        //식량소모
        foodCount -= _dayFoodAmount;

        //던전 인덱스 초기화
        DungeonCount = -1;
        isFirst = true;

        if (DeletePlayerList.Count != 0) DeletePlayerList.Clear();

        if (StageManager.instance != null)
        {
            List<Player> tempPool = StageManager.instance.PlayerPool;
            for (int i = 0; i < tempPool.Count; i++)
            {
                if (tempPool[i].Get_currentHP() <= 0 || tempPool[i].IsInfected)      //플레이어가 방사능에 완전히 중독되었다.
                {
                    diePlayerCount++;

                    // 로비에 귀환하지 못한다.
                    // 즉, 로비에서 참조할 수 없으므로 해당 정보를 삭제
                    // 우선 현재 살아있는 캐릭터 정보 캐싱
                    InfectedPlayer infectedPlayer = new InfectedPlayer();

                    // 현재 살아있는 캐릭터 정보를 돌며..
                    for (int j = 0; j < livingPlayerData.playerList.Count; j++)
                    {
                        //foreach (LivingPlayer livingPlayer in goPlayers)
                        {
                            if (tempPool[i].Get_NickName().Equals(livingPlayerData.playerList[j].characterName))
                            {
                                // 방사능에 완전히 중독된 놈의 정보를..
                                if (tempPool[i].IsInfected)
                                {
                                    // 감염자 파라미터 정의.
                                    infectedPlayer.characterName = livingPlayerData.playerList[j].characterName;
                                    infectedPlayer.str = livingPlayerData.playerList[i].str;
                                    infectedPlayer.dex = livingPlayerData.playerList[i].dex;
                                    infectedPlayer.con = livingPlayerData.playerList[i].con;
                                    infectedPlayer.res = livingPlayerData.playerList[i].res;
                                    infectedPlayer.item = livingPlayerData.playerList[i].privateItem;
                                    // 감염자 스킬 정의.

                                    // 감염자 데이터 저장. 
                                    //다음 던전 중 특수몬스터로 등장한다.
                                    infectedPlayerData.InfectedList.Add(infectedPlayer);
                                }

                                // 삭제
                                DeletePlayerList.Add(livingPlayerData.playerList[j]);
                                //livingPlayerData.playerList.RemoveAt(j);
                                break;
                            }
                        }
                    }
                }
                else
                {
                    for (int j = 0; j < goPlayers.Count; j++)
                    {
                        foreach (LivingPlayer temp2 in livingPlayerData.playerList)
                        {
                            if (goPlayers[j].characterName == temp2.characterName)
                            {
                                temp2.exposure = StageManager.instance.PlayerPool[j].GetCurrInfectedGauge();
                                break;
                            }
                        }
                    }

                }
            }
            
            // 여정 미참여 중인 거주자 회복 (체력 & 방사능 수치)
            int maxHp = 0;
            for (int i = 0; i < livingPlayerData.playerList.Count; i++)
            {
                if (IsContain_goPlayers(livingPlayerData.playerList[i].characterName)) continue;

                livingPlayerData.playerList[i].conHP += _dayHealHPAmount;
                livingPlayerData.playerList[i].exposure -= _dayHealRPAmount;

                //최대 체력 제한.
                maxHp = livingPlayerData.playerList[i].str + livingPlayerData.playerList[i].con * 2 + 10;
                if (livingPlayerData.playerList[i].conHP > maxHp) livingPlayerData.playerList[i].conHP = maxHp;

                //최소 피폭량 제한.
                if (livingPlayerData.playerList[i].exposure < 0) livingPlayerData.playerList[i].exposure = 0;
            }

            // 일괄 삭제
            for (int k = 0; k < DeletePlayerList.Count; k++)
            {
                // 장착 아이템 삭제.
                if(DeletePlayerList[k].privateItem != null) Remove_Item_TotalInventory(DeletePlayerList[k].privateItem.Get_ItemId());
                // 플레이어 데이터 제거.
                livingPlayerData.playerList.Remove(DeletePlayerList[k]);
            }

            //로비에 귀환한다.
            goPlayers.Clear();
            //인벤토리 갱신 : 파티 인벤토리 아이템을 전체 인벤토리로 이전.
            for(int i = 0; i < party_Inventory.Length; i++)
            {
                if (party_Inventory[i].itemId == null || party_Inventory[i].itemId.Equals("")) continue;
                Add_Item_TotalInventory(party_Inventory[i].itemId, party_Inventory[i].itemCount);
                Remove_Item_PartyInventory(party_Inventory[i].itemId, true);
            }
            SceneManager.LoadSceneAsync("LobbyStage");
        }
    }

    public void Go_NextDungeon()
    {
        DungeonCount++;

        //BGM 변경 : 여정씬, 메인스테이지
        if (DungeonCount == 0) SoundManager.instance.Play_BGM_MainIdle();

        if (isFirst)
        {
            //인벤토리 갱신 : 전체 인벤토리에서 파티 인벤토리의 아이템 데이터 삭제.
            for (int i = 0; i < party_Inventory.Length; i++)
            {
                if (party_Inventory[i].itemId == null || party_Inventory[i].itemId.Equals("")) continue;
                Remove_Item_TotalInventory(party_Inventory[i].itemId, true);
            }
        }

        //첫 던전 진입이 아닐 경우, 
        if (DungeonCount > 1)
        {
            isFirst = false;

            //방사능 노출.
            List<Player> currPlayerPool = StageManager.instance.PlayerPool;
            for (int i = 0; i < currPlayerPool.Count; i++)
            {
                if(MS_EventManager.instance.OnSupporter_Accompany)
                {
                    if (currPlayerPool.Count == 3)
                    {
                        if (i == 2) break;
                    }
                    else if (currPlayerPool.Count < 3)
                    {
                        if (i == currPlayerPool.Count - 2) break;
                    }
                }

                currPlayerPool[i].ToxicityUp(8);

                goPlayers[i].conHP = currPlayerPool[i].Get_currentHP();
                goPlayers[i].exposure = currPlayerPool[i].GetCurrInfectedGauge();
            }
        }

        //샘플 장비 아이템 부여
        //Test_PlayerSampleEquipment();

        SceneManager.LoadSceneAsync("MainStage");
    }

    public void Arrived_EndLine()
    {
        bool allInfected = true;

        //전부 감염되었는지 확인.
        List<Player> tempPool = StageManager.instance.PlayerPool;
        for (int i = 0; i < tempPool.Count; i++)
        {
            if (tempPool[i].IsInfected)      //플레이어가 방사능에 완전히 중독되었다.
            {
                allInfected = true;
            }
            else
            {
                allInfected = false;
                break;
            }
        }

        // 모두 감염됬다면 리턴.
        if (allInfected) return;

        //식량 추가.
        foodCount += 20;

        //튜토리얼 중이라면 리턴.
        if (PlayerPrefs.GetString("PlayTutorial", "F").Equals("F")) return;

        //스테이지 카운트 업
        StageCount++;

        //여정씬 진행 미션 초기화
        mainStage_Mission = null;

        //복귀 인덱스 초기화
        StageManager.instance.Delete_BackHomeIndex();
    }

    public void Add_goPlayer(string nickName)
    {
        for (int i = 0; i < livingPlayerData.playerList.Count; i++)
        {
            if(livingPlayerData.playerList[i].characterName.Equals(nickName))
            {
                goPlayers.Add(livingPlayerData.playerList[i]);
                return;
            }
        }
    }

    private bool IsContain_goPlayers(string nickName)
    {
        for(int i = 0; i < StageManager.instance.PlayerPool.Count; i++)
        {
            if (StageManager.instance.PlayerPool[i].Get_NickName().Equals(nickName)) return true;
        }

        return false;
    }

    public List<InfectedPlayer> Get_InfectedPlayerData()
    {
        if (infectedPlayerData == null) return null;

        return infectedPlayerData.InfectedList;
    }

    public void Remove_InfectedPlayerData(string playerName)
    {
        if (infectedPlayerData == null) return;

        for (int i = 0; i < infectedPlayerData.InfectedList.Count; i++)
        {
            if (infectedPlayerData.InfectedList[i].characterName.Equals(playerName))
            {
                infectedPlayerData.InfectedList.RemoveAt(i);
            }
        }
    }

    //EDITOR
    public void Reset_PlayerResources()
    {
        livingPlayerData.playerList.Clear();

        string[] NameList = { "H", "J", "철물점 아저씨", "Jone", "Tom", "Noel", "Vain", "Jimmy" };
        LivingPlayer tempPlayer = new LivingPlayer();
        int availableStat, bonus;

        for (int i = 0; i < NameList.Length; i++)
        {
            tempPlayer.characterName = NameList[i];
            tempPlayer.exposure = 0;

            availableStat = 8;

            //str
            bonus = Random.Range(0, availableStat);
            availableStat -= bonus;
            tempPlayer.str = 3 + bonus;

            //dex
            bonus = Random.Range(0, availableStat);
            availableStat -= bonus;
            tempPlayer.dex = 3 + bonus;

            //con
            bonus = Random.Range(0, availableStat);
            availableStat -= bonus;
            tempPlayer.con = 3 + bonus;

            //res
            tempPlayer.res = 3 + availableStat;

            livingPlayerData.playerList.Add(tempPlayer);
            tempPlayer = new LivingPlayer();
        }

        Update_AssetDataBase();
    }

    public void Update_AssetDataBase()
    {
#if UNITY_EDITOR
        string filepath1 = "Assets/Resources/LivingPlayerData.asset";
        IEnumerable<string> m_oEnum = new string[] { filepath1 };
        UnityEditor.AssetDatabase.ForceReserializeAssets(m_oEnum);
        UnityEditor.AssetDatabase.Refresh();
#endif
    }

    public void Set_StageList()
    {
        if (PlayerPrefs.GetString("PlayTutorial", "F").Equals("F"))
        {
            stageList = m_tutorialMapData.stageList;
        }
        else stageList = m_stageMapData.stageList;
    }

    public void Ed_Delete_PlayerPrefs_BackHomeIndex()
    {
        if (PlayerPrefs.HasKey("BackHomeIndex"))
        {
            PlayerPrefs.DeleteKey("BackHomeIndex");
            Debug.Log("Successly Delete BackHomeIndex.");
        }
        else
        {
            Debug.Log("There is Not PlayerPrefs Data.");
        }
    }    

    public int testIndex = 0;
    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Alpha1))
        {
            if(EventManager.instance == null)
            {
                SceneManager.LoadScene("EventScene", LoadSceneMode.Additive);
            }
            else
            {

                SceneManager.UnloadSceneAsync("EventScene");
            }
        }

        //Free PASS
        if(Input.GetKeyDown(KeyCode.Alpha3))
        {
            Go_PanicRoom();
        }
    }
}
