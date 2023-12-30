using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering;

namespace GameEngine.ImageClassification
{
	public class Preprocess : MonoBehaviour
	{

		private RenderTexture _renderTexture;
		private Vector2 _scale = new Vector2(1, 1);
		private Vector2 _offset = Vector2.zero;

		private UnityAction<byte[]> callback;

		public void ScaleAndCropImage(WebCamTexture webCamTexture, int desiredSize, UnityAction<byte[]> callback)
		{
			this.callback = callback;

			if (_renderTexture == null)
			{
				_renderTexture = new RenderTexture(desiredSize, desiredSize, 0, RenderTextureFormat.ARGB32);
			}

			_scale.x = (float)webCamTexture.height / (float)webCamTexture.width;
			_offset.x = (1 - _scale.x) / 2f;
			Graphics.Blit(webCamTexture, _renderTexture, _scale, _offset);
			AsyncGPUReadback.Request(_renderTexture, 0, TextureFormat.RGB24, OnCompleteReadback);
		}

		void OnCompleteReadback(AsyncGPUReadbackRequest request)
		{
			if (request.hasError)
			{
				Debug.Log("GPU readback error detected.");
				return;
			}

			callback.Invoke(request.GetData<byte>().ToArray());
		}
	}
}