// Copyright (c) 2010-11 Gishu Pillai
// See the file license.txt for copying permission

﻿using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Shapes;

namespace ManiaX.Test.Beacons.Views
{
    /// <summary>
    /// Interaction logic for TextInAShape.xaml
    /// </summary>
    public partial class TextInAShape : UserControl
    {
        public TextInAShape()
        {
            InitializeComponent();
            Refresh(ContainerShape, hostCanvas);
        }



        #region DPs
        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Text.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(TextInAShape), new UIPropertyMetadata(""));

        public BoundingShape ContainerShape
        {
            get { return (BoundingShape)GetValue(ContainerShapeProperty); }
            set { SetValue(ContainerShapeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ContainerShape.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ContainerShapeProperty =
            DependencyProperty.Register("ContainerShape", typeof(BoundingShape), typeof(TextInAShape), 
                                            new UIPropertyMetadata(BoundingShape.Rectangle, ChangeContainerShape));



        public Brush Stroke
        {
            get { return (Brush)GetValue(StrokeProperty); }
            set { SetValue(StrokeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Stroke.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty StrokeProperty =
            DependencyProperty.Register("Stroke", typeof(Brush), typeof(TextInAShape), 
                                            new UIPropertyMetadata(new SolidColorBrush(Colors.Black)));



        public Brush Fill
        {
            get { return (Brush)GetValue(FillProperty); }
            set { SetValue(FillProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Fill.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FillProperty =
            DependencyProperty.Register("Fill", typeof(Brush), typeof(TextInAShape), 
                                            new UIPropertyMetadata(new SolidColorBrush(Colors.Transparent)));


        #endregion

        private static void ChangeContainerShape(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Refresh((BoundingShape)e.NewValue, (d as TextInAShape).hostCanvas);
        }

        private static void Refresh(BoundingShape newShape, Canvas container)
        {
            Shape shape;
            switch (newShape)
            {
                case BoundingShape.Rectangle:
                    shape = new Rectangle();
                    break;

                default:
                    shape = new Ellipse();
                    break;
            }
            BindShapeProperty(shape, HeightProperty, container, "ActualHeight");
            BindShapeProperty(shape, WidthProperty, container, "ActualWidth");
            BindShapeProperty(shape, Shape.FillProperty, container.Parent, "Fill");
            BindShapeProperty(shape, Shape.StrokeProperty, container.Parent, "Stroke");

            if (container.Children[0] is Shape)
            {
                container.Children.RemoveAt(0);
            }
            container.Children.Insert(0, shape);
        }

        private static void BindShapeProperty(Shape shape, DependencyProperty dependencyProperty, object sourceObject, string bindingPath)
        {
            var heightBinding = new Binding(bindingPath) { Source = sourceObject };
            shape.SetBinding(dependencyProperty, heightBinding);
        }


    }

    public enum BoundingShape
    {
        Rectangle,
        Circle
    }
}
