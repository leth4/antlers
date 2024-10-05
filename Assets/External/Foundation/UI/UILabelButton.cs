using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Foundation.UI
{
    public class UILabelButton : UIButton
    {
        [SerializeField] private TMP_Text _label;

        public void SetText(string text)
        {
            _label.SetText(text);
        }
    }
}