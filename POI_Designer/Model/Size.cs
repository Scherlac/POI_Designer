using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        public double Width { get; set; }

        // Height
        [Description("Height")]
        [Display("_Height", "Height of the bounding box", 160)]
        [Category("Base")]

        public double Height { get; set; }
    }
}
