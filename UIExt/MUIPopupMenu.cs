using UnityEngine;

namespace IINS.UIExt
{
    public class MUIPopupMenu : MUIPopupWindow
    {
        public delegate void OnSelected(int index);
        public OnSelected onSelected;
        private Vector2 scroll = Vector2.zero;
        private bool hasScrollbars = true;
        private string[] items;

        private int _selected = -1, selectedItem = -1;
        public int selected { get { return _selected; } set { _selected = value; } }

        private static GUIStyle _itemHoverStyle;

        public void Popup(int selected, string[] items, OnSelected onSelected)
        {
            this.items = items;
            this.selected = selected;
            this.onSelected = onSelected;
            this.Show();
        }

        public static GUIStyle ItemHoverStyle
        {
            get
            {
                if (_itemHoverStyle == null)
                {
                    _itemHoverStyle = new GUIStyle(GUI.skin.label);
                    _itemHoverStyle.hover.textColor = Color.black;
                    _itemHoverStyle.onHover.textColor = Color.blue;
                    _itemHoverStyle.onNormal.textColor = Color.blue;

                    ItemHoverStyle.padding = new RectOffset(8, 0, 0, 0);
                    ItemHoverStyle.margin = new RectOffset(0, 10, 0, 0);
                    _itemHoverStyle.alignment = TextAnchor.MiddleLeft;
                    _itemHoverStyle.fixedHeight = _itemHoverStyle.CalcHeight(new GUIContent("H"), 1024) + 4f;

                    var hover = new Texture2D(1, 1);
                    hover.SetPixel(0, 0, Color.white);
                    hover.Apply();
                    _itemHoverStyle.hover.background = hover;

                    var NormalOn = new Texture2D(1, 1);
                    NormalOn.SetPixel(0, 0, new Color(0.90f, 0.90f, 0.90f, 1));
                    NormalOn.Apply();

                    _itemHoverStyle.onNormal.background = NormalOn;
                    _itemHoverStyle.onHover.background = hover;
                }
                return _itemHoverStyle;
            }
        }

        protected override void OnVisibilityChanged()
        {
            base.OnVisibilityChanged();
            CalcWidthHeight();
        }

        public void CalcWidthHeight()
        {
            int length = 0;
            var H = ItemHoverStyle.fixedHeight * MUISkin.UIView.ratio;
            float width = 100f;
            float height = H;
            string maxLen_s = "MUISKIN MENU";


            if (items != null && items.Length > 0)
            {
                height = H * items.Length;   
                //if (height <= 0) height = H; 
                
                foreach (var s in items)
                {
                    if (length < s.Length)
                    {
                        length = s.Length;
                        maxLen_s = s;
                    }
                }
            }

            float minWidth = 0;
            MUISkin.skin.label.CalcMinMaxWidth(new GUIContent(maxLen_s), out minWidth, out width);
            width = width * MUISkin.UIView.ratio;

            this.size = new Vector2(width + 48f * MUISkin.UIView.ratio, height + 32f * MUISkin.UIView.ratio); // * MUISkin.UIView.ratio;
        }

        public override void DrawWindow()
        {
            GUILayout.Space(8);
            if (items != null)
            {
                if (hasScrollbars)
                {
                    scroll = GUILayout.BeginScrollView(scroll, false, false);
                }

                selectedItem = GUILayout.SelectionGrid(_selected, items, 1, _itemHoverStyle);
                if (selectedItem != _selected)
                {
                    _selected = selectedItem;
                    if (onSelected != null) onSelected(selectedItem);
                    Close();
                }

                if (hasScrollbars)
                {
                    GUILayout.EndScrollView();
                }
            }
            GUILayout.Space(8);
        }
    }
}
