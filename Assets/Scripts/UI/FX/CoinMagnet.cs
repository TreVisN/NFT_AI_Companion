using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using Image = UnityEngine.UI.Image;

namespace UIToolkitDemo
{
    [Serializable]
    public struct MagnetData
    {
        // gold, gems, health potion, power potion
        public ShopItemType ItemType;

        // ParticleSystem pool
        public ObjectPoolBehaviour FXPool;

        // forcefield target
        public ParticleSystemForceField ForceField;
    }

    // FX generator when buying an item from the shop
    public class CoinMagnet : MonoBehaviour
    {

        [Header("UI Elements")]
        [Tooltip("Locate screen space positions from this document's UI elements.")]
        [SerializeField] UIDocument m_Document;
        [Tooltip("Match a target VisualElement by name to each ShopItemType")]
        //[FormerlySerializedAs("m_TargetElementsByType")]
        [SerializeField] List<MagnetData> m_MagnetData;

        [Header("Camera")]
        [Tooltip("Use Camera and Depth to calculate world space positions.")]
        [SerializeField] Camera m_Camera;
        [SerializeField] float m_ZDepth = 10f;
        [Tooltip("3D offset to the particle emission")]
        [SerializeField] Vector3 m_SourceOffset = new Vector3(0f, 0.1f, 0f);

        [SerializeField] private Canvas m_ItemCanvas;
        [SerializeField] private Image m_ItemImage;
        
        private ParticleSystem lastActiveParticleSystem;

        // start and end coordinates for effect
        void OnEnable()
        {
            GameDataManager.TransactionStarted += OnTransactionStarted;
            ShopScreen.ShopItemPurchasingClosed += OnPurchasingClosed;
        }

        void OnDisable()
        {
            GameDataManager.TransactionStarted -= OnTransactionStarted;
            ShopScreen.ShopItemPurchasingClosed -= OnPurchasingClosed;
        }

        ObjectPoolBehaviour GetFXPool(ShopItemType itemType)
        {
            MagnetData magnetData = m_MagnetData.Find(x => x.ItemType == itemType);
            return magnetData.FXPool;
        }

        ParticleSystemForceField GetForcefield(ShopItemType itemType)
        {
            MagnetData magnetData =  m_MagnetData.Find(x => x.ItemType == itemType);
            return magnetData.ForceField;
        }
        
        void PlayPooledFX(Vector2 screenPos, ShopItemType contentType)
        {
            if (lastActiveParticleSystem != null)
                lastActiveParticleSystem.Stop();
            
            Vector3 worldPos = new Vector2(Screen.width/2, Screen.height/2).ScreenPosToWorldPos(m_Camera, m_ZDepth) + m_SourceOffset;

            ObjectPoolBehaviour fxPool = GetFXPool(contentType);

            // initialize ParticleSystem
            lastActiveParticleSystem = fxPool.GetPooledObject().GetComponent<ParticleSystem>();

            if (lastActiveParticleSystem == null)
                return;

            lastActiveParticleSystem.gameObject.SetActive(true);
            lastActiveParticleSystem.gameObject.transform.position = worldPos;
            ParticleSystem.ExternalForcesModule externalForces = lastActiveParticleSystem.externalForces;
            externalForces.enabled = true;

            lastActiveParticleSystem.Play();
        }

        void StopFX()
        {
            if (lastActiveParticleSystem != null)
                lastActiveParticleSystem.Stop();
        }

        // event-handling methods
        

        // buying an item from the ShopScreen
        void OnTransactionStarted(ShopItemSO shopItem, Vector2 screenPos)
        {
            PlayPooledFX(screenPos, shopItem.contentType);
            
            m_ItemCanvas.gameObject.SetActive(true);
            m_ItemImage.sprite = shopItem.sprite;
        }

        void OnPurchasingClosed(ShopItemSO shopItem, bool success)
        {
            m_ItemCanvas.gameObject.SetActive(false);
            StopFX();
        }
    }
}
