using System;
using ClashOfHands.Data;
using DG.Tweening;
using NaughtyAttributes;
using UnityEngine;
using TMPro;

namespace ClashOfHands.UI
{
    [Serializable]
    public struct TweenTimeEase
    {
        public float Time;
        public Ease Ease;

        public static TweenTimeEase InExpo => new TweenTimeEase { Time = 0.2f, Ease = DG.Tweening.Ease.InExpo };
        public static TweenTimeEase OutExpo => new TweenTimeEase { Time = 0.2f, Ease = DG.Tweening.Ease.OutExpo };
    }

    public class StartCountdown : MonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI _timerLabel;

        [SerializeField]
        private float _maxScale;

        [SerializeField]
        private TweenTimeEase _bounceIn;

        [SerializeField]
        private float _idleTime;

        [SerializeField]
        private TweenTimeEase _bounceOut;

        private Sequence _countdownSequence;

        private TweenCallback _setReadyText;
        private TweenCallback _updateTextLabelTween;
        private TweenCallback _setGoText;
        private TweenCallback _hideGameObject;

        private CardData[] _cards;

        private int _iterator = 0;

        public void Initialize(CardData[] cards)
        {
            _cards = cards;
            CreateTween();
        }

        private void CreateTween()
        {
            _updateTextLabelTween = UpdateTextLabel;
            _setReadyText = SetReadyText;
            _setGoText = SetGoText;
            _hideGameObject = Hide;

            _countdownSequence = DOTween.Sequence();

            _iterator = 0;

            _countdownSequence.AppendCallback(_setReadyText);
            AppendBounce(_countdownSequence);

            for (int i = 0; i < _cards.Length; i++)
            {
                _countdownSequence.AppendCallback(_updateTextLabelTween);
                AppendBounce(_countdownSequence);
            }

            _countdownSequence.AppendCallback(_setGoText);
            AppendBounce(_countdownSequence);

            _countdownSequence.AppendCallback(_hideGameObject);

            _countdownSequence.SetAutoKill(false);
            _countdownSequence.Pause();
        }

        private void Hide() => gameObject.SetActive(false);

        private void AppendBounce(Sequence sequence)
        {
            sequence.Append(_timerLabel.transform.DOScale(Vector3.one * _maxScale, _bounceIn.Time)
                .SetEase(_bounceIn.Ease));
            sequence.AppendInterval(_idleTime);
            sequence.Append(_timerLabel.transform.DOScale(Vector3.zero, _bounceOut.Time)
                .SetEase(_bounceOut.Ease));
        }

        public void Animate(TweenCallback onCompleted)
        {
            gameObject.SetActive(true);
            _iterator = 0;
            _countdownSequence.onComplete = onCompleted;
            _countdownSequence.Restart();
            _countdownSequence.Play();
        }

        private void UpdateTextLabel()
        {
            _timerLabel.SetText(_cards[_iterator].DisplayName);
            _iterator++;
        }

        private void SetReadyText()
        {
            _timerLabel.SetText("Ready");
        }

        private void SetGoText()
        {
            _timerLabel.SetText("GO!");
        }

#if UNITY_EDITOR
        [SerializeField]
        private GameData _gameData;

        [Button]
        public void DemoAnimation()
        {
            Animate(() => { print("Done."); });
        }

        [Button]
        public void Rebuild()
        {
            _countdownSequence?.Kill();
            Initialize(_gameData.GameCards);
            CreateTween();
        }
#endif
    }
}