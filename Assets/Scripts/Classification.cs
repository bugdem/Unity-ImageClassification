using UnityEngine;
using Unity.Barracuda;
using System.Collections.Generic;
using System.Linq;
using System.Collections;
using UnityEngine.UI;
using System;
using ImageClassification.Core;

namespace ImageClassification.Core
{
	/// <summary>
	/// Uses classification preprocess variables
	/// https://github.com/onnx/models/tree/main/validated/vision/classification/efficientnet-lite4
	/// </summary>
	public class Classification : MonoBehaviour
	{
		[Header("Model")]
		[SerializeField] private NNModel _modelFile;
		[SerializeField] private TextAsset _labelAsset;

		[Header("Scene")]
		[SerializeField] private CameraView _cameraView;
		[SerializeField] private TMPro.TextMeshProUGUI _outputText;
		[SerializeField] private Preprocess _preprocess;

		private const int IMAGE_SIZE = 224;
		private const string INPUT_NAME = "images";
		private const string OUTPUT_NAME = "Softmax";

		private string[] _labels;
		private IWorker _worker;

		void Start()
		{
			var model = ModelLoader.Load(_modelFile);
			_worker = WorkerFactory.CreateWorker(WorkerFactory.Type.ComputePrecompiled, model);
			LoadLabels();
		}

		void LoadLabels()
		{
			//get only items in quotes
			var stringArray = _labelAsset.text.Split('"').Where((item, index) => index % 2 != 0);
			//get every other item
			_labels = stringArray.Where((x, i) => i % 2 != 0).ToArray();
		}

		void Update()
		{

			WebCamTexture webCamTexture = _cameraView.GetCamImage();

			if (webCamTexture.didUpdateThisFrame && webCamTexture.width > 100)
			{
				_preprocess.ScaleAndCropImage(webCamTexture, IMAGE_SIZE, RunModel);
			}
		}

		void RunModel(byte[] pixels)
		{
			StartCoroutine(RunModelRoutine(pixels));
		}

		IEnumerator RunModelRoutine(byte[] pixels)
		{

			Tensor tensor = TransformInput(pixels);

			var inputs = new Dictionary<string, Tensor> {
			{ INPUT_NAME, tensor }
		};

			_worker.Execute(inputs);
			Tensor outputTensor = _worker.PeekOutput(OUTPUT_NAME);

			//get largest output
			List<float> temp = outputTensor.ToReadOnlyArray().ToList();
			float max = temp.Max();
			int index = temp.IndexOf(max);

			//set UI text
			_outputText.text = _labels[index];

			//dispose tensors
			tensor.Dispose();
			outputTensor.Dispose();
			yield return null;
		}

		//transform from 0-255 to -1 to 1
		Tensor TransformInput(byte[] pixels)
		{
			float[] transformedPixels = new float[pixels.Length];

			for (int i = 0; i < pixels.Length; i++)
			{
				transformedPixels[i] = (pixels[i] - 127f) / 128f;
			}
			return new Tensor(1, IMAGE_SIZE, IMAGE_SIZE, 3, transformedPixels);
		}
	}
}