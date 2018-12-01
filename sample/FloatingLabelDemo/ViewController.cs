using System;
using CoreGraphics;
using Foundation;
using UIKit;

namespace FloatingLabelDemo
{
    public partial class ViewController : UIViewController
    {
        #region Constructor

        protected ViewController(IntPtr handle) : base(handle)
        {
            // Note: this .ctor should not contain any initialization logic.
        }

        #endregion

        #region Lifecycle

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            WrappingScrollView.ContentSize = ContentView.Bounds.Size;

            DismissKeyboardOnBackgroundTap();
            RegisterForKeyboardNotifications();
            FloatingLabelDemoField.Started += TextField_Started;
            FloatingLabelDemoField.Ended += TextField_Ended;
            ErrorTextField.Started += TextField_Started;
            ErrorTextField.Ended += TextField_Ended;

            ErrorTextField.Text = "Error Text";

            SliderTextColorValueChanged(SliderFloatingLabelColor);
            SliderTextActiveColorValueChanged(SliderFloatingLabelActiveColor);
            SliderBorderColorValueChanged(SliderBottomBorderColor);
            SliderBorderActiveColorValueChanged(SliderBottomBorderActiveColor);
            SliderErrorColorValueChanged(SliderErrorColor);
            ErrorTextFieldValueChanged(ErrorTextField);
        }

        public override void DidReceiveMemoryWarning()
        {
            UnregisterFromKeyboardNotification();
            FloatingLabelDemoField.Started -= TextField_Started;
            FloatingLabelDemoField.Ended -= TextField_Ended;
            ErrorTextField.Started -= TextField_Started;
            ErrorTextField.Ended -= TextField_Ended;

            base.DidReceiveMemoryWarning();
            // Release any cached data, images, etc that aren't in use.
        }

        #endregion        

        #region Event Handlers

        partial void SliderTextColorValueChanged(UISlider sender)
        {
            var color = UIColor.FromHSB(sender.Value, sender.Value, sender.Value);
            sender.ThumbTintColor = color;
            sender.MinimumTrackTintColor = color;
            LabelFloatingLabelColor.TextColor = color;
            FloatingLabelDemoField.FloatingLabelTextColor = color;
        }

        partial void SliderTextActiveColorValueChanged(UISlider sender)
        {
            var color = UIColor.FromHSB(sender.Value, sender.Value, sender.Value);
            sender.ThumbTintColor = color;
            sender.MinimumTrackTintColor = color;
            LabelFloatingLabelActiveColor.TextColor = color;
            FloatingLabelDemoField.FloatingLabelActiveTextColor = color;
        }

        partial void SliderBorderColorValueChanged(UISlider sender)
        {
            var color = UIColor.FromHSB(sender.Value, sender.Value, sender.Value);
            sender.ThumbTintColor = color;
            sender.MinimumTrackTintColor = color;
            LabelBottomBorderColor.TextColor = color;
            FloatingLabelDemoField.BottomBorderColor = color;
        }

        partial void SliderBorderActiveColorValueChanged(UISlider sender)
        {
            var color = UIColor.FromHSB(sender.Value, sender.Value, sender.Value);
            sender.ThumbTintColor = color;
            sender.MinimumTrackTintColor = color;
            LabelBottomBorderActiveColor.TextColor = color;
            FloatingLabelDemoField.BottomBorderActiveColor = color;
        }

        partial void SliderErrorColorValueChanged(UISlider sender)
        {
            var color = UIColor.FromHSB(sender.Value, sender.Value, sender.Value);
            sender.ThumbTintColor = color;
            sender.MinimumTrackTintColor = color;
            LabelErrorColor.TextColor = color;
            FloatingLabelDemoField.ErrorLabelTextColor = color;
        }

        partial void ErrorTextFieldValueChanged(UITextField sender)
        {
            FloatingLabelDemoField.SetError(sender.Text);
        }

        protected virtual void TextField_Started(object sender, EventArgs e)
        {
            FirstResponder = (UIView)sender;
            KeyboardWasShown();
        }

        protected virtual void TextField_Ended(object sender, EventArgs e)
        {
            FirstResponder = null;
        }

        #endregion

        #region Keyboard Handling

        #endregion
        #region Keyboard Handling

        private NSObject _keyboardDidShowNotification;
        private NSObject _keyboardWillHideNotification;
        private NSObject _keyboardWillChangeFrameNotification;

        protected UITapGestureRecognizer DismissKeyboardTap { get; private set; }
        public UIView FirstResponder { get; set; }
        protected nfloat CurrentKeyboardHeight { get; private set; }

        protected virtual nfloat GetViewHeaderHeight()
        {
            return UIApplication.SharedApplication.StatusBarFrame.Height + WrappingScrollView.Frame.Y;
        }

        protected virtual void RegisterForKeyboardNotifications()
        {
            _keyboardWillHideNotification = NSNotificationCenter.DefaultCenter.AddObserver(UIKeyboard.WillHideNotification, OnKeyboardNotification);
            _keyboardDidShowNotification = NSNotificationCenter.DefaultCenter.AddObserver(UIKeyboard.WillShowNotification, OnKeyboardNotification);
            _keyboardWillChangeFrameNotification = NSNotificationCenter.DefaultCenter.AddObserver(UIKeyboard.WillChangeFrameNotification, OnKeyboardNotification);
        }

        protected virtual void UnregisterFromKeyboardNotification()
        {
            if (_keyboardDidShowNotification != null)
            {
                NSNotificationCenter.DefaultCenter.RemoveObserver(_keyboardDidShowNotification);
            }

            if (_keyboardWillHideNotification != null)
            {
                NSNotificationCenter.DefaultCenter.RemoveObserver(_keyboardWillHideNotification);
            }

            if (_keyboardWillChangeFrameNotification != null)
            {
                NSNotificationCenter.DefaultCenter.RemoveObserver(_keyboardWillChangeFrameNotification);
            }
        }

