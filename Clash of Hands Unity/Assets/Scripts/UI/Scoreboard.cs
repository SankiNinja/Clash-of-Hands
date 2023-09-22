using System.Collections.Generic;
using ClashOfHands.Systems;
using UnityEngine;

namespace ClashOfHands.UI
{
    public class Scoreboard : MonoBehaviour
    {
        [Header("Arcade UI")]
        [SerializeField]
        private GameObject _arcadeContainer;

        [SerializeField]
        private CardUI _eliminationScoreCard;

        [Header("Elimination UI")]
        [SerializeField]
        private GameObject _eliminationContainer;

        [SerializeField]
        private CardPool _scorecardPool;

        [SerializeField]
        private CardUI _playerScorecardUI;


        private readonly List<CardUI> _activeScorecards = new(4);

        private GameMode _gameMode;

        private int _localPlayerIndex;

        public void Initialize(Sprite[] avatarSprites, int playerIndex, GameMode gameMode)
        {
            if (gameMode == GameMode.Arcade)
            {
                ClearAll();

                _scorecardPool.InitializePool();
                for (var i = 0; i < avatarSprites.Length; i++)
                {
                    var avatarSprite = avatarSprites[i];
                    var card = i == playerIndex ? _playerScorecardUI : _scorecardPool.Get();
                    card.gameObject.SetActive(true);
                    card.UpdateCardUI("0", avatarSprite);
                    _activeScorecards.Add(card);
                }
            }
            else
            {
                _eliminationScoreCard.UpdateCardUI("0", avatarSprites[playerIndex]);
            }

            _localPlayerIndex = playerIndex;
            _arcadeContainer.SetActive(gameMode == GameMode.Arcade);
            _eliminationContainer.SetActive(gameMode == GameMode.Elimination);

            _gameMode = gameMode;
        }

        public void UpdateScoreboard(int[] scores)
        {
            if (_gameMode == GameMode.Arcade)
            {
                for (int i = 0; i < scores.Length; i++)
                {
                    _activeScorecards[i].UpdateCardLabel(scores[i].ToString());
                    _activeScorecards[i].GetComponent<BounceScale>().Bounce(null);
                }
            }
            else
            {
                _eliminationScoreCard.UpdateCardLabel(scores[_localPlayerIndex].ToString());
                _eliminationScoreCard.GetComponent<BounceScale>().Bounce(null);
            }
        }

        private void ClearAll()
        {
            foreach (var scorecard in _activeScorecards)
            {
                if (ReferenceEquals(_playerScorecardUI, scorecard))
                    continue;

                scorecard.Clear();
            }

            _activeScorecards.Clear();
        }
    }
}