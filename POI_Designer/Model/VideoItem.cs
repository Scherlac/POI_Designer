using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POI_Designer.Model
{
    public class VideoItem
    {
        // Path to the video file
        [Description("Path to the video file")]
        public string Path { get; set; }

        // POI group
        [Description("POI group")]
        public POIGroup POIGroup { get; set; }


    }
}
