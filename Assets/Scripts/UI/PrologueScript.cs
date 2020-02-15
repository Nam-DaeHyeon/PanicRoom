using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PrologueScript : MonoBehaviour, IRoomDisplayer
{
    [System.Serializable]
    public struct ScriptInfo
    {
        [TextArea]
        public string scriptText;
        public Sprite scriptImg;
    }
    public ScriptInfo[] scriptInfos;

    // scriptText show Text
    public Text prologueText;
    // scriptImg show Image
    public Image cutSceneScriptImg;
    // prologue black Panel
    public Image prologueBlackPanel;
    // OK Button
    public GameObject btnOK;

    // story fade in time
    public float fadeinTime;

    // story fade out time
    public float fadeoutTime;

    // 각 글자 별 Delay
    public float delayPerChar;


    // script control variable
    private float conTime = 0;
    private bool isFinishLine = false;
    private bool doNextScript = false;




    // ================================================================ public function ==================================================================

    public void PrologueStart()
    {
        gameObject.SetActive(true);

        // 위의 변수들을 바탕으로 스크립팅 시작
        StartCoroutine(Prologing());
    }

    public void OnClickSkip()
    {
        if (!isFinishLine)
        {
            delayPerChar = 0;
            conTime = 20;
        }
    }

    public void OnClickNext()
    {
        doNextScript = true;
    }


    public void OnVisible()
    {

    }

    public void OnInvisible()
    {

    }

    public void Init()
    {

    }

    public void ResetDisplayer()
    {

    }



    // ================================================================ private function ==================================================================


    // skip을 누르거나, 스크립팅이 끝날 때 까지 대사를 출력
    private IEnumerator Prologing()
    {
        // 최초 대기
        yield return new WaitForSeconds(2);

        // For alpha value control 
        Color tempColor = prologueText.color;

        // Line Control variable 
        int conLine = 0;
        int maxLine = scriptInfos.Length;

        // script를 한 줄씩 출력
        while (conLine < maxLine)
        {
            // 이전 script의 정보를 초기화
            prologueText.text = "";
            delayPerChar = 0.12f;
            conTime = 0;

            // scriptImg를 새로운 sprite로 교체
            cutSceneScriptImg.sprite = scriptInfos[conLine].scriptImg;
            cutSceneScriptImg.color = new Color(1, 1, 1, 0);

            btnOK.SetActive(false);
            prologueBlackPanel.gameObject.SetActive(false);

            // 이미지를 표시
            while (conTime < fadeinTime)
            {
                cutSceneScriptImg.color = new Color(1, 1, 1, conTime / fadeinTime);
                prologueText.color = new Color(tempColor.r, tempColor.g, tempColor.b, conTime / fadeinTime);
                conTime += Time.deltaTime;
                yield return null;
            }
            cutSceneScriptImg.color = new Color(1, 1, 1, 1);
            //cutSceneScriptText.color = new Color(tempColor.r, tempColor.g, tempColor.b, 1);
            conTime = 0;

            // 이제부터 화면을 터치 시 Skip or 다음 script 출력
            isFinishLine = false;
            doNextScript = false;


            bool isColor = false;

            // 해당 라인의 글자들을 일정 간격(delayPerChar)으로 입력
            for (int conChar = 0; conChar < scriptInfos[conLine].scriptText.Length; conChar++)
            {
                // 현재 string의 입력값이 코드일 경우 따로 처리. 그 외엔 UI에 표시
                switch (scriptInfos[conLine].scriptText[conChar])
                {
                    case 'd':
                        float T1 = 0.5f;
                        if (delayPerChar == 0)
                            T1 = 0;

                        for (float temp = 0; temp < T1;)
                        {
                            temp += Time.deltaTime;
                            yield return null;
                        }
                        continue;
                    default:
                        // % : 파랑
                        // * : 빨강
                        // 의 색으로 다음 글자를 변경
                        if (scriptInfos[conLine].scriptText[conChar] == '%')
                        {
                            isColor = true;
                            continue;
                        }

                        if (isColor)
                            prologueText.text = prologueText.text + "<color=#568AFFFF>" + scriptInfos[conLine].scriptText[conChar] + "</color>";
                        else
                            prologueText.text = prologueText.text + scriptInfos[conLine].scriptText[conChar];
                        isColor = false;
                        break;
                }

                // 다음 글자를 입력할 때까지 대기
                while (conTime < delayPerChar)
                {
                    conTime += Time.deltaTime;
                    yield return null;
                }
                conTime = 0;
            }
            isFinishLine = true;

            btnOK.SetActive(true);

            // 모든 글자를 출력했으므로 OK를 터치할 때 까지 대기
            while (!doNextScript)
                yield return null;

            // 표시된 정보 초기화를 위해 black panel을 화면에 표시
            conTime = 0;
            prologueBlackPanel.gameObject.SetActive(true);

            while (conTime < fadeinTime)
            {
                prologueBlackPanel.color = new Color(0, 0, 0, conTime / fadeoutTime);

                conTime += Time.deltaTime;
                yield return null;
            }
            prologueBlackPanel.color = new Color(0, 0, 0, 1);

            conLine++;
        }

        gameObject.SetActive(false);
    }
}
