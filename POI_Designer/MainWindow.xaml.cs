using POI_Designer.Model;
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

    public class DemoGUI : NotifyPropertyChangedBase, IConfiguration
    {

        public DemoGUI()
        {
            TheList = new List<string>();
            TheList2 = new List<POI>();
        }

        //private string m_TheLabel;
        [Display("_Label", "This is the label", 160)]
        [Category("Base")]
        public string TheLabel { set; get; }

        [Display("_Mark", "This is the mark the Check", 160)]
        [Category("Base")]
        public bool TheCheckmark { set; get; }

        [Display("_List", "This is the list for me to illustrate how it works", 160)]
        [Category("Base2")]
        public List<string> TheList { set; get; }

        public class MiniObj : NotifyPropertyChangedBase
        {
            public bool m_TheCheckmark1;
            [Display("_Mark1", "This is the mark the Check 1", 160)]
            [Category("Base")]
            public bool TheCheckmark1 { set { SetField(ref m_TheCheckmark1, value); } get { return m_TheCheckmark1; } }

            public bool m_TheCheckmark2;
            [Display("_Mark2", "This is the mark the Check 2", 160)]
            [Category("Base")]
            public bool TheCheckmark2 { set { SetField(ref m_TheCheckmark2, value); } get { return m_TheCheckmark2; } }

        }

        [Display("_List 2", "This is the list 2 for me to illustrate how it works", 160)]
        [Category("Base3")]
        public List<POI> TheList2 { set; get; }

        public CommandTypeList GetCommands(string propertyName)
        {
            return null;
        }
    }


    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private DemoGUI store;

        public MainWindow()
        {
            InitializeComponent();
            var poi = new Model.POI() {
                Center = new Model.Point() { X = 100, Y = 100 },
                Size = new Model.Size() { Width = 50, Height = 50 }
            };
            this.store = new DemoGUI();

            this.Editor.SetDC(DisplayClass.Assign(this.store));
        }

        private void Canvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            //this.Canvas.

            var poi = new Model.POI()
            {

                Center = new Model.Point() { X = e.GetPosition(this).X, Y = e.GetPosition(this).Y },
                Size = new Model.Size() { Width = 50, Height = 50 }
            };

            this.store.TheList2.Add(poi);   

            this.Editor.SetDC(DisplayClass.Assign(this.store));
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