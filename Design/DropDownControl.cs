using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;

namespace Design
{
    public partial class DropDownControl : UserControl
    {
        public delegate void DropDownHandler(object sender, EventArgs e);

        public delegate void TextChangedHandler(object sender, EventArgs e);

        public enum eDockSide
        {
            Left,
            Right
        }

        public enum eDropState
        {
            Closed,
            Closing,
            Dropping,
            Dropped
        }

        private Size _AnchorSize = new Size(121, 26);

        private bool _DesignView = true;

        private Control _dropDownItem;

        public bool CFocused;
        private bool closedWhileInControl;

        private DropDownContainer dropContainer;

        public TextBox inputText = new TextBox();

        protected bool mousePressed;
        private Size storedSize;

        public DropDownControl()
        {
            InitializeComponent();
            DoubleBuffered = true;
            storedSize = Size;
            BackColor = Color.White;
            Text = Name;
            drawTextBox();
        }

        protected eDropState DropState { get; private set; }

        public override string Text
        {
            get => inputText.Text;
            set => inputText.Text = value;
        }

        public Size AnchorSize
        {
            get => _AnchorSize;
            set
            {
                _AnchorSize = value;
                Invalidate();
            }
        }

        public eDockSide DockSide { get; set; }

        [DefaultValue(false)]
        protected bool DesignView
        {
            get => _DesignView;
            set
            {
                if (_DesignView == value) return;

                _DesignView = value;
                if (_DesignView)
                {
                    Size = storedSize;
                }
                else
                {
                    storedSize = Size;
                    Size = _AnchorSize;
                }
            }
        }

        public Rectangle AnchorClientBounds { get; private set; }

        protected virtual bool CanDrop
        {
            get
            {
                if (dropContainer != null)
                    return false;

                if (dropContainer == null && closedWhileInControl)
                {
                    closedWhileInControl = false;
                    return false;
                }

                return !closedWhileInControl;
            }
        }

        public int DropDownWidth
        {
            get => _dropDownItem.Width;
            set
            {
                _dropDownItem.Width = value;
                Invalidate();
            }
        }

        public int DropDownHeight
        {
            get => _dropDownItem.Height;
            set
            {
                _dropDownItem.Height = value;
                Invalidate();
            }
        }

        public event TextChangedHandler OnTextChanged;
        public event DropDownHandler OnDropDown;

        public void InitializeDropDown(Control dropDownItem)
        {
            if (_dropDownItem != null)
                throw new Exception("The drop down item has already been implemented!");
            _DesignView = false;
            DropState = eDropState.Closed;
            Size = _AnchorSize;
            AnchorClientBounds = new Rectangle(2, 2, _AnchorSize.Width - 26, _AnchorSize.Height - 4);
            if (Controls.Contains(dropDownItem))
                Controls.Remove(dropDownItem);
            _dropDownItem = dropDownItem;
        }

        public event EventHandler PropertyChanged;

        protected void OnPropertyChanged()
        {
            if (PropertyChanged != null)
                PropertyChanged(null, null);
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            if (_DesignView)
                storedSize = Size;
            _AnchorSize.Width = Width;
            if (!_DesignView)
            {
                _AnchorSize.Height = Height;
                AnchorClientBounds = new Rectangle(2, 2, _AnchorSize.Width - 26, _AnchorSize.Height - 4);
            }
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            mousePressed = true;

            if (OnDropDown == null) return;

            OnDropDown(this, new EventArgs());

            OpenDropDown();
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            mousePressed = false;
            Invalidate();
        }

        public void RefreshDropDown()
        {
            if (dropContainer != null) dropContainer.Refresh();
        }

