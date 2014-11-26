using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using QuickGraph;
using System.Xml;

namespace NetworkModelViewer
{
    public partial class ModelViewer : UserControl
    {
        private IBidirectionalGraph<object, IEdge<object>> model;
        public IBidirectionalGraph<object, IEdge<object>> ShowModel
        {
            get { return model; }
        }

        public ModelViewer()
        {
        }

        public void setModel(BidirectionalGraph<object, IEdge<object>> m)
        {
            model = m;
        }

    }
}
