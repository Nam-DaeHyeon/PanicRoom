using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="LivingPlayerData"
    , menuName = "Create Player Data"
    , order = 1)]
public class LivingPlayerData : ScriptableObject
{
    public List<LivingPlayer> playerList = new List<LivingPlayer>();
}
