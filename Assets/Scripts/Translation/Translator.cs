using GameEngine.Util;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class LocalizationDictionary : UnitySerializedDictionary<string, string> { }

namespace GameEngine.Translation
{
    public class Translator : Singleton<Translator>
    {
		[SerializeField] private string _sourceLanguage = "en";
		[SerializeField] private string _targetLanguage = "tr";
		[SerializeField] private TranslationService _service;
		[SerializeField] private LocalizationDictionary _localizations;
		private HashSet<string> _requestedLocalizations = new HashSet<string>();

		public void TranslateCachedText(string sourceText, Action<bool, string> callback)
		{
			if (_localizations.ContainsKey(sourceText))
			{
				callback?.Invoke(true, _localizations[sourceText]);
				return;
			}

			// Prevent same key requests that may appear in small time window.
			if (_requestedLocalizations.Contains(sourceText))
			{
				callback?.Invoke(false, sourceText);
				return;
			}

			TranslateText(_sourceLanguage, _targetLanguage, sourceText, (success, tranlatedText) =>
			{
				if (success)
				{
					_localizations.Add(sourceText, tranlatedText);
					callback?.Invoke(success, tranlatedText);
				}
				else
				{
					callback?.Invoke(success, sourceText);
				}
			});

			_requestedLocalizations.Add(sourceText);
		}

		public void TranslateText(string sourceText, Action<bool, string> callback)
		{
			TranslateText(_sourceLanguage, _targetLanguage, sourceText, callback);
		}

		public void TranslateText(string sourceLangugage, string targetLanguage, string sourceText, Action<bool, string> callback)
		{
			_service.TranslateText(sourceLangugage, targetLanguage, sourceText, (success, translatedText) =>
			{
				/*
				if (success) Debug.Log("Translated Text : " + translatedText);
				else Debug.Log("Translator Error!");
				*/

				callback?.Invoke(success, translatedText);
			});
		}
	}
}