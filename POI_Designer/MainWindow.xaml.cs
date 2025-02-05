﻿using POI_Designer.Model;
using System.ComponentModel;
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
using UserInterfaces; 

namespace POI_Designer
{


    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Store store;
        private List<object> viewModel;

        public MainWindow()
        {
            InitializeComponent();
            var poi = new Model.POI() {
                Center = new Model.Point() { X = 100, Y = 100 },
                Size = new Model.Size() { Width = 50, Height = 50 }
            };
            this.store = new Model.Store();

            // Generate the view model
            var items = new List<object>()
            {
                this.store
            };

            items.AddRange(this.store.VideoItems);

            viewModel = DisplayClass.Assign(items);

            this.Editor.SetDC(viewModel);
            //this.POIGroup.DataContext = this.store.VideoItems[this.store.SelectedVideoIndex].POIGroup;
            this.categoryDataGrid.DataContext = this.store.VideoItems[this.store.SelectedVideoIndex].POIGroup.POIs;

            this.Screen.DataContext = this.store.VideoItems[this.store.SelectedVideoIndex].POIGroup.POIs;
            var toolbarCommands = new List<CommandType>() {
                new CommandType("", "Add a new item to the list.", "Images/112_Plus_Green_16x16_72.png", (c) =>
                {
                    //AddItem(1);
                }),
                new CommandType("", "Remove the selected item from the list.", "Images/112_Minus_Orange_16x16_72.png", (c) =>
                {
                    //RemoveItem();
                }),
                new CommandType("", "Move up the selected item.", "Images/112_UpArrowShort_Blue_16x16_72.png", c =>
                {
                    //MoveItem(-1);
                }),
                new CommandType("", "Move down the selected item.", "Images/112_DownArrowShort_Blue_16x16_72.png", c =>
                {
                    //MoveItem(+1);
                }),

                null
                };

            this.Toolbar.DataContext = toolbarCommands;

        }

        private void Canvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            //this.Canvas.
            var canvas = sender as IInputElement ?? this.Screen;

            var poi = new Model.POI()
            {
                
                Center = new Model.Point() { X = e.GetPosition(canvas).X, Y = e.GetPosition(canvas).Y },
                Size = new Model.Size() { Width = 50, Height = 50 }
            };


            this.store.VideoItems[this.store.SelectedVideoIndex].POIGroup.POIs.Add(poi);

        }

        private void Canvas_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            //var poi = new Model.POI()
            //{
            //    Center = new Model.Point() { X = e.GetPosition(this).X, Y = e.GetPosition(this).Y },
            //    Size = new Model.Size() { Width = 50, Height = 50 }
            //};
            //this.Editor.SetDC(DisplayClass.Assign(new DemoGUI()));
        }

        private void Canvas_MouseMove(object sender, MouseEventArgs e)
        {
            //var poi = new Model.POI()
            //{
            //    Center = new Model.Point() { X = e.GetPosition(this).X, Y = e.GetPosition(this).Y },
            //    Size = new Model.Size() { Width = 50, Height = 50 }
            //};
            //this.Editor.SetDC(DisplayClass.Assign(new DemoGUI()));
        }



    }
}