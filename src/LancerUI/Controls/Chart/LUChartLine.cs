using LancerUI.Controls.Chart.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using static System.Net.Mime.MediaTypeNames;

namespace LancerUI.Controls.Chart
{
    public class LUChartLine : Control
    {
        public struct ValueInfo
        {
            public Point Point { get; set; }
            public double Value { get; set; }
            public SolidColorBrush ColorBrush { get; set; }
            public string Label { get; set; }
        }
        public double MaxValue
        {
            get { return (double)GetValue(MaxValueProperty); }
            set { SetValue(MaxValueProperty, value); }
        }
        public static readonly DependencyProperty MaxValueProperty =
            DependencyProperty.Register("MaxValue", typeof(double), typeof(LUChartLine), new PropertyMetadata(0.0));
        //public int GridXLines
        //{
        //    get { return (int)GetValue(GridXLinesProperty); }
        //    set { SetValue(GridXLinesProperty, value); }
        //}
        //public static readonly DependencyProperty GridXLinesProperty =
        //    DependencyProperty.Register("GridXLines", typeof(int), typeof(LUChartLine), new PropertyMetadata(0));
        public SolidColorBrush GridLineBrush
        {
            get { return (SolidColorBrush)GetValue(GridLineBrushProperty); }
            set { SetValue(GridLineBrushProperty, value); }
        }
        public static readonly DependencyProperty GridLineBrushProperty =
            DependencyProperty.Register("GridLineBrush", typeof(SolidColorBrush), typeof(LUChartLine), new PropertyMetadata(new SolidColorBrush(Colors.Black)));
        public List<ChartLineItem> Data
        {
            get { return (List<ChartLineItem>)GetValue(DataProperty); }
            set { SetValue(DataProperty, value); }
        }
        public static readonly DependencyProperty DataProperty =
            DependencyProperty.Register("Data", typeof(List<ChartLineItem>), typeof(LUChartLine), new PropertyMetadata(null));
        public string[] Labels
        {
            get { return (string[])GetValue(LabelsProperty); }
            set { SetValue(LabelsProperty, value); }
        }
        public static readonly DependencyProperty LabelsProperty =
            DependencyProperty.Register("Labels", typeof(string[]), typeof(LUChartLine), new PropertyMetadata(null));
        public SolidColorBrush PositionLinesBrush
        {
            get { return (SolidColorBrush)GetValue(PositionLinesBrushProperty); }
            set { SetValue(PositionLinesBrushProperty, value); }
        }
        public static readonly DependencyProperty PositionLinesBrushProperty =
            DependencyProperty.Register("PositionLinesBrush", typeof(SolidColorBrush), typeof(LUChartLine), new PropertyMetadata(new SolidColorBrush(Colors.Red)));

        public string SelectedPointValue
        {
            get { return (string)GetValue(SelectedPointValueProperty); }
            set { SetValue(SelectedPointValueProperty, value); }
        }
        public static readonly DependencyProperty SelectedPointValueProperty =
            DependencyProperty.Register("SelectedPointValue", typeof(string), typeof(LUChartLine), new PropertyMetadata(""));
        public string SelectedPointLabel
        {
            get { return (string)GetValue(SelectedPointLabelProperty); }
            set { SetValue(SelectedPointLabelProperty, value); }
        }
        public static readonly DependencyProperty SelectedPointLabelProperty =
            DependencyProperty.Register("SelectedPointLabel", typeof(string), typeof(LUChartLine), new PropertyMetadata(""));
        public string Unit
        {
            get { return (string)GetValue(UnitProperty); }
            set { SetValue(UnitProperty, value); }
        }
        public static readonly DependencyProperty UnitProperty =
            DependencyProperty.Register("Unit", typeof(string), typeof(LUChartLine), new PropertyMetadata(""));
        public SolidColorBrush LabelsBrush
        {
            get { return (SolidColorBrush)GetValue(LabelsBrushProperty); }
            set { SetValue(LabelsBrushProperty, value); }
        }
        public static readonly DependencyProperty LabelsBrushProperty =
            DependencyProperty.Register("LabelsBrush", typeof(SolidColorBrush), typeof(LUChartLine), new PropertyMetadata(new SolidColorBrush(Colors.Black)));


