using System.Collections.Generic;
using OpenCvSharp;
using UnityEngine;
using UnityEngine.UI;

namespace OpenCvSharp.Demo {

	public class LiveSketchScript : VideoInput {
		[SerializeField] private float cannyThreshold1 = 10;
		[SerializeField] private float cannyThreshold2 = 70;
		[SerializeField] private float threshold1 = 70, threshold2 = 255;
		[SerializeField] private RawImage templateRawImage, backRawImage;
		[SerializeField] private Text label;
		[SerializeField] private Button action;
		private Texture2D _templateTexture;

		private Mat _templateMat1, _templateMat2, _dilateMat;
		private Point[][] _templateContours;
		private Color32[] _colors;

		private void Awake(){
			TemplateInitialize();

			var width = _templateMat1.Width;
			var height = _templateMat1.Height;
			
			base.Initialize(width, height);

			templateRawImage.rectTransform.sizeDelta = new Vector2(templateRawImage.texture.width, templateRawImage.texture.height);
			backRawImage.rectTransform.sizeDelta = new Vector2(backRawImage.texture.width, backRawImage.texture.height);
			
			_templateTexture = new Texture2D(width, height);
			action.onClick.AddListener(TemplateMatchEdges);
		}

		private void TemplateMatchEdges(){
			_templateMat1 = Unity.TextureToMat((Texture2D)templateRawImage.texture);
			_templateMat2 = Unity.TextureToMat((Texture2D)backRawImage.texture);
		
			var img1 = CVFeatures.GetDilate(_templateMat1, true);
			var img2 = CVFeatures.GetDilate(_templateMat2, true);

			var matchResult = Match(img1, img2);
			Cv2.MinMaxLoc(matchResult, out double minf, out double maxf, out Point min, out Point max);

			Debug.Log("Max "+ maxf + max + "Min "+ minf + min);
			var source = DrawMarkers(img2, max);

			templateRawImage.texture = Unity.MatToTexture(img1);
			backRawImage.texture = Unity.MatToTexture(source);
		}

		private Mat DrawMarkers(Mat source, Point matchPoint){
			var width = _templateMat1.Width;
			var height = _templateMat1.Height;
			Cv2.Rectangle(source, matchPoint, new Point(matchPoint.X + width, matchPoint.Y + height), new Scalar(255, 255, 255), 3);
			return source;
		}

		private Mat Match(Mat template, Mat background){
			Mat matchResult = new Mat();
     		Cv2.MatchTemplate(background, template, matchResult, TemplateMatchModes.CCoeff);
			return matchResult;
		}

		private void TemplateInitialize(){
			_templateMat1 = Unity.TextureToMat((Texture2D)templateRawImage.texture);
			templateRawImage.rectTransform.sizeDelta = new Vector2(_templateMat1.Width, _templateMat1.Height);
			_dilateMat = CVFeatures.GetDilate(_templateMat1, true);

			templateRawImage.texture = SetBlackTransparent(Unity.MatToTexture(_dilateMat));
		}

		private Texture2D SetBlackTransparent(Texture2D texture){
			var pixels = texture.GetPixels();

			for (var i = 0; i < pixels.Length; ++i)
				if (pixels[i] == Color.black)
					pixels[i] = Color.clear;

			texture.SetPixels(pixels);
			texture.Apply();

			return texture;
		}


		public override void ProcessTexture(Texture2D cropedTexture/*, ref Texture outTexture*/){
			//Texture2D text = webcamTexture.GetPixels();

			Mat img = Unity.TextureToMat(cropedTexture);
			var dilate = CVFeatures.GetDilate(img,  true);

		//		Match(_dilateMat,dilate);
			Mat matchResult = new Mat();
			Cv2.MatchTemplate(dilate, _dilateMat, matchResult, TemplateMatchModes.CCoeffNormed);
			Cv2.MinMaxLoc(matchResult, out double minf, out double maxf, out Point min, out Point max);

			var width = _dilateMat.Width;
			var height = _dilateMat.Height;
			Cv2.Rectangle(dilate, max, new Point(max.X + width, max.Y + height), new Scalar(255, 255, 255));


			label.color = Color.red;
			label.text = "Совпадение на " + Mathf.Abs((float)maxf * 100) + "%";

			if (maxf > 0.55f) label.color = Color.green;
			
			//Unity.MatToTexture(dilate, _templateTexture);

			//outTexture = _templateTexture;
			//return /* new Texture2D(10,10); //*/Unity.MatToTexture(dilate);
		}

		//	Mat mask = new Mat();
		//	Cv2.Threshold(edges, mask, threshold1, threshold2, ThresholdTypes.Binary);

		private void DrawContours(Mat mask, Mat img){
			Cv2.FindContours(mask, out var contours, out var hierarchy, RetrievalModes.Tree, ContourApproximationModes.ApproxNone, null);

			//	Cv2
			Debug.Log(contours.Length);
			/*foreach (var contour in contours){*/
			//	Cv2.DrawContours();

			/*double length = Cv2.ArcLength(contour, true);
			
			var color = new Scalar(0, 255, 0);*/

			//}
		}
	}
}


/// Frame top on image
/*
Mat[] rgb = new Mat[3];
Cv2.Split(img, out rgb);

Mat[] rgba = { rgb[0], rgb[1], rgb[2], mask };
Mat result = new();
Cv2.Merge(rgba, result);*/


/*Cv2.FindContours(_dilateMat, out _templateContours, out var hierarchy, RetrievalModes.Tree, ContourApproximationModes.ApproxSimple, null);
for (var i = 0; i < _templateContours.Length; ++i){
	Debug.Log(Cv2.ArcLength(_templateContours[i], false));
}
Cv2.DrawContours(_templateMat, new List<Point[]>(){_templateContours[10]}, -1, new Scalar(Random.Range(0,255), Random.Range(0,255), Random.Range(0,255)), 3);
*/

//_templateMat = Unity.MatToTexture(templateMat);
//	templateRawImage.texture = Unity.MatToTexture(_templateMat1);

//	Cv2.FindContours(dilate, out var contours, out var hierarchy, RetrievalModes.List, ContourApproximationModes.ApproxSimple, null);

/*for (var i = 0; i < _templateContours.Length; ++i){
	for (var j = 0; j < contours.Length; ++j){
		Debug.Log(Cv2.MatchShapes(_templateContours[i], contours[j], ShapeMatchModes.I1));
	}
}*/

//	Cv2.DrawContours(img, contours, -1, new Scalar(0, 255, 0), 2);
//	Debug.Log(contours.Length);


