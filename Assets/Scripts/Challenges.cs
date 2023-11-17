using UnityEngine;

public enum ChallengeType
{
    Physic,
    Love
}

[CreateAssetMenu(menuName = "New Challenge")]
public class Challenge : ScriptableObject
{
    public string challengeName => name;
    public ChallengeType challengeType;
    public int difficulty;
}
