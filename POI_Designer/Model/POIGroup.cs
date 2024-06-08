using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserInterfaces;

namespace POI_Designer.Model
{
    public class POIGroup: NotifyPropertyChangedBase
    {
        // List of POIs
        [Description("List of POIs")]
        [Display("_List of POIs", "The List of POIs", 480)]
        [Category("Base")]
        public List<POI> POIs { get; set; }

        // Conter of the POI group
        [Description("Center of the POI group")]
        [Display("_Center of POIs", "Center of the POI group", 320)]
        [Category("Base")]
        public Point Center { get; set; }
    }
}
