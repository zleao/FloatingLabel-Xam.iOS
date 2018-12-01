// WARNING
//
// This file has been generated automatically by Visual Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using Foundation;
using System;
using System.CodeDom.Compiler;

namespace FloatingLabelDemo
{
    [Register ("ViewController")]
    partial class ViewController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIView ContentView { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITextField ErrorTextField { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        FloatingLabel.FloatingLabelTextField FloatingLabelDemoField { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel LabelBottomBorderActiveColor { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel LabelBottomBorderColor { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel LabelErrorColor { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel LabelFloatingLabelActiveColor { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel LabelFloatingLabelColor { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UISlider SliderBottomBorderActiveColor { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UISlider SliderBottomBorderColor { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UISlider SliderErrorColor { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UISlider SliderFloatingLabelActiveColor { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UISlider SliderFloatingLabelColor { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIScrollView WrappingScrollView { get; set; }

        [Action ("ErrorTextFieldValueChanged:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void ErrorTextFieldValueChanged (UIKit.UITextField sender);

        [Action ("SliderBorderActiveColorValueChanged:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void SliderBorderActiveColorValueChanged (UIKit.UISlider sender);

        [Action ("SliderBorderColorValueChanged:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void SliderBorderColorValueChanged (UIKit.UISlider sender);

        [Action ("SliderErrorColorValueChanged:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void SliderErrorColorValueChanged (UIKit.UISlider sender);

        [Action ("SliderTextActiveColorValueChanged:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void SliderTextActiveColorValueChanged (UIKit.UISlider sender);

        [Action ("SliderTextColorValueChanged:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void SliderTextColorValueChanged (UIKit.UISlider sender);

        void ReleaseDesignerOutlets ()
        {
            if (ContentView != null) {
                ContentView.Dispose ();
                ContentView = null;
            }

            if (ErrorTextField != null) {
                ErrorTextField.Dispose ();
                ErrorTextField = null;
            }

            if (FloatingLabelDemoField != null) {
                FloatingLabelDemoField.Dispose ();
                FloatingLabelDemoField = null;
            }

            if (LabelBottomBorderActiveColor != null) {
                LabelBottomBorderActiveColor.Dispose ();
                LabelBottomBorderActiveColor = null;
            }

            if (LabelBottomBorderColor != null) {
                LabelBottomBorderColor.Dispose ();
                LabelBottomBorderColor = null;
            }

            if (LabelErrorColor != null) {
                LabelErrorColor.Dispose ();
                LabelErrorColor = null;
            }

            if (LabelFloatingLabelActiveColor != null) {
                LabelFloatingLabelActiveColor.Dispose ();
                LabelFloatingLabelActiveColor = null;
            }

            if (LabelFloatingLabelColor != null) {
                LabelFloatingLabelColor.Dispose ();
                LabelFloatingLabelColor = null;
            }

            if (SliderBottomBorderActiveColor != null) {
                SliderBottomBorderActiveColor.Dispose ();
                SliderBottomBorderActiveColor = null;
            }

            if (SliderBottomBorderColor != null) {
                SliderBottomBorderColor.Dispose ();
                SliderBottomBorderColor = null;
            }

            if (SliderErrorColor != null) {
                SliderErrorColor.Dispose ();
                SliderErrorColor = null;
            }

            if (SliderFloatingLabelActiveColor != null) {
                SliderFloatingLabelActiveColor.Dispose ();
                SliderFloatingLabelActiveColor = null;
            }

            if (SliderFloatingLabelColor != null) {
                SliderFloatingLabelColor.Dispose ();
                SliderFloatingLabelColor = null;
            }

            if (WrappingScrollView != null) {
                WrappingScrollView.Dispose ();
                WrappingScrollView = null;
            }
        }
    }
}