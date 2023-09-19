using System;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Serialization;

namespace ClashOfHands.Data
{
    [Serializable]
    public class GameRule
    {
        public CardData Card;

        public CardData[] Defeats;

        public int Evaluate(CardData other)
        {
            if (Card.Equals(other))
                return 0;

            foreach (var defeatCards in Defeats)
                if (defeatCards.Equals(other))
                    return 1;

            return -1;
        }
    }

    [CreateAssetMenu(menuName = "Custom/Game", fileName = "Game_")]
    [Serializable]
    public class GameData : ScriptableObject
    {
        [field: SerializeField]
        [field: Range(2, 10)]
        public int Players { get; private set; } = 2;

        [Header("Cards")]
        public CardData[] GameCards;

        [FormerlySerializedAs("gameRules")]
        [Header("Game Rules")]
        public GameRule[] GameRules;

        public void UpdatePlayerCount(int playerCount)
        {
            Players = playerCount;
        }

        public void Evaluate(in CardData[] cards, int[] results)
        {
            Assert.IsNotNull(cards, "Cards data is null.");
            Assert.IsNotNull(results, "Results container is null.");
            Assert.IsTrue(cards.Length != 0 && cards.Length == results.Length,
                "Cards data and results container are not of the same length or they are empty.");

            var inputLen = cards.Length;

            //Evaluate all cards against one another and score them.
            for (int i = 0; i < inputLen; i++)
            {
                var currentCard = cards[i];
                Assert.IsNotNull(currentCard, "Null card found during evaluation.");

                var score = 0;

                GameRule rule = null;
                foreach (var gameRule in GameRules)
                    if (gameRule.Card.Equals(currentCard))
                    {
                        rule = gameRule;
                        break;
                    }

                Assert.IsNotNull(rule,
                    $"No Rule of the card {currentCard.name} : {currentCard.DisplayName} found in game rules {name}");

                for (int j = 0; j < inputLen; j++)
                {
                    if (i == j)
                        continue;

                    var otherCard = cards[j];
                    Assert.IsNotNull(otherCard, "Null card found during evaluation.");
                    score += rule.Evaluate(otherCard);
                }

                results[i] = score;
            }
        }

        public static void NormalizeScores(in int[] scores, int[] results)
        {
            Assert.IsNotNull(scores, "Score array is null");
            Assert.IsNotNull(results, "Result array is null");
            Assert.IsTrue(scores.Length != 0 && scores.Length == results.Length,
                "Score and Result array has different sizes or are empty.");

            var highestScore = int.MinValue;

            foreach (var score in scores)
            {
                if (score <= highestScore)
                    continue;

                highestScore = score;
            }

            int highScoreCount = 0;
            for (int i = 0; i < scores.Length; i++)
            {
                if (scores[i] == highestScore)
                {
                    results[i] = 1;
                    highScoreCount++;
                }
                else
                    results[i] = int.MinValue;
            }

            if (highScoreCount == 1)
                return;

            Assert.IsTrue(highScoreCount != 0,
                "High score count was 0, something went horribly wrong, attach the debugger.");

            for (int i = 0; i < results.Length; i++)
            {
                if (results[i] == 1)
                    results[i] = 0;
            }
        }

#if UNITY_EDITOR
        [Button]
        public void CreateGameRules()
        {
            UnityEditor.EditorUtility.IsDirty(this);
            if (GameCards == null || GameCards.Length < 2)
            {
                Debug.LogError("Not enough cards to create game rules.", this);
                return;
            }

            if (GameRules != null && GameRules.Length > 0)
            {
                Debug.LogError("Clear the game rules array to create new game rules.", this);
                return;
            }

            GameRules = new GameRule[GameCards.Length];

            for (var i = 0; i < GameCards.Length; i++)
            {
                GameRules[i] = new GameRule
                {
                    Card = GameCards[i],
                    Defeats = new CardData[GameCards.Length - 1]
                };
            }

            foreach (var gameRule in GameRules)
            {
                var cardRules = gameRule.Defeats;
                var card = gameRule.Card;
                var index = 0;
                foreach (var gameCard in GameCards)
                {
                    if (card.Equals(gameCard))
                        continue;

                    cardRules[index++] = gameCard;
                }
            }
        }
#endif
    }
}