using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UIToolkitDemo
{
    // what player is buying
    [System.Serializable]
    public enum ShopItemType
    {
        // soft currency (in-game)
        Gear,

        // hard currency (buy with real money)
        Characters
    }

    // type of currency used to purchase
    [System.Serializable]
    public enum CurrencyType
    {
        ETH
    }

    [CreateAssetMenu(fileName = "Assets/Resources/GameData/ShopItems/ShopItemGameData", menuName = "UIToolkitDemo/ShopItem", order = 4)]
    public class ShopItemSO : ScriptableObject
    {
        public string itemName;
        
        public Sprite sprite;

        // FREE if equal to 0; cost amount in CostInCurrencyType below
        public float cost;

        // UI shows tag if value larger than 0 (percentage off)
        public uint discount;
        
        // if not empty, UI shows a banner with this text
        public string promoBannerText;
        
        // how many potions/coins this item gives the player upon purchase
        public uint contentValue;
        public ShopItemType contentType;

        // SC (gold) costs HC (gems); HC (gems) costs real USD; HealthPotion costs SC (gold); LevelUpPotion costs HC (gems)
        public CurrencyType CostInCurrencyType
        {
            get
            {
                switch (contentType)
                {
                    case (ShopItemType.Gear):
                        return CurrencyType.ETH;

                    case (ShopItemType.Characters):
                        return CurrencyType.ETH;

                    default:
                        return CurrencyType.ETH;
                }
            }
        }
        
        [Header("Nft Identity")]
        // Nft Item Id that corresponds to the ConsenSys-nft system
        public string NftItemId;

        // Nft Listing Id that corresponds to the ConsenSys-nft system
        public string NftListingId;
    }
}