using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DefaultNamespace;

public class FriendshipSimulationUI : MonoBehaviour
{
    [SerializeField]
    private Transform characterHolder;
    [SerializeField]
    private Transform messageHolder;
    [Space]
    [SerializeField]
    private CharacterUI characterPrefab;
    [SerializeField]
    private MessageUI messagePrefab;

    FriendshipSimulation simultation;

    List<CharacterUI> characters = new List<CharacterUI>();
    List<MessageUI> messages = new List<MessageUI>();

    private void Start()
    {
        simultation = FindAnyObjectByType<FriendshipSimulation>();

        characters.Clear();
        foreach(Character chara in simultation.Characters)
        {
            var charUI = Instantiate(characterPrefab, characterHolder);
            charUI.SetCharacter(chara);
            characters.Add(charUI);
        }
    }

    private void Update()
    {
        if (characters.Count > simultation.Characters.Count)
        {
            for(int i = 0; i < characters.Count; i++)
            {
                if (!simultation.Characters.Contains(characters[i].Character))
                {
                    characters[i].gameObject.SetActive(false);
                    characters.RemoveAt(i);
                    break;
                }
            }
        }
    }

    public static void AddMessage(string content, string title, Color color)
    {

    }
}
