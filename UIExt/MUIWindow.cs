using System;
using UnityEngine;
using ColossalFramework;
using ColossalFramework.UI;

namespace IINS.UIExt
{
    public enum MUIDragDropState
    {
        None = 0,
        Dragging = 1,
        Dropped = 2,
        Denied = 3,
        Cancelled = 4,
        CancelledNoTarget = 5,
        DragMoving = 6,
    }

    [RequireComponent(typeof(MUIWinDraw))]
    public class MUIWindow : UIPanel
    {
        public delegate void OnDrawWindowHandler();
        public delegate void OnCloseHandler();
        public delegate void DrawHandler();
        public event OnCloseHandler onClose = null;
        public event OnDrawWindowHandler onDrawWindow = null;

        internal MUIWinDraw m_draw = null;
        internal MUIDragHandle m_drag = null;
        internal MUIResizeHandle m_resize = null;


        private Color _backgroundColor = Color.clear;
        private Color _titleColor = MUISkin.config.titlebarColor;
        private TextAnchor _titleAlignment = TextAnchor.MiddleLeft;
        private string m_backgroundSprite;

        new public string backgroundSprite
        {
            get
            {
                return this.m_backgroundSprite;
            }
            set
            {
                if (value != this.m_backgroundSprite)
                {
                    this.m_backgroundSprite = value;
                    this.Invalidate();
                }
            }
        }
        public Color titleColor
        {
            get
            {
                return _titleColor;
            }
            set
            {
                _titleColor = value;
                if (m_draw != null && m_draw.titleTextture != null)
                {
                    m_draw.titleTextture.SetPixel(0, 0, value);
                    m_draw.titleTextture.Apply();
                }
            }
        }

        public TextAnchor titleAlignment
        {
            get
            {
                return _titleAlignment;
            }
            set
            {
                _titleAlignment = value;
                if (m_draw != null && m_draw.titleStyle != null)
                {
                    m_draw.titleStyle.alignment = _titleAlignment;
                }
            }
        }

        public Color backgroundColor {
            get
            {
                return _backgroundColor;
            }
            set
            {
                _backgroundColor = value;
                if (m_draw != null && m_draw.winTexture != null)
                {
                    m_draw.winTexture.SetPixel(0, 0, value);
                    m_draw.winTexture.Apply();
                }
            }
        }

        private int _titleHeight = 0;
        public float m_titleHeight = 0.0f;
        public int titleHeight
        {
            get
            {
                if ((options & MUIWindow.Options.TitleBar) != MUIWindow.Options.TitleBar)
                    return 0;
                else if (_titleHeight <= 0)
                    return (int)m_titleHeight;
                else return _titleHeight;
            }
            set
            { _titleHeight = value; }
        }


        public int closeButtonSize = 18;
        public Vector2 titleOffset = new Vector2(0.0f, 0.0f);
        public Vector2 closeButtonOffset =  new Vector2(0.0f, 0.0f);
        public Vector2 resizeButtonOffset = new Vector2(0.0f, 0.0f);

        public MUIDragDropState DragDropState = MUIDragDropState.None;

        private Options _options = 0;
        

        public Options options
        { 
            get { return _options; }
            set { _options = value; UpdateOptions(); } 
        }

        private string _title = "";
        public string Title
        {
            get { return _title; }
            set { _title = value; }
        }

        public int windowId {
            get
            {
                if (m_draw == null)
                    return -1;
                else
                    return m_draw.winId;
            }
        }

        public Rect DrawRect { get { return m_draw==null? new Rect(0, 0, 0, 0) : m_draw.rect; } }

        #region Drag & Resize UIHandle
        public class MUIDragHandle : UIDragHandle
        {
            public MUIWindow win = null;
        }

        public class MUIResizeHandle : UIResizeHandle
        {
            public MUIWindow win = null;
            protected override void OnMouseEnter(UIMouseEventParameter p)
            {
                base.OnMouseEnter(p);
                if ((win.m_draw != null) && win.m_draw.enabled)
                {
                    win.m_draw.states |= MUIWinDraw.States.HoverInResize;
                }
            }
            protected override void OnMouseLeave(UIMouseEventParameter p)
            {
                base.OnMouseLeave(p);
                if ((win.m_draw != null) && win.m_draw.enabled)
                {
                    win.m_draw.states &= ~MUIWinDraw.States.HoverInResize;
                }
            }
        }
        #endregion

        #region Window options
        public static Options defaultOptions = Options.TitleBar | Options.Dragabled | Options.CloseButton | Options.Resizable;
        [Flags]
        public enum Options
        {
            TitleBar = 0x01,
            CloseButton = 0x02,
            Resizable = 0x04,
            Dragabled = 0x08,
            FullDragabled = 0x10,
            Transparent = 0x20, //鼠标穿透
            IsPopup = 0x40,
            InFreeCamera = 0x80,

        }
        #endregion


        public MUIWindow()
            : this("MUIWindow", defaultOptions) 
        {}

