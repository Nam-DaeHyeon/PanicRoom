using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectActiveReaction : DelayedReaction {

    public bool isActive;
    public GameObject target;

    protected override void ImmediateReaction()
    {
        target.SetActive(isActive);
    }

}
