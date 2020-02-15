using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class StageManager : MonoBehaviour
{
    private void Set_NPC_Supporter()
    {
        //합류 이전 여부 확인
        if (MS_EventManager.instance.OnSupporter_Meet)
        {
            //콜라이더 비활성화 : 상태창 기능 OFF
            SupporterPrefabs.GetComponent<BoxCollider2D>().enabled = false;

            SupporterPrefabs.transform.position = PositionSet_Monster[1].transform.position;    //위치보정.
            SupporterPrefabs.transform.localScale = new Vector3(-1, 1, 1);  //방향보정.
            SupporterPrefabs.gameObject.SetActive(true);
        }

        //합류 여부 확인
        if (MS_EventManager.instance.OnSupporter_Accompany)
        {
            //플레이어를 3명 미만 데려왔을 경우
            if (PlayerPool.Count < 3)
            {
                Player supporter = SupporterPrefabs;
                supporter.Set_HUDElements(PlayerPrefabs[PlayerPool.Count]);

                //능력치 설정.
                supporter.Set_Random_InitParameters(5, 10);

                //장비 부여 : 임시로 '호신용 가스총(d0007)'
                supporter.privateItem = GameManager.instance.Load_Item_DB("d0007");

                //최종능력치 설정.
                supporter.Set_DetailStatus();

                //체력 초기화
                supporter.Reset_HP();

                supporter.transform.position = PlayerPrefabs[PlayerPool.Count].transform.position;    //위치보정.
                supporter.gameObject.SetActive(true);

                PlayerPool.Add(supporter);
            }
            else
            {
                //세 번째 플레이어 임시 제외.
                PlayerPrefabs[2].gameObject.SetActive(false);
                PlayerPool.RemoveAt(2);

                Player supporter = SupporterPrefabs;
                supporter.Set_HUDElements(PlayerPrefabs[2]);

                //능력치 설정.
                supporter.Set_Random_InitParameters(5, 10);

                //장비 부여 : 임시로 '호신용 가스총(d0007)'
                supporter.privateItem = GameManager.instance.Load_Item_DB("d0007");

                //최종능력치 설정.
                supporter.Set_DetailStatus();

                //체력 초기화
                supporter.Reset_HP();

                supporter.transform.position = PlayerPrefabs[2].transform.position;    //위치보정.
                supporter.gameObject.SetActive(true);

                PlayerPool.Add(supporter);
            }
        }

        //합류 이탈 여부 확인
        if (MS_EventManager.instance.OnSupporter_Escape)
        {
            //콜라이더 비활성화 : 상태창 기능 OFF
            SupporterPrefabs.GetComponent<BoxCollider2D>().enabled = false;

            SupporterPrefabs.transform.position = PositionSet_Player[2].transform.position;
            SupporterPrefabs.gameObject.SetActive(true);
            _NPCObj.Add(SupporterPrefabs.gameObject);
        }
    }

    List<GameObject> _NPCObj = new List<GameObject>();
    private void Set_NPC_Others()
    {
        List<string> actions = MS_EventManager.instance.Get_List_ActionEvents();

        for (int i = 0; i < actions.Count; i++)
        {
            switch (actions[i])
            {
                case "검은양_3명_배치":
                    Instantiate(Resources.Load<GameObject>("폐차"));

                    for(int n = 0; n < 3; n++)
                    {
                        GameObject temp = Instantiate(BlackSheepPrefabs[0].gameObject);
                        temp.transform.position = new Vector2(-PlayerPrefabs[n].transform.position.x, PlayerPrefabs[n].transform.position.y);   //위치 보정.
                        if (n == 0) temp.transform.localScale = new Vector2(-1, 1); //방향 보정.
                        temp.GetComponent<BoxCollider2D>().enabled = false;
                        temp.SetActive(true);
                        _NPCObj.Add(temp);
                    }
                    actions.RemoveAt(i);
                    break;
            }
        }
    }


    public void Action_NPC(string actionKey)
    {
        switch (actionKey)
        {
            case "검은양_3명_맵밖으로":
                for (int n = 0; n < _NPCObj.Count; n++)
                {
                    _NPCObj[n].transform.localScale = new Vector2(-1, 1);
                }
                //종점 도달 확인
                StartCoroutine(IE_NPC_Move_NextLine());
                break;
            case "조력자_?_맵밖으로":
                //종점 도달 확인
                StartCoroutine(IE_NPC_Move_NextLine());
                break;
        }
    }

    public void Skiped_Action_NPC()
    {
        List<string> actions = MS_EventManager.instance.Get_List_ActionEvents_inNarrs();

        for (int i = 0; i < actions.Count; i++)
        {
            switch (actions[i])
            {
                case "검은양_3명_맵밖으로":
                case "조력자_?_맵밖으로":
                    for (int n = 0; n < _NPCObj.Count; n++)
                    {
                        _NPCObj[n].SetActive(false);
                    }
                    break;
            }
        }
    }
    
    IEnumerator IE_NPC_Move_NextLine()
    {
        for(int i = 0; i < _NPCObj.Count; i++)
        {
            if(_NPCObj[i].name.Equals("광인")) _NPCObj[i].transform.localScale = new Vector2(-1, 1);   //광인일 경우, 우측으로 시선 방향 보정.
            StartCoroutine(IE_Move_Object(_NPCObj[i], _nextLine));
        }

        //모두 종점에 도달했는 지 확인 : 도달했다면 해당 오브젝트 비활성화.
        int disabledCount = 0;
        while (disabledCount != _NPCObj.Count)
        {
            disabledCount = 0;
            for (int i = 0; i < _NPCObj.Count; i++)
            {
                if (_NPCObj[i].activeSelf)
                {
                    if (_NPCObj[i].transform.position.x > _nextLine.position.x) _NPCObj[i].SetActive(false);
                }
                else disabledCount++;
            }
            
            yield return null;
        }

        //모두 종점에 도달했다.
        
    }

    IEnumerator IE_Move_Object(GameObject obj, Transform targetPos)
    {
        while(obj.activeSelf)
        {
            float speed = obj.GetComponent<Unit>().Get_MoveSpeed();
            Vector2 dir = new Vector3(targetPos.position.x + 1, obj.transform.position.y) - obj.transform.position;
            obj.transform.Translate(dir * speed * Time.deltaTime);
            yield return null;
        }
    }

    private IEnumerator IE_PopUpMessage(bool prevBeforeNarr)
    {
        //팝업용 메세지가 있을 경우
        string tempStr = MS_EventManager.instance.Get_PopMessage_Index0(prevBeforeNarr);
        MS_EventManager.instance.Remove_PopMessage_Index0(prevBeforeNarr);
        if (tempStr != null || tempStr != "")
        {
            //플레이어 이동제한
            for (int i = 0; i < PlayerPool.Count; i++)
            {
                PlayerPool[i]._inMessage = true;
            }

            MS_EventManager.instance.isClosed_pop = false;
            _popupText.text = tempStr;
            _popupObj.SetActive(true);

            yield return new WaitUntil(() => !_popupObj.activeSelf);
            MS_EventManager.instance.isClosed_pop = true;
            
            //플레이어 이동제한 해제
            for (int i = 0; i < PlayerPool.Count; i++)
            {
                PlayerPool[i]._inMessage = false;
            }
        }
    }

    #region 복귀 당시의 인덱스(BackHomeIndex) 관리 메소드
    public void Set_BackHomeIndex_Int(int index)
    {
        PlayerPrefs.SetInt("BackHomeIndex", index);
    }

    private bool Has_BackHomeIndex()
    {
        return PlayerPrefs.HasKey("BackHomeIndex");
    }

    public int Get_BackHomeIndex()
    {
        if (Has_BackHomeIndex()) return PlayerPrefs.GetInt("BackHomeIndex");

        return -1;
    }

    public void Delete_BackHomeIndex()
    {
        if (Has_BackHomeIndex()) PlayerPrefs.DeleteKey("BackHomeIndex");
    }
    #endregion
}
