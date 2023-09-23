using DG.Tweening;
using TMPro;
using UnityEngine;

namespace ClashOfHands.UI
{
    public class Scoreboard : MonoBehaviour
    {
        [SerializeField]
        private BounceScale[] _hearts;

        [SerializeField]
        private TextMeshProUGUI _scoreText;

        [SerializeField]
        private BounceScale _scoreBounce;

        private int _hideIndex;

        private TweenCallback _hideAtIndex;

        public void Initialize(int hearts)
        {
            gameObject.SetActive(true);
            _scoreText.SetText("Score : {0}", 0);

            for (int i = 0; i < _hearts.Length; i++)
            {
                var heart = _hearts[i];
                if (i < hearts)
                {
                    heart.gameObject.SetActive(true);
                    heart.BounceIn();
                }
                else
                {
                    heart.gameObject.SetActive(false);
                }
            }
        }

        public void UpdateScoreboard(int playerScore, int roundScore, int hearts, out float duration)
        {
            duration = 0;

            for (int i = 0; i < _hearts.Length; i++)
            {
                if (roundScore == 0)
                    return;

                var heart = _hearts[i];
                if (i < hearts)
                {
                    if (heart.gameObject.activeSelf)
                        continue;

                    heart.gameObject.SetActive(true);
                    duration = Mathf.Max(duration, heart.BounceIn());
                }
                else
                {
                    if (heart.gameObject.activeSelf == false)
                        continue;

                    duration = Mathf.Max(duration, heart.BounceOut());
                    _hideIndex = i;

                    _hideAtIndex ??= HideAtIndex;
                    DOVirtual.DelayedCall(duration, _hideAtIndex);
                }
            }

            if (roundScore != 1)
                return;

            _scoreText.SetText("Score : {0}", playerScore);
            _scoreBounce.Bounce(null);
            duration = Mathf.Max(duration, _scoreBounce.Duration);
        }

        private void HideAtIndex()
        {
            _hearts[_hideIndex].gameObject.SetActive(false);
        }
    }
}