using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

namespace UIToolkitDemo
{
    // presenter/controller for the ShopScreen
    public class ShopController : MonoBehaviour
    {
        // notify ShopScreen (pass ShopItem data + screen pos of buy button)
        public static event Action<ShopItemSO, Vector2> ShopItemPurchasing;
        public static event Action<List<ShopItemSO>, List<ShopItemSO>> ShopTabRefilled;

        [Tooltip("Path within the Resources folders for MailMessage ScriptableObjects.")]
        [SerializeField] string m_ResourcePath = "GameData/ShopItems";

        // ScriptableObject game data from Resources
        List<ShopItemSO> m_ShopItems = new();
        List<ShopItemSO> m_OwnedItems = new();

        // game data filtered in categories
        List<ShopItemSO> m_GearShopItems = new();
        List<ShopItemSO> m_CharacterShopItems = new();

        void OnEnable()
        {
            ShopScreen.ShopOpened += OnShopOpened;
            ShopItemComponent.ShopItemClicked += OnTryBuyItem;
            GameDataManager.ItemPurchased += OnItemPurchased;
        }

        void OnDisable()
        {
            ShopScreen.ShopOpened -= OnShopOpened;
            ShopItemComponent.ShopItemClicked -= OnTryBuyItem;
            GameDataManager.ItemPurchased -= OnItemPurchased;
        }

        void OnShopOpened()
        {
            LoadShopData();
            UpdateView();
        }

        void OnItemPurchased(ShopItemSO shopItem)
        {
            var nftManager = FindObjectOfType<NftManager>();

            if (nftManager == null)
                return;
            
            AddUnlockedItem(nftManager, shopItem);
            LoadShopData();
            UpdateView();
        }

        void AddUnlockedItem(NftManager nftManager, ShopItemSO shopItem)
        {
            var nftItemData = nftManager.GetItemById(shopItem.NftItemId);
            
            if (nftItemData != null)
                nftManager.AddUnlockedItem(nftItemData);
        }
        
        // fill the ShopScreen with data
        void LoadShopData()
        {
            m_ShopItems.Clear();
            m_OwnedItems.Clear();
            
            // load the ScriptableObjects from the Resources directory (default = Resources/GameData/MailMessages)
            m_ShopItems.AddRange(Resources.LoadAll<ShopItemSO>(m_ResourcePath));
            m_OwnedItems = FindOwnedItems();

            // sort them by type
            m_GearShopItems = m_ShopItems.Where(c => c.contentType == ShopItemType.Gear).ToList();
            m_CharacterShopItems = m_ShopItems.Where(c => c.contentType == ShopItemType.Characters).ToList();

            m_GearShopItems = SortShopItems(m_GearShopItems);
            m_CharacterShopItems = SortShopItems(m_CharacterShopItems);
        }
        
        List<ShopItemSO> FindOwnedItems()
        {
            var nftManager = FindObjectOfType<NftManager>();
            
            var tempList = new List<ShopItemSO>();

            foreach (var item in m_ShopItems.Where(item => nftManager.GetIsItemOwned(item.NftItemId)))
            {
                Debug.Log($"Added {item.itemName} to the owned shop items because NFT was found.");
                tempList.Add(item);
            }
            
            foreach (var item in m_ShopItems.Where(item => nftManager.GetIsItemPreOwned(item.NftItemId)))
            {
                Debug.Log($"Added {item.itemName} to the owned shop items because preowned item was found.");
                tempList.Add(item);
            }

            return tempList;
        }

        List<ShopItemSO> SortShopItems(List<ShopItemSO> originalList)
        {
            return originalList.OrderBy(x => x.cost).ToList();
        }

        void UpdateView()
        {
            ShopTabRefilled?.Invoke(m_GearShopItems, m_OwnedItems);
            ShopTabRefilled?.Invoke(m_CharacterShopItems, m_OwnedItems);
        }

        void OnTryBuyItem(ShopItemSO shopItemData, Vector2 screenPos)
        {
            if (shopItemData == null)
                return;

            // notify other objects we are trying to buy an item
            ShopItemPurchasing?.Invoke(shopItemData, screenPos);
        }
    }
}