        protected void OpenDropDown()
        {
            if (_dropDownItem == null)
                throw new NotImplementedException(
                    "The drop down item has not been initialized!  Use the InitializeDropDown() method to do so.");

            if (!CanDrop) return;

            dropContainer = new DropDownContainer(_dropDownItem, this);
            dropContainer.Bounds = GetDropDownBounds();
            dropContainer.DropStateChange += dropContainer_DropStateChange;
            dropContainer.FormClosed += dropContainer_Closed;
            ParentForm.Move += ParentForm_Move;
            DropState = eDropState.Dropping;
            dropContainer.Show(this);
            DropState = eDropState.Dropped;
            Invalidate();
        }

        private void ParentForm_Move(object sender, EventArgs e)
        {
            dropContainer.Bounds = GetDropDownBounds();
        }


        public void CloseDropDown()
        {
            if (dropContainer != null)
            {
                DropState = eDropState.Closing;
                dropContainer.Freeze = false;
                dropContainer.Close();
                DropState = eDropState.Closed;
            }
        }

        private void dropContainer_DropStateChange(eDropState state)
        {
            DropState = state;
        }

        private void dropContainer_Closed(object sender, FormClosedEventArgs e)
        {
            if (!dropContainer.IsDisposed)
            {
                dropContainer.DropStateChange -= dropContainer_DropStateChange;
                dropContainer.FormClosed -= dropContainer_Closed;
                ParentForm.Move -= ParentForm_Move;
                dropContainer.Dispose();
            }

            closedWhileInControl = RectangleToScreen(ClientRectangle).Contains(Cursor.Position);
            dropContainer = null;

            DropState = eDropState.Closed;
            Invalidate();
        }

        protected virtual Rectangle GetDropDownBounds()
        {
            var inflatedDropSize = new Size(_dropDownItem.Width, _dropDownItem.Height);
            var screenBounds = DockSide == eDockSide.Left
                ? new Rectangle(Parent.PointToScreen(new Point(Bounds.X, Bounds.Bottom)), inflatedDropSize)
                : new Rectangle(Parent.PointToScreen(new Point(Bounds.Right - _dropDownItem.Width, Bounds.Bottom)),
                    inflatedDropSize);
            var workingArea = Screen.GetWorkingArea(screenBounds);
            //make sure we're completely in the top-left working area
            if (screenBounds.X < workingArea.X) screenBounds.X = workingArea.X;
            if (screenBounds.Y < workingArea.Y) screenBounds.Y = workingArea.Y;

            //make sure we're not extended past the working area's right /bottom edge
            if (screenBounds.Right > workingArea.Right && workingArea.Width > screenBounds.Width)
                screenBounds.X = workingArea.Right - screenBounds.Width;
            if (screenBounds.Bottom > workingArea.Bottom && workingArea.Height > screenBounds.Height)
                screenBounds.Y = Parent.PointToScreen(Location).Y - screenBounds.Height;
            // screenBounds.Y = workingArea.Bottom - screenBounds.Height;

            return screenBounds;
        }


        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            if (ComboBoxRenderer.IsSupported)
            {
                ComboBoxRenderer.DrawTextBox(e.Graphics, new Rectangle(new Point(0, 0), _AnchorSize), getState());

                ComboBoxRenderer.DrawDropDownButton(e.Graphics,
                    new Rectangle(_AnchorSize.Width - 17, 0, 17, _AnchorSize.Height), ComboBoxState.Normal);
            }
            else
            {
                ControlPaint.DrawComboButton(e.Graphics, new Rectangle(
                        _AnchorSize.Width - 15, 0, 15, _AnchorSize.Height),
                    Enabled ? ButtonState.Normal : ButtonState.Inactive);
            }

            using (Brush b = new SolidBrush(BackColor))
            {
                e.Graphics.FillRectangle(b, AnchorClientBounds);
                e.Graphics.DrawRectangle(new Pen(b, 4), new Rectangle(2, 2, Width - 20, Height - 4));
                e.Graphics.FillRectangle(new SolidBrush(Color.White), new Rectangle(2, 2, Width - 20, Height - 4));
                e.Graphics.DrawImage(Resource.dropLight, Width - 17, 0, 17, Height); //26

                //e.Graphics.DrawRectangle(new Pen(new SolidBrush(Color.FromArgb(31, 31, 31)), 1), new Rectangle(0, 0, Width - 1, Height - 1));
                e.Graphics.FillRectangle(b, new Rectangle(Width - 17, 1, 2, Height - 2));
            }

