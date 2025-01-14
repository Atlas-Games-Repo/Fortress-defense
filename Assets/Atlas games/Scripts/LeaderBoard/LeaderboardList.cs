using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "leaderboard", menuName = "Scriptable/LeaderBoardData", order = 1)]
public class LeaderboardList : ScriptableObject
{
   public LeaderboardData[] data;
}
