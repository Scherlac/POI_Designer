using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserInterfaces;

namespace POI_Designer.Model
{
    public class Store : NotifyPropertyChangedBase
    {
        public Store()
        {
            VideoItems = new List<VideoItem>();
            VideoItems.Add(new VideoItem());
        }

        [Display("_SelectedVideoIndex", "Selected video index", 160)]
        [Category("Base1")]
        public int SelectedVideoIndex { get; set; }
    



        //// List of video items
        //[Description("List of video items")]
        //[Display("_VideoItems", "List of video items", 160, IDXName: "SelectedVideoIndex"  )]
        //[Category("Base2")]
        public List<VideoItem> VideoItems { get; set; }



    }
}