            //TextRenderer.DrawText(e.Graphics, _Text, this.Font, this.AnchorClientBounds, this.ForeColor, TextFormatFlags.WordEllipsis);
        }

        private void drawTextBox()
        {
            inputText.BorderStyle = BorderStyle.None;
            inputText.AutoSize = false;
            inputText.Name = "inputText";
            inputText.ForeColor = Color.White;
            inputText.Font = new Font(Font.Name, 8, Font.Style);
            inputText.Location = new Point(5, 4);
            inputText.Size = new Size(Size.Width - 20, 18);
            inputText.MaximumSize = new Size(Size.Width - 20, 18);
            inputText.Padding = new Padding(0, 5, 0, 0);
            inputText.BackColor = Color.FromArgb(62, 62, 66);
            inputText.TextChanged += InputText_TextChanged;
            inputText.GotFocus += InputText_GotFocus;
            inputText.LostFocus += InputText_LostFocus;

            Controls.Add(inputText);
        }

        private void InputText_LostFocus(object sender, EventArgs e)
        {
            CFocused = false;
        }

        private void InputText_GotFocus(object sender, EventArgs e)
        {
            CFocused = true;
        }

        private void InputText_TextChanged(object sender, EventArgs e)
        {
            Text = inputText.Text;
            if (OnTextChanged == null) return;
            OnTextChanged(this, new EventArgs());
        }

        private ComboBoxState getState()
        {
            if (mousePressed || dropContainer != null)
                return ComboBoxState.Pressed;
            return ComboBoxState.Normal;
        }

        public void FreezeDropDown(bool remainVisible)
        {
            if (dropContainer != null)
            {
                dropContainer.Freeze = true;
                if (!remainVisible)
                    dropContainer.Visible = false;
            }
        }

        public void UnFreezeDropDown()
        {
            if (dropContainer != null)
            {
                dropContainer.Freeze = false;
                if (!dropContainer.Visible)
                    dropContainer.Visible = true;
            }
        }


        internal sealed class DropDownContainer : Form, IMessageFilter
        {
            public delegate void DropWindowArgs(eDropState state);

            public bool Freeze;
            private DropDownControl parent;

            public DropDownContainer(Control dropDownItem, DropDownControl _parent)
            {
                parent = _parent;
                FormBorderStyle = FormBorderStyle.None;
                Capture = true;
                DoubleBuffered = true;
                dropDownItem.Location = new Point(0, 0);
                Controls.Add(dropDownItem);
                StartPosition = FormStartPosition.Manual;
                ShowInTaskbar = false;
                Application.AddMessageFilter(this);
            }

            public bool PreFilterMessage(ref Message m)
            {
                if (!Freeze && Visible && (ActiveForm == null || !ActiveForm.Equals(this)))
                {
                    Close();
                    OnDropStateChange(eDropState.Closed);
                }


                return false;
            }

            public event DropWindowArgs DropStateChange;

            protected void OnDropStateChange(eDropState state)
            {
                if (DropStateChange != null)
                    DropStateChange(state);
            }

            protected override void OnPaint(PaintEventArgs e)
            {
                base.OnPaint(e);
                //e.Graphics.DrawRectangle(Pens.Blue, new Rectangle(0, 0, this.ClientSize.Width - 1, this.ClientSize.Height *2));
            }

            protected override void OnClosing(CancelEventArgs e)
            {
                OnDropStateChange(eDropState.Closed);
                Application.RemoveMessageFilter(this);
                Controls.RemoveAt(0); //prevent the control from being disposed
                base.OnClosing(e);
            }
        }
    }
}