using UnityEngine;
using UnityEngine.UIElements;

using ZXing;
using ZXing.QrCode;

namespace UIToolkitDemo
{
    // This controls general settings for the game. Many of these options are non-functional in this demo but
    // show how to sync data from a UI with the GameDataManager.
    public class LoginScreen : MenuScreen
    {
        // string IDs
        const string k_LoginButton = "login__qr-button";

        const string k_PanelActiveClass = "login__panel";
        const string k_PanelInactiveClass = "login__panel--inactive";
        const string k_BackgroundActiveClass = "login__screen-background";
        const string k_BackgroundInactiveClass = "login__screen-background--inactive";
        
        const string k_LoginInformation1 = "login__login-information1";
        const string k_LoginInformation2 = "login__login-information2";
        const string k_LoginInformation3 = "login__login-information3";
        
        // visual elements
        Button m_LoginButton;
        Label[] m_LoginInfo;

        // root node for transitions
        VisualElement m_Panel;
        VisualElement m_Background;

        public override void ShowScreen()
        {
            base.ShowScreen();

            // add active style
            m_Panel.RemoveFromClassList(k_PanelInactiveClass);
            m_Panel.AddToClassList(k_PanelActiveClass);
            
            m_Background.RemoveFromClassList(k_BackgroundInactiveClass);
            m_Background.AddToClassList(k_BackgroundActiveClass);
        }

        protected override void SetVisualElements()
        {
            base.SetVisualElements();
            
            m_Panel = m_Root.Q(k_PanelActiveClass);
            m_Background = m_Root.Q(k_BackgroundActiveClass);
            m_LoginButton = m_Root.Q<Button>(k_LoginButton);

            m_LoginInfo = new Label[3];
            m_LoginInfo[0] = m_Root.Q<Label>(k_LoginInformation1);
            m_LoginInfo[1] = m_Root.Q<Label>(k_LoginInformation2);
            m_LoginInfo[2] = m_Root.Q<Label>(k_LoginInformation3);
        }

        protected override void RegisterButtonCallbacks()
        {
            LoginManager.LoginNotReady += ClosePanel;
            LoginManager.LoginReady += ShowScreen;
            LoginManager.ShowConnectMobile += ShowConnectButton;
            LoginManager.UrlGenerated += ShowQrCode;
            LoginManager.LoginStepChanged += LoginStepChanged;
            LoginManager.LoggedIn += ClosePanel;
        }
        
        void ClosePanel()
        {
            m_Panel.RemoveFromClassList(k_PanelActiveClass);
            m_Panel.AddToClassList(k_PanelInactiveClass);
            
            m_Background.RemoveFromClassList(k_BackgroundActiveClass);
            m_Background.AddToClassList(k_BackgroundInactiveClass);

            AudioManager.PlayDefaultButtonSound();

            HideScreen();
        }

        void ShowConnectButton()
        {
            // Enable Connect button for Android and iOS
#if (UNITY_ANDROID || UNITY_IOS) && !UNITY_EDITOR
            m_LoginButton.text = "CONNECT";
            m_LoginButton.style.height = 94;
            m_LoginButton.style.marginTop = 70;
            m_LoginButton.style.unityBackgroundImageTintColor = new StyleColor(new Color(1, 1, 1, 1));
            m_LoginButton.clickable.clicked += LoginManager.ConnectMetaMaskAction;
            
            m_LoginInfo[0].text = "1. Press Connect";
            m_LoginInfo[1].text = "2. Open Metamask and press Connect";
            m_LoginInfo[2].text = "3. Open Metamask and press Sign";
#endif
        }

        void ShowQrCode(string url)
        {
            // Generate QR code for Desktop and Unity Editor
#if UNITY_EDITOR || (!UNITY_ANDROID && !UNITY_IOS)
            var generatedTex = GenerateQrTexture(url);
            m_LoginButton.style.backgroundImage = generatedTex;
            m_LoginButton.text = "";
            m_LoginButton.style.height = 300;
            m_LoginButton.style.marginTop = 4;
#endif
        }

        void LoginStepChanged(LoginManager.LoginStep loginStep)
        {
            HighlightLoginStep((int)loginStep);
        }

        void HighlightLoginStep(int step)
        {
            for (var i = 0; i < m_LoginInfo.Length; i++)
                m_LoginInfo[i].SetEnabled(step == i);
        }
        
        private static Texture2D GenerateQrTexture(string text)
        {
            Texture2D encoded = new Texture2D(256, 256, TextureFormat.RGBA32, false);
            var color32 = EncodeToQr(text, encoded.width, encoded.height);

            for (var i = 0; i < color32.Length; i++)
            {
                var col = color32[i];
                if (col.Equals(new Color32(255,255,255,255)))
                {
                    color32[i] = new Color32(0, 0, 0, 0);
                }
                else
                {
                    color32[i] = new Color32(0, 0, 0, 255);
                }
            }
            
            encoded.SetPixels32(color32);
            encoded.Apply();
            return encoded;
        }
        
        private static Color32[] EncodeToQr(string textForEncoding, int width, int height)
        {
            var writer = new BarcodeWriter
            {
                Format = BarcodeFormat.QR_CODE,
                Options = new QrCodeEncodingOptions
                {
                    Height = height,
                    Width = width
                }
            };
            return writer.Write(textForEncoding);
        }
    }
}