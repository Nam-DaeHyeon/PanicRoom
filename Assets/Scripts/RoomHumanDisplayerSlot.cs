using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class RoomHumanDisplayerSlot : MonoBehaviour
{
    [HideInInspector] public string characterName;
    [HideInInspector] public float conHp;
    [HideInInspector] public float maxHp;
    [HideInInspector] public float conRp;
    [HideInInspector] public float attack;

    [HideInInspector] public int str;
    [HideInInspector] public int dex;
    [HideInInspector] public int con;
    [HideInInspector] public int res;
    private float defaultHp = 10;
    
    public Text characterNameText;
    public Text hpText;
    public Text rpText;
    public Text attackText;
    public Text defensiveText;
    public Text criticalText;

    public Image btn;
    private bool beforeClicked = false;

    [Header("Weapon Slot Info")]
    private RoomInventoryDisplayer inventory;
    public RoomItemSlotScript weaponSlot1;
    public RoomItemSlotScript weaponSlot2; // not using!!

    // ============================================================== public functuin ====================================================================

    public void Init(string characterName, float exposure, int mSTR, int mDEX, int mCON, int mRES)
    {
        this.characterName = characterName;
        characterNameText.text = this.characterName.ToString();

        conRp = exposure;
        rpText.text = conRp.ToString();

        str = mSTR;

        dex = mDEX;

        con = mCON;

        res = mRES;

        maxHp = str + con + defaultHp;
        hpText.text = maxHp.ToString();

        attack = str;
        attackText.text = attack.ToString();

        inventory = FindObjectOfType<RoomDisplayerManager>().inventory;
        weaponSlot1.Init(0);
        weaponSlot1.GetComponent<Button>().onClick.AddListener(delegate { inventory.OnVisibleAll(); } );
        //weaponSlot2.Init(0);
    }
    /*
    public void Init(string chracterName, float exposure, float maxHp, float attack)
    {
        characterName = chracterName;
        characterNameText.text = chracterName.ToString();

        conRp = exposure;
        rpText.text = exposure.ToString();
        
        this.maxHp = maxHp;
        hpText.text = maxHp.ToString();

        this.attack = attack;
        attackText.text = attack.ToString();
    }*/

    public void OnClickRegistration()
    {
        // 등록 유무에 따라 다른 루틴
        if (beforeClicked == false)
        {
            // 최대 인원을 초과한 상태이면 루틴 탈출
            if (GameManager.instance.goPlayers.Count >= 3)
            {
                GameObject.FindObjectOfType<RoomHumanDisplayer>().tooMuchManDisplayer.SetActive(true);
                return;
            }

            beforeClicked = true;

            btn.color = new Color32(120, 170, 124, 255);

            // GamaManager에 현재 slot이 가지는 캐릭터 정보를 전송.
            LivingPlayer temp = new LivingPlayer();
            temp.characterName = characterName;
            temp.exposure = conRp;
            temp.str = str;
            temp.dex = dex;
            temp.con = con;
            temp.res = res;
            temp.privateItem = GameManager.instance.Load_Item_DB(weaponSlot1.itemIndex);
            //temp.maxHp = maxHp;
            //temp.attack = attack;
            GameManager.instance.goPlayers.Add(temp);
        }
        else
        {
            beforeClicked = false;

            btn.color = new Color32(200, 90, 90, 255);

            // GameManager가 가지는 현재 slot의 캐릭터 정보를 삭제.
            for(int i = 0; i < GameManager.instance.goPlayers.Count; i++)
            {
                if(GameManager.instance.goPlayers[i].characterName == characterName)
                {
                    GameManager.instance.goPlayers.RemoveAt(i);
                    break;
                }
            }
        }
    }

    public void ItemRegistration()
    {
        for (int i = 0; i < GameManager.instance.goPlayers.Count; i++)
        {
            if (GameManager.instance.goPlayers[i].characterName == characterName)
            {
                GameManager.instance.goPlayers[i].privateItem = GameManager.instance.Load_Item_DB(weaponSlot1.itemIndex);
                break;
            }
        }
    }

}