        private Canvas _canvas;
        private Border _canvasBorder;
        private Popup _popup;

        private double _drawStartX;
        private double _drawStartY;
        private double _drawMaxX;
        private double _drawMaxY;
        private double _drawCanvasWidth;
        private double _drawCanvasHeight;
        private int _gridXLines;
        private List<Rectangle> _responsePointAreaList = new List<Rectangle>();
        private bool _isCanMoveXLine = true;

        private Border _yAxisBorder;
        private TextBlock _yAxisText;
        public LUChartLine()
        {
            DefaultStyleKey = typeof(LUChartLine);
            Unloaded += LUChartLine_Unloaded;
        }

        private void LUChartLine_Unloaded(object sender, RoutedEventArgs e)
        {
            Unloaded -= LUChartLine_Unloaded;
            UnBindingEvent();
        }

        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            base.OnRenderSizeChanged(sizeInfo);
            _canvas.Width = _canvasBorder.ActualWidth;
            _canvas.Height = _canvasBorder.ActualHeight;
            Draw();
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            _canvas = GetTemplateChild("PART_Canvas") as Canvas;
            _canvasBorder = GetTemplateChild("CanvasBorder") as Border;
            _popup = GetTemplateChild("PART_Popup") as Popup;
            //_popup.Placement = PlacementMode.Mouse;
            _popup.Placement = PlacementMode.MousePoint;
            _canvas.Background = new SolidColorBrush(Colors.Transparent);
        }

        private void CreateYAxisTooltip()
        {
            _yAxisBorder = new Border();
            _yAxisBorder.Background = new SolidColorBrush(Colors.White);
            _yAxisBorder.BorderBrush = new SolidColorBrush(Colors.Black);
            _yAxisBorder.BorderThickness = new Thickness(1);
            _yAxisBorder.CornerRadius = new CornerRadius(5);
            _yAxisBorder.Padding = new Thickness(5);
            _yAxisBorder.Visibility = Visibility.Hidden;

            _yAxisText = new TextBlock();
            _yAxisText.FontSize = 12;
            _yAxisBorder.Child = _yAxisText;
            _yAxisBorder.SetValue(Canvas.ZIndexProperty, 1);
            _yAxisBorder.SetValue(Canvas.LeftProperty, _drawMaxX);
            _canvas.Children.Add(_yAxisBorder);
        }
        private void Draw()
        {
            if (_canvas == null || Data == null || Data.Count == 0) return;

            UnBindingEvent();

            //  计算绘制范围
            if (Labels == null || Labels.Length == 0)
            {
                //  没有默认标签时
                _drawStartX = 0;
                _drawMaxX = _canvas.Width;
                _drawMaxY = _canvas.Height;
            }
            else
            {
                //  有默认标签时，计算标签宽度
                double valueMaxWidth = 0;
                double labelMaxWidth = 0;

                double startX = 0;
                var maxLengthLabel = Labels.OrderByDescending(x => x.Length).First();
                if (Data != null && Data.Count > 0)
                {
                    if (Data[0].Values.Length > 0)
                    {
                        var maxLengthValue = Data.OrderByDescending(x => x.Values.Max()).First().Values.Max().ToString();
                        var text = new TextBlock();
                        text.Text = maxLengthValue;
                        text.FontSize = 12;
                        //  计算midvaluetext的宽度
                        text.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                        valueMaxWidth = text.DesiredSize.Width;
                    }
                }
                var label = new TextBlock();
                label.Text = maxLengthLabel;
                label.FontSize = 12;
                label.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                labelMaxWidth = label.DesiredSize.Width;

                if (labelMaxWidth / 2 > valueMaxWidth)
                {
                    startX = labelMaxWidth / 2;
                }
                else
                {
                    startX = valueMaxWidth;
                }

                _drawCanvasWidth = _canvas.Width - startX * 2;
                _drawCanvasHeight = _canvas.Height - label.DesiredSize.Height * 2 - 5;

                _drawStartX = startX;
                _drawStartY = label.DesiredSize.Height;

                _drawMaxX = _canvas.Width - _drawStartX;
                _drawMaxY = _canvas.Height - label.DesiredSize.Height - 5;
            }

            //  最大值处理
            double maxValue = Data.Max(x => x.Values.Max());
            if (MaxValue <= 0 || MaxValue < maxValue)
            {
                MaxValue = maxValue;
            }

            //  计算Y轴网格线数
            _gridXLines = (int)(_drawCanvasHeight / 60);
            _gridXLines = _gridXLines < 3 ? 3 : _gridXLines;
            if (_gridXLines % 2 == 0)
            {
                _gridXLines++;
            }

            //  清除画布
            _canvas.Children.Clear();
            CreateYAxisTooltip();
            DrawGridLines();
            DrawDataLines();
            DrawBorderLines();
            DrawLabels();
            DrawYLabels();
            DrawResponseArea();

            BindingEvent();
        }

