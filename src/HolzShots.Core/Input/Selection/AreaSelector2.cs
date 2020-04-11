using System;
using System.Diagnostics;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using unvell.D2DLib;
using unvell.D2DLib.WinForm;

namespace HolzShots.Input.Selection
{
    public partial class AreaSelector2 : D2DForm, IAreaSelector
    {
        private static TaskCompletionSource<Rectangle> _tcs;
        private D2DBitmap _image;
        private D2DBitmapGraphics _dimmedImage;
        private Rectangle _imageBounds;
        private readonly D2DBrush _blackOverlayBrush;

        private static readonly D2DColor _overlayColor = new D2DColor(0.8f, D2DColor.Black);
        private static readonly D2DColor _selectionBorder = new D2DColor(0.6f, D2DColor.White);

        private static Cursor _cursor = new Cursor(Properties.Resources.CrossCursor.Handle);

        public AreaSelector2()
        {
            InitializeComponent();

            BackColor = Color.Black;

            EscapeKeyToClose = false;
            ShowFPS = false;
            AnimationDraw = false;

            StartPosition = FormStartPosition.Manual;
            WindowState = FormWindowState.Normal;
            FormBorderStyle = FormBorderStyle.None;
            // DesktopLocation = new Point(0, 0);

            Bounds = SystemInformation.VirtualScreen;
#if !DEBUG
            TopMost = true;
#endif

            _blackOverlayBrush = Device.CreateSolidColorBrush(_overlayColor);
        }

        public Task<Rectangle> PromptSelectionAsync(Bitmap image)
        {
            Debug.Assert(image != null);
            Debug.Assert(_image == null);
            Debug.Assert(_tcs == null);

            if (_tcs != null)
                return _tcs.Task;

            _imageBounds = new Rectangle(0, 0, image.Width, image.Height);
            _image = Device.CreateBitmapFromGDIBitmap(image);
            _dimmedImage = CreateDimemdImage(image.Width, image.Height);

            _tcs = new TaskCompletionSource<Rectangle>();

            Visible = true;
            Cursor = _cursor;

            return _tcs.Task;
        }

        private D2DBitmapGraphics CreateDimemdImage(int width, int height)
        {
            var res = Device.CreateBitmapGraphics(width, height);
            res.BeginRender();
            res.DrawBitmap(_image, _imageBounds);
            res.FillRectangle(_imageBounds, _blackOverlayBrush);
            res.EndRender();
            return res;
        }

        #region Window State
        private SelectionState _state = new InitialState();

        abstract class SelectionState { }
        class InitialState : SelectionState { }
        abstract class RectangleState : SelectionState
        {
            public Point UserSelectionStart { get; protected set; }
            public Point CursorPosition { get; protected set; }
            protected RectangleState(Point userSelectionStart, Point cursorPosition)
            {
                UserSelectionStart = userSelectionStart;
                CursorPosition = cursorPosition;
            }

            public Rectangle GetSelectedOutline(Rectangle canvasBounds)
            {
                var start = UserSelectionStart;
                var cursor = CursorPosition;
                var x = Math.Min(start.X, cursor.X);
                var y = Math.Min(start.Y, cursor.Y);
                var width = Math.Abs(start.X - cursor.X);
                var height = Math.Abs(start.Y - cursor.Y);

                var unconstrainedSelection = new Rectangle(x, y, width, height);

                return Rectangle.Intersect(unconstrainedSelection, canvasBounds);
            }
        }
        class ResizingRectangleState : RectangleState
        {
            public ResizingRectangleState(Point userSelectionStart, Point cursorPosition) : base(userSelectionStart, cursorPosition) { }
            public void UpdateCursorPosition(Point newCursorPosition) => CursorPosition = newCursorPosition;
        }

        class MovingRectangleState : RectangleState
        {
            public MovingRectangleState(Point userSelectionStart, Point cursorPosition) : base(userSelectionStart, cursorPosition) { }
            public void MoveByNewCursorPosition(Point newCursorPosition)
            {
                var prevStart = UserSelectionStart;
                var prev = CursorPosition;
                var offset = new Point(
                    newCursorPosition.X - prev.X,
                    newCursorPosition.Y - prev.Y
                );
                UserSelectionStart = new Point(
                    prevStart.X + offset.X,
                    prevStart.Y + offset.Y
                );
                CursorPosition = newCursorPosition;
            }
        }
        class FinalState : SelectionState
        {
            public Rectangle Result { get; }
            public FinalState(Rectangle result) => Result = result;
        }
        #endregion

