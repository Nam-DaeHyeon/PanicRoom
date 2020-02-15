using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoroutineHelper : MonoBehaviour {




    // 새로운 코루틴을 실행
    public static GameObject StartNewCoroutine(IEnumerator enumerator)
    {
        // CoroutineHelper 이름을 가지는 GameObject를 생성한다.
        GameObject tempObject = new GameObject("CoroutineHelper");

        // CoroutineHelper 이름의 GameObject에 CoroutineHelper 스크립트를 붙인 뒤,
        CoroutineHelper coroutineHelper = tempObject.AddComponent<CoroutineHelper>();
        
        // 이 객체를 통해 코루틴을 실행한다.
        coroutineHelper.StartCoroutine(enumerator);

        // 코루틴 종료 후 생성한 객체를 삭제해야 하므로 이 객체를 반환한다.
        return tempObject;
    }
}
