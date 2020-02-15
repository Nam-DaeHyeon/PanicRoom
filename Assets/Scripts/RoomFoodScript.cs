using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoomFoodScript : MonoBehaviour
{
    private int conFood;
    private Text conFoodText;

    public RectTransform foodPercent;

    // Start is called before the first frame update
    void Start()
    {
        conFoodText = GetComponent<Text>();

        GetConFood();
    }

    // Update is called once per frame
    void Update()
    {

    }


    // ============================================================== public functuin ====================================================================

    public void GetConFood()
    {
        // 이전에 남아있던 food의 양을 캐싱
        conFood = GameManager.foodCount;
        if (GameManager.foodCount > 100)
            conFood = GameManager.foodCount = 100;

        foodPercent.localScale = new Vector3(conFood / 100.0f, 1, 1);

        // 이전에 남아있던 food의 양을 화면에 표시.
        conFoodText.text = conFood.ToString();
    }

    public void UseFood(int usedFood)
    {
        // 이전에 남아있던 food의 양에서 사용한 음식을 계산.
        conFood = conFood - usedFood;

        // 이전에 남아있던 food의 양을 화면에 표시.
        conFoodText.text = conFood.ToString();

        // 남아있는 음식이 없으면 Game Over
        if (conFood <= 0)
        {
            // GameOver();
            Debug.Log("게임 끝!");
        }
    }

    public void GameOver()
    {

    }



    // ============================================================== private functuin ====================================================================




}
