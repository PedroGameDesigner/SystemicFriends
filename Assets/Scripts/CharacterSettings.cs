using System.Collections.Generic;
using UnityEngine;

namespace DefaultNamespace
{
    [CreateAssetMenu(menuName = "SystemicFriendship/Character")]
    public class CharacterSettings : ScriptableObject
    {
        public string characterName => name;
        public Personality personality;
        public List<ChallengeSkill> skills;
    }
}