using LancerUI.Controls.Chart.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Shapes;

namespace LancerUI.Controls.Chart
{
    public class LUChartRadar : Control
    {
        /// <summary>
        /// 雷达图大小
        /// </summary>
        public double Size { get => (double)GetValue(SizeProperty); set => SetValue(SizeProperty, value); }
        public static readonly DependencyProperty SizeProperty = DependencyProperty.Register("Size", typeof(double), typeof(LUChartRadar), new PropertyMetadata(150.0,new PropertyChangedCallback(OnLadarPropertyChanged)));

        private static void OnLadarPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as LUChartRadar;
            control?.Draw();
        }

        /// <summary>
        /// 层数
        /// </summary>
        public int LayerCount { get => (int)GetValue(LayerCountProperty); set => SetValue(LayerCountProperty, value); }
        public static readonly DependencyProperty LayerCountProperty = DependencyProperty.Register("LayerCount", typeof(int), typeof(LUChartRadar), new PropertyMetadata(5, new PropertyChangedCallback(OnLadarPropertyChanged)));

        public SolidColorBrush GridLineBrush
        {
            get { return (SolidColorBrush)GetValue(GridLineBrushProperty); }
            set { SetValue(GridLineBrushProperty, value); }
        }
        public static readonly DependencyProperty GridLineBrushProperty =
            DependencyProperty.Register("GridLineBrush", typeof(SolidColorBrush), typeof(LUChartRadar), new PropertyMetadata(new SolidColorBrush(Colors.Black)));
        public string[] Labels
        {
            get { return (string[])GetValue(LabelsProperty); }
            set { SetValue(LabelsProperty, value); }
        }
        public static readonly DependencyProperty LabelsProperty =
            DependencyProperty.Register("Labels", typeof(string[]), typeof(LUChartRadar), new PropertyMetadata(null));
        public SolidColorBrush LabelsBrush
        {
            get { return (SolidColorBrush)GetValue(LabelsBrushProperty); }
            set { SetValue(LabelsBrushProperty, value); }
        }
        public static readonly DependencyProperty LabelsBrushProperty =
            DependencyProperty.Register("LabelsBrush", typeof(SolidColorBrush), typeof(LUChartRadar), new PropertyMetadata(new SolidColorBrush(Colors.Black)));
        public List<ChartItem> Data
        {
            get { return (List<ChartItem>)GetValue(DataProperty); }
            set { SetValue(DataProperty, value); }
        }
        public static readonly DependencyProperty DataProperty =
            DependencyProperty.Register("Data", typeof(List<ChartItem>), typeof(LUChartRadar), new PropertyMetadata(null, new PropertyChangedCallback(OnLadarPropertyChanged)));
        public string SelectedPointValue
        {
            get { return (string)GetValue(SelectedPointValueProperty); }
            set { SetValue(SelectedPointValueProperty, value); }
        }
        public static readonly DependencyProperty SelectedPointValueProperty =
            DependencyProperty.Register("SelectedPointValue", typeof(string), typeof(LUChartRadar), new PropertyMetadata(""));
        public string SelectedPointLabel
        {
            get { return (string)GetValue(SelectedPointLabelProperty); }
            set { SetValue(SelectedPointLabelProperty, value); }
        }
        public static readonly DependencyProperty SelectedPointLabelProperty =
            DependencyProperty.Register("SelectedPointLabel", typeof(string), typeof(LUChartRadar), new PropertyMetadata(""));
        public string Unit
        {
            get { return (string)GetValue(UnitProperty); }
            set { SetValue(UnitProperty, value); }
        }
        public static readonly DependencyProperty UnitProperty =
            DependencyProperty.Register("Unit", typeof(string), typeof(LUChartRadar), new PropertyMetadata(""));
        /// <summary>
        /// 是否显示图例
        /// </summary>
        public bool IsLegendVisible
        {
            get { return (bool)GetValue(IsLegendVisibleProperty); }
            set { SetValue(IsLegendVisibleProperty, value); }
        }
        public static readonly DependencyProperty IsLegendVisibleProperty =
            DependencyProperty.Register("IsLegendVisible", typeof(bool), typeof(LUChartRadar), new PropertyMetadata(true));
        private Canvas _canvas;
        private Border _canvasBorder;
        private Popup _popup;

