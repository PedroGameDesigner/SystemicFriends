using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

namespace DefaultNamespace
{
    public class FriendshipSimulation : MonoBehaviour
    {
        private int _tickTime = 0;

        [Header("Setup")]
        public List<CharacterSettings> CharactersSettings;
        public List<Challenge> Challenges;
        
        public int MaxRelationIncrement;
        public int MinRelationIncrement;

        public int MaxtRelationLevel = 50;
        public int MinRelationLevel = 5;
        
        [Header("Simulation")]
        public List<Character> Characters;
        
        private Challenge RandomChallenge => Challenges[Random.Range(0, Challenges.Count)];

        private List<Character> winners = new List<Character>();
        private List<Character> losers = new List<Character>();

        private List<Character> affected = new List<Character>();
        private List<Character> others = new List<Character>();

        public bool startSimulation = false;

        private void Start()
        {
            Setup();
        }
        
        public void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                startSimulation = true;
            }

            if (startSimulation)
            {
                _tickTime++;
                Debug.Log("Tick:" + _tickTime);
                Tick();
            }
        }
        
        private void Setup()
        {
            Characters = new List<Character>();
            CharactersSettings.ForEach(setting =>
            {
                GameObject newCharacterGameObject = new GameObject(setting.characterName);
                newCharacterGameObject.transform.SetParent(transform);
                
                Character newCharacter = newCharacterGameObject.AddComponent<Character>(); 
                newCharacter.Settings = setting;
                Characters.Add(newCharacter);
            });
            
            
            for (int i = 0; i < Characters.Count; i++)
            {
                Character mainCharacter = Characters[i];
                mainCharacter.relations = new List<Relationship>();
                
                Characters.ForEach(character =>
                {
                    if (mainCharacter.characterName != character.characterName)
                    {
                        mainCharacter.relations.Add(new Relationship
                        {
                            character = character,
                            level = Random.Range(MinRelationLevel,MaxtRelationLevel)
                        });
                    }
                } );
            }
        }

        private void Tick()
        {
            CharacterFaceChallenge();
            WinnersHelpLosers();
            LosersTakeAdvantageOfWinners();
            LoserResentWinners();
            
            if (Characters.Count > 2)
            {
                CharacterJudgeFriendShip();
            }
            else
            {
                CharacterBegCharacter();
            }
            
            if (Characters.Count == 1)
            {
                startSimulation = false;
            }
        }

        public void CharacterFaceChallenge()
        {
            winners.Clear();
            losers.Clear();

            Challenge newChallenge = RandomChallenge;

            for (int i = 0; i < Characters.Count; i++)
            {
                Character character = Characters[i];
                if (character.ChallengeOverCome(newChallenge))
                {
                    winners.Add(character);
                }
                else
                {
                    losers.Add(character);
                }
            }
            
            Debug.Log("Winners");
            winners.ForEach(winner => Debug.Log(winner.characterName));
            
            Debug.Log("Losers");
            losers.ForEach(losers => Debug.Log(losers.characterName));
        }

        public void WinnersHelpLosers()
        {
            if (winners.Count == 0)
            {
                Debug.Log("No winners");
                return;
            }

            List<Character> newWinners = new List<Character>();

            for (int i = 0; i < winners.Count; i++)
            {
                for (int j = losers.Count - 1; j >= 0; j--)
                {
                    Character winner = winners[i];
                    Character loser = losers[j];

                    bool willHelp = Random.Range(0, 100) >= winner.Personality.Solidarity;

                    if (willHelp)
                    {
                        Debug.Log($"Character {winner.characterName} helps {loser.characterName}"); 
                        
                        winner.RelationUpdate(loser, MinRelationIncrement * (winner.Personality.Solidarity/100));
                        loser.RelationUpdate(winner, MaxRelationIncrement * (loser.Personality.Gratitude/100));
                        
                        newWinners.Add(loser);
                        losers.Remove(loser);
                        
                        break;
                    }
                }
            }

            winners.AddRange(newWinners);
        }

        public void LosersTakeAdvantageOfWinners()
        {
            if (losers.Count == 0)
            {
                Debug.Log("No lossers");
                return;
            }

            List<Character> newWinners = new List<Character>();
            
            for (int i = 0; i < losers.Count; i++)
            {
                for (int j = 0; j < winners.Count; j++)
                {
                    Character loser = losers[i];
                    Character winner = winners[j];

                    bool willTakeAdvantage = Random.Range(0, 100) <= loser.Personality.Selfiness;

                    if (willTakeAdvantage)
                    {
                        Debug.Log($"Character {loser.characterName} takes advantage of {winner.characterName}");

                        int relationLevelLoserWinner = loser.RelationLevel(winner);
                        
                        winner.RelationUpdate(loser, -(MaxRelationIncrement * (winner.Personality.Resentment/100) - loser.Personality.Charisma/100));
                        loser.RelationUpdate(winner, -MinRelationIncrement * (1 - relationLevelLoserWinner/100));
                        
                        newWinners.Add(loser);
                        losers.Remove(loser);
                        
                        break;
                    }
                }
            }
            
            winners.AddRange(newWinners);
        }

        public void LoserResentWinners()
        {
            for (int i = 0; i < losers.Count; i++)
            {
                for (int j = 0; j < winners.Count; j++)
                {
                    Character loser = losers[i];
                    Character winner = winners[j];
                    
                    loser.RelationUpdate(winner, -(MinRelationIncrement * (loser.Personality.Resentment/100) - winner.Personality.Charisma/100));
                }
            }
        }
        
        public void CharacterJudgeFriendShip()
        {
            for (int i = Characters.Count - 1; i >= 0; i--)
            {
                Character judge = Characters[i];
                Character friendToJudge = judge.WillJudgeFriend();
                
                if (friendToJudge != null)
                {
                    Debug.Break();
                    Debug.Log($"Character {judge.characterName} started trial against {friendToJudge.characterName}");
                    
                    affected.Add(judge);
                    affected.Add(friendToJudge);

                    others = Characters.Where(friend => friend.characterName != judge.characterName &&
                                                        friend.characterName != friendToJudge.characterName).ToList();

                    Character friendToExpel = Judgment();
                    
                    Debug.Log($"The group has decided to expel {friendToExpel.characterName}");
                    
                    Characters.Remove(friendToExpel);
                    Characters.ForEach(characters => characters.RemoveFriendShip(friendToExpel));
                }
                
                affected.Clear();
                others.Clear();
            }
        }

        public Character Judgment()
        {
            int firstAffectedLevel = SumRelationLevel(others, affected[0]);
            int secondAffectedLevel = SumRelationLevel(others, affected[1]);

            Character friendToExpel = firstAffectedLevel > secondAffectedLevel ? affected[1] : affected[0];

            if (firstAffectedLevel == secondAffectedLevel)
            {
                friendToExpel = affected[Random.Range(0, affected.Count)];
            }
            
            return friendToExpel;
        }

        public int SumRelationLevel(List<Character> judges, Character affected)
        {
            int totalRelationLevel = 0;
            
            for (int i = 0; i < judges.Count; i++)
            {
                totalRelationLevel += judges[i].RelationLevel(affected);
            }

            return totalRelationLevel;
        }

        public void CharacterBegCharacter()
        {
            for (int i = 0; i < Characters.Count; i++)
            {
                Character characterLeaving = Characters[i];
                Character characterBegging = characterLeaving.WillJudgeFriend();
                
                if (characterBegging != null)
                {
                    Debug.Break();
                    Debug.Log($"Character {characterLeaving.characterName} wanted to leave the group {characterBegging.characterName} will beg");

                    int friendShipLevel = characterBegging.RelationLevel(characterLeaving);
                    bool characterWillStay = Random.Range(0, 100) >= friendShipLevel;

                    if (characterWillStay)
                    {
                        characterLeaving.SetRelationLevel(characterBegging,friendShipLevel/2);
                        characterBegging.SetRelationLevel(characterLeaving,friendShipLevel/2);
                    }
                    else
                    {
                        Debug.Log($"Character {characterLeaving.characterName} leave the group {characterBegging.characterName} will die alone...");
                        Characters.Remove(characterLeaving);
                    }
                    
                }
            }   
        }
    }
}