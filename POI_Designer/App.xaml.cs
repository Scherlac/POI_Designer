using POI_Designer.Model;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Globalization;
using System.Text.Json;
using System.Windows;
using System.Windows.Data;

namespace POI_Designer
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        // The Video Item
        private VideoItem store;

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            // Load POI data from a json file 
            // { "Center": { "X": 100, "Y": 100 }, "Size": { "Width": 50, "Height": 50 } }
            // this.store = App.Load("POI.json");


        }



        /// <summary>
        /// Load Video Item from a json file
        /// </summary>
        /// <param name="jsonFile">The json file</param>
        /// <returns>The Video Item</returns>
        public static VideoItem Load(string jsonFile)
        {
            // load using json deserializer text.json
            var json = System.IO.File.ReadAllText(jsonFile);
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            var videoItem = JsonSerializer.Deserialize<VideoItem>(json, options);
            return videoItem;

        }

        /// <summary>
        /// Save Video Item to a json file
        /// </summary>
        /// <param name="jsonFile">The json file</param>
        /// <param name="videoItem">The Video Item</param>
        public void Save(string jsonFile, VideoItem videoItem)
        {
            // save using json serializer text.json
            var json = JsonSerializer.Serialize(videoItem);
            System.IO.File.WriteAllText(jsonFile, json);
        }



    }


    [ValueConversion(typeof(object), typeof(String))]
    public class DescriptionConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var type = parameter.GetType();
            var attr = type.GetCustomAttributes(typeof(DescriptionAttribute), true).FirstOrDefault() as DescriptionAttribute;
            return attr?.Description ?? "";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

}
