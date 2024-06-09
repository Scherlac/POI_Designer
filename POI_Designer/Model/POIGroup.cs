using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserInterfaces;

namespace POI_Designer.Model
{
    public class POIGroup: NotifyPropertyChangedBase
    {
        public POIGroup()
        {
            POIs = new ObservableCollection<POI>();
            //POIs.Add(new POI());
            Center = new Point();
        }

        // Conter of the POI group
        [Description("Center of the POI group")]
        [Display("_Center of POIs", "Center of the POI group", 320)]
        [Category("Base")]
        public Point Center { get; set; }


        // List of POIs
        [Description("List of POIs")]
        [Display("_List of POIs", "The List of POIs", 480)]
        [Category("Base2")]
        public ObservableCollection<POI> POIs { get; set; }

    }
}
