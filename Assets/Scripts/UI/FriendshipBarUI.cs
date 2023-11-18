using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FriendshipBarUI : MonoBehaviour
{
    [SerializeField]
    private Image bar;

    Relationship relation;

    public void SetRelation(Relationship relation)
    {
        this.relation = relation;
        bar.color = relation.character.Settings.color;
        bar.fillAmount = (float)relation.level / 100;
    }
}
