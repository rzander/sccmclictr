using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ClientCenter.Controls
{
    /// <summary>
    /// Interaction logic for TimeLine.xaml
    /// </summary>
    public partial class TimeLine : UserControl
    {
        public TimeLine()
        {
            InitializeComponent();
        }

        public double BlockHeight = 0;
        
        private void UserControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            stackPanel1.Children.Clear();
            if (canvasTimeLine.Children.Count > 1)
            {
                canvasTimeLine.Children.RemoveRange(1, canvasTimeLine.Children.Count - 1);
            }
            int iMax = 24;
            for (int i = 1; i <= iMax; i++)
            {
                Rectangle oRect = new Rectangle();
                oRect.Width = e.NewSize.Width;
                oRect.Height = Math.Round(e.NewSize.Height / iMax);
                BlockHeight = oRect.Height;
                oRect.HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;
                oRect.Stroke = Brushes.Black;
                oRect.StrokeThickness = 0.2;
                oRect.Name = "lHour" + i.ToString();
                oRect.Margin = new Thickness(0, -1, 0, 0);
                stackPanel1.Children.Add(oRect);

                Label lText = new Label();
                lText.Content = i.ToString("D2") + ":00";
                lText.Margin = new Thickness(5, -8, 0, 0);
                //lText.Height = Math.Round(e.NewSize.Height / iMax);
                lText.HorizontalContentAlignment = System.Windows.HorizontalAlignment.Right;
                lText.VerticalContentAlignment = System.Windows.VerticalAlignment.Center;
                
                lText.Background = new SolidColorBrush(Colors.WhiteSmoke);
                lText.Name = "lTime" + i.ToString();
                //Canvas.SetLeft(lText, 10);
                Canvas.SetTop(lText, ((BlockHeight - 1) * (i - 1)) + (BlockHeight / 2));

                if (BlockHeight < 20)
                {
                    if (BlockHeight > 8)
                    {
                        if (i % 2 != 0)
                        {
                            lText.FontSize = 10;
                            canvasTimeLine.Children.Add(lText);
                        }
                    }
                    else
                    {
                        if (i == 6)
                        {
                            lText.FontSize = 10;
                            canvasTimeLine.Children.Add(lText);
                        }
                        if (i == 12)
                        {
                            lText.FontSize = 10;
                            canvasTimeLine.Children.Add(lText);
                        }
                        if (i == 18)
                        {
                            lText.FontSize = 10;
                            canvasTimeLine.Children.Add(lText);
                        }
                    }
                }
                else
                {
                    lText.FontSize = 12;
                    canvasTimeLine.Children.Add(lText);
                }


            }
        }
    }
}
