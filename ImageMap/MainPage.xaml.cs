using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using SkiaSharp;
using SkiaSharp.Views.Forms;
using Xamarin.Forms;
using ImageMap.Extensions;
using FFImageLoading.Args;
using System.Diagnostics;

namespace ImageMap
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class MainPage : ContentPage
    {
        const int POINT_WIDTH = 10;

        Tuple<float, float>[] _position;
        double _oriWidth;
        double _oriHeight;
        double _ratio;

        public MainPage()
        {
            InitializeComponent();
            image.Success += OnImageLoaded;

            _position = new Tuple<float, float>[]
            {
                new Tuple<float, float>(498, 511),
            };
        }

        private async void OnImageLoaded(object sender, SuccessEventArgs e)
        {
            _oriWidth = e.ImageInformation.OriginalWidth;
            _oriHeight = e.ImageInformation.OriginalHeight;
            _ratio = image.Width / _oriWidth;

            var rooms = InitializeRooms();
            int totalRoom = rooms.Count;

            await Device.InvokeOnMainThreadAsync(() => {
                pointView.Children.Clear();

                for (int i = 0; i < totalRoom; i++)
                {
                    var point1 = NewFrame(1);
                    point1.Clicked += Point1_Clicked;
                    var random = new Random();
                    var x = (rooms[i].Position.Item1 * _ratio) - POINT_WIDTH / 2;
                    var y = (rooms[i].Position.Item2 * _ratio) - POINT_WIDTH / 2;

                    AbsoluteLayout.SetLayoutBounds(point1, new Rectangle(x, y, POINT_WIDTH, POINT_WIDTH));
                    AbsoluteLayout.SetLayoutFlags(point1, AbsoluteLayoutFlags.None);

                    pointView.Children.Add(point1);
                }
            });

            await StartRealtime();
        }

        private async Task StartRealtime()
        {
            await Task.Delay(2000);

            var roads = InitializeRoads();
            var point = pointView.Children[0];
            double currentX = 0;
            double currentY = 0;

            while (true)
            {
                var realX = point.X + currentX;
                var realY = point.Y + currentY;

                //Caculate X
                var expectXRoad = roads.FirstOrDefault(r => (r.Position * _ratio) - realX < Math.Abs(40) && (r.Position * _ratio) - realX > Math.Abs(10) && r.IsVertical);
                var expectX = expectXRoad != null ? expectXRoad.Position * _ratio - realX - POINT_WIDTH / 2 : currentX;

                //Caculate
                var expectYRoad = expectX == currentX ? roads.FirstOrDefault(r => (r.Position * _ratio) - realY < Math.Abs(40) && (r.Position * _ratio) - realY > Math.Abs(10) && !r.IsVertical) : null;
                var expectY = expectYRoad != null ? expectYRoad.Position * _ratio - realY - POINT_WIDTH / 2 : currentY;

                if (expectX == currentX && expectY == currentY)
                {
                    Debug.WriteLine("Not found");

                    var largerX = roads.FirstOrDefault(r => (r.Position * _ratio) > realX && r.IsVertical);
                    var IsInHorizontalRoad = roads.Exists(r => (r.Position * _ratio) - realY < Math.Abs(10) && !r.IsVertical);
                    if (largerX != null)
                    {
                        if (IsInHorizontalRoad)
                            expectX = largerX.Position * _ratio - realX - POINT_WIDTH / 2;
                    }
                }

                //Move
                Debug.WriteLine($"Move to {expectX},{expectY}");
                await pointView.Children[0].TranslateTo(expectX, expectY, 1500);

                currentX = expectX;
                currentY = expectY;
            }
        }

        private void Point1_Clicked(object sender, EventArgs e)
        {
            Navigation.PushModalAsync(new DetailPage());
        }

        Button NewFrame(int id)
        {
            return new Button()
            {
                WidthRequest = POINT_WIDTH,
                HeightRequest = POINT_WIDTH,
                CornerRadius = POINT_WIDTH/2,
                Padding = 0,
                BorderColor = Color.Black,
                BackgroundColor = Color.FromHex("#e31231"),
            };
        }

        List<Room> InitializeRooms()
        {
            return new List<Room>()
            {
                new Room() { Extension = "Seminar", Position = new Tuple<float,float>(498, 511)},
                new Room() { Extension = "335", Position = new Tuple<float,float>(1901, 583)},
                new Room() { Extension = "365", Position = new Tuple<float,float>(1442, 1252)},
                new Room() { Extension = "364", Position = new Tuple<float,float>(1442, 1685)},
                new Room() { Extension = "336", Position = new Tuple<float,float>(990, 1888)},
                new Room() { Extension = "358", Position = new Tuple<float,float>(2905, 609)},
                new Room() { Extension = "Computer lab ", Position = new Tuple<float,float>(498, 1921)},
            };
        }

        List<Road> InitializeRoads()
        {
            return new List<Road>()
            {
                new Road() { IsVertical = true, Position = 714 },
                new Road() { IsVertical = true, Position = 1370 },
                new Road() { IsVertical = false, Position = 688 },
            };
        }
    }

    public class Room
    {
        public string Extension { get; set; }
        public Tuple<float, float> Position { get; set; }
    }

    public class Road
    {
        public bool IsVertical { get; set; }
        public float Position { get; set; }
    }
}
