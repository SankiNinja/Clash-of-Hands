using System;
using ClashOfHands.Systems;
using DG.Tweening;
using NaughtyAttributes;
using UnityEngine;
using TMPro;

namespace ClashOfHands.UI
{
    public class StartCountdown : MonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI _timerLabel;

        [SerializeField]
        private float _maxScale;

        [SerializeField]
        private GameUtils.TweenTimeEase _bounceIn;

        [SerializeField]
        private float _idleTime;

        [SerializeField]
        private GameUtils.TweenTimeEase _bounceOut;

        private Sequence _countdownSequence;

        private TweenCallback _setReadyText;
        private TweenCallback _updateTextLabelTween;
        private TweenCallback _setGoText;
        private TweenCallback _hideGameObject;

        private const int StartCount = 3;

        private int _iterator = 3;

        public void Initialize()
        {
            gameObject.SetActive(true);
            CreateTween();
        }

        private void CreateTween()
        {
            _updateTextLabelTween = UpdateTextLabel;
            _setReadyText = SetReadyText;
            _setGoText = SetGoText;
            _hideGameObject = Hide;

            _countdownSequence = DOTween.Sequence();

            _iterator = 3;

            _countdownSequence.AppendCallback(_setReadyText);
            AppendBounce(_countdownSequence);

            for (int i = 0; i < StartCount; i++)
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
            _iterator = StartCount;
            _countdownSequence.onComplete = onCompleted;
            _countdownSequence.Restart();
            _countdownSequence.Play();
        }

        private void UpdateTextLabel()
        {
            _timerLabel.SetText(_iterator.ToString());
            _iterator--;
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
        [Button]
        public void DemoAnimation()
        {
            Animate(() => { print("Done."); });
        }

        [Button]
        public void Rebuild()
        {
            _countdownSequence?.Kill();
            Initialize();
            CreateTween();
        }
#endif
    }
}