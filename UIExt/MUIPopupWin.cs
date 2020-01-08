using UnityEngine;

namespace IINS.UIExt
{
    public class MUIPopupWindow : MUIWindow
    {
        public MUIPopupWindow() : base("MUIWindow", Options.IsPopup)
        { }

        public static T Create<T>() where T : MUIPopupWindow
        {
            return MUISkin.UIView.AddUIComponent(typeof(T)) as T;
        }

        public override void Awake()
        {
            base.Awake();

            this.backgroundColor = Color.black; // new Color32(210, 210, 210, 255);

            this.titleColor = Color.clear;
            this.backgroundSprite = "popupWindow";
        }

        public override void Start()
        {
            base.Start();
            this.atlas = MUITextureAtlas.atlas;
        }

        public override void Close()
        {
            base.Close();
            Destroy(this);
        }

        //public override void DrawWindow() { }
    }
}
