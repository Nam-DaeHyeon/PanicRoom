using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActiveObjectInEvent : MonoBehaviour {
    public List<GameObject> objects;

    public void ActiveTrueObjects()
    {
        foreach (GameObject temp in objects)
            temp.SetActive(true);
    }

    public void ActiveFalseObjects()
    {
        foreach (GameObject temp in objects)
            temp.SetActive(false);
    }

    public GameObject GetObject(string name)
    {
        foreach (GameObject temp in objects)
            if (temp.name == name)
                return temp;

        return null;
    }
}
