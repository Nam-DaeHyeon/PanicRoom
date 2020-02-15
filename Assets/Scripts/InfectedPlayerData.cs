using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "InfectedPlayerData"
    , menuName = "Create Infected Player Data"
    , order = 1)]
public class InfectedPlayerData : ScriptableObject
{
    public List<InfectedPlayer> InfectedList = new List<InfectedPlayer>();
}
