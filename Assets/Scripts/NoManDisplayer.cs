using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoManDisplayer : MonoBehaviour, IRoomDisplayer
{
    public void Init()
    {

    }

    // IComparable 의 OnVisible 메서드 구현
    public void OnVisible()
    {
        ResetDisplayer();

        gameObject.SetActive(true);
    }

    // IComparable 의 OnInvisible 메서드 구현
    public void OnInvisible()
    {
        gameObject.SetActive(false);
    }

    public void ResetDisplayer()
    {

    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


}
