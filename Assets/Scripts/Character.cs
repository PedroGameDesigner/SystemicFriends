using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;


[CreateAssetMenu(menuName = "SystemicFriendship/Character")]
public class Character : ScriptableObject
{
    public string characterName => name;
    public Personality personality;
    public List<Relationship> relations;
    public List<ChallengeSkill> skills;
    

    public bool ChallengeOverCome(Challenge challenge)
    {
        ChallengeSkill challengeSkill = skills.First(skill => skill.challenge == challenge.challengeType);
        int skill = Random.Range(0, 100) + challengeSkill.skillLevel;

        return skill >= challenge.difficulty;
    }

    public void RelationUpdate(Character character, int level)
    {
        try
        {
            Relationship relation = relations.First(relationship => relationship.character.characterName == character.characterName);
            int newRelationLevel = relation.level + level;
            relation.level = Mathf.Clamp(newRelationLevel, 0, 100);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            Debug.Break();
            throw;
        }
    }

    public int RelationLevel(Character character)
    {
        Relationship relation = relations.First(relationship => relationship.character.characterName == character.characterName);
        return relation.level;
    }

    public Character WillJudgeFriend()
    {
        Relationship relation = relations.FirstOrDefault(relationship => relationship.level == 0);
        
        return relation?.character;
    }

    public void RemoveFriendShip(Character character)
    {
        relations.RemoveAll(relationship => relationship.character == character);
    }
}


[Serializable]
public class Relationship
{
    public Character character;
    public int level;
}

[Serializable]
public class ChallengeSkill
{
    public ChallengeType challenge;
    [FormerlySerializedAs("skill")] public int skillLevel;
}