        public MUIWindow(string title, Options options = 0)
        {
            _title = title;
            _options = options;
            this.size = new Vector3(300, 200);
            this.canFocus = true;
            this.isInteractive = false; 
            this.m_IsVisible = false;
            this.backgroundSprite = "";
            this.m_MinSize = new Vector3(200, 100);

            _backgroundColor = MUISkin.config.backgroundColor;
            titleColor = MUISkin.config.titlebarColor;
            titleOffset = MUISkin.config.titleOffset;
            closeButtonOffset = MUISkin.config.closeButtonOffset;
            resizeButtonOffset = MUISkin.config.resizeButtonOffset;
        }

        public override void Awake()
        {
            base.Awake();
            this.name = this.GetType().Name;
        }

        public static Vector2 getScreenCenterPostion(Vector2 size)
        {
            var screen = MUISkin.UIView.GetScreenResolution();
            var pos = new Vector2((screen.x - size.x) / 2, (screen.y - size.y) / 2);
            return pos;
        }

        public override void Start()
        {
            base.Start();

            m_draw = gameObject.AddComponent<MUIWinDraw>();
            m_draw.win = this;
            
            this.canFocus = true;
            this.isInteractive = true;

            UpdateOptions();           
        }

        public override void Update()
        {
            base.Update();
        }

        public event DrawHandler DrawWindowHandler;
       

        public virtual void OnGUI()
        {
            if (isVisible) OnDrawWindow();

            if (Input.anyKeyDown && ((options & Options.IsPopup) == Options.IsPopup))
            {
                if ((Event.current.keyCode == 0))
                {
                    var pos = Input.mousePosition;
                    pos.y = Screen.height - pos.y;
                    if (((m_draw != null) && !m_draw.rect.Contains(pos)))
                        this.Close();
                }
                else
                {
                    this.Close();
                }
            }

            if (m_draw != null)
            {
                m_draw.enabled = isVisible &&
                    (((options & Options.InFreeCamera) == Options.InFreeCamera) ? true : !MUISkin.comeraController.m_freeCamera);

                if (Title == "")
                    m_titleHeight = 8;
                else
                {

                    m_titleHeight = MUISkin.CalcTextHeight();
                }

                if (m_resize != null)
                {
                    var ratio = UIView.GetAView().ratio;
                    m_resize.relativePosition = new Vector3(m_draw.resizeRect.x * ratio, m_draw.resizeRect.y * ratio, 0);
                }
            }
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            if (m_draw != null) Destroy(m_draw);
            if (m_drag != null) Destroy(m_drag);
            if (m_resize != null) Destroy(m_resize);
            Destroy(gameObject); // 删除实体
        }

        public bool isDragMoving
        {
            get { return (DragDropState == MUIDragDropState.DragMoving); }
        }

        public void UpdateWindow()
        {
            if (Title == "")
                m_titleHeight = 8;
            else
                m_titleHeight = MUISkin.CalcTextHeight();

            if (m_drag != null)
            {
                m_drag.width = width;
                if ((options & Options.FullDragabled) == Options.FullDragabled)
                {
                    float h = height;
                    if (m_resize != null)
                    {
                        if (height < 50)
                            m_drag.width = width - m_resize.width - 2;
                        else
                            h = height - m_resize.height - 2;
                    }
                    m_drag.height = h;
                }
                else
                {
                    if (m_titleHeight > 0)
                        m_drag.height = m_titleHeight - 2;
                    else
                        m_drag.height = 20;
                }
            }

            if (m_resize != null)
            {
                m_resize.relativePosition = new Vector3(width - m_resize.width - 1, height - m_resize.height - 1);
            }

            //this.m_MinSize = minSize;

            if (m_draw != null)
            {
                var ratio = UIView.GetAView().ratio;
                m_draw.rect = new Rect(absolutePosition.x / ratio, absolutePosition.y / ratio, width / ratio, height / ratio);
                if (m_resize != null)
                    m_draw.resizeRect = new Rect(m_draw.rect.width - MUISkin.config.resizeButtonWidth - 3, m_draw.rect.height - MUISkin.config.resizeButtonWidth + 4,
                        MUISkin.config.resizeButtonWidth + 2, MUISkin.config.resizeButtonWidth - 5);
            }
        }