        /// <summary>
        /// 绘制边框线条
        /// </summary>
        private void DrawBorderLines()
        {
            var lineY = new Line
            {
                Stroke = BorderBrush,
                X1 = _drawStartX,
                Y1 = _drawStartY,
                X2 = _drawStartX,
                Y2 = _drawMaxY,
                StrokeThickness = 1,
                SnapsToDevicePixels = true
            };
            lineY.SetValue(RenderOptions.EdgeModeProperty, EdgeMode.Aliased);
            var lineX = new Line
            {
                Stroke = BorderBrush,
                X1 = _drawStartX,
                Y1 = _drawMaxY,
                X2 = _drawMaxX,
                Y2 = _drawMaxY,
                StrokeThickness = 1,
                SnapsToDevicePixels = true
            };
            lineX.SetValue(RenderOptions.EdgeModeProperty, EdgeMode.Aliased);

            _canvas.Children.Add(lineX);
            _canvas.Children.Add(lineY);
        }

        /// <summary>
        /// 绘制网格线
        /// </summary>
        private void DrawGridLines()
        {
            int valuesCount = Data[0].Values.Count();
            double margin = _drawCanvasWidth / (valuesCount - 1);

            //  绘制垂直线
            for (int i = 1; i < valuesCount; i++)
            {
                var line = new Line
                {
                    Stroke = GridLineBrush,
                    X1 = i * margin + _drawStartX,
                    Y1 = _drawStartY,
                    X2 = i * margin + _drawStartX,
                    Y2 = _drawMaxY,
                    StrokeThickness = 1,
                    StrokeDashArray = new DoubleCollection(new double[] { 2, 2 })
                };
                line.SetValue(RenderOptions.EdgeModeProperty, EdgeMode.Aliased);

                _canvas.Children.Add(line);
            }

            //  绘制水平线
            double xMargin = _drawCanvasHeight / (_gridXLines - 1);
            for (int i = 0; i < _gridXLines - 1; i++)
            {
                var line = new Line
                {
                    Stroke = GridLineBrush,
                    X1 = _drawStartX,
                    Y1 = i * xMargin + _drawStartY,
                    X2 = _drawMaxX,
                    Y2 = i * xMargin + _drawStartY,
                    StrokeThickness = 1,
                    StrokeDashArray = new DoubleCollection(new double[] { 2, 2 })
                };
                line.SetValue(RenderOptions.EdgeModeProperty, EdgeMode.Aliased);

                _canvas.Children.Add(line);
            }
        }

