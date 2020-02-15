using UnityEngine;
using System.Collections.Generic;

public class ObjectDestroyReaction : DelayedReaction
{
    public List<GameObject> targets;

    protected override void ImmediateReaction()
    {
        foreach(GameObject target in targets)
            Destroy(target);
    }
}
