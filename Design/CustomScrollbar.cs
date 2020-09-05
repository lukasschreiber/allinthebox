using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace Design
{
    [Designer(typeof(ScrollbarControlDesigner))]
    public class CustomScrollbar : UserControl
    {
        protected bool moAutoSize = false;

        protected Color moChannelColor = Color.Empty;

        //protected Image moUpArrowImage_Over = null;
        //protected Image moUpArrowImage_Down = null;
        protected Image moDownArrowImage;
        protected int moExtraSmallChange = 1;

        protected int moLargeChange = 10;
        protected int moMaximum = 100;
        protected int moMinimum;

        protected int moSmallChange = 5;

        //protected Image moDownArrowImage_Over = null;
        //protected Image moDownArrowImage_Down = null;
        protected Image moThumbArrowImage = null;
        protected Image moThumbBottomImage;
        protected Image moThumbBottomSpanImage;

        private bool moThumbDown;
        private bool moThumbDragging;
        protected Image moThumbMiddleImage;

        protected int moThumbTop;

        protected Image moThumbTopImage;
        protected Image moThumbTopSpanImage;
        protected Image moUpArrowImage;

        private bool mousedown;
        protected int moValue;
        private int nClickPoint;


        private Point ptPoint;

        public CustomScrollbar()
        {
            InitializeComponent();
            SetStyle(ControlStyles.ResizeRedraw, true);
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.DoubleBuffer, true);

            moChannelColor = Color.FromArgb(24, 24, 24);
            UpArrowImage = Resource.uparrow;
            DownArrowImage = Resource.downarrow;


            ThumbBottomImage = Resource.ThumbBottom;
            ThumbBottomSpanImage = Resource.ThumbSpanBottom;
            ThumbTopImage = Resource.ThumbTop;
            ThumbTopSpanImage = Resource.ThumbSpanTop;
            ThumbMiddleImage = Resource.ThumbMiddle;

            Width = UpArrowImage.Width;
            base.MinimumSize = new Size(UpArrowImage.Width,
                UpArrowImage.Height + DownArrowImage.Height + GetThumbHeight());
        }

        [EditorBrowsable(EditorBrowsableState.Always)]
        [Browsable(true)]
        [DefaultValue(false)]
        [Category("Behavior")]
        [Description("LargeChange")]
        public int LargeChange
        {
            get => moLargeChange;
            set
            {
                moLargeChange = value;
                Invalidate();
            }
        }

        [EditorBrowsable(EditorBrowsableState.Always)]
        [Browsable(true)]
        [DefaultValue(false)]
        [Category("Behavior")]
        [Description("SmallChange")]
        public int SmallChange
        {
            get => moSmallChange;
            set
            {
                moSmallChange = value;
                Invalidate();
            }
        }

        [EditorBrowsable(EditorBrowsableState.Always)]
        [Browsable(true)]
        [DefaultValue(false)]
        [Category("Behavior")]
        [Description("ExtraSmallChange")]
        public int ExtraSmallChange
        {
            get => moExtraSmallChange;
            set
            {
                moExtraSmallChange = value;
                Invalidate();
            }
        }

        [EditorBrowsable(EditorBrowsableState.Always)]
        [Browsable(true)]
        [DefaultValue(false)]
        [Category("Behavior")]
        [Description("Minimum")]
        public int Minimum
        {
            get => moMinimum;
            set
            {
                moMinimum = value;
                Invalidate();
            }
        }

        [EditorBrowsable(EditorBrowsableState.Always)]
        [Browsable(true)]
        [DefaultValue(false)]
        [Category("Behavior")]
        [Description("Maximum")]
        public int Maximum
        {
            get => moMaximum;
            set
            {
                moMaximum = value;
                Invalidate();
            }
        }

        [EditorBrowsable(EditorBrowsableState.Always)]
        [Browsable(true)]
        [DefaultValue(false)]
        [Category("Behavior")]
        [Description("Value")]
        public int Value
        {
            get => moValue;
            set
            {
                moValue = value;

                var nTrackHeight = Height - (UpArrowImage.Height + DownArrowImage.Height);
                var fThumbHeight = LargeChange / (float) Maximum * nTrackHeight;
                var nThumbHeight = (int) fThumbHeight;

                if (nThumbHeight > nTrackHeight)
                {
                    nThumbHeight = nTrackHeight;
                    fThumbHeight = nTrackHeight;
                }

                if (nThumbHeight < 20)
                {
                    nThumbHeight = 20;
                    fThumbHeight = 20;
                }

                //figure out value
                var nPixelRange = nTrackHeight - nThumbHeight;
                var nRealRange = Maximum - Minimum - LargeChange;
                var fPerc = 0.0f;
                if (nRealRange != 0) fPerc = moValue / (float) nRealRange;

                var fTop = fPerc * nPixelRange;
                moThumbTop = (int) fTop;


                Invalidate();
            }
        }


        [EditorBrowsable(EditorBrowsableState.Always)]
        [Browsable(true)]
        [DefaultValue(false)]
        [Category("Skin")]
        [Description("Channel Color")]
        public Color ChannelColor
        {
            get => moChannelColor;
            set => moChannelColor = value;
        }

        [EditorBrowsable(EditorBrowsableState.Always)]
        [Browsable(true)]
        [DefaultValue(false)]
        [Category("Skin")]
        [Description("Up Arrow Graphic")]
        public Image UpArrowImage
        {
            get => moUpArrowImage;
            set => moUpArrowImage = value;
        }

        [EditorBrowsable(EditorBrowsableState.Always)]
        [Browsable(true)]
        [DefaultValue(false)]
        [Category("Skin")]
        [Description("Up Arrow Graphic")]
        public Image DownArrowImage
        {
            get => moDownArrowImage;
            set => moDownArrowImage = value;
        }

        [EditorBrowsable(EditorBrowsableState.Always)]
        [Browsable(true)]
        [DefaultValue(false)]
        [Category("Skin")]
        [Description("Up Arrow Graphic")]
        public Image ThumbTopImage
        {
            get => moThumbTopImage;
            set => moThumbTopImage = value;
        }

        [EditorBrowsable(EditorBrowsableState.Always)]
        [Browsable(true)]
        [DefaultValue(false)]
        [Category("Skin")]
        [Description("Up Arrow Graphic")]
        public Image ThumbTopSpanImage
        {
            get => moThumbTopSpanImage;
            set => moThumbTopSpanImage = value;
        }

        [EditorBrowsable(EditorBrowsableState.Always)]
        [Browsable(true)]
        [DefaultValue(false)]
        [Category("Skin")]
        [Description("Up Arrow Graphic")]
        public Image ThumbBottomImage
        {
            get => moThumbBottomImage;
            set => moThumbBottomImage = value;
        }

        [EditorBrowsable(EditorBrowsableState.Always)]
        [Browsable(true)]
        [DefaultValue(false)]
        [Category("Skin")]
        [Description("Up Arrow Graphic")]
        public Image ThumbBottomSpanImage
        {
            get => moThumbBottomSpanImage;
            set => moThumbBottomSpanImage = value;
        }

        [EditorBrowsable(EditorBrowsableState.Always)]
        [Browsable(true)]
        [DefaultValue(false)]
        [Category("Skin")]
        [Description("Up Arrow Graphic")]
        public Image ThumbMiddleImage
        {
            get => moThumbMiddleImage;
            set => moThumbMiddleImage = value;
        }

        public override bool AutoSize
        {
            get => base.AutoSize;
            set
            {
                base.AutoSize = value;
                if (base.AutoSize) Width = moUpArrowImage.Width;
            }
        }

        public new event EventHandler Scroll;
        public event EventHandler ValueChanged;

        public int GetThumbHeight()
        {
            var nTrackHeight = Height - (UpArrowImage.Height + DownArrowImage.Height);
            var fThumbHeight = LargeChange / (float) Maximum * nTrackHeight;
            var nThumbHeight = (int) fThumbHeight;

            if (nThumbHeight > nTrackHeight)
            {
                nThumbHeight = nTrackHeight;
                fThumbHeight = nTrackHeight;
            }

            if (nThumbHeight < 20)
            {
                nThumbHeight = 20;
                fThumbHeight = 20;
            }

            return nThumbHeight;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.InterpolationMode = InterpolationMode.NearestNeighbor;

            if (UpArrowImage != null)
                e.Graphics.DrawImage(UpArrowImage,
                    new Rectangle(new Point(0, 0), new Size(Width, UpArrowImage.Height)));

            Brush oBrush = new SolidBrush(moChannelColor);
            Brush oWhiteBrush = new SolidBrush(Color.FromArgb(255, 255, 255));

            //draw channel left and right border colors
            e.Graphics.FillRectangle(oWhiteBrush,
                new Rectangle(0, UpArrowImage.Height, 0, Height - DownArrowImage.Height));
            e.Graphics.FillRectangle(oWhiteBrush,
                new Rectangle(Width, UpArrowImage.Height, 0, Height - DownArrowImage.Height));

            //draw channel
            e.Graphics.FillRectangle(oBrush,
                new Rectangle(0, UpArrowImage.Height, Width, Height - DownArrowImage.Height));

            //draw thumb
            var nTrackHeight = Height - (UpArrowImage.Height + DownArrowImage.Height);
            var fThumbHeight = LargeChange / (float) Maximum * nTrackHeight;
            var nThumbHeight = (int) fThumbHeight;

            if (nThumbHeight > nTrackHeight)
            {
                nThumbHeight = nTrackHeight;
                fThumbHeight = nTrackHeight;
            }

            if (nThumbHeight < 20)
            {
                nThumbHeight = 20;
                fThumbHeight = 20;
            }


            var fSpanHeight = (fThumbHeight - (ThumbTopImage.Height + ThumbBottomImage.Height)) / 2.0f;
            var nSpanHeight = (int) fSpanHeight; //ThumbMiddleImage.Height + 

            var nTop = moThumbTop;
            nTop += UpArrowImage.Height;

            //draw top
            e.Graphics.DrawImage(ThumbTopImage, new Rectangle(1, nTop, Width - 2, ThumbTopImage.Height));

            nTop += ThumbTopImage.Height;
            //draw top span
            var rect = new Rectangle(1, nTop, Width - 2, nSpanHeight);


            e.Graphics.DrawImage(ThumbTopSpanImage, 1.0f, nTop, Width - 2.0f, fSpanHeight * 2);

            nTop += nSpanHeight;
            //draw middle
            //e.Graphics.DrawImage(ThumbMiddleImage, new Rectangle(1, nTop, this.Width - 2, ThumbMiddleImage.Height));

            // nTop += ThumbMiddleImage.Height;
            //draw top span
            rect = new Rectangle(1, nTop, Width - 2, nSpanHeight * 2);
            e.Graphics.DrawImage(ThumbBottomSpanImage, rect);

            nTop += nSpanHeight;
            //draw bottom
            e.Graphics.DrawImage(ThumbBottomImage, new Rectangle(1, nTop, Width - 2, nSpanHeight));

            if (DownArrowImage != null)
                e.Graphics.DrawImage(DownArrowImage,
                    new Rectangle(new Point(0, Height - DownArrowImage.Height),
                        new Size(Width, DownArrowImage.Height)));
        }

        private void InitializeComponent()
        {
            SuspendLayout();
            // 
            // CustomScrollbar
            // 
            Name = "CustomScrollbar";
            MouseDown += CustomScrollbar_MouseDown;
            MouseMove += CustomScrollbar_MouseMove;
            MouseUp += CustomScrollbar_MouseUp;
            ResumeLayout(false);
        }

        private void CustomScrollbar_MouseDown(object sender, MouseEventArgs e)
        {
            mousedown = true;
            ptPoint = PointToClient(Cursor.Position);
            var nTrackHeight = Height - (UpArrowImage.Height + DownArrowImage.Height);
            var fThumbHeight = LargeChange / (float) Maximum * nTrackHeight;
            var nThumbHeight = (int) fThumbHeight;

            if (nThumbHeight > nTrackHeight)
            {
                nThumbHeight = nTrackHeight;
                fThumbHeight = nTrackHeight;
            }

            if (nThumbHeight < 20)
            {
                nThumbHeight = 20;
                fThumbHeight = 20;
            }

            var nTop = moThumbTop;
            nTop += UpArrowImage.Height;


            var thumbrect = new Rectangle(new Point(1, nTop + 1), new Size(ThumbMiddleImage.Width, nThumbHeight - 1));
            if (thumbrect.Contains(ptPoint))
            {
                //hit the thumb
                nClickPoint = ptPoint.Y - nTop;
                moThumbDown = true;
            }

            var uparrowrect = new Rectangle(new Point(1, 0), new Size(UpArrowImage.Width, nTop));
            if (uparrowrect.Contains(ptPoint))
            {
                var ScrollUnit = SmallChange;

                var realuparrowrect = new Rectangle(1, 0, UpArrowImage.Width, UpArrowImage.Height);
                if (realuparrowrect.Contains(ptPoint))
                    ScrollUnit = ExtraSmallChange;
                else
                    ScrollUnit = SmallChange;

                var nRealRange = Maximum - Minimum - LargeChange;
                var nPixelRange = nTrackHeight - nThumbHeight;
                if (nRealRange > 0)
                    if (nPixelRange > 0)
                    {
                        if (moThumbTop - ScrollUnit < 0)
                            moThumbTop = 0;
                        else
                            moThumbTop -= ScrollUnit;

                        //figure out value
                        var fPerc = moThumbTop / (float) nPixelRange;
                        var fValue = fPerc * (Maximum - LargeChange);

                        moValue = (int) fValue;

                        if (ValueChanged != null)
                            ValueChanged(this, new EventArgs());

                        if (Scroll != null)
                            Scroll(this, new EventArgs());

                        Invalidate();
                    }
            }

            var downarrowrect = new Rectangle(new Point(1, UpArrowImage.Height + nTop),
                new Size(UpArrowImage.Width, UpArrowImage.Height + nTrackHeight));
            if (downarrowrect.Contains(ptPoint))
            {
                var ScrollUnit = SmallChange;
                var realdownarrowrect = new Rectangle(1, UpArrowImage.Height + nTrackHeight, UpArrowImage.Width,
                    UpArrowImage.Height);
                if (realdownarrowrect.Contains(ptPoint))
                    ScrollUnit = ExtraSmallChange;
                else
                    ScrollUnit = SmallChange;
                var nRealRange = Maximum - Minimum - LargeChange;
                var nPixelRange = nTrackHeight - nThumbHeight;

                if (nRealRange > 0)
                    if (nPixelRange > 0)
                    {
                        if (moThumbTop + ScrollUnit > nPixelRange)
                            moThumbTop = nPixelRange;
                        else
                            moThumbTop += ScrollUnit;

                        //figure out value
                        var fPerc = moThumbTop / (float) nPixelRange;
                        var fValue = fPerc * (Maximum - LargeChange);

                        moValue = (int) fValue;

                        if (ValueChanged != null)
                            ValueChanged(this, new EventArgs());

                        if (Scroll != null)
                            Scroll(this, new EventArgs());

                        Invalidate();
                    }
            }
        }

        private void CustomScrollbar_MouseUp(object sender, MouseEventArgs e)
        {
            mousedown = false;
            moThumbDown = false;
            moThumbDragging = false;
        }

        private void MoveThumb(int y)
        {
            var nRealRange = Maximum - Minimum;
            var nTrackHeight = Height - (UpArrowImage.Height + DownArrowImage.Height);
            var fThumbHeight = LargeChange / (float) Maximum * nTrackHeight;
            var nThumbHeight = (int) fThumbHeight;

            if (nThumbHeight > nTrackHeight)
            {
                nThumbHeight = nTrackHeight;
                fThumbHeight = nTrackHeight;
            }

            if (nThumbHeight < 20)
            {
                nThumbHeight = 20;
                fThumbHeight = 20;
            }

            var nSpot = nClickPoint;
            var lastOperation = 0;
            var nPixelRange = nTrackHeight - nThumbHeight;
            if (moThumbDown && nRealRange > 0)
                if (nPixelRange > 0)
                {
                    var nNewThumbTop = y - (UpArrowImage.Height + nSpot);

                    if (nNewThumbTop - moThumbTop <= -1)
                    {
                        nNewThumbTop = moThumbTop - 1;
                        lastOperation = -1;
                    }
                    else if (nNewThumbTop - moThumbTop >= 1)
                    {
                        nNewThumbTop = moThumbTop + 1;
                        lastOperation = 1;
                    }
                    else if (nNewThumbTop - moThumbTop == 0)
                    {
                        nNewThumbTop = moThumbTop + lastOperation;
                        lastOperation = 0;
                    }
                    else if (nNewThumbTop - moThumbTop != 0)
                    {
                        nNewThumbTop = moThumbTop + lastOperation;
                    }

                    if (nNewThumbTop < 0)
                        moThumbTop = nNewThumbTop = 0;
                    else if (nNewThumbTop > nPixelRange)
                        moThumbTop = nNewThumbTop = nPixelRange;
                    else
                        moThumbTop = y - (UpArrowImage.Height + nSpot);

                    //figure out value
                    var fPerc = moThumbTop / (float) nPixelRange;
                    var fValue = fPerc * (Maximum - LargeChange);
                    moValue = (int) fValue;

                    Application.DoEvents();

                    Invalidate();
                }
        }

        private void CustomScrollbar_MouseMove(object sender, MouseEventArgs e)
        {
            if (moThumbDown) moThumbDragging = true;

            if (moThumbDragging)
            {
                MoveThumb(e.Y);
                if (ValueChanged != null)
                    ValueChanged(this, new EventArgs());

                if (Scroll != null)
                    Scroll(this, new EventArgs());
            }
        }
    }

    internal class ScrollbarControlDesigner : ControlDesigner
    {
        public override SelectionRules SelectionRules
        {
            get
            {
                var selectionRules = base.SelectionRules;
                var propDescriptor = TypeDescriptor.GetProperties(Component)["AutoSize"];
                if (propDescriptor != null)
                {
                    var autoSize = (bool) propDescriptor.GetValue(Component);
                    if (autoSize)
                        selectionRules = SelectionRules.Visible | SelectionRules.Moveable |
                                         SelectionRules.BottomSizeable | SelectionRules.TopSizeable;
                    else
                        selectionRules = SelectionRules.Visible | SelectionRules.AllSizeable | SelectionRules.Moveable;
                }

                return selectionRules;
            }
        }
    }
}