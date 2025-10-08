
using System.Threading;
using Cysharp.Threading.Tasks;
using Darkmatter.Core.Services.AudioService.Data;

namespace Darkmatter.Core.Services.AudioService.Interfaces
{
    /// <summary>
    /// Interface for audio services.
    /// </summary>
    public interface IAudioService
    {
        void Initialize();
        void PlayAudio(string clipName, AudioChannelType channel, AudioPlayMode playMode = AudioPlayMode.OneShot);
        void AddAudioClipsSO(AudioClipsSO audioClipsSO);
        void RemoveAudioClipsSO(AudioClipsSO audioClipsSO);
        UniTask PlayAudioAsync(string clipName, AudioChannelType channel, CancellationTokenSource cancellationTokenSource, AudioPlayMode playMode = AudioPlayMode.OneShot);
        void StopAudio(string clipName);
        void StopAllAudio();
        void SetSoundVolume(float volume , AudioChannelType channel = AudioChannelType.Master);
    }
}