using UnityEngine;
using UnityEngine.UI;

namespace ImageClassification.Core
{
	public class CameraView : MonoBehaviour
	{
		private RawImage _rawImage;
		private AspectRatioFitter _fitter;

		private WebCamTexture _webcamTexture;
		private bool _ratioSet;

		void Start()
		{
			_rawImage = GetComponent<RawImage>();
			_fitter = GetComponent<AspectRatioFitter>();
			InitWebCam();
		}

		void Update()
		{
			if (_webcamTexture.width > 100 && !_ratioSet)
			{
				_ratioSet = true;
				SetAspectRatio();
			}
		}

		void SetAspectRatio()
		{
			_fitter.aspectRatio = (float)_webcamTexture.width / (float)_webcamTexture.height;
		}

		void InitWebCam()
		{
			string camName = WebCamTexture.devices[0].name;
			_webcamTexture = new WebCamTexture(camName, Screen.width, Screen.height, 30);
			_rawImage.texture = _webcamTexture;
			_webcamTexture.Play();
		}

		public WebCamTexture GetCamImage()
		{
			return _webcamTexture;
		}
	}
}