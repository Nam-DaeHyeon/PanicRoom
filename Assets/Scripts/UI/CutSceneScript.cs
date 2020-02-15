using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;



public class CutSceneScript : MonoBehaviour
{
    [System.Serializable]
    public struct ScriptInfo
    {
        public Sprite scriptImg;

        [TextArea]
        public string scriptText;
    }
    public ScriptInfo[] scriptInfos;

    public CanvasGroup cutSceneCanvas;
    public GameObject okBtn;
    public Text cutSceneText;
    public Image cutSceneImage;

    // 각 글자 별 Delay
    public float delayPerChar = 0.1f;

    // script control variable
    private float conTime = 0;
    private bool doNextScript = false;




    // ================================================================= public function ==================================================================

    public void CutSceneStart(int index)
    {
        // 위의 변수들을 바탕으로 스크립팅 시작
        StartCoroutine(CutSceneStarting(index));
    }

    public void OnClickBtn()
    {
        delayPerChar = 0;
        conTime = 20;
    }

    public void OnClickOkBtn()
    {
        doNextScript = true;
    }


    IEnumerator CutSceneStarting(int index)
    {
        cutSceneText.text = "";
        delayPerChar = 0.1f;
        doNextScript = false;
        cutSceneImage.color = new Color(1, 1, 1, 0);
        cutSceneCanvas.alpha = 1;

        cutSceneImage.sprite = scriptInfos[index].scriptImg;

        conTime = 0;
        while(conTime < 1)
        {
            cutSceneImage.color = new Color(1, 1, 1, conTime);
            conTime += Time.deltaTime;
            yield return null;
        }
        conTime = 0;

        yield return new WaitForSeconds(0.5f);

        bool isColor = false;
        // 해당 라인의 글자들을 일정 간격(delayPerChar)으로 입력
        for (int conChar = 0; conChar < scriptInfos[index].scriptText.Length; conChar++)
        {
            // 현재 string의 입력값이 코드일 경우 따로 처리. 그 외엔 UI에 표시
            switch (scriptInfos[index].scriptText[conChar])
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
                    if (scriptInfos[index].scriptText[conChar] == '%')
                    {
                        isColor = true;
                        continue;
                    }

                    if (isColor)
                        cutSceneText.text = cutSceneText.text + "<color=#568AFFFF>" + scriptInfos[index].scriptText[conChar] + "</color>";
                    else
                        cutSceneText.text = cutSceneText.text + scriptInfos[index].scriptText[conChar];
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

        okBtn.SetActive(true);

        // 모든 글자를 출력했으므로 화면을 터치할 때까지 대기
        while (!doNextScript)
            yield return null;

        conTime = 1;
        while (conTime > 0)
        {
            cutSceneCanvas.alpha = conTime;
            conTime -= Time.deltaTime;
            yield return null;
        }
        cutSceneCanvas.alpha = 0;

        okBtn.SetActive(false);
        gameObject.SetActive(false);
    }

}
