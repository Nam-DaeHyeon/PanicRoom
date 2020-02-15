using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Monster : Unit
{
    [SerializeField] private float _virusAmount;

    [Header("Skill")]
    [SerializeField] private MonsterSkillList _skillName;
    [SerializeField] private int _skillPower;
    [SerializeField] private int _skillToxicPower;
    [SerializeField] private int _skillActiveRate = 100;
    [SerializeField] private int _skillMaxCoolTime = 3;
    private MonsterSkill _skill;

    private Player _target;
    private float _distance;

    [Header("UI")]
    public Transform HUDPos;
    public Image UI_HPBar;

    private void Awake()
    {
        //스킬 등록
        switch(_skillName)
        {
            case MonsterSkillList.산성비:
                gameObject.AddComponent<MonsterSkill_AcidRain>();
                break;
        }

        _skill = GetComponent<MonsterSkill>();
        if(_skill != null) _skill.Set_Parameters(_skillName.ToString(), _skillPower, _skillToxicPower, _skillActiveRate, _skillMaxCoolTime);
    }

    private void Start()
    {
        _currHp = _maxHp;
    }

    public override void Order()
    {
        if (_skill != null)
        {
            if (_skill.Get_CurrentCoolTime() > 0) _skill.CoolDown_CurrentTimer();
        }

        base.Order();

        if (StageManager.instance.TurnEnd) return;

        //아이템 장착 여부 확인.
        if (privateItem == null)
        {
            //아이템 아이콘 비활성화
        }
        else
        {
            //장착 아이템 이미지 변경.

            //아이템 아이콘 활성화.
        }

        //타겟 결정.
        Select_Target();

        //공격 방식 결정 : 일반공격 / 스킬사용
        if (_skill != null)
        {
            //스킬 대기시간 여부 확인.
            if (_skill.Get_CurrentCoolTime() <= 0)
            {
                //스킬 시전확률 확인.
                if (_skill.Get_ActiveRate() >= Random.Range(0, 100))
                {
                    _skill.Cast(_target);
                    return;
                }
            }
        }

        StartCoroutine(IE_MonsterBattleProcess());
    }

    void AttackMove(Vector3 targetPos)
    {
        Vector2 dir = targetPos - transform.position;
        dir = dir.normalized;
        transform.Translate(dir * 10f * Time.deltaTime);
    }

    IEnumerator IE_MonsterBattleProcess()
    {
        StageManager.instance.Set_RenderSet_MyTurnStart(this);

        //공격 모션 : 타겟앞으로 이동.
        do
        {
            _distance = Vector2.Distance(_target.transform.position, transform.position);
            AttackMove(_target.transform.position);
            yield return null;
        } while (_distance > 0.3f);
        
        //공격 모션 : 타겟 공격.
        //1. 회피 여부
        if (_avoidRate <= Random.Range(0, 100))
        {
            SoundManager.instance.Play_WeaponSE(privateItem);

            //2. 치명타 여부
            _EndDamage = (_criticalRate <= Random.Range(0, 100)) ? _attack * _criticalDamage : _attack;

            _target.TakeDamage(_EndDamage);
            _target.ToxicityUp(_virusAmount);
            if(privateItem != null) privateItem.Use_Item(_target);
        }
        else
        {
            //Miss
            SoundManager.instance.Play_SE_Miss();
            StageManager.instance.Get_HitLog("MISS", _target.Get_HUDPosition(), "WHITE");
        }

        StageManager.instance.Set_RenderSet_MyTurnEnd(this);

        //공격 모션 : 원래 자리로 이동.
        do
        {
            _distance = Vector2.Distance(originPos, transform.position);
            AttackMove(originPos);
            yield return null;
        } while (_distance > 0.1f);

        //턴 종료
        End_MyTurn();

        yield break;
    }

    public override void TakeDamage(float damage)
    {
        base.TakeDamage(damage);
        
        StageManager.instance.Get_HitLog(_EndDamage.ToString(), HUDPos.position, "WHITE");

        if (_currHp <= 0)
        {
            if(IsInfectedPlayer)
            {
                //이전에 플레이어였다면 리소스 데이터에서 삭제한다.
                GameManager.instance.Remove_InfectedPlayerData(_nickName);
            }

            UI_HPBar.gameObject.SetActive(false);
            gameObject.SetActive(false);
        }
    }

    public override void TakeDebuffDamage(float damage)
    {
        base.TakeDebuffDamage(damage);

        //StageManager.instance.Get_HitLog(_EndDamage.ToString(), HUDPos.position, "RED");

        if (_currHp <= 0)
        {
            if (IsInfectedPlayer)
            {
                //이전에 플레이어였다면 리소스 데이터에서 삭제한다.
                GameManager.instance.Remove_InfectedPlayerData(_nickName);
            }

            UI_HPBar.gameObject.SetActive(false);
            gameObject.SetActive(false);
        }
    }

    public void Active_HPBar()
    {
        UI_HPBar.gameObject.SetActive(true);

        StartCoroutine(IE_Active_HUD());
    }

    IEnumerator IE_Active_HUD()
    {
        while(StageManager.instance.gameState.Equals(GameState.BATTLE))
        {
            UI_HPBar.transform.position = HUDPos.position;

            UI_HPBar.fillAmount = _currHp / _maxHp;

            yield return null;
        }
    }

    private void Select_Target()
    {
        //타겟 결정
        List<Player> tempPool = new List<Player>();
        for (int i = 0; i < StageManager.instance.PlayerPool.Count; i++)
        {
            if (StageManager.instance.PlayerPool[i].Get_currentHP() > 0)
                tempPool.Add(StageManager.instance.PlayerPool[i]);
        }
        if (tempPool.Count == 0) return;

        int randIndex = Random.Range(0, tempPool.Count);
        if (randIndex == tempPool.Count) randIndex--;
        _target = tempPool[randIndex];
    }

    public void End_MyTurn()
    {
        transform.position = originPos;
        StageManager.instance.TurnEnd = true;
        Set_Down_DebuffTurn();
    }
    
    public override Vector3 Get_HUDPosition() { return HUDPos.position; }
}
