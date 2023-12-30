using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace GameEngine.Translation
{
	public class GoogleTranslator : TranslationService
	{
		// Your Google API Key for cloud translation.
		// WARNING: You should check API quota as it may charge these requests!
		private const string APIKey = "YOUR GOOGLE API KEY";
		private const string APIUri = "https://translation.googleapis.com/language/translate/v2";

		protected override IEnumerator TranslateTextRoutine(string sourceLanguage, string targetLanguage, string sourceText, Action<bool, string> callback)
		{
			var formData = new List<IMultipartFormSection>
			{
				new MultipartFormDataSection("Content-Type", "application/json; charset=utf-8"),
				new MultipartFormDataSection("source", sourceLanguage),
				new MultipartFormDataSection("target", targetLanguage),
				new MultipartFormDataSection("format", "text"),
				new MultipartFormDataSection("q", sourceText),
				new MultipartFormDataSection("key", APIKey),
				new MultipartFormDataSection("model", "base")
			};

			// var uri = $"{APIUri}{APIKey}";
			var uri = APIUri;
			var webRequest = UnityWebRequest.Post(uri, formData);

			yield return webRequest.SendWebRequest();

			if (webRequest.result != UnityWebRequest.Result.Success)
			{
				// Debug.LogError(webRequest.error);
				callback.Invoke(false, string.Empty);

				yield break;
			}

			// Debug.Log(webRequest.downloadHandler.text);

			var parsedData = JsonUtility.FromJson<GoogleTranslationResponse>(webRequest.downloadHandler.text);
			var translatedText = parsedData.data.translations[0].translatedText;
			// var translatedText = parsedTexts["data"]["translations"][0]["translatedText"];

			callback.Invoke(true, translatedText);
		}


		[Serializable]
		class GoogleTranslationResponse
		{
			public GoogleTranslationResponseData data;
		}

		[Serializable]
		class GoogleTranslationResponseData
		{
			public GoogleTranslationResponseTranslation[] translations;
		}

		[Serializable]
		class GoogleTranslationResponseTranslation
		{
			public string translatedText;
			public string model;
		}
	}
}