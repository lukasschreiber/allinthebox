using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Design
{
    public partial class DropDownControl : UserControl
    {
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

        DropDownContainer dropContainer;
        Control _dropDownItem;
        bool closedWhileInControl;
        private Size storedSize;

        private eDropState _dropState;
        protected eDropState DropState
        {
            get { return _dropState; }
        }

        public override string Text
        {
            get { return inputText.Text; }
            set
            {
                inputText.Text = value;
                
            }
        }

        public delegate void TextChangedHandler(object sender, EventArgs e);
        public event TextChangedHandler OnTextChanged;

        public delegate void DropDownHandler(object sender, EventArgs e);
        public event DropDownHandler OnDropDown;

        public DropDownControl()
        {
            InitializeComponent();
            this.DoubleBuffered = true;
            this.storedSize = this.Size;
            this.BackColor = Color.White;
            this.Text = this.Name;
            drawTextBox();
        }

        public void InitializeDropDown(Control dropDownItem)
        {
            if (_dropDownItem != null)
                throw new Exception("The drop down item has already been implemented!");
            _DesignView = false;
            _dropState = eDropState.Closed;
            this.Size = _AnchorSize;
            this._AnchorClientBounds = new Rectangle(2, 2, _AnchorSize.Width - 26, _AnchorSize.Height - 4);
            if (this.Controls.Contains(dropDownItem))
                this.Controls.Remove(dropDownItem);
            _dropDownItem = dropDownItem;
        }

        private Size _AnchorSize = new Size(121, 26);
        public Size AnchorSize
        {
            get { return _AnchorSize; }
            set
            {
                _AnchorSize = value;
                this.Invalidate();
            }
        }

        private eDockSide _DockSide;
        public eDockSide DockSide
        {
            get { return _DockSide; }
            set { _DockSide = value; }
        }

        private bool _DesignView = true;
        [DefaultValue(false)]
        protected bool DesignView
        {
            get { return _DesignView; }
            set
            {
                if (_DesignView == value) return;

                _DesignView = value;
                if (_DesignView)
                {
                    this.Size = storedSize;
                }
                else
                {
                    storedSize = this.Size;
                    this.Size = _AnchorSize;
                }

            }
        }

        public event EventHandler PropertyChanged;
        protected void OnPropertyChanged()
        {
            if (PropertyChanged != null)
                PropertyChanged(null, null);
        }

        private Rectangle _AnchorClientBounds;
        public Rectangle AnchorClientBounds
        {
            get { return _AnchorClientBounds; }
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            if (_DesignView)
                storedSize = this.Size;
            _AnchorSize.Width = this.Width;
            if (!_DesignView)
            {
                _AnchorSize.Height = this.Height;
                this._AnchorClientBounds = new Rectangle(2, 2, _AnchorSize.Width - 26, _AnchorSize.Height - 4);
            }
        }

        protected bool mousePressed;

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
            this.Invalidate();
        }

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

        public void RefreshDropDown() {
            if (dropContainer != null)
            {
                dropContainer.Refresh();
            }
        }

        protected void OpenDropDown()
        {
            if (_dropDownItem == null)
                throw new NotImplementedException("The drop down item has not been initialized!  Use the InitializeDropDown() method to do so.");

            if (!CanDrop) return;

            dropContainer = new DropDownContainer(_dropDownItem,this);
            dropContainer.Bounds = GetDropDownBounds();
            dropContainer.DropStateChange += new DropDownContainer.DropWindowArgs(dropContainer_DropStateChange);
            dropContainer.FormClosed += new FormClosedEventHandler(dropContainer_Closed);
            this.ParentForm.Move += new EventHandler(ParentForm_Move);
            _dropState = eDropState.Dropping;
            dropContainer.Show(this);
            _dropState = eDropState.Dropped;
            this.Invalidate();


        }

        void ParentForm_Move(object sender, EventArgs e)
        {
            dropContainer.Bounds = GetDropDownBounds();
        }


        public void CloseDropDown()
        {

            if (dropContainer != null)
            {
                _dropState = eDropState.Closing;
                dropContainer.Freeze = false;
                dropContainer.Close();
                _dropState = eDropState.Closed;

            }
        }

        void dropContainer_DropStateChange(DropDownControl.eDropState state)
        {
            _dropState = state;
        }
        void dropContainer_Closed(object sender, FormClosedEventArgs e)
        {
            if (!dropContainer.IsDisposed)
            {
                dropContainer.DropStateChange -= dropContainer_DropStateChange;
                dropContainer.FormClosed -= dropContainer_Closed;
                this.ParentForm.Move -= ParentForm_Move;
                dropContainer.Dispose();
            }
            closedWhileInControl = (this.RectangleToScreen(this.ClientRectangle).Contains(Cursor.Position));
            dropContainer = null;

            _dropState = eDropState.Closed;
            this.Invalidate();

        }

        public int DropDownWidth
        {
            get { return _dropDownItem.Width; }
            set {
                _dropDownItem.Width = value;
                Invalidate();
            }
        }

        public int DropDownHeight
        {
            get { return _dropDownItem.Height; }
            set
            {
                _dropDownItem.Height = value;
                Invalidate();
            }
        }

        protected virtual Rectangle GetDropDownBounds()
        {
            Size inflatedDropSize = new Size(_dropDownItem.Width, _dropDownItem.Height);
            Rectangle screenBounds = _DockSide == eDockSide.Left ?
                new Rectangle(this.Parent.PointToScreen(new Point(this.Bounds.X, this.Bounds.Bottom)), inflatedDropSize)
                : new Rectangle(this.Parent.PointToScreen(new Point(this.Bounds.Right - _dropDownItem.Width, this.Bounds.Bottom)), inflatedDropSize);
            Rectangle workingArea = Screen.GetWorkingArea(screenBounds);
            //make sure we're completely in the top-left working area
            if (screenBounds.X < workingArea.X) screenBounds.X = workingArea.X;
            if (screenBounds.Y < workingArea.Y) screenBounds.Y = workingArea.Y;

            //make sure we're not extended past the working area's right /bottom edge
            if (screenBounds.Right > workingArea.Right && workingArea.Width > screenBounds.Width)
                screenBounds.X = workingArea.Right - screenBounds.Width;
            if (screenBounds.Bottom > workingArea.Bottom && workingArea.Height > screenBounds.Height)
                screenBounds.Y = this.Parent.PointToScreen(Location).Y - screenBounds.Height;
               // screenBounds.Y = workingArea.Bottom - screenBounds.Height;

            return screenBounds;
        }



        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            if (ComboBoxRenderer.IsSupported)
            {
                ComboBoxRenderer.DrawTextBox(e.Graphics, new Rectangle(new Point(0, 0), _AnchorSize), getState());

                ComboBoxRenderer.DrawDropDownButton(e.Graphics, new Rectangle(_AnchorSize.Width - 17, 0, 17, _AnchorSize.Height), System.Windows.Forms.VisualStyles.ComboBoxState.Normal);
            }
            else
            {
                ControlPaint.DrawComboButton(e.Graphics, new Rectangle(
                    _AnchorSize.Width - 15, 0, 15, _AnchorSize.Height),
                    (this.Enabled) ? ButtonState.Normal : ButtonState.Inactive);
            }

            using (Brush b = new SolidBrush(this.BackColor))
            {
                e.Graphics.FillRectangle(b, this.AnchorClientBounds);
                e.Graphics.DrawRectangle(new Pen(b,4), new Rectangle(2, 2, Width-20, Height-4));
                e.Graphics.FillRectangle(new SolidBrush(Color.White), new Rectangle(2, 2, this.Width - 20, this.Height - 4));
                e.Graphics.DrawImage(Resource.dropLight, Width - 17, 0 , 17, Height);//26

                //e.Graphics.DrawRectangle(new Pen(new SolidBrush(Color.FromArgb(31, 31, 31)), 1), new Rectangle(0, 0, Width - 1, Height - 1));
                e.Graphics.FillRectangle(b, new Rectangle(Width-17,1,2,Height-2));
            }

            //TextRenderer.DrawText(e.Graphics, _Text, this.Font, this.AnchorClientBounds, this.ForeColor, TextFormatFlags.WordEllipsis);
        }

        public TextBox inputText = new TextBox();
        public bool CFocused = false;

        private void drawTextBox() {
            inputText.BorderStyle = BorderStyle.None;
            inputText.AutoSize = false;
            inputText.Name = "inputText";
            inputText.ForeColor = Color.White;
            inputText.Font = new Font(Font.Name, 8, Font.Style);
            inputText.Location = new Point(5, 4);
            inputText.Size = new Size(this.Size.Width - 20, 18);
            inputText.MaximumSize = new Size(this.Size.Width - 20, 18);
            inputText.Padding = new Padding(0, 5, 0, 0);
            inputText.BackColor = Color.FromArgb(62, 62, 66);
            inputText.TextChanged += InputText_TextChanged;
            inputText.GotFocus += InputText_GotFocus;
            inputText.LostFocus += InputText_LostFocus;

            this.Controls.Add(inputText);
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
            this.Text = inputText.Text;
            if (OnTextChanged == null) return;
            OnTextChanged(this, new EventArgs());
        }

        private System.Windows.Forms.VisualStyles.ComboBoxState getState()
        {
            if (mousePressed || dropContainer != null)
                return System.Windows.Forms.VisualStyles.ComboBoxState.Pressed;
            else
                return System.Windows.Forms.VisualStyles.ComboBoxState.Normal;
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
            public bool Freeze;
            private DropDownControl parent;

            public DropDownContainer(Control dropDownItem, DropDownControl _parent)
            {
                this.parent = _parent;
                this.FormBorderStyle = FormBorderStyle.None;
                this.Capture = true;
                this.DoubleBuffered = true;
                dropDownItem.Location = new Point(0, 0);
                this.Controls.Add(dropDownItem);
                this.StartPosition = FormStartPosition.Manual;
                this.ShowInTaskbar = false;
                Application.AddMessageFilter(this);

            }

            public bool PreFilterMessage(ref Message m)
            {
                if (!Freeze && this.Visible && (Form.ActiveForm == null || !Form.ActiveForm.Equals(this)))
                {
                    this.Close();
                    OnDropStateChange(eDropState.Closed);
                }


                return false;
            }

            public delegate void DropWindowArgs(eDropState state);
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
                this.Controls.RemoveAt(0); //prevent the control from being disposed
                base.OnClosing(e);

            }
        }
    }
}
