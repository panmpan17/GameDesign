using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName="Game/Statistic")]
public class Statistic : ScriptableObject
{
    [Header("Movement")]
    public float WalkDistance;
    public float RollDistance;
    public int RollCount;
    public int JumpCount;

    [Header("Aim & Shoot")]
    public float DrawBowTotalSeconds;
    public int InvalidDrawBowCount;
    public int DrawBowInteruptByRollCount;
    public int SuccessfulDrawBowCount;
    public int MaxOutDrawBowCount;

    [Header("Enemy Kill")]
    public int TotalCoreDamagedCount;
    public int NormalSlimeKilled;
    public int CannonSlimeKilled;
    public int RabbitSlimeKilled;
    public int PigSlimeKilled;
    public int VegetableSlimeKilled;
    public int KingSlimeKilled;

    public int DeathCount;
}