        private void OnKeyboardNotification(NSNotification notification)
        {
            if (!IsViewLoaded || WrappingScrollView == null)
            {
                return;
            }

            //Check if the keyboard is becoming visible
            var isShowing = (notification.Name == UIKeyboard.WillShowNotification || notification.Name == UIKeyboard.WillChangeFrameNotification);

            //Start an animation, using values from the keyboard
            UIView.BeginAnimations("AnimateForKeyboard");
            UIView.SetAnimationBeginsFromCurrentState(true);
            UIView.SetAnimationDuration(UIKeyboard.AnimationDurationFromNotification(notification));
            UIView.SetAnimationCurve((UIViewAnimationCurve)UIKeyboard.AnimationCurveFromNotification(notification));

            //Pass the notification, calculating keyboard height, etc.
            bool landscape = InterfaceOrientation == UIInterfaceOrientation.LandscapeLeft || InterfaceOrientation == UIInterfaceOrientation.LandscapeRight;
            var keyboardFrame = isShowing
                                    ? UIKeyboard.FrameEndFromNotification(notification)
                                    : UIKeyboard.FrameBeginFromNotification(notification);

            CurrentKeyboardHeight = (landscape ? keyboardFrame.Width : keyboardFrame.Height);

            OnKeyboardChanged(isShowing);

            //Commit the animation
            UIView.CommitAnimations();
        }

        protected virtual void OnKeyboardChanged(bool isShowing)
        {
            if (isShowing)
            {
                KeyboardWasShown();
            }
            else
            {
                KeyboardWillBeHidden();
            }
        }

        protected virtual void KeyboardWasShown()
        {
            var tabBarHeight = UIScreen.MainScreen.Bounds.Height - GetViewHeaderHeight() - WrappingScrollView.Frame.Height;

            UIEdgeInsets contentInsets = new UIEdgeInsets(0.0f, 0.0f, CurrentKeyboardHeight - tabBarHeight, 0.0f);
            WrappingScrollView.ContentInset = contentInsets;
            WrappingScrollView.ScrollIndicatorInsets = contentInsets;

            ScrollFirstResponderIfHidden();
        }

        public virtual void ScrollFirstResponderIfHidden(bool scrollToTop = true)
        {
            if (FirstResponder != null && CurrentKeyboardHeight >= 0)
            {
                // If active text field is hidden by keyboard, scroll it so it's visible
                // Your application might not need or want this behavior.
                var screenHeight = UIScreen.MainScreen.Bounds.Height;
                var aRect = new CGRect(View.Frame.X, WrappingScrollView.ContentOffset.Y, View.Frame.Width, screenHeight - CurrentKeyboardHeight - View.Frame.Y);

                var firstResponderPoint = new CGPoint(FirstResponder.Frame.Location.X, FirstResponder.Frame.Location.Y + FirstResponder.Frame.Height);
                firstResponderPoint = FirstResponder.Superview.ConvertPointToView(firstResponderPoint, View);
                if (!aRect.Contains(firstResponderPoint))
                {
                    var tabBarHeight = UIScreen.MainScreen.Bounds.Height - GetViewHeaderHeight() - WrappingScrollView.Frame.Height;
                    var scrollViewFrameHeight = (WrappingScrollView.Frame.Height - WrappingScrollView.Frame.Y);
                    var heightToKeyboard = (scrollViewFrameHeight - (CurrentKeyboardHeight - tabBarHeight));
                    CGPoint scrollPoint = CGPoint.Empty;

                    if (scrollToTop)
                    {
                        var maxYScrollPos = WrappingScrollView.ContentSize.Height - heightToKeyboard - tabBarHeight;
                        scrollPoint = new CGPoint(0.0f, Math.Min(maxYScrollPos, FirstResponder.Frame.Location.Y));
                    }
                    else
                    {
                        var yScrollPos = Math.Max(0, FirstResponder.Frame.Location.Y - heightToKeyboard + 20); //should not be less zero
                        scrollPoint = new CGPoint(0.0f, yScrollPos);
                    }

                    WrappingScrollView.SetContentOffset(scrollPoint, true);
                }
            }
        }

        protected virtual void KeyboardWillBeHidden()
        {
            UIEdgeInsets contentInsets = UIEdgeInsets.Zero;
            WrappingScrollView.ContentInset = contentInsets;
            WrappingScrollView.ScrollIndicatorInsets = contentInsets;
            WrappingScrollView.SetContentOffset(new CGPoint(0, 0), true);
        }

        /// <summary>
        /// To hide the keyboard when a tap is made outside of the keyboard
        /// By default is not used. To use, just call this method on the ViewDidLoad of the ViewController
        /// </summary>
        protected void DismissKeyboardOnBackgroundTap()
        {
            if (DismissKeyboardTap != null)
            {
                View.RemoveGestureRecognizer(DismissKeyboardTap);
                DismissKeyboardTap.Dispose();
                DismissKeyboardTap = null;
            }

            // Add gesture recognizer to hide keyboard
            DismissKeyboardTap = new UITapGestureRecognizer { CancelsTouchesInView = false };
            DismissKeyboardTap.AddTarget(OnDismissKeyboard);
            View.AddGestureRecognizer(DismissKeyboardTap);
        }

        protected virtual void OnDismissKeyboard()
        {
            View.EndEditing(true);
        }

        #endregion
    }
}
