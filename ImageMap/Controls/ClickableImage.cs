using System;
using Xamarin.Forms;

namespace ImageMap.Controls
{
    public class ClickableImage : Image, IClickableImageController
    {
        public event EventHandler Touched;

        public void SendTouched()
        {
            Touched?.Invoke(this, EventArgs.Empty);
        }

        public Tuple<float, float> TouchedCoordinate
        {
            get { return (Tuple<float, float>)GetValue(TouchedCoordinateProperty); }
            set { SetValue(TouchedCoordinateProperty, value); }
        }

        public static readonly BindableProperty TouchedCoordinateProperty =
            BindableProperty.Create(
               propertyName: "TouchedCoordinate",
                returnType: typeof(Tuple<float, float>),
               declaringType: typeof(ClickableImage),
               defaultValue: new Tuple<float, float>(0, 0),
               propertyChanged: OnPropertyChanged);

        public static void OnPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
        }
    }
}