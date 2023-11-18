using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "SystemicFriendship/Personality")]
public class Personality : ScriptableObject
{
    [SerializeField]
    private int solidarity;
    public int Solidarity => solidarity;

    [SerializeField]
    private int selfiness;
    public int Selfiness => selfiness;

    [SerializeField]
    private int gratitude;
    public int Gratitude => gratitude;

    [SerializeField]
    private int charisma;
    public int Charisma => charisma;

    [SerializeField]
    private int resentment;
    public int Resentment => resentment;
}
