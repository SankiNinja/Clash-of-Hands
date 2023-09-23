using System.Collections.Generic;
using ClashOfHands.Systems;
using TMPro;
using UnityEngine;

namespace ClashOfHands.UI
{
    public class Scoreboard : MonoBehaviour
    {
        [Header("Arcade UI")]
        [SerializeField]
        private GameObject _arcadeContainer;

        [Header("Elimination UI")]
        [SerializeField]
        private GameObject _eliminationContainer;

        [SerializeField]
        private CardUI _eliminationScoreCard;

        [SerializeField]
        private CardPool _scorecardPool;

        [SerializeField]
        private CardUI _playerScorecardUI;

        [SerializeField]
        private GameObject[] _hearts;

        [SerializeField]
        private TextMeshProUGUI _scoreText;

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

        public void UpdateScoreboard(int[] scores, int[] roundScore, out float duration)
        {
            duration = 0;
            if (_gameMode == GameMode.Arcade)
            {
                for (int i = 0; i < scores.Length; i++)
                {
                    if (roundScore[i] == 0)
                        continue;

                    _activeScorecards[i].UpdateCardLabel(scores[i].ToString());
                    var bounce = _activeScorecards[i].GetComponent<BounceScale>();
                    duration = Mathf.Max(duration, bounce.Duration);
                    bounce.Bounce(null);
                }
            }
            else
            {
                if (roundScore[_localPlayerIndex] == 0)
                    return;

                _eliminationScoreCard.UpdateCardLabel(scores[_localPlayerIndex].ToString());
                var bounce = _eliminationScoreCard.GetComponent<BounceScale>();
                duration = Mathf.Max(duration, bounce.Duration);
                bounce.Bounce(null);
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