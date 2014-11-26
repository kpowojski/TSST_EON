using GraphX;
using System;
using YAXLib;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace NetworkModelViewer
{
    /* DataVertex is the data class for the vertices. It contains all custom vertex data specified by the user.
     * This class also must be derived from VertexBase that provides properties and methods mandatory for
     * correct GraphX operations.
     * Some of the useful VertexBase members are:
     *  - ID property that stores unique positive identfication number. Property must be filled by user.
     *  
     */

    public class DataVertex: VertexBase
    {
        /// <summary>
        /// Some string property for example purposes
        /// </summary>
        public string Text { get; set; }
        public string Name { get; set; }
        public string Profession { get; set; }
        public string Gender { get; set; }
        public int Age { get; set; }

        [YAXDontSerialize]
        public ImageSource DataImage { get; set; }

        [YAXDontSerialize]
        public ImageSource PersonImage { get; set; }
 
        #region Calculated or static props
        [YAXDontSerialize]
        public DataVertex Self
        {
            get { return this; }
        }

        public override string ToString()
        {
            return Text;
        }

        private string[] imgArray = new string[3]
        {
            @"pack://application:,,,/GraphX.Controls;component/Images/help_black.png",
            @"pack://application:,,,/NetworkModelViewer;component/Images/node.png",
            @"pack://application:,,,/NetworkModelViewer;component/Images/client.png",
        };
        private string[] textArray = new string[3]
        {
            @"",
            @"Node",
            @"Client",
        };


        #endregion

        /// <summary>
        /// Default parameterless constructor for this class
        /// (required for YAXLib serialization)
        /// </summary>
        public DataVertex():this("")
        {
        }

        public DataVertex(string text = "")
        {
            var num = 1;
            if (string.IsNullOrEmpty(text)) Text = num == 0 ? text : textArray[num];
            else Text = text;
            DataImage = new BitmapImage(new Uri(imgArray[num], UriKind.Absolute)) { CacheOption = BitmapCacheOption.OnLoad };
        }
    }
}
