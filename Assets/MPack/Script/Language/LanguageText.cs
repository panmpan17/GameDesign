using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MPack {
    public class LanguageText : MonoBehaviour
    {
		[SerializeField]
		[LauguageID]
		private int id = -1;
		public int ID { get { return id; } }

		private bool initialed = false;
		private TextMeshPro textMeshPro;
		public TextMeshPro TextMeshPro { get {
			if (!initialed) InitialGetTextComponent();
			return textMeshPro;
		} }
		private TextMeshProUGUI textMeshProUI;
		private Text uiText;

        public delegate string LanguageProcess(string text, TextMeshPro textMeshPro, TextMeshProUGUI textMeshProUGUI);
		private LanguageProcess languageProcess;
        private System.Action<Vector2> sizeChangeEvent;

		public string Text {
			set {
				if (!initialed) InitialGetTextComponent();

                if (textMeshPro != null) {
					if (languageProcess != null && value != null) textMeshPro.text = languageProcess.Invoke(value, textMeshPro, null);
					else textMeshPro.text = value;
					
					if (sizeChangeEvent != null) {
						Rect rect = textMeshPro.GetPixelAdjustedRect();
                        sizeChangeEvent.Invoke(textMeshPro.textBounds.size);
					}
                }
				else if (textMeshProUI != null) {
					if (languageProcess != null && value != null) textMeshProUI.text = languageProcess.Invoke(value, null, textMeshProUI);
					else textMeshProUI.text = value;
				}
				else if (uiText != null) uiText.text = value;
				else Debug.LogError("missing", gameObject);
			}
		}

		/// <summary>
		/// Change the Language id of text, will automatically apply language after id changed
		/// </summary>
		/// <param name="_id">The new Id</param>
		public void ChangeId(int _id, bool forceRefresh=false) {
            if (!initialed) InitialGetTextComponent();

			if (id != _id || forceRefresh) {
				id = _id;

				if (LanguageMgr.DataLoaded) Text = LanguageMgr.GetTextById(id);
			}
		}

		/// <summary>
		/// Initial the text component
		/// </summary>
		private void InitialGetTextComponent() {
			initialed = true;
			
            textMeshPro = GetComponent<TextMeshPro>();
            textMeshProUI = GetComponent<TextMeshProUGUI>();
            uiText = GetComponent<Text>();

			if (textMeshPro == null && uiText == null && textMeshProUI == null) {
			#if UNTIY_EDITOR
				Debug.LogError("LanguageText require Text or TextMesh or TextMeshPro or TextMeshProUGUI", gameObject);
			#endif
				enabled = false;
				return;
			}

			#if UNTIY_EDITOR
			if (uiText != null)
				Debug.LogError("Avoid using UI Text, use TextMeshProUGUI instead", gameObject);
			#endif
		}

        private void Awake()
		{
			if (!initialed) InitialGetTextComponent();
		}

        private void Start()
        {
            if (LanguageMgr.DataLoaded) Text = LanguageMgr.GetTextById(id);
        }

		private void OnEnable()
		{
            if (LanguageMgr.DataLoaded) Text = LanguageMgr.GetTextById(id);
		}

		public void Reload()
		{
            if (LanguageMgr.DataLoaded) Text = LanguageMgr.GetTextById(id);
		}

		/// <summary>
		/// Add text processor, called before value actually get set
		/// </summary>
		/// <param name="process">Delegate</param>
		public void SetupLanguageProcesor(LanguageProcess process) {
            languageProcess = process;
        }

        /// <summary>
        /// Add event to sizeChangeEvent, called when language switch, Only work with TextMeshPro
        /// </summary>
        /// <param name="newEvent">The size of rendered text</param>
        public void SetupSizeChangeEvent(System.Action<Vector2> newEvent)
        {
            sizeChangeEvent = newEvent;
        }
    }
}