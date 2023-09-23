using System;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

namespace ClashOfHands.Data
{
    [CreateAssetMenu(menuName = "Custom/Announcer Data", fileName = "Announcer_Data_")]
    public class AnnouncerData : ScriptableObject
    {
        public List<Announcement> Interactions = new List<Announcement>(16);

        public string GetAnnouncementFor(CardData cardA, CardData cardB)
        {
            foreach (var interaction in Interactions)
            {
                if (interaction.CardA.Equals(cardA) && interaction.CardB.Equals(cardB))
                {
                    return interaction.Text;
                }

                if (interaction.CardA.Equals(cardB) && interaction.CardB.Equals(cardA))
                {
                    return interaction.Text;
                }
            }

            Debug.LogError($"Interactions for {cardA.DisplayName} and {cardB.DisplayName} were not found.");

            return string.Empty;
        }

#if UNITY_EDITOR

        [Header("Editor Values")]
        [SerializeField]
        private CardData[] _cards;

        [Button]
        public void GenerateCardInteractions()
        {
            if (Interactions == null || Interactions.Count != 0)
            {
                Debug.LogError("Clear interactions list to generate interaction data.");
                return;
            }

            for (int i = 0; i < _cards.Length; i++)
            {
                for (int j = i; j < _cards.Length; j++)
                {
                    Interactions.Add(new Announcement
                    {
                        CardA = _cards[i],
                        CardB = _cards[j]
                    });
                }
            }
        }
#endif
    }


    [Serializable]
    public class Announcement
    {
        public CardData CardA;
        public CardData CardB;

        [TextArea(2, 4)]
        public string Text;
    }
}