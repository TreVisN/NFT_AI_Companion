using UnityEngine;
using System;
using System.IO;
using System.Threading.Tasks;
using Infura.SDK.Organization;

namespace UIToolkitDemo
{
    [RequireComponent(typeof(SaveManager))]
    public class GameDataManager : MonoBehaviour
    {
        public static event Action UpdateBalance;
        public static event Action<float> FundsUpdated;
        public static event Action<GameData> PotionsUpdated;
        public static event Action<ShopItemSO, Vector2> TransactionStarted;
        public static event Action<ShopItemSO> TransactionEnded;
        public static event Action<ShopItemSO> TransactionFailed;
        public static event Action<ShopItemSO> MintingFailed;
        public static event Action<ShopItemSO> ItemPurchased;
        public static event Action<bool> LevelUpButtonEnabled;
        public static event Action<bool> CharacterLeveledUp;
        public static event Action<string> HomeMessageShown;

        [SerializeField] GameData m_GameData;
        public GameData GameData { set => m_GameData = value; get => m_GameData; }

        SaveManager m_SaveManager;
        bool m_IsLoginSkipped;

        void OnEnable()
        {
            ShopController.ShopItemPurchasing += OnPurchaseItem;
            
            SettingsScreen.SettingsUpdated += OnSettingsUpdated;
            SettingsScreen.ResetPlayerFunds += OnResetFunds;
            
            CharScreenController.CharacterShown += OnCharacterShown;
            CharScreenController.LevelPotionUsed += OnLevelPotionUsed;
            
            LoginManager.LoggedIn += ShowWelcomeMessage;
            LoginManager.LoginSkipped += OnLoginSkipped;
            LoginManager.BalanceReceived += SetFunds;
        }

        void OnDisable()
        {
            ShopController.ShopItemPurchasing -= OnPurchaseItem;

            SettingsScreen.SettingsUpdated -= OnSettingsUpdated;
            SettingsScreen.ResetPlayerFunds -= OnResetFunds;
            
            CharScreenController.CharacterShown -= OnCharacterShown;
            CharScreenController.LevelPotionUsed -= OnLevelPotionUsed;
            
            LoginManager.LoggedIn -= ShowWelcomeMessage;
            LoginManager.LoginSkipped -= OnLoginSkipped;
            LoginManager.BalanceReceived -= SetFunds;
        }

        void Awake()
        {
            m_SaveManager = GetComponent<SaveManager>();
        }

        void Start()
        {
            //if saved data exists, load saved data
            m_SaveManager?.LoadGame();
            
        }

        // transaction methods 
        void UpdateFunds()
        {
            if (m_GameData != null)
                FundsUpdated?.Invoke(m_GameData.gold);
        }

        void UpdatePotions()
        {
            if (m_GameData != null)
                PotionsUpdated?.Invoke(m_GameData);
        }
        
        void SetFunds(float amount)
        {
            if (m_GameData == null)
                return;
            
            if (amount >= 0)
                m_GameData.gold = amount;
            
            FundsUpdated?.Invoke(m_GameData.gold);
        }

        bool HasSufficientFunds(ShopItemSO shopItem)
        {
            if (shopItem == null)
                return false;

            CurrencyType currencyType = shopItem.CostInCurrencyType;

            float discountedPrice = (((100 - shopItem.discount) / 100f) * shopItem.cost);

            switch (currencyType)
            {
                case CurrencyType.ETH:
                    return m_GameData.gold >= discountedPrice;
                default:
                    return false;
            }
        }

        // do we have enough potions to level up?
        bool CanLevelUp(CharacterData character)
        {
            if (m_GameData == null || character == null)
                return false;

            return (character.GetXPForNextLevel() <= m_GameData.levelUpPotions);
        }

        void PayTransaction(ShopItemSO shopItem)
        {
            if (shopItem == null)
                return;

            CurrencyType currencyType = shopItem.CostInCurrencyType;

            float discountedPrice = (((100 - shopItem.discount) / 100f) * shopItem.cost);

            switch (currencyType)
            {
                case CurrencyType.ETH:
                    SetFunds(m_GameData.gold - discountedPrice);
                    break;
            }
        }

        void PayLevelUpPotions(uint numberPotions)
        {
            if (m_GameData != null)
            {
                m_GameData.levelUpPotions -= numberPotions;
                PotionsUpdated?.Invoke(m_GameData);
            }
        }

        void ReceivePurchasedGoods(ShopItemSO shopItem)
        {
            if (shopItem == null)
                return;

            ReceiveContent(shopItem);
        }

        void ReceiveContent(ShopItemSO shopItem)
        {
            var nftManager = FindObjectOfType<NftManager>();
            ItemData nftItemData = nftManager.GetItemById(shopItem.NftItemId);

            if (nftItemData == null)
            {
                PostMinting(shopItem, "");
                return;
            }
            
            if (!string.IsNullOrEmpty(shopItem.NftListingId))
                PurchaseItem(shopItem, nftItemData);
            else
                MintItem(shopItem, nftItemData);
        }

