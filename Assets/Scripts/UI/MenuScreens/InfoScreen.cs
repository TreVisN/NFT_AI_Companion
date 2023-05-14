using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace UIToolkitDemo
{

    // links to additional UI Toolkit resources
    public class InfoScreen : MenuScreen
    {
        [Header("URLs")]
        [SerializeField] string m_VisitPageURL = "https://consensys.net/";
        [SerializeField] string m_MetaMaskURL = "https://metamask.io/sdk/";
        [SerializeField] string m_InfuraURL = "https://www.infura.io/use-cases/nft";
        [SerializeField] string m_TruffleURL = "https://trufflesuite.com/";
        [SerializeField] string m_BlogURL = "https://c0f4f41c-2f55-4863-921b-sdk-docs.github.io/guide/metamask-sdk-unity.html";

        const string k_GetInfoButton = "info-signup__button";
        const string k_MetaMaskButton = "info-content__docs-button";
        const string k_InfuraButton = "info-content__forum-button";
        const string k_TruffleButton = "info-content__blog-button";
        const string k_BlogButton = "info-content__asset-button";

        Button m_GetInfoButton;
        Button m_MetaMaskButton;
        Button m_InfuraButton;
        Button m_TruffleButton;
        Button m_BlogButton;

        protected override void SetVisualElements()
        {
            base.SetVisualElements();
            m_GetInfoButton = m_Root.Q<Button>(k_GetInfoButton);
            m_MetaMaskButton = m_Root.Q<Button>(k_MetaMaskButton);
            m_InfuraButton = m_Root.Q<Button>(k_InfuraButton);
            m_TruffleButton = m_Root.Q<Button>(k_TruffleButton);
            m_BlogButton = m_Root.Q<Button>(k_BlogButton);
        }

        protected override void RegisterButtonCallbacks()
        {
            base.RegisterButtonCallbacks();
            m_GetInfoButton.RegisterCallback<ClickEvent>(evt => OpenURL(m_VisitPageURL));
            m_MetaMaskButton.RegisterCallback<ClickEvent>(evt => OpenURL(m_MetaMaskURL));
            m_InfuraButton.RegisterCallback<ClickEvent>(evt => OpenURL(m_InfuraURL));
            m_TruffleButton.RegisterCallback<ClickEvent>(evt => OpenURL(m_TruffleURL));
            m_BlogButton.RegisterCallback<ClickEvent>(evt => OpenURL(m_BlogURL));

        }

        static void OpenURL(string URL)
        {
            AudioManager.PlayDefaultButtonSound();
            Application.OpenURL(URL);
        }
    }
}