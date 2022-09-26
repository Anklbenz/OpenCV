	namespace OpenCvSharp.Demo {
		public static class CVFeatures {

			public static Mat GetDilate(Mat input, bool binary = false, bool blurred = true){
				var edges = GetCannyEdges(input);

				Cv2.Dilate(edges, edges, new Mat(), null, 1);
				return edges;
			}

			public static Mat GetBinaryThreshold(Mat input, float threshold1, float threshold2){
				Cv2.Threshold(input, input, threshold1, threshold2, ThresholdTypes.Binary);
				return input;
			}

			public static Mat GetCannyEdges(Mat input, bool blurred = true, bool binary = false, float thres1 = 10, float thres2 = 100){
				//Mat imgGray = new Mat();
				Cv2.CvtColor(input, input, ColorConversionCodes.BGR2GRAY);

				if (blurred)
					Cv2.GaussianBlur(input, input, new Size(5, 5), 0);

				//Mat cannyEdges = new Mat();
				Cv2.Canny(input, input, thres1, thres2);

				if (binary)
					Cv2.Threshold(input, input, 70, 255, ThresholdTypes.Binary);
				return input;
			}
		}
	}