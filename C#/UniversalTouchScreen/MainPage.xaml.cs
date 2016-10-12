using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Windows.Input;

using Windows.UI;
using Windows.UI.Popups;

using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

using Windows.UI.Xaml.Shapes;
using Windows.Devices.Input;

using System.Diagnostics;

namespace UniversalTouchScreen
{
    // Thanks ProfessorWeb.ru site - > http://professorweb.ru/my/windows8/rt-ext/level1/1_4.php   (please use Google Translate)
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
        }

        private SortedSet<uint> setId = new SortedSet<uint>();

        private Dictionary<uint, Color> colorDic = new Dictionary<uint, Color>();

        private Random r = new Random(1000);

        void Page_Loaded(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void gridPad_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            Debug.WriteLine("PointerEntered");
        }

        private void gridPad_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            Debug.WriteLine("PointerExited");
        }

        private void gridPad_PointerCanceled(object sender, PointerRoutedEventArgs e)
        {
            Debug.WriteLine("PointerCanceled");
        }

        private void gridPad_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            Debug.WriteLine("PointerPressed");
            Debug.WriteLine(e.Pointer.PointerId);

            byte[] rgb = new byte[3];

            Color color = Color.FromArgb(255, (byte)r.Next(255), (byte)r.Next(255), (byte)r.Next(255));
            colorDic.Add(e.Pointer.PointerId, color);
            
        }

        private void gridPad_PointerReleased(object sender, PointerRoutedEventArgs e)
        {
            Debug.WriteLine("PointerReleased");
            Debug.WriteLine(e.Pointer.PointerId);

            colorDic.Remove(e.Pointer.PointerId);
        }


        private void gridPad_PointerMoved(object sender, PointerRoutedEventArgs e)
        {
            Debug.WriteLine("PointerMoved");
            Debug.WriteLine(e.Pointer.PointerId);

            if (colorDic.ContainsKey(e.Pointer.PointerId))
            {
                // if PointerId is set 1, this function works with click and drag.    

                IList<Windows.UI.Input.PointerPoint> IlistPointer = e.GetIntermediatePoints(gridPad);
                int intPointerCount = IlistPointer.Count();

                Color color = colorDic[e.Pointer.PointerId];

                var blnIsMouse = e.Pointer.PointerDeviceType == PointerDeviceType.Mouse;

                //Pointer saved in reversed mode ...
                for (int i = intPointerCount - 1; i >= 0; i--)
                {
                    Windows.UI.Input.PointerPoint pointer = IlistPointer[i];

                    // User PointerId for identify sequence (line) - if needed
                    setId.Add(pointer.PointerId); // Add to Set - Set Automatically does not include duplicate Id ... 

                    Point point = pointer.Position;

                    // Prevent adding ellipse if mouse over grid and not left pressed ...
                    if (blnIsMouse && !pointer.Properties.IsLeftButtonPressed) continue;

                    //Properties -  https://msdn.microsoft.com/en-us/library/windows.ui.input.pointerpointproperties.aspx
                    //if your devise has stylus ...
                    float pressure = pointer.Properties.Pressure;

                    // 48 just randomly chosen value...
                    // value pressure always 0.5 if not pen (stylus) ...


                    // Pay attention about simulator - pressure will be very small 
                    pressure = 1; // use for simulate

                    double w = 48.0 * pressure;
                    double h = 48.0 * pressure;

                    if (point.X < w / 2.0 || point.X > gridPad.ActualWidth - w / 2)
                    {
                        continue;  // add ellipse only on grid
                    }

                    if (point.Y < h / 2.0 || point.Y > gridPad.ActualHeight - h / 2)
                    {
                        continue; // add ellipse only on grid
                    }

                    var tr = new TranslateTransform();
                    tr.X = point.X - gridPad.ActualWidth / 2.0;
                    tr.Y = point.Y - gridPad.ActualHeight / 2.0;

                    Ellipse el = new Ellipse()
                    {
                        Width = w,
                        Height = h,
                        Fill = new SolidColorBrush(color),
                        RenderTransform = tr,
                        Visibility = Visibility.Visible
                    };

                    gridPad.Children.Add(el);

                }

                txtInfo.Text = " " + setId.Count().ToString() + " lines ...";

                // base.OnPointerMoved(e); //You can see differences if uncomment this line
            }
        }

        private void btnClearAll_Click(object sender, RoutedEventArgs e)
        {
            while (gridPad.Children.Count != 0) // because below removed only first occurrence
            {
                foreach (UIElement uie in gridPad.Children)
                {
                    gridPad.Children.Remove(uie);
                }
            }

            setId = new SortedSet<uint>();
            txtInfo.Text = "0 - lines ...";

        }

        private void gridPad_ManipulationDelta(object sender, ManipulationDeltaRoutedEventArgs e) { }

        private void gridPad_ManipulationCompleted(object sender, ManipulationCompletedRoutedEventArgs e) { }

        private void gridPad_ManipulationStarted(object sender, ManipulationStartedRoutedEventArgs e) { }

    }
}

//GestureRecognizer class - > https://msdn.microsoft.com/library/windows/apps/br241937
//Some interesting idea   - > https://software.intel.com/en-us/articles/mixing-stylus-and-touch-input-on-windows-8
