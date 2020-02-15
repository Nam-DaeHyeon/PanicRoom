using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;


public class TalkReaction : DelayedReaction
{
    // 말풍선
    private GameObject talkWindow;

    [Serializable]
    public class TalkObject
    {
        // 말풍선을 표시할 오브젝트
        public Player target;
        // 표시한 text
        [TextArea]
        public string text;
    }
    // 대사 처리 관련 자료구조
    public List<TalkObject> talkObjects;
    // 말풍선 캐싱 자료구조
    private List<Text> SpeechBubble;

    // 말풍선이 들어갈 canvas
    private GameObject HUDCanvas;

    // 코루틴 사용을 도와줄 객체
    private GameObject coroutineHelper;

    // 타이핑 속도
    public float typingSpeed = 1;

    protected override void SpecificInit()
    {
        // 말풍선 캐싱
        talkWindow = Resources.Load<GameObject>("Prefabs/Talk");
        // 생성할 말풍선들을 저장할 자료구조 초기화
        SpeechBubble = new List<Text>();

        // 캔버스 캐싱
        HUDCanvas = GameObject.Find("TalkCanvas");
    }

    protected override void ImmediateReaction()
    {
        // HUDCanvas 캔버스에 말풍선들을 생성
        for (int i = 0; i < talkObjects.Count; i++)
        {
            // 말풍선 생성
            GameObject tempTalk = Instantiate<GameObject>(talkWindow, HUDCanvas.transform);
            tempTalk.GetComponent<RectTransform>().position = new Vector3((talkObjects[i].target.transform.position.x),
              (talkObjects[i].target.transform.position.y + 1), (talkObjects[i].target.transform.position.y + 1) - 55);
            // 생성한 말풍선 저장
            SpeechBubble.Add(tempTalk.transform.Find("Image").Find("Text").GetComponent<Text>());
        }

        // 타이핑 코루틴 시작
        coroutineHelper = CoroutineHelper.StartNewCoroutine(Talking());
    }

    IEnumerator Talking()
    {
        FSLocator.textDisplayer.reactionButton.enabled = false;

        // 각 캐릭터가 대사를 끝냈는지 조사할 변수
        bool[] isTalkFinish = new bool[talkObjects.Count];
        // 최초 모든 말풍선은 채워지지 않았다.
        for (int i = 0; i < talkObjects.Count; i++)
            isTalkFinish[i] = false;

        // 모든 대사 처리가 끝났는지 조사할 변수
        bool isNotFinish;
        // 현재 가져오는 글자 index
        int stringIndex = 0;
        do
        {
            isNotFinish = false;
            // 각 말풍선들을 할 글자씩 채운다.
            for (int i = 0; i < SpeechBubble.Count; i++)
            {
                // 이 말풍선은 대사 처리가 남았다.
                if (isTalkFinish[i] == false)
                {
                    // 즉, 모든 대사 처리가 종료되지 않았으므로 반복문의 탈출을 막는다.
                    isNotFinish = true;

                    // 대사 추가
                    SpeechBubble[i].text += talkObjects[i].text[stringIndex];

                    // 다음 글자가 있는지 조사
                    if (talkObjects[i].text.Length - 1 == stringIndex)
                        isTalkFinish[i] = true;
                }
                yield return null;
            }

            // 타이핑 속도 제어를 위한 대기
            float conTime = 0;
            while(true)
            {
                conTime += Time.deltaTime * typingSpeed;
                if (conTime >= 0.1f)
                    break;
                yield return null;
            }

            stringIndex++;
        } while (isNotFinish);

        // 말풍선 삭제전 잠시 대기
        yield return new WaitForSeconds(0.8f);

        // 대사 처리가 끝났으므로 모든 말풍선을 삭제.
        for (int i = 0; i < SpeechBubble.Count; i++)
            Destroy(SpeechBubble[i].transform.parent.parent.gameObject);

        // 다음 이벤트 내용이 담긴 코루틴을 실행할 수 있도록 한다.
        FSLocator.textDisplayer.reactionButton.enabled = true;
        FSLocator.textDisplayer.reactionButton.onClick.Invoke();

        // 코루틴 사용 종료
        Destroy(coroutineHelper);
    }
}
