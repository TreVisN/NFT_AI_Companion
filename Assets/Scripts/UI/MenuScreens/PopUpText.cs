using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using System;

namespace UIToolkitDemo
{
    public class PopUpText : MenuScreen
    {
        // rich text color highlight
        public static string TextHighlight = "F8BB19";

        const string k_PopUpText = "main-menu-popup_text";

        // contains basic text styling
        const string k_PopUpTextClass = "popup-text";

        // each message contains its own styles

        // ShopScreen message classes
        const string k_ShopActiveClass = "popup-text-active";
        const string k_ShopInactiveClass = "popup-text-inactive";

        // CharScreen message classes
        const string k_GearActiveClass = "popup-text-active--left";
        const string k_GearInactiveClass = "popup-text-inactive--left";

        // HomeScreen message classes
        const string k_HomeActiveClass = "popup-text-active--home";
        const string k_HomeInactiveClass = "popup-text-inactive--home";

        // delay between welcome messages
        const float k_HomeMessageCooldown = 15f;

        float m_TimeToNextHomeMessage = 0f;


        Label m_PopUpText;

        // customizes active/inactive styles, duration, and delay for each text prompt
        float m_Delay = 0f;
        float m_Duration = 1f;
        string m_ActiveClass;
        string m_InactiveClass;

        void OnEnable()
        {
            InventoryScreen.GearSelected += OnGearSelected;
            ShopScreen.ShopItemPurchasingClosed += OnTransactionProcessed;
            GameDataManager.TransactionFailed += OnTransactionFailed;
            GameDataManager.MintingFailed += OnMintingFailed;
            GameDataManager.CharacterLeveledUp += OnCharacterLeveledUp;
            GameDataManager.HomeMessageShown += OnHomeMessageShown;
        }
        
        void OnDisable()
        {
            InventoryScreen.GearSelected -= OnGearSelected;
            ShopScreen.ShopItemPurchasingClosed -= OnTransactionProcessed;
            GameDataManager.MintingFailed -= OnMintingFailed;
            GameDataManager.TransactionFailed -= OnTransactionFailed;
            GameDataManager.CharacterLeveledUp -= OnCharacterLeveledUp;
            GameDataManager.HomeMessageShown -= OnHomeMessageShown;
        }

        protected override void Awake()
        {
            base.Awake();
            SetVisualElements();

            m_ActiveClass = k_ShopActiveClass;
            m_InactiveClass = k_ShopInactiveClass;

            SetupText();
            HideText();
        }

        protected override void SetVisualElements()
        {
            base.SetVisualElements();
            m_PopUpText = m_Root.Q<Label>(k_PopUpText);

            if (m_PopUpText != null)
            {
                m_PopUpText.text = string.Empty;
            }
        }

        void ShowMessage(string message)
        {
            if (m_PopUpText == null || string.IsNullOrEmpty(message))
            {
                return;
            }

            StartCoroutine(ShowMessageRoutine(message));
        }

        IEnumerator ShowMessageRoutine(string message)
        {
            if (m_PopUpText != null)
            {
                m_PopUpText.text = message;

                // reset any leftover Selectors
                SetupText();

                // hide text
                HideText();

                // wait for delay
                yield return new WaitForSeconds(m_Delay);

                // show text and wait for duration
                ShowText();
                yield return new WaitForSeconds(m_Duration);

                // hide text again
                HideText();
            }
        }

        void HideText()
        {
            m_PopUpText?.RemoveFromClassList(m_ActiveClass);
            m_PopUpText?.AddToClassList(m_InactiveClass);
        }

        void ShowText()
        {
            m_PopUpText?.RemoveFromClassList(m_InactiveClass);
            m_PopUpText?.AddToClassList(m_ActiveClass);
        }

        // clear any remaining Selectors and add base styling 
        void SetupText()
        {
            m_PopUpText?.ClearClassList();
            m_PopUpText?.AddToClassList(k_PopUpTextClass);
        }

        // event-handling methods
        
        void OnMintingFailed(ShopItemSO shopItemSO)
        {
            Debug.Log("OnMintingFailed");
            
            // use a longer delay, messages are longer here
            m_Delay = 0f;
            m_Duration = 1.2f;

            // centered on ShopScreen
            m_ActiveClass = k_ShopActiveClass;
            m_InactiveClass = k_ShopInactiveClass;

            string failMessage = "Failed to mint <color=#" + PopUpText.TextHighlight + ">" + shopItemSO.itemName + "</color>. Please try again.";
            ShowMessage(failMessage);
        }

        void OnTransactionFailed(ShopItemSO shopItemSO)
        {
            Debug.Log("OnTransactionFailed");

            // use a longer delay, messages are longer here
            m_Delay = 0f;
            m_Duration = 1.2f;

            // centered on ShopScreen
            m_ActiveClass = k_ShopActiveClass;
            m_InactiveClass = k_ShopInactiveClass;

            string failMessage = "Insufficient funds for <color=#" + PopUpText.TextHighlight + ">" + shopItemSO.itemName + "</color>.";
            ShowMessage(failMessage);
        }

        void OnGearSelected(EquipmentSO gear)
        {
            Debug.Log("OnGearSelected");

            m_Delay = 0f;
            m_Duration = 0.8f;

            // centered on CharScreen
            m_ActiveClass = k_GearActiveClass;
            m_InactiveClass = k_GearInactiveClass;

            string equipMessage = "<color=#" + PopUpText.TextHighlight + ">" + gear.equipmentName + "</color> equipped.";
            ShowMessage(equipMessage);
        }

        void OnCharacterLeveledUp(bool state)
        {
            Debug.Log("OnCharacterLeveledUp");
            // only show text warning on failure
            if (state)
                return;
            
            // timing and position
            m_Delay = 0f;
            m_Duration = 0.8f;

            // centered on CharScreen
            m_ActiveClass = k_GearActiveClass;
            m_InactiveClass = k_GearInactiveClass;

            if (m_PopUpText != null)
            {
                string equipMessage = "Insufficient potions to level up.";
                ShowMessage(equipMessage);
            }
        }
        void OnHomeMessageShown(string message)
        {
            Debug.Log("OnHomeMessageShown");

            if (!(Time.time >= m_TimeToNextHomeMessage))
                return;
            
            m_Delay = 0.25f;
            m_Duration = 1.5f;

            m_ActiveClass = k_HomeActiveClass;
            m_InactiveClass = k_HomeInactiveClass;

            ShowMessage(message);

            // cooldown delay to avoid spamming
            m_TimeToNextHomeMessage = Time.time + k_HomeMessageCooldown;
        }

        void OnTransactionProcessed(ShopItemSO shopItem, bool success)
        {
            Debug.Log("OnTransactionProcessed");

            if (!success)
                return;

            if (shopItem.contentType != ShopItemType.Characters && shopItem.contentType != ShopItemType.Gear)
                return;
            
            m_Delay = 1f;
            m_Duration = 1.2f;

            m_ActiveClass = k_ShopActiveClass;
            m_InactiveClass = k_ShopInactiveClass;

            string buyMessage = "<color=#" + PopUpText.TextHighlight + ">" + shopItem.itemName + "</color> added to inventory.";
            ShowMessage(buyMessage);
        }
    }
}