        private async void PurchaseItem(ShopItemSO shopItem, ItemData item)
        {
            // Delay at least 100 ms because of visual elements
            await Task.Delay(100);

            if (m_IsLoginSkipped)
            {
                PostMinting(shopItem, "TestHash");
                return;
            }
            
            var nftManager = FindObjectOfType<NftManager>();
            var resultHash = string.Empty;

            try
            {
                var receipt = await nftManager.PurchaseItem(item);
                resultHash = receipt.TransactionHash;
            }
            catch (IOException ex)
            {
                if (ex.Message.Contains("Current policy max_per_user limit"))
                    resultHash = "AlreadyPurchased";
            }
            catch (Exception ex)
            {
                // Ignore
                Debug.LogError($"GameDataManager: PurchaseItem error: {ex.Message}");
            }

            UpdateBalanceDelayed();
            
            // The minting can take a while, so we just run it and don't wait for the result
            if (!string.IsNullOrEmpty(resultHash))
            {
#pragma warning disable CS4014
                nftManager.MintItem(item);
#pragma warning restore CS4014
                await Task.Delay(3000);
            }

            PostMinting(shopItem, resultHash);
        }

        private async void UpdateBalanceDelayed()
        {
            await Task.Delay(2000);
            UpdateBalance?.Invoke();
        }

        private async void MintItem(ShopItemSO shopItem, ItemData item)
        {
            // Delay at least 100 ms because of visual elements
            await Task.Delay(100);

            if (m_IsLoginSkipped)
            {
                PostMinting(shopItem, "TestHash");
                return;
            }
            
            var nftManager = FindObjectOfType<NftManager>();
            
            // Minting can take a while, so we can start the minting process and let it run in the background
            //var txHash = await nftManager.MintItem(item);
#pragma warning disable CS4014
            nftManager.MintItem(item);
#pragma warning restore CS4014
            await Task.Delay(3000);
            
            PostMinting(shopItem, "VeryNiceTxHash");
        }

        private void PostMinting(ShopItemSO shopItemSo, string txHash)
        {
            Debug.Log($"Post minting an NFT. TxHash: {txHash}");
            
            if (string.IsNullOrEmpty(txHash))
            {
                // If unsuccessful - invoke transaction failed
                MintingFailed?.Invoke(shopItemSo);
            }
            else
            {
                // Else - complete the purchase and add the item to the inventory
                TransactionEnded?.Invoke(shopItemSo);
                ItemPurchased?.Invoke(shopItemSo);
            }
        }
        
        void ShowWelcomeMessage()
        {
            string message = "Successfully logged in!";
            HomeMessageShown?.Invoke(message);
        }

        void OnLoginSkipped()
        {
            m_IsLoginSkipped = true;
        }

        // buying item from ShopScreen, pass button screen position 
        void OnPurchaseItem(ShopItemSO shopItem, Vector2 screenPos)
        {
            if (shopItem == null)
                return;

            // invoke transaction succeeded or failed
            if (HasSufficientFunds(shopItem))
            {
                //PayTransaction(shopItem);
                ReceivePurchasedGoods(shopItem);
                TransactionStarted?.Invoke(shopItem, screenPos);

                AudioManager.PlayDefaultTransactionSound();
            }
            else
            {
                // notify listeners (PopUpText, sound, etc.)
                TransactionFailed?.Invoke(shopItem);
                AudioManager.PlayDefaultWarningSound();
            }
        }

        // update values from SettingsScreen
        void OnSettingsUpdated(GameData gameData)
        {
            Debug.Log("GameDataManager: SettingsUpdated");

            if (gameData == null)
                return;

            m_GameData.sfxVolume = gameData.sfxVolume;
            m_GameData.musicVolume = gameData.musicVolume;
            m_GameData.dropdownSelection = gameData.dropdownSelection;
            m_GameData.isSlideToggled = gameData.isSlideToggled;
            m_GameData.isToggled = gameData.isToggled;
            m_GameData.theme = gameData.theme;
            m_GameData.username = gameData.username;
            m_GameData.buttonSelection = gameData.buttonSelection;
        }

        // attempt to level up the character using a potion
        void OnLevelPotionUsed(CharacterData charData)
        {
            if (charData == null)
                return;

            bool isLeveled = false;
            if (CanLevelUp(charData))
            {
                PayLevelUpPotions(charData.GetXPForNextLevel());
                isLeveled = true;
                AudioManager.PlayVictorySound();
            }
            else
            {
                AudioManager.PlayDefaultWarningSound();
            }
            // notify other objects if level up succeeded or failed
            CharacterLeveledUp?.Invoke(isLeveled);
        }

        void OnResetFunds()
        {
            m_GameData.gold = 0;
            m_GameData.healthPotions = 0;
            m_GameData.levelUpPotions = 0;
            UpdateFunds();
            UpdatePotions();
        }

        void OnCharacterShown(CharacterData charData)
        {
            // notify the CharScreen to enable or disable the LevelUpButton VFX
            LevelUpButtonEnabled?.Invoke(CanLevelUp(charData));
        }

    }
}
