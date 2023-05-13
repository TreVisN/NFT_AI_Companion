using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using MetaMask;
using MetaMask.Models;
using MetaMask.Transports.Unity;
using MetaMask.Unity;
using Nethereum.Signer.EIP712;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace UIToolkitDemo
{
    public class LoginManager : MonoBehaviour, IMetaMaskUnityTransportListener
    {
        public enum LoginStep
        {
            Offline,
            Connecting,
            Signing,
            LoggedIn
        }
        
        public static event Action<LoginStep> LoginStepChanged;
        public static event Action LoginNotReady;
        public static event Action LoginReady;

        // Only used for Android and iOS
#pragma warning disable CS0067
        public static event Action ShowConnectMobile;
#pragma warning restore CS0067

        public static event Action EnteredMainMenu;
        public static event Action ConnectPressed;
        public static event Action<string> UrlGenerated;
        public static event Action<string> WalletReceived;
        public static event Action<float> BalanceReceived;
        public static event Action LoggedIn;
        public static event Action LoginSkipped;

        [Header("MetaMask")]
        [SerializeField]
        private MetaMaskUnity MetaMaskObj;
        
        // Can be used just for testing purposes - skips MetaMask login prompt
        [Header("Testing")]
        [SerializeField]
        private bool SkipLogin;

        private LoginStep step;
        
        private string signingMessageId;
        private string signingMessage;

        void Awake()
        {
            // Only keep 1 instance of LoginManager alive
            var objs = FindObjectsOfType<LoginManager>();

            if (objs.Length > 1)
            {
                Destroy(gameObject);
                return;
            }

            DontDestroyOnLoad(gameObject);
            DontDestroyOnLoad(MetaMaskObj);

            SceneManager.activeSceneChanged += OnSceneChange;
            GameDataManager.UpdateBalance += SendBalanceRequest;
        }

        void Start()
        {
            InitializeLoginScreen();
        }

        void OnSceneChange(Scene oldScene, Scene newScene)
        {
            // Not a main menu scene
            if (newScene.buildIndex != 0)
                return;
            
            // If was not initialized yet
            if (!SkipLogin && step != LoginStep.LoggedIn)
                return;
            
            if (SkipLogin)
                LoginSkipped?.Invoke();

            EnteredMainMenu?.Invoke();
            
            StartCoroutine(SkipLoginDelayed());
        }

        private void InitializeLoginScreen()
        {
            if (SkipLogin)
            {
                EnteredMainMenu?.Invoke();
                LoginSkipped?.Invoke();
                StartCoroutine(SkipLoginDelayed());
                return;
            }
            
            MetaMaskObj.Wallet.WalletConnected += WalletConnected;
            MetaMaskObj.Wallet.WalletDisconnected += WalletDisconnected;
            MetaMaskObj.Wallet.WalletReady += WalletReady;
            MetaMaskObj.Wallet.WalletAuthorized += WalletAuthorized;
            MetaMaskObj.Wallet.EthereumRequestResultReceived += TransactionResult;

            ConnectPressed += ConnectMetaMask;
            
            // If Android or iOS - show Connect button for the DeepLink
#if (UNITY_ANDROID || UNITY_IOS) && !UNITY_EDITOR
            ShowConnectMobile?.Invoke();
#else
            MetaMaskObj.Wallet.Connect();
#endif
            
            step = LoginStep.Offline;
            LoginStepChanged?.Invoke(step);

            StartCoroutine(OpenLoginDelayed());
        }

        private void ConnectMetaMask()
        {
            MetaMaskObj.Wallet.Connect();
        }

        // Used for Android and iOS
        public static void ConnectMetaMaskAction()
        {
            ConnectPressed?.Invoke();
        }

        public void OnMetaMaskConnectRequest(string url)
        {
            ShowMetaMaskQr(url);
        }

        private IEnumerator OpenLoginDelayed()
        {
            yield return new WaitForSeconds(0.1f);
            LoginNotReady?.Invoke();
            
            yield return new WaitForSeconds(0.5f);
            LoginReady?.Invoke();
        }

        private IEnumerator SkipLoginDelayed()
        {
            yield return new WaitForSeconds(0.3f);

            LoggedIn?.Invoke();
            
            WalletReceived?.Invoke(step == LoginStep.LoggedIn ? MetaMaskObj.Wallet.SelectedAddress : "Login Skipped");
            BalanceReceived?.Invoke(-1);
        }

        private void WalletConnected(object sender, EventArgs e)
        {
            Debug.Log($"WalletConnected");
            
            step = LoginStep.Connecting;
            LoginStepChanged?.Invoke(step);
        }
        
        private void WalletDisconnected(object sender, EventArgs e)
        {
            Debug.Log($"WalletDisconnected");
        }

        private void WalletReady(object sender, EventArgs e)
        {
            Debug.Log($"WalletReady");
        }
        
        private void WalletAuthorized(object sender, EventArgs e)
        {
            Debug.Log($"WalletAuthorized");
            
            step = LoginStep.Signing;
            LoginStepChanged?.Invoke(step);
            
            SendSignRequest();
            SendBalanceRequest();
        }
        
        private async void SendBalanceRequest()
        {
            // Wait until the MetaMask SDK will set the wallet address
            while (string.IsNullOrEmpty(MetaMaskObj.Wallet.SelectedAddress))
            {
                await Task.Delay(100);
            }
            
            var request = new MetaMaskEthereumRequest
            {
                Method = "eth_getBalance",
                Parameters = new object[] { MetaMaskObj.Wallet.SelectedAddress, "latest" }
            };
            
            var response = await MetaMaskUnity.Instance.Wallet.Request(request);
            
            try
            {
                // Convert balance response to string and shift it by 18 decimal points to the left
                // to get the real value
                float balance = Convert.ToInt64(response.GetString(), 16) / (float)Math.Pow(10, 18);
                BalanceReceived?.Invoke(balance);
            }
            catch (Exception ex)
            {
                Debug.LogError($"Exception was caught while getting balance response.\n{ex.Message}");
                BalanceReceived?.Invoke(0f);
            }
        }
        
        private async void SendSignRequest()
        {
            while (string.IsNullOrEmpty(MetaMaskObj.Wallet.SelectedAddress))
            {
                await Task.Delay(100);
            }

            // Generating a signing message id, so that we know what to look for in the TransactionResult event
            signingMessageId = Guid.NewGuid().ToString();
            
            // Getting the EIP-712: Typed structured data for the signing message
            signingMessage = GetSignMessageJson(MetaMaskObj.Wallet.SelectedAddress, MetaMaskObj.Wallet.SelectedChainId);

            object paramsArray = new [] { MetaMaskObj.Wallet.SelectedAddress, signingMessage };
            
            var request = new MetaMaskEthereumRequest
            {
                Method = "eth_signTypedData_v4",
                Parameters = paramsArray
            };
            
            await MetaMaskObj.Wallet.Request(request, signingMessageId);
        }
        
        private void TransactionResult(object sender, MetaMaskEthereumRequestResultEventArgs e)
        {
            // If not signing - don't accept results
            if (step != LoginStep.Signing)
                return;

            // Deserialize sign transaction result message
            Dictionary<string, string> transactionMessage;
            try
            {
                transactionMessage = JsonConvert.DeserializeObject<Dictionary<string, string>>(e.Result.ToString());
            }
            catch
            {
                // Fail silently - not the correct message
                return;
            }
            
            // If sign message transaction result
            if (transactionMessage == null || signingMessageId != transactionMessage["id"] || SkipLogin)
                return;

            var success = ValidateSignTransaction(transactionMessage);

            if (!success)
            {
                // Ideally here you'd go back to connecting
                return;
            }

            // Turn off login screen
            LoggedIn?.Invoke();
            WalletReceived?.Invoke(MetaMaskObj.Wallet.SelectedAddress);
        }

        private bool ValidateSignTransaction(Dictionary<string, string> signTransactionMessage)
        {
            var addressRecovered = Eip712TypedDataSigner.Current.RecoverFromSignatureV4(signingMessage, signTransactionMessage["result"]).ToLower();
            
            if (addressRecovered == MetaMaskObj.Wallet.SelectedAddress)
            {
                step = LoginStep.LoggedIn;
                return true;
            }
            
            Debug.LogError($"The message was signed by the wrong person.\n" +
                           $"Expected wallet: {MetaMaskObj.Wallet.SelectedAddress}\n" +
                           $"Recovered: {addressRecovered}");
            return false;
        }
        
        private void ShowMetaMaskQr(string url)
        {
            UrlGenerated?.Invoke(url);
        }

        private string GetSignMessageJson(string receiverWallet, string chainId)
        {
            var fullMessage = new Dictionary<string, object>
            {
                {
                    "domain", new Dictionary<string, object>
                    {
                        { "name", "MetaMask / Infura SDK Demo" },
                        { "version", "1" },
                        { "chainId", chainId }
                    }
                },
                {
                    "message", new Dictionary<string, object>()
                    {
                        { "action", "Authentication to the MetaMask / Infura Demo Web3 Game with my account." },
                        { "wallet", receiverWallet },
                    }
                },
                { "primaryType", "Mail" },
                { "types", new Dictionary<string, object>()
                {
                    {
                        "EIP712Domain", new []
                        {
                            new Dictionary<string, string>()
                            {
                                {"name", "name"},
                                {"type", "string"},
                            },
                            new Dictionary<string, string>()
                            {
                                {"name", "version"},
                                {"type", "string"},
                            },
                            new Dictionary<string, string>()
                            {
                                {"name", "chainId"},
                                {"type", "uint256"},
                            }
                        }
                    },
                    {
                        "Mail", new []
                        {
                            new Dictionary<string, string>()
                            {
                                {"name", "action"},
                                {"type", "string"},
                            },
                            new Dictionary<string, string>()
                            {
                                {"name", "wallet"},
                                {"type", "address"},
                            }
                        }
                    }
                } }
            };

            return JsonConvert.SerializeObject(fullMessage);
        }
        
        public void OnMetaMaskRequest(string id, MetaMaskEthereumRequest request) { }
        public void OnMetaMaskFailure(Exception error) { }
        public void OnMetaMaskSuccess() { }
    }
}
