using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterSkill_AcidRain : MonsterSkill
{
    //전체공격 스킬.
    //상태이상없음.

    public override void Cast(Unit target)
    {
        base.Cast(target);

        StartCoroutine(IE_MainCasting(target));
    }

    IEnumerator IE_MainCasting(Unit target)
    {
        //시작 연출
        yield return new WaitForSeconds(1f);

        //스킬 연출
        SoundManager.instance.Play_SkillSE(_skillName);
        
        //데미지 전달
        for(int i = 0; i < StageManager.instance.PlayerPool.Count; i++)
        {
            Player tempPlayer = StageManager.instance.PlayerPool[i];
            if (tempPlayer.Get_currentHP() <= 0) continue;

            tempPlayer.TakeDamage(_skillPower);
            tempPlayer.ToxicityUp(_toxicPower);
        }

        //마무리 연출
        yield return new WaitForSeconds(1.7f);

        //턴 종료
        _caster.End_MyTurn();

        yield break;
    }
}
