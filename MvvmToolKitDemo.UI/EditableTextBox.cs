using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;

namespace MvvmToolKitDemo.UI
{
    public class EditableTextBox : TextBox
    {
        public static readonly DependencyProperty EditOnMouseDownProperty;
        public static readonly DependencyProperty InEditModeProperty;
        public static readonly DependencyProperty PreventEditModeProperty;
        public static readonly DependencyProperty CancelOnValidationErrorProperty;
        public static readonly DependencyProperty ReadOnlyTextProperty;
        public static readonly DependencyProperty RestoreFocusTargetProperty;
        public static readonly DependencyProperty TextTrimmingProperty;

        public static readonly RoutedEvent BeginningEditEvent;
        public static readonly RoutedEvent EditEndingEvent;

        private bool _internalModeChange;
        private bool _mouseCaptured;
        private string _unmodifiedText = string.Empty;

        static EditableTextBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(EditableTextBox), new FrameworkPropertyMetadata(typeof(EditableTextBox)));

            EditOnMouseDownProperty = DependencyProperty.Register(nameof(EditOnMouseDown), typeof(bool), typeof(EditableTextBox), new PropertyMetadata(true));
            InEditModeProperty = DependencyProperty.Register(nameof(InEditMode), typeof(bool), typeof(EditableTextBox), new PropertyMetadata(false, new PropertyChangedCallback(OnInEditModeChanged), new CoerceValueCallback(CoerceEditMode)));
            PreventEditModeProperty = DependencyProperty.Register(nameof(PreventEditMode), typeof(bool), typeof(EditableTextBox), new PropertyMetadata(false, new PropertyChangedCallback(OnPreventEditModeChanged)));
            CancelOnValidationErrorProperty = DependencyProperty.Register(nameof(CancelOnValidationError), typeof(bool), typeof(EditableTextBox), new PropertyMetadata(true));
            ReadOnlyTextProperty = DependencyProperty.Register(nameof(ReadOnlyText), typeof(string), typeof(EditableTextBox), new PropertyMetadata(null));
            RestoreFocusTargetProperty = DependencyProperty.Register(nameof(RestoreFocusTarget), typeof(FrameworkElement), typeof(EditableTextBox), new PropertyMetadata(null));
            TextTrimmingProperty = DependencyProperty.Register(nameof(TextTrimming), typeof(TextTrimming), typeof(EditableTextBox), new PropertyMetadata(TextTrimming.None));

