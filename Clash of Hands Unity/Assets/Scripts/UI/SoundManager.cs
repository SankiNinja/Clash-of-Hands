using System;
using ClashOfHands.UI;
using UnityEngine;
using DG.Tweening;

namespace ClashOfHands.Systems
{
    public class SoundManager : MonoBehaviourSingleton<SoundManager>, ISettingChangeReceiver
    {
        [Header("SFX")]
        [SerializeField]
        private AudioSource _sfxPlayer;

        [SerializeField]
        private AudioClip _buttonFx;

        [SerializeField]
        private AudioClip _winFx;

        [SerializeField]
        private AudioClip _loseFx;


        [Header("Music")]
        [SerializeField]
        private AudioSource _bgmPlayer;

        [SerializeField]
        private AudioClip _menuMusic;

        [SerializeField]
        private AudioClip _gameMusic;

        [SerializeField]
        private GameUtils.TweenTimeEase _fadeOut;

        [SerializeField]
        private GameUtils.TweenTimeEase _fadeIn;

        private TweenCallback _loadMenuTrack;
        private TweenCallback _loadGameTrack;
        private Sequence _bgmFadeSequence;

        private SettingsValues _settingsValues;

        private void Start()
        {
            MainMenuManager.Instance.RegisterForSettingsChanges(this);
        }

        public void PlayButtonSFX()
        {
            _sfxPlayer.clip = _buttonFx;
            _sfxPlayer.Play();
        }

        public void PlayWinSFX()
        {
            _sfxPlayer.clip = _winFx;
            _sfxPlayer.Play();
        }

        public void PlayLoseSFX()
        {
            _sfxPlayer.clip = _loseFx;
            _sfxPlayer.Play();
        }

        public void SetMenuMusic()
        {
            _loadMenuTrack ??= LoadMenuTrack;
            FadeBGM(_loadMenuTrack);
        }

        private void LoadMenuTrack()
        {
            _bgmPlayer.clip = _menuMusic;
            _bgmPlayer.Play();
        }

        public void SetGameMusic()
        {
            _loadGameTrack ??= LoadGameTrack;
            FadeBGM(_loadGameTrack);
        }

        private void LoadGameTrack()
        {
            _bgmPlayer.clip = _gameMusic;
            _bgmPlayer.Play();
        }

        private void FadeBGM(TweenCallback trackSwitchCallback)
        {
            _bgmFadeSequence?.Kill();

            _bgmFadeSequence = DOTween.Sequence();
            _bgmFadeSequence.Append(_bgmPlayer.DOFade(0, _fadeOut.Time).SetEase(_fadeOut.Ease));
            _bgmFadeSequence.AppendCallback(trackSwitchCallback);
            _bgmFadeSequence.Append(_bgmPlayer.DOFade(1 * _settingsValues.Music / 10f, _fadeIn.Time)
                .SetEase(_fadeIn.Ease));

            _bgmFadeSequence.Play();
        }

        public void OnSettingsChanged(SettingsValues values)
        {
            _settingsValues = values;

            _bgmPlayer.volume = values.Music / 10f;
            _sfxPlayer.volume = values.SFX / 10f;
        }
    }
}