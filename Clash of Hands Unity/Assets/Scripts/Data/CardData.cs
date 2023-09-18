using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace ClashOfHands.Data
{
    [CreateAssetMenu(menuName = "Custom/Cards", fileName = "Card_")]
    [Serializable]
    public class CardData : ScriptableObject, IEquatable<CardData>
    {
        [FormerlySerializedAs("Name")]
        public string DisplayName;
        public Sprite Sprite;

        public bool Equals(CardData other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return DisplayName == other.DisplayName;
        }
    }
}