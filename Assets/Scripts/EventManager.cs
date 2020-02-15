using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EventManager : MonoBehaviour
{
    public static EventManager instance;
    private void Awake()
    {
        instance = this;
    }

    [SerializeField] Image _eventMainImage;
    [SerializeField] Text _eventComment;

    [SerializeField] List<Sprite> _eventImagePool;
    [TextArea(3, 10)]
    [SerializeField] List<string> _eventsPool;
    
    private void OnEnable()
    {
        Test_Load_Event();
    }

    public void Load_Event(string eventId)
    {
        //parse...
    }
    
    public void Test_Load_Event()
    {
        _eventMainImage.sprite = _eventImagePool[GameManager.instance.testIndex];
        if (_eventsPool[GameManager.instance.testIndex] != null) _eventComment.text = _eventsPool[GameManager.instance.testIndex];
        else _eventComment.text = "NULL";

        GameManager.instance.testIndex++;
        if (GameManager.instance.testIndex >= _eventsPool.Count) GameManager.instance.testIndex = 0;
    }


}
