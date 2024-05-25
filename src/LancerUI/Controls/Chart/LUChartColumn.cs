using LancerUI.Controls.Chart.Model;
using LancerUI.Controls.Types;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace LancerUI.Controls.Chart
{
    public class LUChartColumn : Control
    {
        public struct ColumnItem
        {
            public double Value { get; set; }
            public string Label { get; set; }
            public SolidColorBrush ColorBrush { get; set; }
            public string ValueString
            {
                get => Value.ToString("0.#");
            }
        }
        /// <summary>
        /// 最大值
        /// </summary>
        public double MaxValue
        {
            get { return (double)GetValue(MaxValueProperty); }
            set { SetValue(MaxValueProperty, value); }
        }
        public static readonly DependencyProperty MaxValueProperty =
            DependencyProperty.Register("MaxValue", typeof(double), typeof(LUChartColumn), new PropertyMetadata(0.0));
        /// <summary>
        /// 网格线颜色
        /// </summary>
        public SolidColorBrush GridLineBrush
        {
            get { return (SolidColorBrush)GetValue(GridLineBrushProperty); }
            set { SetValue(GridLineBrushProperty, value); }
        }
        public static readonly DependencyProperty GridLineBrushProperty =
            DependencyProperty.Register("GridLineBrush", typeof(SolidColorBrush), typeof(LUChartColumn), new PropertyMetadata(new SolidColorBrush(Colors.Black)));
        /// <summary>
        /// 数据
        /// </summary>
        public List<ChartItem> Data
        {
            get { return (List<ChartItem>)GetValue(DataProperty); }
            set { SetValue(DataProperty, value); }
        }
        public static readonly DependencyProperty DataProperty =
            DependencyProperty.Register("Data", typeof(List<ChartItem>), typeof(LUChartColumn), new PropertyMetadata(null, new PropertyChangedCallback(OnDataPropertyChanged)));

        private static void OnDataPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as LUChartColumn;
            if (e.NewValue != e.OldValue)
            {
                control.Draw();
            }
        }

        /// <summary>
        /// 标签
        /// </summary>
        public string[] Labels
        {
            get { return (string[])GetValue(LabelsProperty); }
            set { SetValue(LabelsProperty, value); }
        }
        public static readonly DependencyProperty LabelsProperty =
            DependencyProperty.Register("Labels", typeof(string[]), typeof(LUChartColumn), new PropertyMetadata(null, new PropertyChangedCallback(OnDataPropertyChanged)));
        /// <summary>
        /// 数值单位
        /// </summary>
        public string Unit
        {
            get { return (string)GetValue(UnitProperty); }
            set { SetValue(UnitProperty, value); }
        }
        public static readonly DependencyProperty UnitProperty =
            DependencyProperty.Register("Unit", typeof(string), typeof(LUChartColumn), new PropertyMetadata(""));
        /// <summary>
        /// 标签颜色
        /// </summary>
        public SolidColorBrush LabelsBrush
        {
            get { return (SolidColorBrush)GetValue(LabelsBrushProperty); }
            set { SetValue(LabelsBrushProperty, value); }
        }
        public static readonly DependencyProperty LabelsBrushProperty =
            DependencyProperty.Register("LabelsBrush", typeof(SolidColorBrush), typeof(LUChartColumn), new PropertyMetadata(new SolidColorBrush(Colors.Black)));
        /// <summary>
        /// 显示样式
        /// </summary>
        public ChartColumnType DisplayType
        {
            get { return (ChartColumnType)GetValue(DisplayTypeProperty); }
            set { SetValue(DisplayTypeProperty, value); }
        }
        public static readonly DependencyProperty DisplayTypeProperty =
            DependencyProperty.Register("DisplayType", typeof(ChartColumnType), typeof(LUChartColumn), new PropertyMetadata(ChartColumnType.Combine,new PropertyChangedCallback(OnDataPropertyChanged)));
        /// <summary>
        /// 刻度定位线颜色
        /// </summary>
        public SolidColorBrush ScalePositionLineBrush
        {
            get { return (SolidColorBrush)GetValue(ScalePositionLineBrushProperty); }
            set { SetValue(ScalePositionLineBrushProperty, value); }
        }
        public static readonly DependencyProperty ScalePositionLineBrushProperty =
            DependencyProperty.Register("ScalePositionLineBrush", typeof(SolidColorBrush), typeof(LUChartColumn), new PropertyMetadata(new SolidColorBrush(Colors.Black)));
        /// <summary>
        /// 选中列数据源
        /// </summary>
        public object SelectedColumnItemsSource
        {
            get { return (object)GetValue(SelectedColumnItemsSourceProperty); }
            set { SetValue(SelectedColumnItemsSourceProperty, value); }
        }
        public static readonly DependencyProperty SelectedColumnItemsSourceProperty =
            DependencyProperty.Register("SelectedColumnItemsSource", typeof(object), typeof(LUChartColumn), new PropertyMetadata(null));
        /// <summary>
        /// 是否显示图例
        /// </summary>
        public bool IsLegendVisible
        {
            get { return (bool)GetValue(IsLegendVisibleProperty); }
            set { SetValue(IsLegendVisibleProperty, value); }
        }
        public static readonly DependencyProperty IsLegendVisibleProperty =
            DependencyProperty.Register("IsLegendVisible", typeof(bool), typeof(LUChartColumn), new PropertyMetadata(true));

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
        private double _columnMargin;
        private double _columnWidth;
        private double _maxValue;

        private Line _scalePositionLine;
        private bool _isCanScalePositionLineMove = true;

        private Border _scalePositionTextBorder;
        private TextBlock _scalePositionText;

        private List<Rectangle> _responseAreaList = new List<Rectangle>();
        public LUChartColumn()
        {
            DefaultStyleKey = typeof(LUChartColumn);
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

            _canvas.Background = new SolidColorBrush(Colors.Transparent);
        }

        private void CreateScalePositionTextControl()
        {
            _scalePositionTextBorder = new Border();
            _scalePositionTextBorder.Background = new SolidColorBrush(Colors.White);
            _scalePositionTextBorder.BorderBrush = new SolidColorBrush(Colors.Black);
            _scalePositionTextBorder.BorderThickness = new Thickness(1);
            _scalePositionTextBorder.CornerRadius = new CornerRadius(5);
            _scalePositionTextBorder.Padding = new Thickness(5);
            _scalePositionTextBorder.Visibility = Visibility.Hidden;

            _scalePositionText = new TextBlock();
            _scalePositionText.FontSize = 12;
            _scalePositionTextBorder.Child = _scalePositionText;
            _scalePositionTextBorder.SetValue(Canvas.ZIndexProperty, 1);
            _scalePositionTextBorder.SetValue(Canvas.LeftProperty, _drawMaxX);
            _canvas.Children.Add(_scalePositionTextBorder);
        }

        private void Draw()
        {
            if (_canvas != null)
            {
                _canvas.Children.Clear();
            }
            if (_canvas == null || Data == null || Data.Count == 0) return;

            CalculateMaxValue();

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
                        //var maxLengthValue = Data.OrderByDescending(x => x.Values.Max()).First().Values.Max().ToString();
                        var text = new TextBlock();
                        text.Text = _maxValue.ToString();
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
                startX += 5;

                _drawCanvasWidth = _canvas.Width - startX * 2;
                _drawCanvasHeight = _canvas.Height - label.DesiredSize.Height * 2 - 5;

                _drawStartX = startX;
                _drawStartY = label.DesiredSize.Height;

                _drawMaxX = _canvas.Width - _drawStartX;
                _drawMaxY = _canvas.Height - label.DesiredSize.Height - 5;
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
            UnBindingEvent();
            CreateScalePositionTextControl();
            CalculateColumnInfo();
            DrawGridLines();
            DrawLabels();
            DrawYLabels();
            DrawData();
            DrawBorderLines();
            DrawScalePositionLine();
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
        /// 绘制标签
        /// </summary>
        private void DrawLabels()
        {
            if (Labels == null || Labels.Length == 0) return;

            double margin = _drawCanvasWidth / Labels.Length;

            //  计算标签预估宽度
            string maxLengthLabel = Labels.OrderByDescending(x => x.Length).First();
            var preLabelText = new TextBlock
            {
                Text = maxLengthLabel,
                FontSize = 12,
            };
            preLabelText.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));

            //  是否省略部分标签
            double preLabelWidth = preLabelText.DesiredSize.Width * Labels.Length + Labels.Length * 4;
            bool isEllipsis = preLabelWidth >= _drawCanvasWidth;

            //Debug.WriteLine("isEllipsis:" + isEllipsis + ",preLabelText.DesiredSize.Width:" + preLabelText.DesiredSize.Width + ",_drawCanvasWidth:" + _drawCanvasWidth + ",preLabelWidth:" + preLabelWidth + ",margin:" + margin);
            for (int i = 0; i < Labels.Length; i++)
            {
                if (!isEllipsis || (isEllipsis && i % 2 == 0))
                {
                    var text = new TextBlock
                    {
                        Text = Labels[i],
                        FontSize = 12,
                        Foreground = LabelsBrush
                    };
                    text.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                    text.SetValue(Canvas.LeftProperty, i * margin + _drawStartX - text.DesiredSize.Width / 2 + _columnMargin + _columnWidth / 2);
                    text.SetValue(Canvas.TopProperty, _drawMaxY + 5);
                    _canvas.Children.Add(text);
                }
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
                    Text = (_maxValue / (_gridXLines - 1) * i).ToString("0.#"),
                    FontSize = 12,
                    Foreground = LabelsBrush
                };
                text.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                text.SetValue(Canvas.LeftProperty, _drawStartX - text.DesiredSize.Width - 5);
                text.SetValue(Canvas.TopProperty, _drawMaxY - i * margin - text.DesiredSize.Height / 2);
                _canvas.Children.Add(text);
            }
        }

        /// <summary>
        /// 计算最大值
        /// </summary>
        private void CalculateMaxValue()
        {
            if (Data == null || Data.Count == 0) return;

            //  最大值处理
            if (DisplayType == ChartColumnType.Combine)
            {
                //  合并样式
                _maxValue = Data.Max(x => x.Values.Max());
            }
            else
            {
                //  堆叠样式
                double[] values = new double[Data[0].Values.Length];

                for (int i = 0; i < Data.Count; i++)
                {
                    for (int j = 0; j < Data[i].Values.Length; j++)
                    {
                        values[j] += Data[i].Values[j];
                    }
                }
                _maxValue = values.Max();

            }

            if (MaxValue > _maxValue)
            {
                _maxValue = MaxValue;
            }
        }
        /// <summary>
        /// 计算列尺寸间隔数据
        /// </summary>
        private void CalculateColumnInfo()
        {
            //  间距计算公式：平均列宽 ÷ 列数 × 2

            int length = Data[0].Values.Length;
            double avgColumnWidth = _drawCanvasWidth / length;
            _columnMargin = avgColumnWidth / length * 2;
            _columnWidth = avgColumnWidth - _columnMargin * 2;
        }
        /// <summary>
        /// 绘制网格线
        /// </summary>
        private void DrawGridLines()
        {
            int valuesCount = Data[0].Values.Count();
            double margin = _drawCanvasWidth / valuesCount;

            //  绘制垂直线
            for (int i = 1; i < valuesCount + 1; i++)
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

        #region 绘制数据
        private void DrawData()
        {
            _responseAreaList.Clear();
            int length = Data[0].Values.Length;
            int middleZindex = 99;
            for (int i = 0; i < length; i++)
            {
                int maxZIndex = 99, minZIndex = 100;
                double maxValue = 0, minValue = 0;

                var valueBorder = new Border
                {
                    Width = _columnWidth,
                    CornerRadius = new CornerRadius(6, 6, 0, 0),
                    ClipToBounds = true,
                    VerticalAlignment = VerticalAlignment.Bottom
                };
                var valuesContainerGrid = new Grid();
                valueBorder.Child = valuesContainerGrid;

                var clipBorder = new Border()
                {
                    CornerRadius = new CornerRadius(6, 6, 0, 0),
                    Background = new SolidColorBrush(Colors.White),
                };

                var valuesContainer = new StackPanel()
                {
                    ClipToBounds = true,
                    VerticalAlignment = VerticalAlignment.Bottom
                };
                var opacityMask = new VisualBrush()
                {
                    Visual = clipBorder
                };
                valuesContainerGrid.Children.Add(clipBorder);
                if (DisplayType == ChartColumnType.Stack)
                {
                    valuesContainer.OpacityMask = opacityMask;
                    valuesContainerGrid.Children.Add(valuesContainer);
                }
                else
                {
                    valuesContainerGrid.OpacityMask = opacityMask;
                }
                double x = i * (_columnWidth + _columnMargin * 2) + _columnMargin + _drawStartX;
                if (i == 0)
                {
                    x = _drawStartX + _columnMargin;
                }
                valueBorder.SetValue(Canvas.LeftProperty, x);
                valueBorder.SetValue(Canvas.BottomProperty, _canvas.Height - _drawCanvasHeight - _drawStartY);

                _canvas.Children.Add(valueBorder);

                //  响应区域
                var responseArea = new Rectangle
                {
                    Fill = ScalePositionLineBrush,
                    Opacity = 0,
                    Width = _drawCanvasWidth / length,
                    Height = _drawCanvasHeight,
                };

                responseArea.SetValue(Canvas.ZIndexProperty, 999);
                responseArea.SetValue(Canvas.LeftProperty, x - _columnMargin);
                responseArea.SetValue(Canvas.BottomProperty, _canvas.Height - _drawCanvasHeight - _drawStartY);
                responseArea.MouseEnter += ResponseArea_MouseEnter;
                responseArea.MouseLeave += ResponseArea_MouseLeave;

                _responseAreaList.Add(responseArea);
                _canvas.Children.Add(responseArea);

                var colDataList = new List<ColumnItem>();
                for (int j = 0; j < Data.Count; j++)
                {
                    string label = Data[j].Label;
                    var color = Data[j].ColorBrush;
                    if (color == null)
                    {
                        color = new SolidColorBrush(Colors.Black);
                    }
                    double value = Data[j].Values[i];
                    var columnBlock = new Rectangle
                    {
                        Fill = color,
                        Width = _columnWidth,
                        Height = _drawCanvasHeight * value / _maxValue,
                    };

                    if (DisplayType == ChartColumnType.Stack)
                    {
                        valuesContainer.Children.Add(columnBlock);
                    }
                    else
                    {
                        columnBlock.VerticalAlignment = VerticalAlignment.Bottom;
                        valuesContainerGrid.Children.Add(columnBlock);
                        if (value > maxValue)
                        {
                            maxZIndex--;
                            columnBlock.SetValue(Canvas.ZIndexProperty, maxZIndex);
                            maxValue = value;
                        }
                        else if (value < minValue)
                        {
                            minZIndex++;
                            columnBlock.SetValue(Canvas.ZIndexProperty, minZIndex);
                            minValue = value;
                        }
                        else
                        {
                            columnBlock.SetValue(Canvas.ZIndexProperty, middleZindex);
                        }
                    }

                    colDataList.Add(new ColumnItem
                    {
                        Value = value,
                        Label = label,
                        ColorBrush = color
                    });
                }
                responseArea.Tag = colDataList;
            }
        }
        #endregion

        #region 绘制定位线
        private void DrawScalePositionLine()
        {
            _scalePositionLine = new Line
            {
                Stroke = ScalePositionLineBrush,
                X1 = _drawStartX,
                Y1 = _drawStartY,
                X2 = _drawMaxX + 10,
                Y2 = _drawStartY,
                StrokeThickness = 1,
                StrokeDashArray = new DoubleCollection(new double[] { 2, 2 }),
                Visibility = Visibility.Hidden
            };
            _scalePositionLine.SetValue(RenderOptions.EdgeModeProperty, EdgeMode.Aliased);
            _canvas.Children.Add(_scalePositionLine);
        }
        #endregion

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
            _responseAreaList.ForEach(x =>
            {
                x.MouseEnter -= ResponseArea_MouseEnter;
                x.MouseLeave -= ResponseArea_MouseLeave;
            });
        }
        private void ResponseArea_MouseLeave(object sender, MouseEventArgs e)
        {
            var responseArea = sender as Rectangle;
            responseArea.Opacity = 0;
            _popup.IsOpen = false;
        }

        private void ResponseArea_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            var responseArea = sender as Rectangle;
            responseArea.Opacity = .1;
            SelectedColumnItemsSource = responseArea.Tag;
            //  计算高度
            var popupBorder = GetTemplateChild("PART_Popup_Border") as Border;
            popupBorder.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
            _popup.Placement = PlacementMode.MousePoint;
            _popup.VerticalOffset = -popupBorder.RenderSize.Height - popupBorder.Margin.Top;
            _popup.IsOpen = true;
        }

        private void _canvas_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            _scalePositionLine.Visibility = Visibility.Hidden;
            _scalePositionTextBorder.Visibility = Visibility.Hidden;

        }

        private void _canvas_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            _scalePositionLine.Visibility = Visibility.Visible;
            _scalePositionTextBorder.Visibility = Visibility;
        }

        private void _canvas_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            var point = e.MouseDevice.GetPosition(_canvas);

            if (_isCanScalePositionLineMove)
            {
                double y = point.Y - _drawStartY;
                y = point.Y < _drawStartY ? 0 : y;
                y = point.Y > _drawMaxY ? _drawCanvasHeight : y;

                _scalePositionLine.SetValue(Canvas.TopProperty, y);

                _scalePositionTextBorder.SetValue(Canvas.TopProperty, y);
                _scalePositionText.Text = (_maxValue - (_maxValue / _drawCanvasHeight * y)).ToString("0.#");
            }
        }
    }
}