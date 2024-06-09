using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserInterfaces;

namespace POI_Designer.Model
{
    public class VideoItem : NotifyPropertyChangedBase
    {
        public VideoItem()
        {
            POIGroup = new POIGroup();
        }

        // Path to the video file
        [Description("Path to the video file")]
        [Display("_Path", "Path to the video file", 160)]
        [Category("Base")]
        public string Path { get; set; }

        // POI group
        [Description("POI group")]
        [Display("_POIGroup", "POI group", 160)]
        [Category("Base2")]
        public POIGroup POIGroup { get; set; }


    }
}
