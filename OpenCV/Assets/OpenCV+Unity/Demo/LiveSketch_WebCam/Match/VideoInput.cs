using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public abstract class VideoInput : MonoBehaviour {
    [SerializeField] private RawImage rawImage;

    private WebCamTexture _webCamTexture;
    private Texture2D _croppedTexture;
    private Texture _renderTexture;

    private RectInt _croppedCenter;

    public void Initialize(int width, int height){
   
        WebCamDevice device = WebCamTexture.devices.Last();

        _webCamTexture = new WebCamTexture(device.name)
        {
            requestedFPS = 1,
            /*requestedWidth = 1,
            requestedHeight = 1 */
        };

        _croppedTexture = new Texture2D(width, height);
        _webCamTexture.Play();
        rawImage.rectTransform.sizeDelta = new Vector2(_webCamTexture.width , _webCamTexture.height);
        _croppedCenter = GetCroppedRect(_webCamTexture.width, _webCamTexture.height, new Vector2Int(width,height));
    }

    private RectInt GetCroppedRect(int frameWidth, int frameHeight, Vector2Int cropSize){
        var x = frameWidth / 2 - cropSize.x / 2;
        var y = frameHeight / 2 - cropSize.y / 2;
        return new RectInt(x, y, cropSize.x, cropSize.y);
    }

    public abstract void ProcessTexture(Texture2D webcamTexture/*, ref Texture outTexture*/);

    private void Update(){
        if (_webCamTexture.didUpdateThisFrame){
         
            _croppedTexture.SetPixels(_webCamTexture.GetPixels(_croppedCenter.x, _croppedCenter.y,_croppedCenter.width,_croppedCenter.height));

            ProcessTexture(_croppedTexture/*, ref _renderTexture*/);
            rawImage.texture = _webCamTexture;
        }
    }

    private void OnDestroy(){
        _webCamTexture.Stop();
    }
}