        private Vector3 minSize;
        private void UpdateOptions()
        {
            minSize = new Vector3(8, 8);
            // dragabled 拖动
            if ((options & Options.Dragabled) == Options.Dragabled)
            {
                if (m_drag == null)
                {
                    m_drag = AddUIComponent<MUIDragHandle>();
                    m_drag.width = width;
                    m_drag.height = 25;
                    m_drag.relativePosition = Vector3.zero;
                    m_drag.target = this;
                    m_drag.win = this;
                    //m_drag.enabled = false;
                    minSize.y += 20;
                }
            }
            else
            {
                if (m_drag != null) Destroy(m_drag);
            }

            // resizabled 调整大小
            if ((options & Options.Resizable) == Options.Resizable)
            {
                if (m_resize == null)
                {
                    var w = MUISkin.config.resizeButtonWidth + 4;
                    m_resize = AddUIComponent<MUIResizeHandle>();
                    m_resize.size = new Vector3(w, w);
                    m_resize.relativePosition = new Vector3(width - w - 1, height - w - 1);
                    m_resize.backgroundSprite = ""; // --DEBUG "buttonresize";                    
                    m_resize.win = this;
                    minSize.y += m_resize.height + 2;
                    m_resize.limits = new Vector4(0, 0, minSize.x, minSize.y);                    
                }
            }
            else
            {
                if (m_resize != null) Destroy(m_resize);
            }

            //Mouse through 鼠标穿透
            if ((options & Options.Transparent) == Options.Transparent)
                this.color = Color.clear;
            else
                this.color = new Color32(255, 255, 255, 255); //MUISkin.config.backgroundColor;

            UpdateWindow();
        }


        protected override void OnVisibilityChanged()
        {
            base.OnVisibilityChanged();
            UpdateWindow();
        }

        protected override void OnPositionChanged()
        {
            base.OnPositionChanged();
            UpdateWindow();            
        }

        protected override void OnSizeChanged()
        {
            base.OnSizeChanged();
            UpdateWindow();
        }

        protected override void OnMouseDown(UIMouseEventParameter p)
        {
            DragDropState = MUIDragDropState.None;
            base.OnMouseDown(p);
            
            if ((options & Options.Transparent) != Options.Transparent)
                BringToFront(); 

            if (m_draw != null)
                m_draw.closeCapture = m_draw.isHoverInClose();
        }

        protected override void OnMouseUp(UIMouseEventParameter p)
        {
            DragDropState = MUIDragDropState.None;
            base.OnMouseUp(p);

            if (m_draw != null)
            {
                var isclose = m_draw.closeCapture;
                m_draw.closeCapture = false;
                if (m_draw.isHoverInClose() && isclose)
                    Close();
            }
        }

        protected override void OnDragStart(UIDragEventParameter p)
        {
            base.OnDragStart(p);
           
            DragDropState = MUIDragDropState.Dragging;
            
            if (m_draw != null && m_drag != null)
            {
                if (((options & Options.FullDragabled) == Options.FullDragabled) || m_draw.isHoverInTitle())
                    DragDropState = MUIDragDropState.DragMoving;
                // ** is not necessary
                //if ((options & Options.FullDragabled) != Options.FullDragabled)
                //{
                //    m_draw.states |= MUIWinDraw.States.HoverInTitlebar;
                //}

                if (m_draw.isHoverInClose() && ((options & Options.Transparent) != Options.Transparent))
                {
                    DragDropState = MUIDragDropState.None;
                    m_drag.enabled = false;                    
                }
            }            
        }

 
        protected override void OnMouseMove(UIMouseEventParameter p)
        {
            base.OnMouseMove(p);

            if (m_drag != null) m_drag.enabled = true;
            
            if ((!isDragMoving) && ((m_draw != null) && m_draw.enabled))
            {
                var mousePos = Input.mousePosition;
                mousePos.y = Screen.height - mousePos.y;
                mousePos = new Vector2(mousePos.x - m_draw.rect.x, mousePos.y - m_draw.rect.y);

                if (m_draw.titleRect.Contains(mousePos) || m_draw.closeRect.Contains(mousePos))
                {
                    m_draw.states |= MUIWinDraw.States.HoverInTitlebar;

                    if (m_draw.closeRect.Contains(mousePos))
                        m_draw.states |= MUIWinDraw.States.HoverInCloseButton;
                    else
                        m_draw.states &= ~MUIWinDraw.States.HoverInCloseButton;
                }
                else
                    m_draw.states &= ~MUIWinDraw.States.HoverInTitlebar;
            }
        }

        protected override void OnMouseLeave(UIMouseEventParameter p)
        {
            base.OnMouseLeave(p);

            if ((!isDragMoving) && (m_draw != null) && m_draw.enabled)
            {
                m_draw.states &= ~MUIWinDraw.States.HoverInTitlebar;
                m_draw.states &= ~MUIWinDraw.States.HoverInResize;
                m_draw.states &= ~MUIWinDraw.States.HoverInCloseButton;
            }

            DragDropState = MUIDragDropState.None;
        }

        // 这里可以加入代码控制窗口是否可以被关闭 closebutton click / ispopup 时。
        public virtual bool canClose() { return true; }

        public virtual void Close()
        {
            if (canClose())
            {
                Hide();
                isVisible = false;
            }
            if (onClose != null) onClose();
        }

        protected virtual void OnDrawWindow()
        {
            if (isVisible && onDrawWindow != null) onDrawWindow();
        }

        public virtual void DrawWindow()
        {
            if (isVisible && DrawWindowHandler != null)
                DrawWindowHandler();
        }
    }
}
