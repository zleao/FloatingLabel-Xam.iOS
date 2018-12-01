//The MIT License(MIT)

//Copyright(c) 2018 José Pereira
//Original Xamarin.iOS implementation by Greg Shackles
//  https://github.com/gshackles/FloatLabeledEntry
//Original implementation by Jared Verdi
//  https://github.com/jverdi/JVFloatLabeledTextField
//Original Concept by Matt D.Smith
//  http://dribbble.com/shots/1254439--GIF-Mobile-Form-Interaction?list=users


//Permission is hereby granted, free of charge, to any person obtaining a copy
//of this software and associated documentation files (the "Software"), to deal
//in the Software without restriction, including without limitation the rights
//to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//copies of the Software, and to permit persons to whom the Software is
//furnished to do so, subject to the following conditions:

//The above copyright notice and this permission notice shall be included in all
//copies or substantial portions of the Software.

//THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.IN NO EVENT SHALL THE
//AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
//SOFTWARE.

using System;
using System.ComponentModel;
using CoreGraphics;
using Foundation;
using UIKit;

namespace FloatingLabel
{
    [Register("FloatingLabelTextField"), DesignTimeVisible(true)]
    public class FloatingLabelTextField : UITextField, IComponent
    {
        #region IComponent Implementation

        public ISite Site { get; set; }
        public event EventHandler Disposed;

        #endregion

        #region Fields

        private bool _errorLabelAddedAsSubView;
        private UIView _bottomLine;

        #endregion

        #region Properties

        [DisplayName("Labels Left Padding"), Export("FloatingLabelLeftPadding"), Browsable(true)]
        public float LabelsLeftPadding
        {
            get => _labelsLeftPadding; 
            set 
            {
                _labelsLeftPadding = value;
                SetNeedsLayout();
                LayoutIfNeeded();
            }
        }
        private float _labelsLeftPadding = 15f;

        [DisplayName("Label Color"), Export("FloatingLabelTextColor"), Browsable(true)]
        public UIColor FloatingLabelTextColor
        {
            get => _floatingLabelTextColor; 
            set 
            {
                _floatingLabelTextColor = value;
                SetNeedsLayout();
                LayoutIfNeeded();
            }
        }
        private UIColor _floatingLabelTextColor = UIColor.Black.ColorWithAlpha(0.7f);

        [DisplayName("Label Active Color"), Export("FloatingLabelActiveTextColor"), Browsable(true)]
        public UIColor FloatingLabelActiveTextColor
        {
            get => _floatingLabelActiveTextColor; 
            set 
            {
                _floatingLabelActiveTextColor = value;
                SetNeedsLayout();
                LayoutIfNeeded();
            }
        }
        private UIColor _floatingLabelActiveTextColor = UIColor.Blue;

        public UIFont FloatingLabelTextFont
        {
            get { return FloatingLabel.Font; }
            set { FloatingLabel.Font = value; }
        }

        public override string Placeholder
        {
            get => base.Placeholder;
            set => SetPlaceholderAndFloatingLabel(value ?? string.Empty);
        }

        public int MaxLength { get; set; }

        protected UILabel FloatingLabel { get; private set; }

        /// <summary>
        /// Flag to be used to completely bypass the base control layout and set a custom one.
        /// </summary>
        protected bool BypassBaseControlLayout { get; set; }

        protected float AnimationSpeed { get; set; } = 0.3f;

        [DisplayName("Bottom Border Color"), Export("FloatingLabelBottomBorderColor"), Browsable(true)]
        public UIColor BottomBorderColor
        {
            get => _bottomBorderColor; 
            set 
            {
                _bottomBorderColor = value;
                SetNeedsLayout();
                LayoutIfNeeded();
            }
        }
        private UIColor _bottomBorderColor = UIColor.Gray;

        [DisplayName("Bottom Border Active Color"), Export("FloatingLabelBottomBorderActiveColor"), Browsable(true)]
        public UIColor BottomBorderActiveColor
        {
            get => _bottomBorderActiveColor; 
            set 
            {
                _bottomBorderActiveColor = value;
                SetNeedsLayout();
                LayoutIfNeeded();
            }
        }
        private UIColor _bottomBorderActiveColor = UIColor.Blue;

        [DisplayName("Error Label Color"), Export("FloatingLabelErrorTextColor"), Browsable(true)]
        public UIColor ErrorLabelTextColor 
        {
            get => _errorLabelTextColor; 
            set 
            {
                _errorLabelTextColor = value;
                SetNeedsLayout();
                LayoutIfNeeded(); 
            }
        }
        private UIColor _errorLabelTextColor = UIColor.Red;
        