        private List<Ellipse> _dataPointList = new List<Ellipse>();
        public struct ValueInfo
        {
            public double Value { get; set; }
            public SolidColorBrush ColorBrush { get; set; }
            public string Label { get; set; }
        }
        public LUChartRadar()
        {
            DefaultStyleKey = typeof(LUChartRadar);
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            _canvas = GetTemplateChild("PART_Canvas") as Canvas;
            _canvasBorder = GetTemplateChild("PART_CanvasBorder") as Border;
            _popup = GetTemplateChild("PART_Popup") as Popup;
            
            _canvas.RenderTransformOrigin = new Point(0.5, 0.5);
            _canvas.RenderTransform = new RotateTransform()
            {
                Angle = -90
            };
            Draw();
        }

        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            base.OnRenderSizeChanged(sizeInfo);
            //_canvas.Width = _canvasBorder.ActualWidth;
            //_canvas.Height = _canvasBorder.ActualHeight;
            //_canvas.RenderTransformOrigin = new Point(0.5, 0.5);
            //_canvas.RenderTransform = new RotateTransform()
            //{
            //    Angle = -90
            //};
            //Draw();
        }
        private void Draw()
        {
            if (_canvas == null) return;

            _canvas.Width = Size;
            _canvas.Height = Size;
            _canvas.Children.Clear();
            UnBindingEvent();
            if (Data == null || Data.Count == 0 || Labels == null || Labels.Length == 0)
            {
                return;
            }
            double size = _canvas.Width;
            int count = Labels.Length;
            double maxValue = Data.Max(x => x.Values.Max());

            //  边长
            double lineWidth = size / 2;

            //  角度
            double angle = (Math.PI * 2) / count;

            var r = lineWidth / count;

            if (count % 2 != 0)
            {
                //  空余的高度
                double freeHeight = lineWidth - lineWidth * Math.Sin((180 - 360 / count) / 2 * Math.PI / 180);
                _canvas.Margin = new Thickness(0, 0, 0, -freeHeight);
            }
            //  多边形边框
            for (int i = 0; i < LayerCount; i++)
            {
                var points = new List<Point>();
                //  当前半径
                var currR = lineWidth / LayerCount * (i + 1);

                for (int j = 0; j < count; j++)
                {
                    points.Add(new Point(lineWidth + currR * Math.Cos(angle * j), lineWidth + currR * Math.Sin(angle * j)));
                }
                //  画出边框
                for (int K = 0; K < points.Count; K++)
                {
                    var line = new Line();
                    line.X1 = points[K].X;
                    line.Y1 = points[K].Y;

                    line.X2 = K == points.Count - 1 ? points[0].X : points[K + 1].X;
                    line.Y2 = K == points.Count - 1 ? points[0].Y : points[K + 1].Y;

                    line.StrokeThickness = i == 0 ? 1 : 1;

                    line.Stroke = GridLineBrush;

                    _canvas.Children.Add(line);

                }
            }

            for (var i = 0; i < count; i++)
            {
                //  顶点连线
                var x = (double)(lineWidth + (lineWidth) * Math.Cos((angle) * i));
                var y = (double)(lineWidth + (lineWidth) * Math.Sin((angle) * i));

                var line = new Line();
                line.X1 = lineWidth;
                line.Y1 = lineWidth;

                line.X2 = x;
                line.Y2 = y;

                line.StrokeThickness = 1;
                line.Stroke = GridLineBrush;

                _canvas.Children.Add(line);

                //  绘制标签
                double labelAngle = (int)((angle * i) / (Math.PI / 180));
                //string t = "";
                //  y轴偏移值
                double offsetY = 0, offsetX = 0;


                var text = new TextBlock();
                text.FontSize = 12;
                text.Foreground = LabelsBrush;
                text.RenderTransform = new RotateTransform()
                {
                    Angle = 90
                };
                text.Text = Labels[i];

                text.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));

                if (labelAngle > 0 && labelAngle < 90 || labelAngle > 270 && labelAngle < 360)
                {
                    offsetX = text.DesiredSize.Height;
                }

                if (labelAngle == 0)
                {
                    //t = "顶点";
                    offsetX = text.DesiredSize.Height;
                    offsetY = -text.DesiredSize.Width / 2;
                }
                else if (labelAngle == 90)
                {
                    offsetX = text.DesiredSize.Height / 2;
                }
                else if (labelAngle == 270)
                {
                    offsetX = text.DesiredSize.Height / 2;
                    offsetY = -text.DesiredSize.Width;
                }
                else if (labelAngle == 180)
                {
                    //t = "底部";
                    offsetY = -text.DesiredSize.Width / 2;
                }
                else if (labelAngle > 0 && labelAngle < 180)
                {
                    //t = "右边";
                }
                else if (labelAngle > 180)
                {
                    //t = "左边";
                    offsetY = -text.DesiredSize.Width;
                }

                var lableX = (double)(lineWidth + ((lineWidth + 5) * Math.Cos((angle) * i)));
                var labelY = (double)(lineWidth + ((lineWidth + 5) * Math.Sin((angle) * i)));

                Canvas.SetLeft(text, lableX + offsetX);
                Canvas.SetTop(text, labelY + offsetY);

                //  y决定左右，x决定上下

                _canvas.Children.Add(text);

                //  绘制数据
                for (int j = 0; j < Data.Count; j++)
                {
                    var item = Data[j];
                    var pc = new PointCollection();
                    var color = item.ColorBrush == null ? new SolidColorBrush(Colors.Black) : item.ColorBrush;

                    for (int k = 0; k < item.Values.Length; k++)
                    {
                        double value = item.Values[k] / maxValue;
                        value = value == 1 ? 0.97 : value;
                        var valueX = (double)(lineWidth + (lineWidth) * Math.Cos((angle) * k) * value);
                        var valueY = (double)(lineWidth + (lineWidth) * Math.Sin((angle) * k) * value);
                        pc.Add(new Point(valueX, valueY));

                        //  响应点
                        var dataPoint = new Ellipse();
                        dataPoint.Width = 10;
                        dataPoint.Height = 10;
                        dataPoint.Fill = new SolidColorBrush(Colors.Transparent);
                        dataPoint.Stroke = new SolidColorBrush(Colors.Transparent);
                        dataPoint.StrokeThickness = 4;
                        dataPoint.MouseEnter += dataPoint_MouseEnter;
                        dataPoint.MouseLeave += dataPoint_MouseLeave;
                        dataPoint.Tag = new ValueInfo()
                        {
                            Value = item.Values[k],
                            Label = item.Label,
                            ColorBrush = color
                        };
                        Canvas.SetZIndex(dataPoint, 100);
                        Canvas.SetLeft(dataPoint, valueX - dataPoint.Width / 2);
                        Canvas.SetTop(dataPoint, valueY - dataPoint.Width / 2);
                        _canvas.Children.Add(dataPoint);
                    }
                    var p = new Polygon();
                    p.Stroke = color;
                    p.Fill = new SolidColorBrush(color.Color) { Opacity = .1 };
                    p.StrokeThickness = 1;
                    p.HorizontalAlignment = HorizontalAlignment.Left;
                    p.VerticalAlignment = VerticalAlignment.Center;
                    p.Points = pc;
                    p.StrokeLineJoin = PenLineJoin.Round;
                    _canvas.Children.Add(p);
                }
            }

        }
        private void UnBindingEvent()
        {
            _dataPointList.ForEach(x =>
            {
                x.MouseEnter -= dataPoint_MouseEnter;
                x.MouseLeave -= dataPoint_MouseLeave;
            });

        }
        private void dataPoint_MouseLeave(object sender, MouseEventArgs e)
        {
            var responsePointArea = (Ellipse)sender;
            responsePointArea.Fill = new SolidColorBrush(Colors.Transparent);
            responsePointArea.Stroke = new SolidColorBrush(Colors.Transparent);
            _popup.IsOpen = false;
        }

        private void dataPoint_MouseEnter(object sender, MouseEventArgs e)
        {
            var responsePointArea = (Ellipse)sender;
            var valueInfo = (ValueInfo)responsePointArea.Tag;
            responsePointArea.Opacity = 1;
            responsePointArea.Fill = valueInfo.ColorBrush;
            var strokeBrush = valueInfo.ColorBrush.CloneCurrentValue();
            strokeBrush.Opacity = 0.3;
            responsePointArea.Stroke = strokeBrush;
            responsePointArea.StrokeThickness = responsePointArea.ActualWidth / 2;


            SelectedPointValue = valueInfo.Value.ToString("0.#");
            SelectedPointLabel = valueInfo.Label;
            //  计算高度
            var popupBorder = GetTemplateChild("PART_Popup_Border") as Border;
            popupBorder.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
            _popup.Placement = PlacementMode.MousePoint;
            _popup.PlacementTarget = responsePointArea;
            _popup.VerticalOffset = -popupBorder.RenderSize.Height - popupBorder.Margin.Top;
            _popup.IsOpen = true;
        }
    }
}
