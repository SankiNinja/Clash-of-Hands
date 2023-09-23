using ClashOfHands.Data;
using DG.Tweening;
using TMPro;
using UnityEngine;

namespace ClashOfHands.UI
{
    public class GameTitle : MonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI _titleText;

        [SerializeField]
        private BounceScale _bounce;

        private CardData[] _cards;

        private Sequence _sequence;
        private TweenCallback _bounceCallback;
        private TweenCallback _updateTextCallback;
        private int _cardIterator;

        public void Initialize(CardData[] cards)
        {
            _cards = cards;

            _cardIterator = 0;

            _bounceCallback = Bounce;
            _updateTextCallback = UpdateText;

            CreateSequence();
        }

        private void CreateSequence()
        {
            _sequence = DOTween.Sequence();

            _sequence.SetAutoKill(false);
            _sequence.SetLoops(-1);

            _sequence.AppendCallback(_updateTextCallback);
            _sequence.AppendCallback(_bounceCallback);
            _sequence.AppendInterval(_bounce.Duration);

            _sequence.Play();
        }

        public void Pause()
        {
            _sequence.Pause();
        }

        public void Play()
        {
            _sequence.Play();
        }

        private void UpdateText()
        {
            _titleText.SetText(_cards[_cardIterator].DisplayName);
            _cardIterator++;
            _cardIterator = _cardIterator % _cards.Length;
        }

        private void Bounce()
        {
            _bounce.Bounce(null);
        }
    }
}