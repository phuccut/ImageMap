using System;
using ImageMap.Extensions;
using Xamarin.Forms;

namespace ImageMap.Controls
{
    public class PinchToZoomContainer : ContentView
    {
        double currentScale = 1;
        double startScale = 1;
        double xOffset = 0;
        double yOffset = 0;

        public PinchToZoomContainer()
        {
            var pinchGesture = new PinchGestureRecognizer();
            pinchGesture.PinchUpdated += OnPinchUpdated;

            var swipeGesture = new PanGestureRecognizer();
            swipeGesture.PanUpdated += SwipeGesture_PanUpdated; ;

            GestureRecognizers.Add(swipeGesture);
            GestureRecognizers.Add(pinchGesture);
        }

        public void SetScale(double number)
        {
            Content.ScaleTo(1, 500, Easing.SinIn);
        }

        private void SwipeGesture_PanUpdated(object sender, PanUpdatedEventArgs e)
        {
            if (currentScale == 1)
                return;
            switch (e.StatusType)
            {
                case GestureStatus.Running:
                    Content.TranslateTo(xOffset + e.TotalX, yOffset + e.TotalY, 0);
                    return;
                case GestureStatus.Completed:
                    xOffset = Content.TranslationX;
                    yOffset = Content.TranslationY;
                    return;
            }
        }

        void OnPinchUpdated(object sender, PinchGestureUpdatedEventArgs e)
        {
            if (e.Status == GestureStatus.Started)
            {
                // Store the current scale factor applied to the wrapped user interface element,  
                // and zero the components for the center point of the translate transform.  
                startScale = Content.Scale;
                Content.AnchorX = 0;
                Content.AnchorY = 0;
            }
            if (e.Status == GestureStatus.Running)
            {
                // Calculate the scale factor to be applied.  
                currentScale += (e.Scale - 1) * startScale;
                currentScale = Math.Max(1, currentScale);

                // The ScaleOrigin is in relative coordinates to the wrapped user interface element,  
                // so get the X pixel coordinate.  
                double renderedX = Content.X + xOffset;
                double deltaX = renderedX / Width;
                double deltaWidth = Width / (Content.Width * startScale);
                double originX = (e.ScaleOrigin.X - deltaX) * deltaWidth;

                // The ScaleOrigin is in relative coordinates to the wrapped user interface element,  
                // so get the Y pixel coordinate.  
                double renderedY = Content.Y + yOffset;
                double deltaY = renderedY / Height;
                double deltaHeight = Height / (Content.Height * startScale);
                double originY = (e.ScaleOrigin.Y - deltaY) * deltaHeight;

                // Calculate the transformed element pixel coordinates.  
                double targetX = xOffset - (originX * Content.Width) * (currentScale - startScale);
                double targetY = yOffset - (originY * Content.Height) * (currentScale - startScale);

                // Apply translation based on the change in origin.  
                Content.TranslationX = targetX.Clamp(-Content.Width * (currentScale - 1), 0);
                Content.TranslationY = targetY.Clamp(-Content.Height * (currentScale - 1), 0);

                // Apply scale factor  
                Content.Scale = currentScale;
            }
            if (e.Status == GestureStatus.Completed)
            {
                // Store the translation delta's of the wrapped user interface element.  
                xOffset = Content.TranslationX;
                yOffset = Content.TranslationY;

                if (currentScale == 1)
                {
                    Content.TranslationX = 0;
                    Content.TranslationY = 0;
                }
            }
        }
    }
}
