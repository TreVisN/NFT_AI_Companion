using UIToolkitDemo;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using ZXing;
using ZXing.QrCode;
using Button = UnityEngine.UIElements.Button;

public class MetamaskLoginView : MonoBehaviour
{
    public GameObject Companion;   
    public RawImage QRImage;   
    [SerializeField]
    // public GameObject ConnectButton;
    
    // Button ConnectButtonComponent;


    public void OnLoginClick()
    {
        
    }
    
    // Start is called before the first frame update
    void Start()
    {
        LoginManager.UrlGenerated += ShowQrCode;
        LoginManager.LoggedIn += (() =>
        {
            QRImage.gameObject.SetActive(false);
            Companion.SetActive(true);
        });
        // ConnectButtonComponent = ConnectButton.AddComponent<Button>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    void ShowQrCode(string url)
    {
        // Generate QR code for Desktop and Unity Editor
#if UNITY_EDITOR || (!UNITY_ANDROID && !UNITY_IOS)
        var generatedTex = GenerateQrTexture(url);
        
        // ConnectButton.style.backgroundImage = generatedTex;
        QRImage.texture = generatedTex;
        // ConnectButtonComponent.text = "";
        // ConnectButtonComponent.style.height = 300;
        // ConnectButtonComponent.style.marginTop = 4;
#endif
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