        /// <summary>
        /// 绘制数据线
        /// </summary>
        private void DrawDataLines()
        {
            _responsePointAreaList.Clear();
            int valuesCount = Data[0].Values.Count();
            //double maxValue = Data.Max(x => x.Values.Max());
            double margin = _drawCanvasWidth / (valuesCount - 1);

            for (int i = 0; i < Data.Count; i++)
            {
                var dataColor = Data[i].ColorBrush;
                dataColor = dataColor == null ? new SolidColorBrush(Colors.Black) : dataColor;

                var line = new Polyline
                {
                    Stroke = dataColor,
                    StrokeThickness = 1,
                    Points = new PointCollection(),
                    StrokeLineJoin = PenLineJoin.Round

                };
                var bg = new LinearGradientBrush()
                {
                    StartPoint = new Point(0.5, 0),
                    EndPoint = new Point(0.5, 1),
                };

                var bgColor = Color.FromArgb(dataColor.Color.A, dataColor.Color.R, dataColor.Color.G, dataColor.Color.B);
                bgColor.A = 50;
                bg.GradientStops.Add(new GradientStop(bgColor, 0));
                bg.GradientStops.Add(new GradientStop(Colors.Transparent, 1));

                var bgLayer = new Polyline
                {
                    Fill = bg,
                    Points = new PointCollection(),
                };


                for (int j = 0; j < Data[i].Values.Count(); j++)
                {
                    double value = Data[i].Values[j];
                    double x = j * margin + _drawStartX;
                    double y = _drawCanvasHeight - (value / MaxValue * _drawCanvasHeight) + _drawStartY;

                    //  折线
                    line.Points.Add(new Point(x, y));

                    //  背景
                    bgLayer.Points.Add(new Point(x, y));

                    //var point = new Ellipse()
                    //{
                    //    Width = 5,
                    //    Height = 5,
                    //    Stroke = new SolidColorBrush(Colors.Black),
                    //    StrokeThickness = 2
                    //};
                    //point.SetValue(Canvas.LeftProperty, x);
                    //point.SetValue(Canvas.TopProperty, y);


                    //  响应区域
                    double size = margin / 2 / 2;
                    size = size < 10 ? 10 : size;
                    size = size > 20 ? 20 : size;
                    var responsePointArea = new Rectangle
                    {
                        Fill = new SolidColorBrush(Colors.Transparent),
                        Width = size,
                        Height = size,
                        RadiusX = size,
                        RadiusY = size
                    };
                    responsePointArea.SetValue(Canvas.LeftProperty, x - size / 2);
                    responsePointArea.SetValue(Canvas.TopProperty, y - size / 2);
                    responsePointArea.SetValue(Canvas.ZIndexProperty, 2);
                    responsePointArea.Tag = new ValueInfo
                    {
                        Point = new Point(x, y - _drawStartY),
                        Value = value,
                        ColorBrush = dataColor,
                        Label = Data[i].Label
                    };
                    responsePointArea.MouseEnter += ResponsePointArea_MouseEnter;
                    responsePointArea.MouseLeave += ResponsePointArea_MouseLeave;
                    _canvas.Children.Add(responsePointArea);
                    _responsePointAreaList.Add(responsePointArea);
                }
                bgLayer.Points.Add(new Point(_drawMaxX, _drawMaxY));
                bgLayer.Points.Add(new Point(_drawStartX, _drawMaxY));

                _canvas.Children.Add(line);
                _canvas.Children.Add(bgLayer);
            }
            Debug.WriteLine("最大值：" + MaxValue + "间隔：" + margin + "画布宽高：" + _drawMaxX + "（" + _canvas.ActualWidth + "）x" + _drawMaxY);
        }

        private void ResponsePointArea_MouseLeave(object sender, MouseEventArgs e)
        {
            var responsePointArea = (Rectangle)sender;

            _isCanMoveXLine = true;
            isCanMove = true;
            responsePointArea.Fill = new SolidColorBrush(Colors.Transparent);
            responsePointArea.StrokeThickness = 0;
            _popup.IsOpen = false;
            _yAxisBorder.Visibility = Visibility.Visible;
        }