        [DisplayName("Error Label Font"), Export("FloatingLabelErrorTextFont"), Browsable(true)]
        public UIFont ErrorLabelFont
        { 
            get => _errorLabelFont; 
            set 
            {
                _errorLabelFont = value;
                SetNeedsLayout();
                LayoutIfNeeded();
            }
        }
        private UIFont _errorLabelFont = UIFont.PreferredCaption1;

        protected UILabel ErrorLabel { get; set; }

        public bool HasError => string.IsNullOrEmpty(ErrorLabel?.Text) == false;

        /// <summary>
        /// DESIGN-MODE ONLY
        /// This property exists only to provide a way to see an error message 
        /// when designing the view in a designer tool (e.g. Storyboard)
        /// </summary>
        /// <value>The error text.</value>
        [DisplayName("Error Text (Design Only)"), Export("FloatingLabelErrorText"), Browsable(true)]
        public virtual string ErrorText { get; set; }

        #endregion

        #region Constructor

        public FloatingLabelTextField(CGRect frame)
            : base(frame)
        {
            #pragma warning disable RECS0021 // Warns about calls to virtual member functions occuring in the constructor
            InitializeControl();
            #pragma warning restore RECS0021 // Warns about calls to virtual member functions occuring in the constructor
        }

        public FloatingLabelTextField(IntPtr handle)
            : base(handle)
        {
        }

        #endregion

        #region Methods

        public override void AwakeFromNib() => InitializeControl();

        protected virtual void InitializeControl()
        {
            ShouldChangeCharacters = HandleShouldChangeCharacters;

            VerticalAlignment = UIControlContentVerticalAlignment.Bottom;

            FloatingLabel = new UILabel
            {
                Alpha = 0.0f,
                Font = UIFont.PreferredCaption1
            };

            AddSubview(FloatingLabel);

            Placeholder = Placeholder; // sets up label frame

            _bottomLine = new UIView();
            AddSubview(_bottomLine);

            ErrorLabel = new UILabel
            {
                Font = ErrorLabelFont,
                Lines = 1,
                LineBreakMode = UILineBreakMode.TailTruncation
            };

            if(Site != null && Site.DesignMode)
            {
                //Only used for DesignMode, to be able to see how the error state looks like
                SetError(ErrorText);
            }
        }

        protected virtual void SetPlaceholderAndFloatingLabel(string placeholderText)
        {
            base.Placeholder = placeholderText;

            RefreshPlaceholderUI();

            //Update floating label text and size
            FloatingLabel.Text = placeholderText;
            FloatingLabel.SizeToFit();
            FloatingLabel.Frame =
                new CGRect(LabelsLeftPadding,
                           FloatingLabel.Font.LineHeight,
                           FloatingLabel.Frame.Size.Width,
                           FloatingLabel.Frame.Size.Height);
        }

        public void RefreshPlaceholderUI()
        {
            //Set placeholder style
            AttributedPlaceholder = CreateTextAttributes(base.Placeholder, Font, FloatingLabelTextColor);
        }

        public override CGRect TextRect(CGRect forBounds)
        {
            return InsetRect(base.TextRect(forBounds), new UIEdgeInsets(0, LabelsLeftPadding, GetErrorTextFieldHeight() + 7, 0));
        }

        public override CGRect EditingRect(CGRect forBounds)
        {
            return InsetRect(base.EditingRect(forBounds), new UIEdgeInsets(0, LabelsLeftPadding, GetErrorTextFieldHeight() + 7, 0));
        }

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();

            if (BypassBaseControlLayout)
            {
                return;
            }

            RefreshPlaceholderUI();

            ErrorLabel.Font = ErrorLabelFont;

            var borderThickness = (HasError ? 2.0f : 1.0f);
            _bottomLine.Frame = new CGRect(0, Frame.Height - borderThickness - GetErrorTextFieldHeight() - 2, this.Bounds.Width, borderThickness);

