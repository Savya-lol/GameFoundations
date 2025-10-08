using System.Collections.Generic;
using UnityEngine;

namespace Darkmatter.Core.Services.AudioService.Data
{
    [CreateAssetMenu(fileName = "AudioClipsSO", menuName = "GameFoundations/Audio/AudioClipsSO", order = 1)]
    public class AudioClipsSO : ScriptableObject
    {
        public List<AudioData> audioClips;
    }
}