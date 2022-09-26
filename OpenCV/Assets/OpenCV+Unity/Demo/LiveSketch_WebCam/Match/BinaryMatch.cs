namespace OpenCvSharp.Demo {
    using UnityEngine;
    using UnityEngine.UI;
    using OpenCvSharp;

    public class BinaryMatch : MonoBehaviour {
        [SerializeField] private Texture2D tempTexture, backTexture;
        [SerializeField] private RawImage template, background;
        [SerializeField] private Button action;

        [SerializeField] private float cannyThreshold1 = 10, cannyThreshold2 = 70;


        void Start(){
            template.rectTransform.sizeDelta = new Vector2(tempTexture.width, tempTexture.height);
            template.texture = tempTexture;
            background.rectTransform.sizeDelta = new Vector2(backTexture.width, backTexture.height);
            background.texture = backTexture;
            action.onClick.AddListener(ShapeMatch);
        }

        private void TemplateMatch(){
            Mat templateImageBinary = GetEdges(Unity.TextureToMat(tempTexture));
            Mat backImageBinary = GetEdges(Unity.TextureToMat(backTexture));
            Mat backImage = Unity.TextureToMat(backTexture);

            Mat result = new Mat();
            Cv2.MatchTemplate(backImageBinary, templateImageBinary, result, TemplateMatchModes.CCoeffNormed);
            Cv2.MinMaxLoc(result, out double minf , out double maxf, out Point min, out Point max);
            Debug.Log(maxf);

            var width = templateImageBinary.Width;
            var height = templateImageBinary.Height;
            Cv2.Rectangle(backImage, max, new Point(max.X + width, max.Y + height), new Scalar(0, 255, 0));
            
            backTexture = Unity.MatToTexture(backImage);
            background.texture = backTexture;
        }

        private void ShapeMatch(){
            Mat templateImageBinary = GetEdges(Unity.TextureToMat(tempTexture));
            Mat backImageBinary = GetEdges(Unity.TextureToMat(backTexture));

            
            Debug.Log(Cv2.MatchShapes(templateImageBinary, backImageBinary, ShapeMatchModes.I2));
        }

        private Mat GetEdges(Mat input){
            Mat imgGray = new Mat();
            Cv2.CvtColor(input, imgGray, ColorConversionCodes.BGR2GRAY);
          //   Mat imgGrayBlur = new Mat();
          //  Cv2.GaussianBlur(imgGray, imgGrayBlur, new Size(5, 5), 0);

            Mat cannyEdges = new Mat();
            Cv2.Canny(imgGray, cannyEdges, cannyThreshold1, cannyThreshold2);

            Mat mask = new Mat();
            Cv2.Threshold(cannyEdges, mask, 70.0, 255.0, ThresholdTypes.Binary);

            return mask;
            // output = Unity.MatToTexture(img, output);
        }
    }
}