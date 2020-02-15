using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MonsterSkillList
{
    NONE,
    산성비
}

public class MonsterSkill : MonoBehaviour
{
    [SerializeField] protected string _skillName;       //스킬 이름
    [SerializeField] protected int _skillPower;         //스킬 파워
    [SerializeField] protected int _toxicPower;         //감염도 전달 수치
    [SerializeField] protected int _skillActiveRate;    //시전확률
    [SerializeField] protected int _currentCoolTime;    //잔여시전대기시간
    [SerializeField] protected int _maxCoolTime = 3;        //최대시전대기시간

    protected Monster _caster;

    public virtual void Cast(Unit target)
    {
        //Debug.Log("Monster casted Skill - " + _skillName + ".");
        _caster = GetComponent<Monster>();

        //쿨타임 적용
        _currentCoolTime = _maxCoolTime;

        //override...
    }

    public void Set_Parameters(string name, int power, int toxic, int activeRate, int maxCool)
    {
        _skillName = name;
        _skillPower = power;
        _toxicPower = toxic;
        _skillActiveRate = activeRate;
        _maxCoolTime = maxCool;
    }

    public void CoolDown_CurrentTimer()
    {
        _currentCoolTime--;
        if (_currentCoolTime < 0) _currentCoolTime = 0;
    }

    public int Get_ActiveRate() { return _skillActiveRate; }
    public int Get_CurrentCoolTime() { return _currentCoolTime; }
    public int Get_MaxCoolTime() { return _maxCoolTime; }

}
