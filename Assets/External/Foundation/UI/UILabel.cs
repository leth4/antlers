using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Foundation.UI
{
    public class UILabel : MonoBehaviour
    {
        [SerializeField] private TMP_Text _label;

        private const float LETTER_SHOW_TIME = .02f;

        private Coroutine _textAppearRoutine;

        public void SetText(string text)
        {
            _label.SetText(text);
        }

        public void SetText<T>(T obj)
        {
            _label.SetText(obj.ToString());
        }

        public void ShowTextAnimated(string text)
        {
            SkipActiveAnimation();
            SetText(text);
            _textAppearRoutine = StartCoroutine(TextAppearRoutine());
        }

        public void HideTextAnimated()
        {
            SkipActiveAnimation();
            _textAppearRoutine = StartCoroutine(TextDisappearRoutine());
        }

        public void SkipActiveAnimation()
        {
            if (_textAppearRoutine == null) return;
            StopCoroutine(_textAppearRoutine);
            _label.color = _label.color.SetA(1);
        }

        private IEnumerator TextAppearRoutine()
        {
            _label.color = _label.color.SetA(0);
            _label.ForceMeshUpdate();

            var textInfo = _label.textInfo;

            for (int i = 0; i < textInfo.characterCount; i++)
            {
                if (!textInfo.characterInfo[i].isVisible) continue;

                var materialIndex = textInfo.characterInfo[i].materialReferenceIndex;
                var newVertexColors = textInfo.meshInfo[materialIndex].colors32;
                var vertexIndex = textInfo.characterInfo[i].vertexIndex;

                newVertexColors[vertexIndex + 0].a = 255;
                newVertexColors[vertexIndex + 1].a = 255;
                newVertexColors[vertexIndex + 2].a = 255;
                newVertexColors[vertexIndex + 3].a = 255;

                _label.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32);

                yield return new WaitForSeconds(LETTER_SHOW_TIME);
            }
        }

        private IEnumerator TextDisappearRoutine()
        {
            _label.color = _label.color.SetA(1);
            _label.ForceMeshUpdate();

            var textInfo = _label.textInfo;

            for (int i = textInfo.characterCount - 1; i >= 0; i--)
            {
                if (!textInfo.characterInfo[i].isVisible) continue;

                var materialIndex = textInfo.characterInfo[i].materialReferenceIndex;
                var newVertexColors = textInfo.meshInfo[materialIndex].colors32;
                var vertexIndex = textInfo.characterInfo[i].vertexIndex;

                newVertexColors[vertexIndex + 0].a = 0;
                newVertexColors[vertexIndex + 1].a = 0;
                newVertexColors[vertexIndex + 2].a = 0;
                newVertexColors[vertexIndex + 3].a = 0;

                _label.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32);

                yield return new WaitForSeconds(LETTER_SHOW_TIME);
            }
        }
    }
}