        private void ResponsePointArea_MouseEnter(object sender, MouseEventArgs e)
        {
            //  鼠标进入响应数据点区域

            var responsePointArea = (Rectangle)sender;
            var valueInfo = (ValueInfo)responsePointArea.Tag;
            Debug.WriteLine("进入:" + valueInfo.Value);
            _isCanMoveXLine = false;
            isCanMove = false;
            responsePointArea.Fill = valueInfo.ColorBrush;
            var strokeBrush = valueInfo.ColorBrush.CloneCurrentValue();
            strokeBrush.Opacity = 0.3;
            responsePointArea.Stroke = strokeBrush;
            responsePointArea.StrokeThickness = responsePointArea.ActualWidth / 2;
            responseXLine.SetValue(Canvas.TopProperty, valueInfo.Point.Y);
            responseYLine.SetValue(Canvas.LeftProperty, valueInfo.Point.X);

            //_yAxisBorder.SetValue(Canvas.TopProperty, valueInfo.Point.Y);
            //_yAxisText.Text = (MaxValue - (MaxValue / _drawCanvasHeight * valueInfo.Point.Y)).ToString("0.#");
            _yAxisBorder.Visibility = Visibility.Hidden;

            SelectedPointValue = valueInfo.Value.ToString("0.#");
            SelectedPointLabel = valueInfo.Label;
            //  计算高度
            var popupBorder = GetTemplateChild("PART_Popup_Border") as Border;
            popupBorder.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
            _popup.Placement = PlacementMode.RelativePoint;
            _popup.PlacementTarget = responsePointArea;
            _popup.VerticalOffset = -popupBorder.RenderSize.Height - popupBorder.Margin.Top;
            _popup.IsOpen = true;
        }

        /// <summary>
        /// 绘制标签
        /// </summary>
        private void DrawLabels()
        {
            if (Labels == null || Labels.Length == 0) return;

            double margin = _drawCanvasWidth / (Labels.Length - 1);
            for (int i = 0; i < Labels.Length; i++)
            {
                var text = new TextBlock
                {
                    Text = Labels[i],
                    FontSize = 12,
                    Foreground = LabelsBrush
                };
                text.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                text.SetValue(Canvas.LeftProperty, i * margin + _drawStartX - text.DesiredSize.Width / 2);
                text.SetValue(Canvas.TopProperty, _drawMaxY + 5);
                _canvas.Children.Add(text);
            }
        }
        /// <summary>
        /// 绘制Y轴刻度
        /// </summary>
        private void DrawYLabels()
        {
            if (Data == null || Data.Count == 0) return;


            double margin = _drawCanvasHeight / (_gridXLines - 1);
            for (int i = 0; i < _gridXLines; i++)
            {
                var text = new TextBlock
                {
                    Text = (MaxValue / (_gridXLines - 1) * i).ToString("0.#"),
                    FontSize = 12,
                    Foreground = LabelsBrush
                };
                text.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                text.SetValue(Canvas.LeftProperty, _drawStartX - text.DesiredSize.Width - 5);
                text.SetValue(Canvas.TopProperty, _drawMaxY - i * margin - text.DesiredSize.Height / 2);
                _canvas.Children.Add(text);
            }
        }

        //绘制响应区域
        private Line responseYLine, responseXLine;
        private bool isCanMove = true;
        private List<Rectangle> responseAreaList = new List<Rectangle>();

