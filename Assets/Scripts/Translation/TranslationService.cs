using System;
using System.Collections;
using UnityEngine;

namespace GameEngine.Translation
{
    public abstract class TranslationService : MonoBehaviour
    {
		public void TranslateText(string sourceLanguage, string targetLanguage, string sourceText, Action<bool, string> callback)
		{
			StartCoroutine(TranslateTextRoutine(sourceLanguage, targetLanguage, sourceText, callback));
		}

		protected abstract IEnumerator TranslateTextRoutine(string sourceLanguage, string targetLanguage, string sourceText, Action<bool, string> callback);
	}
}