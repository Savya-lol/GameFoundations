using System.Collections.Generic;
using System.Threading;
using Darkmatter.Core.Services.LoggingService;
using UnityEngine;
using Cysharp.Threading.Tasks;
using Darkmatter.Core.Services.AudioService.Data;
using Darkmatter.Core.Services.AudioService.Interfaces;

namespace Darkmatter.Core.Services.AudioService
{
    public class AudioService : IAudioService
    {
        [SerializeField] private AudioSource _masterSource;
        [SerializeField] private AudioSource _musicSource;
        [SerializeField] private AudioSource _sfxSource;
        [SerializeField] private AudioSource _voiceSource;
        [SerializeField] private AudioSource _ambientSource;

        private readonly List<AudioClipsSO> _audioClipsSOList = new List<AudioClipsSO>();
        private readonly Dictionary<AudioChannelType, AudioSource> _audioSourceByChannel = new Dictionary<AudioChannelType, AudioSource>();

        public void Initialize()
        {
            _audioSourceByChannel.Add(AudioChannelType.Music, _musicSource);
            _audioSourceByChannel.Add(AudioChannelType.SFX, _sfxSource);
            _audioSourceByChannel.Add(AudioChannelType.Voice, _voiceSource);
            _audioSourceByChannel.Add(AudioChannelType.Ambient, _ambientSource);
            _audioSourceByChannel.Add(AudioChannelType.Master, _masterSource);
        }
        public void AddAudioClipsSO(AudioClipsSO audioClipsSO)
        {
            if (!_audioClipsSOList.Contains(audioClipsSO))
            {
                _audioClipsSOList.Add(audioClipsSO);
            }
        }
        public void RemoveAudioClipsSO(AudioClipsSO audioClipsSO)
        {
            if (_audioClipsSOList.Contains(audioClipsSO))
            {
                _audioClipsSOList.Remove(audioClipsSO);
            }
        }
        public void PlayAudio(string clipName, AudioChannelType channel, AudioPlayMode playMode = AudioPlayMode.OneShot)
        {
            if (!TryPlayAudioClip(clipName, channel, playMode, out _))
            {
                LogService.LogWarning($"Failed to play audio clip: {clipName} on channel: {channel}");
            }
        }

        public async UniTask PlayAudioAsync(string clipName, AudioChannelType channel, CancellationTokenSource cancellationTokenSource, AudioPlayMode playMode = AudioPlayMode.OneShot)
        {
            if (TryPlayAudioClip(clipName, channel, playMode, out var audioClip))
            {
                await UniTask.WaitForSeconds(audioClip.length).AttachExternalCancellation(cancellationTokenSource.Token);
                return;
            }
            LogService.LogWarning($"Failed to play audio clip: {clipName} on channel: {channel}");
        }

        public void SetSoundVolume(float volume, AudioChannelType channel = AudioChannelType.Master)
        {
            if (IsChannelValid(channel))
            {
                _audioSourceByChannel[channel].volume = Mathf.Clamp01(volume);
            }
            else
            {
                LogService.LogWarning($"Invalid audio channel: {channel}");
            }
        }

        public void StopAudio(string clipName)
        {
            foreach (var kvp in _audioSourceByChannel)
            {
                var audioSource = kvp.Value;
                if (audioSource != null && audioSource.clip != null && audioSource.clip.name == clipName)
                {
                    audioSource.Stop();
                    return;
                }
            }
            LogService.LogWarning($"No audio source is playing the clip: {clipName}");
        }

        public void StopAllAudio()
        {
            foreach (var audioSource in _audioSourceByChannel.Values)
            {
                audioSource?.Stop();
            }
        }

        private bool TryPlayAudioClip(string clipName, AudioChannelType channel, AudioPlayMode playMode, out AudioClip audioClip)
        {
            audioClip = null;
            if (!IsChannelValid(channel))
            {
                LogService.LogWarning($"Invalid audio channel: {channel}");
                return false;
            }

            if (TryGetAudioClip(clipName, out AudioClip clip))
            {
                if (_audioSourceByChannel.TryGetValue(channel, out AudioSource audioSource))
                {
                    if (playMode == AudioPlayMode.Loop)
                    {
                        audioSource.clip = clip;
                        audioSource.loop = true;
                        audioSource.Play();
                    }
                    else
                    {
                        audioSource.PlayOneShot(clip);
                    }
                    audioClip = clip;
                    return true;
                }
            }
            LogService.LogWarning($"Audio clip not found: {clipName}");
            return false;
        }

        private bool TryGetAudioClip(string clipName, out AudioClip audioClip)
        {
            foreach (var audioClipsSO in _audioClipsSOList)
            {
                foreach (var audioData in audioClipsSO.audioClips)
                {
                    if (audioData.clipName == clipName)
                    {
                        audioClip = audioData.clip;
                        return true;
                    }
                }
            }
            audioClip = null;
            return false;
        }

        private bool IsChannelValid(AudioChannelType channel)
        {
            return _audioSourceByChannel.ContainsKey(channel) && _audioSourceByChannel[channel] != null;
        }
    }
}