using System;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Threading.Tasks;

namespace MazeRuner.Panels
{
    public class CustomUniform : Panel
    {
        int _minWide = 75;
        int _Row;
        public int Row
        {
            get => _Row;
            set
            {
                if (value > _Row)
                {
                    int count = value * _Column - _Row * _Column;
                    for (int i = 0; i < count; i++)
                    {
                        var rec = new Rectangle();
                        rec.Fill = Brushes.WhiteSmoke;
                        rec.StrokeThickness = 1;
                        rec.Stroke = Brushes.Black;
                        rec.Width = _minWide;
                        rec.Height = _minWide;
                        Children.Add(rec);
                    }
                    ResetRectColour();
                }
                if (value < _Row)
                {
                    int count = _Row * _Column - value * _Column;
                    for (int i = 0; i < count; i++) Children.RemoveAt(Children.Count - 1);
                    ResetRectColour();
                }
                _Row = value;
            }
        }
        int _Column;
        public int Column
        {
            get => _Column;
            set
            {
                if (value > _Column)
                {
                    int count = value * _Row - _Row * _Column;
                    for (int i = 0; i < count; i++)
                    {
                        var rec = new Rectangle();
                        rec.Fill = Brushes.WhiteSmoke;
                        rec.StrokeThickness = 1;
                        rec.Stroke = Brushes.Black;
                        rec.Width = _minWide;
                        rec.Height = _minWide;
                        Children.Add(rec);
                    }
                    ResetRectColour();
                }
                if (value < _Column)
                {
                    int count = _Row * _Column - value * _Row;
                    for (int i = 0; i < count; i++) Children.RemoveAt(Children.Count - 1);
                    ResetRectColour();
                }
                _Column = value;
            }
        }
        public Brush this[int i,int j]
        {
            get => (Children[i * _Column + j] as Rectangle).Fill;
            set => (Children[i * _Column + j] as Rectangle).Fill = value;
        }
        protected override Size MeasureOverride(Size availableSize) => new Size(_Column*_minWide,_Row*_minWide);
        protected override Size ArrangeOverride(Size finalSize)
        {
            Point p = new Point(0, 0);
            var rectangle = new Rect(p, new Size(_minWide, _minWide));
            for (int i = 0; i < _Row; i++)
                for (int j = 0; j < _Column; j++)
                {
                    rectangle.Location = new Point(j * _minWide, i * _minWide);
                    Children[i * _Column + j].Arrange(rectangle);
                }
            return finalSize;
        }
        void ResetRectColour()
        {
            foreach (var child in Children)
            {
                (child as Rectangle).Fill= Brushes.WhiteSmoke;
            }
        }
    }
}