        #region Mouse and Keyboard Stuff
        protected override void OnMouseDown(MouseEventArgs e)
        {
            if (_state is FinalState)
                Debug.Fail("OnMouseDown after final state");

            Debug.Assert(e != null);
            var currentPos = e.Location;
            switch (e.Button)
            {
                case MouseButtons.Left:

                    switch (_state)
                    {
                        case InitialState _:
                            _state = new ResizingRectangleState(currentPos, currentPos);
                            break;
                        case ResizingRectangleState _: break; // Pressing the left mouse button without leaving first is not possible
                        case MovingRectangleState _: break; // This should do nothing
                        case FinalState _: Debug.Fail("Unhandled State"); break; // Not possible
                        default: Debug.Fail("Unhandled State"); break;
                    }
                    break;
                case MouseButtons.Right:
                    switch (_state)
                    {
                        case InitialState _: break; // Pressing the right mouse button without a selected rectangle is not possible
                        case ResizingRectangleState resizing:
                            _state = new MovingRectangleState(resizing.UserSelectionStart, resizing.CursorPosition);
                            break;
                        case MovingRectangleState _: break;  // Pressing the right mouse button without leaving first is not possible
                        case FinalState _: Debug.Fail("Unhandled State"); break; // Not possible
                        default: Debug.Fail("Unhandled State"); break;
                    }
                    break;
                default:
                    break; // Ignore all other mouse buttons
            }

            Invalidate();
        }
        protected override void OnMouseUp(MouseEventArgs e)
        {
            if (_state is FinalState)
                Debug.Fail("OnMouseUp after final state");

            Debug.Assert(e != null);
            switch (e.Button)
            {
                case MouseButtons.Left:
                    switch (_state)
                    {
                        case InitialState _: break; // Releasing the left mouse button without being in some state should not be possible
                        case RectangleState availableSelection:
                            var res = availableSelection.GetSelectedOutline(_imageBounds);
                            var finalState = new FinalState(new Rectangle(res.X, res.Y, res.Width, res.Height));
                            _state = finalState;
                            FinishSelection(finalState);
                            break;
                        case FinalState _: Debug.Fail("Unhandled State"); break; // Not possible
                        default: Debug.Fail("Unhandled State"); break;
                    }
                    break;
                case MouseButtons.Right:
                    switch (_state)
                    {
                        case InitialState initial: break; // Releasing the left mouse button without being in some state should not be possible
                        case ResizingRectangleState resizing: break;// Releasing the right mouse button without leaving first is not possible
                        case MovingRectangleState moving:
                            _state = new ResizingRectangleState(moving.UserSelectionStart, moving.CursorPosition);
                            break;
                        case FinalState _: Debug.Fail("Unhandled State"); break; // Not possible
                        default: Debug.Fail("Unhandled State"); break;
                    }
                    break;
                default:
                    break; // Ignore all other mouse buttons
            }

            Invalidate();
        }
        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (_state is FinalState)
                Debug.Fail("OnMouseMove after final state");

            Debug.Assert(e != null);
            var currentPos = e.Location;

            switch (_state)
            {
                case InitialState _: break; // Nothing to be updated
                case ResizingRectangleState resizing:
                    resizing.UpdateCursorPosition(currentPos);
                    break;
                case MovingRectangleState moving:
                    moving.MoveByNewCursorPosition(currentPos);
                    break;
                case FinalState _: break; // Nothing to be updated
                default: Debug.Fail("Unhandled State"); break;
            }
            Invalidate();
        }

        protected override void OnKeyUp(KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Escape:
                    CancelSelection();
                    break;
                case Keys.Space:
                    // TODO: toggle magnifier
                    break;
                default: break;
            }
            base.OnKeyUp(e);
        }
        #endregion

        protected override void OnRender(D2DGraphics g)
        {
            if (_state is FinalState)
                return;

            Debug.Assert(_blackOverlayBrush != null);

            g.Antialias = false;
            g.DrawBitmap(_dimmedImage, _imageBounds);

            switch (_state)
            {
                case InitialState _: break; // Nothing to be updated
                case RectangleState availableSelection:

                    var outline = availableSelection.GetSelectedOutline(_imageBounds);
                    D2DRect rect = outline; // Caution: implicit conversion which we don't want to do twice

                    g.DrawBitmap(_image, rect, rect);

                    // We need to widen the rectangle by 0.5px so that the result will be exactly 1px wide.
                    // Otherwise, it will be 2px and darker.
                    var selectionOutline = new D2DRect(
                        outline.X - 0.5f,
                        outline.Y - 0.5f,
                        outline.Width + 1f,
                        outline.Height + 1f
                    );
                    g.DrawRectangle(selectionOutline, _selectionBorder);

                    break;
                case FinalState _: break; // Nothing to be updated
                default: Debug.Fail("Unhandled State"); break;
            }
#if DEBUG
            g.DrawTextCenter(_state.GetType().Name, D2DColor.White, SystemFonts.DefaultFont.Name, 36, ClientRectangle);
#endif
        }

        private void CancelSelection()
        {
            Debug.Assert(_tcs != null);
            _tcs.SetCanceled();
            CloseInternal();
        }
        private void FinishSelection(FinalState state)
        {
            if (state.Result.Width < 1 || state.Result.Height < 1)
            {
                CancelSelection();
                return;
            }

            Debug.Assert(_tcs != null);
            _tcs.SetResult(state.Result);
            CloseInternal();
        }

        void CloseInternal()
        {
            Close();
            Visible = false;
            CleanUp();
        }

        private void CleanUp()
        {
            _tcs = null;
            _state = null;
            // TODO: Maybe move this to some dispose method
            _image?.Dispose();
            _dimmedImage?.Dispose();
            _blackOverlayBrush?.Dispose();
        }
    }
}
