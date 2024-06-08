using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using UserInterfaces;


namespace POI_Designer.Model
{
    /// <summary>
    /// Represents a POI (Point of Interest) in a video file
    /// </summary>
    public class POI: NotifyPropertyChangedBase
    {
        public POI()
        {
            Center = new Point();
            Size = new Size();
        }


        /// <summary>
        /// Center coordinate of the POI
        /// </summary>
        [Display("_Center", "Center of the bounding box", 160)]
        [Category("Center")]

        public Point Center { get; set; }

        /// <summary>  
        /// Size of the POI
        /// </summary>
        [Display("_Size", "Size of the bounding box", 160)]
        [Category("Size")]
        public Size Size { get; set; }


    }



}
