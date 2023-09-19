using System.Collections.Generic;
using UnityEngine;

namespace ClashOfHands.UI
{
    public class Scoreboard : MonoBehaviour
    {
        [SerializeField]
        private CardPool _scorecardPool;

        [SerializeField]
        private CardUI _playerScorecardUI;

        [SerializeField]
        private List<CardUI> _activeScorecards;

        public void Initialize(Sprite[] avatarSprites, Sprite playerSprite)
        {
            ClearAll();

            _playerScorecardUI.UpdateCardUI("0", playerSprite);

            foreach (var avatarSprite in avatarSprites)
            {
                var card = _scorecardPool.Get();
                card.UpdateCardUI("0", avatarSprite);
                _activeScorecards.Add(card);
            }
        }

        public void UpdateScoreboard(int playerIndex, int score)
        {
            _activeScorecards[playerIndex].UpdateCardLabel(score.ToString());
        }

        public void UpdateLocalPlayerScore(int score)
        {
            _playerScorecardUI.UpdateCardLabel(score.ToString());
        }

        private void ClearAll()
        {
            if (_activeScorecards == null)
                return;

            foreach (var scorecard in _activeScorecards)
                scorecard.Clear();

            _activeScorecards.Clear();
        }
    }
}