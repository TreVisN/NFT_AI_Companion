using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;
using Vector2 = UnityEngine.Vector2;

namespace UIToolkitDemo
{
    [RequireComponent(typeof(TabbedMenu))]
    public class ShopScreen : MenuScreen
    {
        // string IDs
        const string k_GoldScrollview = "shop__gold-scrollview";
        const string k_GemScrollview = "shop__gem-scrollview";
        const string k_PotionScrollview = "shop__potion-scrollview";
        const string k_ResourcePath = "GameData/GameIcons";

        const string k_PurchaseOverlay = "shop__purchase-overlay";
        const string k_PurchaseOverlayButton = "shop__purchase-overlay-button";
        const string k_PurchaseOverlayLabel = "shop__purchase-overlay-minting";

        [Header("Shop Item")]
        [Tooltip("ShopItem Element Asset to instantiate ")]
        [SerializeField] VisualTreeAsset m_ShopItemAsset;
        [SerializeField] GameIconsSO m_GameIconsData;

        // visual elements
        // each tab/parent element for each category of ShopItem
        VisualElement m_GoldScrollview;
        VisualElement m_GemsScrollview;
        VisualElement m_PotionScrollview;
        VisualElement m_PurchaseOverlay;
        Button m_PurchaseOverlayButton;
        Label m_PurchaseOverlayLabel;

        TabbedMenu m_TabbedMenu;
        
        public static event Action ShopOpened;
        public static event Action<ShopItemSO, bool> ShopItemPurchasingClosed;

        void OnEnable()
        {
            SetVisualElements();

            ShopController.ShopTabRefilled += RefillShopTab;
            GameDataManager.TransactionStarted += EnableBackgroundOverlay;
            GameDataManager.TransactionEnded += EnableBackgroundOverlayButtons;
            GameDataManager.MintingFailed += DisableOverlayFailed;
        }

        void OnDisable()
        {
            ShopController.ShopTabRefilled -= RefillShopTab;
            GameDataManager.TransactionStarted -= EnableBackgroundOverlay;
            GameDataManager.TransactionEnded -= EnableBackgroundOverlayButtons;
            GameDataManager.MintingFailed -= DisableOverlayFailed;
        }

        protected override void Awake()
        {
            base.Awake();

            // this ScriptableObject pairs data types (ShopItems, Skills, Rarity, Classes, etc.) with specific icons 
            // (default path = Resources/GameData/GameIcons)
            m_GameIconsData = Resources.Load<GameIconsSO>(k_ResourcePath);

            if (m_TabbedMenu == null)
                m_TabbedMenu = GetComponent<TabbedMenu>();
        }

        public override void ShowScreen()
        {
            base.ShowScreen();
            m_TabbedMenu?.SelectFirstTab();
            
            ShopOpened?.Invoke();
        }

        protected override void SetVisualElements()
        {
            base.SetVisualElements();
            m_GoldScrollview = m_Root.Q<VisualElement>(k_GoldScrollview);
            m_GemsScrollview = m_Root.Q<VisualElement>(k_GemScrollview);
            m_PotionScrollview = m_Root.Q<VisualElement>(k_PotionScrollview);
            m_PurchaseOverlay = m_Root.Q<VisualElement>(k_PurchaseOverlay);
            m_PurchaseOverlayButton = m_Root.Q<Button>(k_PurchaseOverlayButton);
            m_PurchaseOverlayLabel = m_Root.Q<Label>(k_PurchaseOverlayLabel);
        }

        // fill a tab with content
        private void RefillShopTab(List<ShopItemSO> shopItems, List<ShopItemSO> ownedItems)
        {
            if (shopItems == null || shopItems.Count == 0)
                return;

            VisualElement parentTab = null;
            switch (shopItems[0].contentType)
            {
                case ShopItemType.Gear:
                    parentTab = m_GoldScrollview;
                    break;
                case ShopItemType.Characters:
                    parentTab = m_GemsScrollview;
                    break;
                default:
                    parentTab = m_PotionScrollview;
                    break;
            }

            parentTab.Clear();

            foreach (ShopItemSO shopItem in shopItems)
            {
                bool isItemOwned = ownedItems.Any(x => x.itemName == shopItem.itemName);
                CreateShopItemElement(shopItem, parentTab, isItemOwned);
            }
        }

        void CreateShopItemElement(ShopItemSO shopItemData, VisualElement parentElement, bool itemOwned)
        {
            if (parentElement == null || shopItemData == null || m_ShopItemAsset == null)
                return;

            // instantiate a new Visual Element from a template UXML
            TemplateContainer shopItemElem = m_ShopItemAsset.Instantiate();

            // sets up the VisualElements and game data per ShopItem
            ShopItemComponent shopItemController = new ShopItemComponent(m_GameIconsData, shopItemData);
            shopItemController.SetVisualElements(shopItemElem);
            shopItemController.SetGameData(shopItemElem);

            if (itemOwned)
                shopItemController.MarkAsOwned();
            else
                shopItemController.RegisterCallbacks();

            parentElement.Add(shopItemElem);
        }

        // TO-DO: replace with an Event Handler
        // jump to a specific tab (used from OptionsBar)
        public void SelectTab(string tabName)
        {
            m_TabbedMenu?.SelectTab(tabName);
        }

        void EnableBackgroundOverlay(ShopItemSO shopItem, Vector2 pos)
        {
            m_PurchaseOverlay.style.display = DisplayStyle.Flex;
            m_PurchaseOverlayLabel.style.display = DisplayStyle.Flex;
            m_PurchaseOverlayButton.style.display = DisplayStyle.None;
        }

        void EnableBackgroundOverlayButtons(ShopItemSO shopItem)
        {
            m_PurchaseOverlayLabel.style.display = DisplayStyle.None;
            m_PurchaseOverlayButton.style.display = DisplayStyle.Flex;
            m_PurchaseOverlayButton.clicked += () =>
            {
                DisableOverlay(shopItem, true);
            };
        }
        
        void DisableOverlayFailed(ShopItemSO shopItem)
        {
            DisableOverlay(shopItem, false);
        }
        

        void DisableOverlay(ShopItemSO shopItem, bool success)
        {
            m_PurchaseOverlay.style.display = DisplayStyle.None;
            m_PurchaseOverlayButton.style.display = DisplayStyle.None;
            
            ShopItemPurchasingClosed?.Invoke(shopItem, success);
        }
    }
}