        private void DrawResponseArea()
        {
            if (Data == null || Data.Count == 0) return;

            responseAreaList.Clear();
            responseYLine = new Line
            {
                Stroke = PositionLinesBrush,
                X1 = 0,
                Y1 = _drawStartY,
                X2 = 0,
                Y2 = _drawMaxY,
                StrokeThickness = 1,
                StrokeDashArray = new DoubleCollection(new double[] { 2, 2 }),
                Visibility = Visibility.Hidden
            };
            responseYLine.SetValue(RenderOptions.EdgeModeProperty, EdgeMode.Aliased);

            responseXLine = new Line
            {
                Stroke = PositionLinesBrush,
                X1 = _drawStartX,
                Y1 = _drawStartY,
                X2 = _drawMaxX + 10,
                Y2 = _drawStartY,
                StrokeThickness = 1,
                StrokeDashArray = new DoubleCollection(new double[] { 2, 2 }),
                Visibility = Visibility.Hidden
            };
            responseXLine.SetValue(RenderOptions.EdgeModeProperty, EdgeMode.Aliased);
            _canvas.Children.Add(responseYLine);
            _canvas.Children.Add(responseXLine);

            int valuesCount = Data[0].Values.Count();
            double margin = _drawCanvasWidth / (valuesCount - 1);

            for (int i = 0; i < valuesCount; i++)
            {
                var responseArea = new Rectangle
                {
                    Fill = new SolidColorBrush(Colors.Transparent),
                    Width = margin / 2,
                    Height = _drawCanvasHeight
                };

                double x = i * margin + _drawStartX - responseArea.Width / 2;
                responseArea.SetValue(Canvas.LeftProperty, x);
                responseArea.SetValue(Canvas.TopProperty, _drawStartY);
                _canvas.Children.Add(responseArea);

                responseArea.Tag = x + responseArea.Width / 2;
                responseArea.MouseEnter += ResponseArea_MouseEnter;
                responseArea.MouseLeave += ResponseArea_MouseLeave;
                responseAreaList.Add(responseArea);
            }
        }

        private void ResponseArea_MouseLeave(object sender, MouseEventArgs e)
        {
            isCanMove = true;
        }

        private void ResponseArea_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            isCanMove = false;
            responseYLine.SetValue(Canvas.LeftProperty, (double)((Rectangle)sender).Tag);
        }

        private void BindingEvent()
        {
            _canvas.MouseMove += _canvas_MouseMove;
            _canvas.MouseEnter += _canvas_MouseEnter;
            _canvas.MouseLeave += _canvas_MouseLeave;
        }

        private void UnBindingEvent()
        {
            _canvas.MouseMove -= _canvas_MouseMove;
            _canvas.MouseEnter -= _canvas_MouseEnter;
            _canvas.MouseLeave -= _canvas_MouseLeave;
            responseAreaList.ForEach(x =>
            {
                x.MouseEnter -= ResponseArea_MouseEnter;
                x.MouseLeave -= ResponseArea_MouseLeave;
            });
            _responsePointAreaList.ForEach(x =>
            {
                x.MouseEnter -= ResponsePointArea_MouseEnter;
                x.MouseLeave -= ResponsePointArea_MouseLeave;
            });
        }

        private void _canvas_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            responseXLine.Visibility = Visibility.Hidden;
            responseYLine.Visibility = Visibility.Hidden;
            _yAxisBorder.Visibility = Visibility.Hidden;

        }

        private void _canvas_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            responseXLine.Visibility = Visibility.Visible;
            responseYLine.Visibility = Visibility.Visible;
            _yAxisBorder.Visibility = Visibility;
        }

        private void _canvas_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            var point = e.MouseDevice.GetPosition(_canvas);
            if (isCanMove)
            {
                responseYLine.SetValue(Canvas.LeftProperty, point.X);
            }

            if (_isCanMoveXLine)
            {
                double y = point.Y - _drawStartY;
                y = point.Y < _drawStartY ? 0 : y;
                y = point.Y > _drawMaxY ? _drawCanvasHeight : y;

                responseXLine.SetValue(Canvas.TopProperty, y);

                _yAxisBorder.SetValue(Canvas.TopProperty, y);
                _yAxisText.Text = (MaxValue - (MaxValue / _drawCanvasHeight * y)).ToString("0.#");
            }


        }
    }
}