            BeginningEditEvent = EventManager.RegisterRoutedEvent("BeginningEdit", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(EditableTextBox));
            EditEndingEvent = EventManager.RegisterRoutedEvent("EditEnding", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(EditableTextBox));
        }

        public EditableTextBox()
        {
            CommandBindings.Add(new CommandBinding(EditableTextBoxRoutedCommands.Edit, (o, e) => Edit(), (o, e) => e.CanExecute = !InEditMode));
            EventManager.RegisterClassHandler(typeof(EditableTextBox), Mouse.PreviewMouseDownOutsideCapturedElementEvent, new MouseButtonEventHandler(OnPreviewMouseDownOutsideCapturedElement));
        }

        public event RoutedEventHandler BeginningEdit
        {
            add => AddHandler(BeginningEditEvent, value);
            remove => RemoveHandler(BeginningEditEvent, value);
        }

        public event RoutedEventHandler EditEnding
        {
            add => AddHandler(EditEndingEvent, value);
            remove => RemoveHandler(EditEndingEvent, value);
        }

        public bool CancelOnValidationError
        {
            get => (bool)GetValue(CancelOnValidationErrorProperty);
            set => SetValue(CancelOnValidationErrorProperty, value);
        }

        public bool EditOnMouseDown
        {
            get => (bool)GetValue(EditOnMouseDownProperty);
            set => SetValue(EditOnMouseDownProperty, value);
        }

        public bool InEditMode
        {
            get => (bool)GetValue(InEditModeProperty);
            set => SetValue(InEditModeProperty, value);
        }

        public bool PreventEditMode
        {
            get => (bool)GetValue(PreventEditModeProperty);
            set => SetValue(PreventEditModeProperty, value);
        }

        public string ReadOnlyText
        {
            get => (string)GetValue(ReadOnlyTextProperty);
            set => SetValue(ReadOnlyTextProperty, value);
        }

        public FrameworkElement RestoreFocusTarget
        {
            get => (FrameworkElement)GetValue(RestoreFocusTargetProperty);
            set => SetValue(RestoreFocusTargetProperty, value);
        }

        public TextTrimming TextTrimming
        {
            get => (TextTrimming)GetValue(TextTrimmingProperty);
            set => SetValue(TextTrimmingProperty, value);
        }

        protected override void OnContextMenuClosing(ContextMenuEventArgs e)
        {
            base.OnContextMenuClosing(e);

            _mouseCaptured = CaptureMouse();
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);

            if (!InEditMode)
                return;

            switch (e.Key)
            {
                case Key.Return:
                    Accept();
                    e.Handled = true;
                    break;
                case Key.Escape:
                    Cancel();
                    e.Handled = true;
                    break;
            }
        }

        protected override void OnGotKeyboardFocus(KeyboardFocusChangedEventArgs e)
        {
            base.OnGotKeyboardFocus(e);

            if (!IsTabStop
                || e.OldFocus == null
                || !EditOnMouseDown
                && InputManager.Current.MostRecentInputDevice is not KeyboardDevice)
                return;

            if (!Equals(e.OldFocus, ContextMenu) && Mouse.LeftButton != MouseButtonState.Pressed)
                _mouseCaptured = CaptureMouse();

            BeginEdit();
        }

        protected override void OnLostKeyboardFocus(KeyboardFocusChangedEventArgs e)
        {
            base.OnLostKeyboardFocus(e);

            if (InputManager.Current.IsInMenuMode)
                return;

            if ((ContextMenu != null && ContextMenu.IsOpen) || !CancelOnValidationError || Accept())
                return;

            Cancel();
        }

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            base.OnMouseDown(e);

            if (!EditOnMouseDown || !IsKeyboardFocused)
                return;

            BeginEdit();
        }

        protected override void OnMouseUp(MouseButtonEventArgs e)
        {
            base.OnMouseUp(e);

            if (IsMouseCaptured || !IsKeyboardFocused || !InEditMode)
                return;

            _mouseCaptured = CaptureMouse();
        }

        private static void OnInEditModeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((EditableTextBox)d).OnInEditModeChanged((bool)e.NewValue);
        }

        private static object CoerceEditMode(DependencyObject d, object baseValue)
        {
            return ((EditableTextBox)d).CoerceEditMode((bool)baseValue);
        }

        private static void OnPreventEditModeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            d.SetValue(InEditModeProperty, false);
        }

        private static void OnPreviewMouseDownOutsideCapturedElement(object sender, MouseButtonEventArgs e)
        {
            ((EditableTextBox)sender).PreviewMouseDownOutsideCapturedElement(e);
        }

        private bool CoerceEditMode(bool editMode)
        {
            if (PreventEditMode)
                return false;

            return editMode;
        }

        private void OnInEditModeChanged(bool inEditMode)
        {
            if (inEditMode)
            {
                _unmodifiedText = Text;
                IsReadOnly = false;

                if (!IsKeyboardFocused)
                    Focus();

                RaiseEvent(new RoutedEventArgs(BeginningEditEvent, this));
            }
            else
            {
                if (_mouseCaptured)
                {
                    ReleaseMouseCapture();
                    _mouseCaptured = false;
                }

                IsReadOnly = true;

                if (!_internalModeChange)
                    Accept();

                RestoreFocusTarget?.Focus();
                RaiseEvent(new RoutedEventArgs(EditEndingEvent, this));
            }
        }

        private bool Accept()
        {
            if (HasValidationError())
                return false;

            _internalModeChange = true;
            EndEdit();
            _internalModeChange = false;
            return true;
        }

        private bool HasValidationError()
        {
            var textBindingExpression = BindingOperations.GetBindingExpressionBase(this, TextProperty);
            if (textBindingExpression == null)
                return false;

            return textBindingExpression.HasValidationError;
        }

        private void Edit()
        {
            _internalModeChange = true;
            BeginEdit();
            _internalModeChange = false;
        }

        private void Cancel()
        {
            Text = _unmodifiedText;
            _internalModeChange = true;
            EndEdit();
            _internalModeChange = false;
        }

        private void BeginEdit()
        {
            if (InEditMode)
                return;
            InEditMode = true;
        }

        private void EndEdit()
        {
            if (!InEditMode)
                return;

            SetCurrentValue(InEditModeProperty, false);
        }

        private void PreviewMouseDownOutsideCapturedElement(MouseButtonEventArgs e)
        {
            Window window = Window.GetWindow(this);
            if (window == null)
                EndEdit();
            else
            {
                if (VisualTreeHelper.HitTest(window, e.GetPosition(window))?.VisualHit is Visual visualHit && visualHit.IsDescendantOf(this))
                    return;

                EndEdit();
            }
        }
    }

    public static class EditableTextBoxRoutedCommands
    {
        public static RoutedUICommand Edit { get; } = new RoutedUICommand(
            nameof(Edit),
            nameof(Edit),
            typeof(EditableTextBoxRoutedCommands),
            new InputGestureCollection(new List<InputGesture>() { new KeyGesture(Key.F2) })
        );
    }
}
