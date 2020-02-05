using System;
using ImageMap.iOS.Renderer;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(Button), typeof(CustomButtonRenderer))]
namespace ImageMap.iOS.Renderer
{
    public class CustomButtonRenderer : ButtonRenderer
    {
        public CustomButtonRenderer()
        {
        }

        protected override void OnElementChanged(ElementChangedEventArgs<Button> e)
        {
            base.OnElementChanged(e);
            if (Layer == null || e.NewElement == null)
            {
                return;
            }

            Button element = Element as Button;
            Control.ContentEdgeInsets = new UIKit.UIEdgeInsets((nfloat)element.Padding.Top,
                (nfloat)element.Padding.Left,
                (nfloat)element.Padding.Bottom,
                (nfloat)element.Padding.Right);
        }
    }
}
