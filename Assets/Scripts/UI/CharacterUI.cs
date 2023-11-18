using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CharacterUI : MonoBehaviour
{
    [SerializeField]
    TextMeshProUGUI nameLabel;
    [SerializeField]
    TextMeshProUGUI personalityLabel;
    [SerializeField]
    Transform friendshipHolder;
    [SerializeField]
    FriendshipBarUI friendshipPrefab;

    Character character;
    List<FriendshipBarUI> friendshipBarUIs = new List<FriendshipBarUI>();

    public Character Character => character;

    public void SetCharacter(Character character)
    {
        this.character = character;
        nameLabel.text = character.name;
        personalityLabel.text = character.Personality.name;
        nameLabel.color = character.Settings.color;
        
        foreach(Relationship relation in character.relations)
        {
            var bar = Instantiate(friendshipPrefab, friendshipHolder);
            bar.SetRelation(relation);
            friendshipBarUIs.Add(bar);
        }
    }

    private void Update()
    {
        for (int i = 0; i < friendshipBarUIs.Count; i++)
        {
            if (i < character.relations.Count && character.relations[i].level > 0)
            {
                friendshipBarUIs[i].gameObject.SetActive(true);
                friendshipBarUIs[i].SetRelation(character.relations[i]);
            }
            else
                friendshipBarUIs[i].gameObject.SetActive(false);
        }
    }
}
