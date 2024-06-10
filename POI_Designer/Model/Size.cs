using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;
using UserInterfaces;

namespace POI_Designer.Model
{
    // The size class
    public class Size: NotifyPropertyChangedBase
    {
        // Width
        [Description("Width")]
        [Display("_Width", "Width of the bounding box", 160)]
        [Category("Base")]
        public double Width { get => width; set => SetField(ref width, value); }
        private double width;

        // Height
        [Description("Height")]
        [Display("_Height", "Height of the bounding box", 160)]
        [Category("Base")]

        public double Height { get => height; set => SetField(ref height, value); }
        private double height;
    }
}
