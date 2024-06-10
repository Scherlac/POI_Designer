using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserInterfaces;

namespace POI_Designer.Model
{
    public class Point: NotifyPropertyChangedBase
    {
        // X coordinate
        [Description("X coordinate")]
        [Display("_X", "X coordinate", 160)]
        [Category("Base")]
        public double X { get => x; set => SetField(ref x, value); }
        private double x;


        // Y coordinate
        [Description("Y coordinate")]
        [Display("_Y", "Y coordinate", 160)]
        [Category("Base")]
        public double Y { get => y; set => SetField(ref y, value); }
        private double y;
    }
}
