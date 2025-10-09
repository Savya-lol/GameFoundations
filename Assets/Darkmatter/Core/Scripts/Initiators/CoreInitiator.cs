using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Darkmatter.Core.Audio;
using Darkmatter.Core.Services.AudioService.Interfaces;
using Darkmatter.Core.Services.LoggingService;
using Darkmatter.Core.Services.SceneService;
using UnityEngine;
using VContainer;

namespace Darkmatter.Core.Initiators
{
    public class CoreInitiator : MonoBehaviour
    {
        private GameInputs _gameInputs;
        private ISceneLoaderService _sceneLoaderService;
        private IAudioService _audioService;
        private CoreAudioClipsSO _coreAudioClipsScriptableObject;

        [Inject]
        private void Setup(GameInputs gameInputs, ISceneLoaderService sceneLoaderService, IAudioService audioService, CoreAudioClipsSO coreAudioClipsScriptableObject)
        {
            _gameInputs = gameInputs;
            _sceneLoaderService = sceneLoaderService;
            _audioService = audioService;
            _coreAudioClipsScriptableObject = coreAudioClipsScriptableObject;
        }

        private void Start()
        {
            _ = InitEntryPoint(CancellationTokenSource.CreateLinkedTokenSource(Application.exitCancellationToken));
        }

        private UniTask InitEntryPoint(CancellationTokenSource cancellationTokenSource)
        {
            try
            {
                UpdateApplicationSettings();
                InitializeServices();
                _audioService.AddAudioClipsSO(_coreAudioClipsScriptableObject);
                // await LoadGameScene(cancellationTokenSource);
            }
            catch (OperationCanceledException)
            {
                LogService.Log("Operation init core was cancelled");
            }
            catch (Exception e)
            {
                LogService.LogError(e.Message);
                throw;
            }
            return UniTask.CompletedTask;
        }

        private void UpdateApplicationSettings()
        {
            Screen.sleepTimeout = SleepTimeout.NeverSleep;
            Application.targetFrameRate = 60;
        }

        private void InitializeServices()
        {
            _gameInputs.Enable();
            _audioService.Initialize();
            _sceneLoaderService.Initialize();
        }
    }
}