            if (IsFirstResponder)
            {
                _bottomLine.BackgroundColor = (HasError ? ErrorLabelTextColor : BottomBorderActiveColor);

                var shouldFloat = true;
                var isFloating = (FloatingLabel.Alpha == 1f);

                if (shouldFloat == isFloating)
                {
                    UpdateLabels();
                    OnUpdateLabelsComplete();
                }
                else
                {
                    if(Site != null && Site.DesignMode)
                    {
                        //Support for design mode
                        //No need for animations in design mode
                        UpdateLabels();
                        OnUpdateLabelsComplete();
                    }
                    else
                    {
                        AnimateNotify(AnimationSpeed, 0.0f,
                                      UIViewAnimationOptions.BeginFromCurrentState | UIViewAnimationOptions.CurveEaseOut,
                                      UpdateLabels,
                                      (isFinished) => OnUpdateLabelsComplete());
                    }
                }
            }
            else
            {
                _bottomLine.BackgroundColor = (HasError ? ErrorLabelTextColor : BottomBorderColor);

                if (Site != null && Site.DesignMode)
                {
                    //Support for design mode
                    //No need for animations in design mode
                    UpdateLabels();
                    OnUpdateLabelsComplete();
                }
                else
                {
                    AnimateNotify(AnimationSpeed, 0.0f,
                                  UIViewAnimationOptions.BeginFromCurrentState | UIViewAnimationOptions.CurveEaseIn,
                                  UpdateLabels,
                                  (isFinished) => OnUpdateLabelsComplete());
                }

                LayoutIfNeeded();
            }
        }

        protected virtual void UpdateLabels()
        {
            if (!string.IsNullOrEmpty(Text))
            {
                FloatingLabel.Alpha = 1.0f;
                FloatingLabel.Frame =
                    new CGRect(
                        FloatingLabel.Frame.Location.X,
                        9.0f,
                        FloatingLabel.Frame.Size.Width,
                        FloatingLabel.Frame.Size.Height);
            }
            else
            {
                FloatingLabel.Alpha = 0.0f;
                FloatingLabel.Frame =
                    new CGRect(
                        FloatingLabel.Frame.Location.X,
                        FloatingLabel.Font.LineHeight,
                        FloatingLabel.Frame.Size.Width,
                        FloatingLabel.Frame.Size.Height);
            }

            if (IsFirstResponder || !string.IsNullOrEmpty(Text))
            {
                FloatingLabel.TextColor = (HasError ? ErrorLabelTextColor : (IsFirstResponder ? FloatingLabelActiveTextColor : FloatingLabelTextColor));
            }
            else
            {
                FloatingLabel.TextColor = (HasError ? ErrorLabelTextColor : FloatingLabelTextColor);
            }
        }

        protected virtual void OnUpdateLabelsComplete()
        {
            if (HasError)
            {
                if (!_errorLabelAddedAsSubView)
                {
                    AddSubview(ErrorLabel);
                    _errorLabelAddedAsSubView = true;
                }
                var errorHeight = GetErrorTextFieldHeight();
                ErrorLabel.Frame = new CGRect(LabelsLeftPadding, Frame.Height - errorHeight, Bounds.Width, errorHeight);
                ErrorLabel.TextColor = ErrorLabelTextColor;
            }
            else
            {
                if (_errorLabelAddedAsSubView)
                {
                    ErrorLabel.RemoveFromSuperview();
                    _errorLabelAddedAsSubView = false;
                }
            }
        }

        public virtual void SetError(string errorText)
        {
            if(ErrorLabel != null && ErrorLabel.Text != errorText)
            {
                ErrorLabel.Text = errorText;
                LayoutIfNeeded();
                SetNeedsLayout();
            }
        }

        public virtual void ClearError()
        {
            if(ErrorLabel != null && !string.IsNullOrEmpty(ErrorLabel.Text))
            {
                ErrorLabel.Text = string.Empty;
                LayoutIfNeeded();
                SetNeedsLayout();
            }
        }

        protected virtual float GetErrorTextFieldHeight()
        {
            return (float)ErrorLabel.Font.LineHeight + 6f;
        }

        #endregion

        #region Delegates handling

        protected virtual bool HandleShouldChangeCharacters(UITextField textField, NSRange range, string replacementString)
        {
            if (MaxLength > 0)
            {
                var newLength = textField.Text.Length + replacementString.Length - range.Length;
                return newLength <= MaxLength;
            }

            return true;
        }

        #endregion

        #region Helper Methods

        protected static CGRect InsetRect(CGRect rect, UIEdgeInsets insets) =>
            new CGRect(
                rect.X + insets.Left,
                rect.Y + insets.Top,
                rect.Width - insets.Left - insets.Right,
                rect.Height - insets.Top - insets.Bottom);

        protected static NSMutableAttributedString CreateTextAttributes(string text, UIFont font, UIColor foregroundColor)
        {
            var attributedText = new NSMutableAttributedString(text ?? string.Empty);

            //apply the font
            attributedText.AddAttribute(UIStringAttributeKey.Font, FromObject(font), new NSRange(0, text?.Length ?? 0));

            //apply the text color
            attributedText.AddAttribute(UIStringAttributeKey.ForegroundColor, FromObject(foregroundColor), new NSRange(0, text?.Length ?? 0));

            return attributedText;
        }

        #endregion
    }
}
