using System;
using UnityEngine;
using ColossalFramework;
using ColossalFramework.UI;

namespace IINS.UIExt
{
    public class MUIPanel: UIPanel
    {
        private float lastwidth = 0f;
        private float lastheight = 0f;
        protected MUIWindow drawPanel;
        protected static CameraController cameraController;
        public MUIPanel()
        {
            isVisible = false;
            drawPanel = this.AddUIComponent<MUIWindow>();
            drawPanel.DrawWindowHandler += OnDrawPanel;
            drawPanel.options = MUIWindow.Options.Transparent; //.Dragabled | MUIWindow.Options.FullDragabled;
            drawPanel.backgroundColor = Color.clear;
            drawPanel.isVisible = false;
            drawPanel.minimumSize = new Vector2(0, 0);
            if (cameraController==null)
                cameraController = GameObject.FindObjectOfType<CameraController>();
        }

        public override void OnDestroy()
        {
            base.OnDestroy();

            Destroy(drawPanel);
            drawPanel = null;
        }

        private Component[] Lables = null;

        public virtual bool CanOnGUIDraw()
        {
            return isVisible && (cameraController != null && cameraController.enabled);
        }

        public virtual void OnDrawPanel()
        {
            if (CanOnGUIDraw())
            {
                if (Lables == null || Lables.Length == 0)
                {
                    Lables = GetComponentsInChildren(typeof(MUILabel));
                }

                if (Lables != null && Lables.Length > 0)
                {
                    for (int i = 0; i < Lables.Length; i++)
                    {
                        var label = Lables[i].GetComponent<MUILabel>();
                        if (label.isVisible) label.DrawLabel();
                    }
                }
            }
        }

        protected override void OnSizeChanged()
        {
            base.OnSizeChanged();
            resetDrawPanel();
        }

        protected override void OnVisibilityChanged()
        {
            base.OnVisibilityChanged();
            resetDrawPanel();
        }

        protected virtual void resetDrawPanel()
        {
            if (drawPanel != null)
            {
                drawPanel.isVisible = isVisible;
                if (this.isVisible)
                {
                    drawPanel.size = this.size;
                    drawPanel.absolutePosition = this.absolutePosition;
                    drawPanel.relativePosition = new Vector3(1, 0);
                    drawPanel.UpdateWindow();
                }
            }
        }

        public virtual void OnScreenSizeChagned()
        {

        }

        public virtual void OnGUI()
        {
            // 适应分频率变化
            if (lastwidth != Screen.width || lastheight != Screen.height)
            {
                lastwidth = Screen.width;
                lastheight = Screen.height;
                
                OnScreenSizeChagned();
                resetDrawPanel();                
            }
        }

    }
}
