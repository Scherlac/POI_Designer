using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POI_Designer.Model
{
    public class Store
    {
        // List of video items
        [Description("List of video items")]
        public List<VideoItem> VideoItems { get; set; }

    }
}
