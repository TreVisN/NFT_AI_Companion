using System;
using System.Collections;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;
using UnityEngine.UIElements;

namespace UIToolkitDemo
{
    public class OptionsBar : MenuScreen
    {
        // string IDs
        const string k_OptionsBar = "options-bar";
        const string k_OptionsButton = "options-bar__button";
        const string k_GoldCount = "options-bar__gold-count";
        const string k_WalletAddress = "options-bar__wallet-address";

        const float k_LerpTime = 0.6f;

        VisualElement m_OptionsBar;
        VisualElement m_OptionsButton;
        Label m_GoldLabel;
        Label m_WalletAddressLabel;

        private void OnEnable()
        {
            GameDataManager.FundsUpdated += OnFundsUpdated;
            LoginManager.WalletReceived += OnWalletReceived;
        }

        private void OnDisable()
        {
            GameDataManager.FundsUpdated -= OnFundsUpdated;
            LoginManager.WalletReceived -= OnWalletReceived;

        }

        // identify visual elements by name
        protected override void SetVisualElements()
        {
            base.SetVisualElements();

            m_OptionsBar = m_Root.Q(k_OptionsBar);
            m_OptionsButton = m_Root.Q(k_OptionsButton);
            m_GoldLabel = m_Root.Q<Label>(k_GoldCount);
            m_WalletAddressLabel = m_Root.Q<Label>(k_WalletAddress);

            EnableOptionsBar(false);
        }

        // set up button click events
        protected override void RegisterButtonCallbacks()
        {
            m_OptionsButton?.RegisterCallback<ClickEvent>(ShowOptionsScreen);
        }

        void ShowOptionsScreen(ClickEvent evt)
        {
            AudioManager.PlayDefaultButtonSound();
            m_MainMenuUIManager?.ShowSettingsScreen();
        }
        
        void OnWalletReceived(string address)
        {
            if (string.IsNullOrEmpty(address))
                return;

            m_WalletAddressLabel.text = address != "Login Skipped" ? $"{address[..6]}...{address[^4..]}" : $"{address}";
        }
        
        public void SetGold(float amount)
        {
            uint startValue = 0;
            StartCoroutine(LerpRoutine(m_GoldLabel, startValue, amount));
        }

        void OnFundsUpdated(float amount)
        {
            EnableOptionsBar(true);   
            SetGold(amount);
        }

        void EnableOptionsBar(bool enable)
        {
            m_OptionsBar.style.display = enable ? DisplayStyle.Flex : DisplayStyle.None;
        }
        

        // animated Label counter
        IEnumerator LerpRoutine(Label label, float startValue, float endValue)
        {
            float lerpValue = (float) startValue;
            float t = 0f;
            label.text = string.Empty;

            while (Mathf.Abs((float)endValue - lerpValue) > 0.05f)
            {
                t += Time.deltaTime / k_LerpTime;

                lerpValue = Mathf.Lerp(startValue, endValue, t);
                label.text = $"{lerpValue:F3}";
                yield return null;
            }
            label.text = $"{endValue:F3}";
        }
    }
}