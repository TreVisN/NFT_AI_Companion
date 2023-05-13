using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System;

namespace UIToolkitDemo
{
    // non-UI logic for HomeScreen
    public class HomeScreenController : MonoBehaviour
    {
        // events
        public static event Action<LevelSO> ShowLevelInfo;
        public static event Action MainMenuExited;

        [Header("Level Data")]
        [SerializeField] LevelSO[] m_LevelsData;
        private int currentLevelIndex;

        void Awake()
        {
            currentLevelIndex = 0;
        }

        void OnEnable()
        {
            HomeScreen.PlayButtonClicked += OnPlayGameLevel;
            NftManager.OwnerNftsRetrieved += OnOwnerNftsRetrieved;
        }

        void OnDisable()
        {
            HomeScreen.PlayButtonClicked -= OnPlayGameLevel;
            NftManager.OwnerNftsRetrieved -= OnOwnerNftsRetrieved;
        }

        void Start()
        {
            ShowLevelInfo?.Invoke(m_LevelsData[currentLevelIndex]);
        }
        
        void OnOwnerNftsRetrieved()
        {
            var nftManager = FindObjectOfType<NftManager>();

            if (nftManager == null)
                return;

            for (int i = 0; i < m_LevelsData.Length; i++)
            {
                var level = m_LevelsData[i];

                if (nftManager.GetIsItemOwned(level.nftItemId) || nftManager.GetIsItemPreOwned(level.nftItemId))
                {
                    currentLevelIndex = i + 1;
                }
            }

            ShowLevelInfo?.Invoke(m_LevelsData[currentLevelIndex]);
        }

        // scene-management methods
        private void OnPlayGameLevel()
        {
            if (m_LevelsData.Length == 0)
                return;
            
            MainMenuExited?.Invoke();

            var currentLevel = m_LevelsData[currentLevelIndex];

            if (!currentLevel.isLocked)
                SceneManager.LoadSceneAsync(currentLevel.sceneName);
            else
                Application.OpenURL(currentLevel.sceneName);
        }
    }
}
