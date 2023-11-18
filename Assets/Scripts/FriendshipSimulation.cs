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

        [Header("Setup")] public List<CharacterSettings> CharactersSettings;
        public List<Challenge> Challenges;

        public int MaxRelationIncrement;
        public int MinRelationIncrement;

        public int MaxtRelationLevel = 50;
        public int MinRelationLevel = 5;

        [Header("Simulation")] public int simulationTicks = 5;
        public List<Character> Characters;

        private Challenge RandomChallenge => Challenges[Random.Range(0, Challenges.Count)];

        private List<Character> winners = new List<Character>();
        private List<Character> losers = new List<Character>();

        private List<Character> affected = new List<Character>();
        private List<Character> others = new List<Character>();

        public bool simulating = false;
        public bool gameOver = false;

        private int currentTick;

        private void Awake()
        {
            Setup();
        }

        public void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space) && !simulating && !gameOver)
            {
                simulating = true;
                currentTick = 0;

                while (currentTick < simulationTicks)
                {
                    currentTick++;
                    _tickTime++;
                    FriendshipSimulationUI.AddMessage($"Tick{_tickTime}", "Simulation step", Color.black);
                    Tick();
                }
                
                simulating = false;
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
                            level = Random.Range(MinRelationLevel, MaxtRelationLevel)
                        });
                    }
                });
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
                gameOver = true;
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

            FriendshipSimulationUI.AddMessage($"The group has to {newChallenge.challengeName}. Will they be successful?", "Challenge", Color.black);
            string winnersText = "";
            winners.ForEach(winner => winnersText += winner.characterName + ", ");
            FriendshipSimulationUI.AddMessage(winnersText, "Winners", Color.green);
            string losersText = "";
            losers.ForEach(loser => losersText += loser.characterName + ", ");
            FriendshipSimulationUI.AddMessage(losersText, "Losers", Color.red);
        }

        public void WinnersHelpLosers()
        {
            if (winners.Count == 0)
            {
                FriendshipSimulationUI.AddMessage("No winners", "Challenges", Color.black);
                return;
            }

            List<Character> newWinners = new List<Character>();

            for (int i = 0; i < winners.Count; i++)
            {
                for (int j = losers.Count - 1; j >= 0; j--)
                {
                    Character winner = winners[i];
                    Character loser = losers[j];

                    int randomDice = Random.Range(0, 100);
                    bool willHelp = randomDice <= winner.Personality.Solidarity;

                    FriendshipSimulationUI.AddMessage(
                        $"May Help {loser.characterName}? DiceRoll:{randomDice} Solidarity:{winner.Personality.Solidarity}",
                        winner.characterName,
                        winner.characterNameColor);

                    if (willHelp)
                    {
                        FriendshipSimulationUI.AddMessage($"Don't fear, {loser.characterName}, I will save you!", winner.characterName,
                            winner.characterNameColor);

                        FriendshipSimulationUI.AddMessage($"Thank you, {winner.characterName}. I would be lost without you.", loser.characterName,
                            loser.characterNameColor);

                        float winnerLoserRelation = MinRelationIncrement * (1 + winner.Personality.Solidarity / 100f);
                        winner.RelationUpdate(loser, (int)winnerLoserRelation);

                        float loserWinnerRelation = MaxRelationIncrement * (1 + loser.Personality.Gratitude / 100f);
                        loser.RelationUpdate(winner, (int)loserWinnerRelation);

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

                    int randomDice = Random.Range(0, 100);
                    bool willTakeAdvantage = randomDice <= loser.Personality.Selfiness;
                    
                    FriendshipSimulationUI.AddMessage(
                        $"Should I Takes Advantage of {winner.characterName}? DiceRoll:{randomDice}  Selfiness:{loser.Personality.Selfiness}",
                        loser.characterName,
                        loser.characterNameColor);

                    if (willTakeAdvantage)
                    {
                        
                        FriendshipSimulationUI.AddMessage($"Sorry, {winner.characterName} but I need to win!", loser.characterName,
                            loser.characterNameColor);

                        FriendshipSimulationUI.AddMessage($"How you dare, {loser.characterName}?! I will remember this!.", winner.characterName,
                            winner.characterNameColor);

                        float winnerLoserRelation = MaxRelationIncrement * (1 + (winner.Personality.Resentment / 100f -
                            loser.Personality.Charisma / 100f));
                        winner.RelationUpdate(loser, (int)-winnerLoserRelation);

                        int relationLevelLoserWinner = loser.RelationLevel(winner);
                        float loserWinnerRelation = MinRelationIncrement * (1 + relationLevelLoserWinner / 100f);
                        loser.RelationUpdate(winner, (int)-loserWinnerRelation);

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

                    float loserWinner = MinRelationIncrement * (1 + (loser.Personality.Resentment / 100f -
                                                                     winner.Personality.Charisma / 100f));
                    loser.RelationUpdate(winner, (int)-loserWinner);
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
                    FriendshipSimulationUI.AddMessage($"I can't stand {friendToJudge.characterName} any longer! Everyone, get them out of the team or I will leave!", judge.characterName,
                        judge.characterNameColor);
                    
                    affected.Add(judge);
                    affected.Add(friendToJudge);

                    others = Characters.Where(friend => friend.characterName != judge.characterName &&
                                                        friend.characterName != friendToJudge.characterName).ToList();

                    Character friendToExpel = Judgment();
                    
                    if (friendToExpel == friendToJudge)
                        FriendshipSimulationUI.AddMessage($"The group has decided to expel {friendToExpel.characterName}", "The group",
                            Color.black);
                    else
                        FriendshipSimulationUI.AddMessage($"Fine! Then I have nothing more to do with you...", friendToExpel.characterName,
                            friendToExpel.characterNameColor);


                    Characters.Remove(friendToExpel);
                    Characters.ForEach(characters => characters.RemoveFriendShip(friendToExpel));
                    currentTick = simulationTicks;
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
                    FriendshipSimulationUI.AddMessage($"That was the last straw, {characterBegging.characterName}. we can't be together any longer", characterLeaving.characterName,
                        characterLeaving.characterNameColor);

                    FriendshipSimulationUI.AddMessage($"No, {characterLeaving.characterName}, please don't go. Think about our friendship!", characterBegging.characterName,
                        characterBegging.characterNameColor);

                    int friendShipLevel = characterBegging.RelationLevel(characterLeaving);
                    bool characterWillStay = Random.Range(0, 100) <= friendShipLevel;

                    if (characterWillStay)
                    {
                        FriendshipSimulationUI.AddMessage($"You are right, {characterBegging.characterName}, I forgive you this time.", characterLeaving.characterName,
                            characterLeaving.characterNameColor);

                        characterLeaving.SetRelationLevel(characterBegging, friendShipLevel / 2);
                        characterBegging.SetRelationLevel(characterLeaving, friendShipLevel / 2);
                        
                    }
                    else
                    {
                        FriendshipSimulationUI.AddMessage($"Goodbye, {characterBegging.characterName}.", characterLeaving.characterName,
                            characterLeaving.characterNameColor);
                        FriendshipSimulationUI.AddMessage($"Then {characterBegging.characterName} leaves. {characterBegging.characterName} will die alone", "Narration",
                            Color.black);
                        Characters.Remove(characterLeaving);
                    }

                    currentTick = simulationTicks;
                }
            }
        }
    }
}