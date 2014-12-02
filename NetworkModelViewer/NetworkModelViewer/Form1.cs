using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Forms;
using QuickGraph;
using System.Xml;
using GraphX.Controls;
using GraphX.GraphSharp.Algorithms.Layout.Simple.FDP;
using GraphX.GraphSharp.Algorithms.OverlapRemoval;
using GraphX.Logic;
using System.Windows.Media.Imaging;

namespace NetworkModelViewer
{
    public partial class Form1 : Form
    {
        BidirectionalGraph<object, IEdge<object>> g = new BidirectionalGraph<object, IEdge<object>>();

        List<DataEdge> links = new List<DataEdge>();
        //List<DataVertex> nodes = new List<DataVertex>();
        List<string> nodeNames = new List<string>();

        private ZoomControl zoomctrl;
        private GraphAreaExample gArea;
        private GraphExample dataGraph = new GraphExample();

        public Form1()
        {
            InitializeComponent();
            loadConfiguration(@"Config/NetworkTopology.xml");
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            openFileDialog.ShowDialog();
        }

        private void openFileDialog_FileOk(object sender, CancelEventArgs e)
        {
            loadConfiguration(openFileDialog.FileName);
        }

        public void readTopology(string fileName)
        {
            XmlDocument xml = new XmlDocument();
            xml.Load(fileName);
            readLinks(xml);
        }

        public void readLinks(XmlDocument xml)
        {
            foreach (XmlNode xnode in xml.SelectNodes("//Link[@ID]"))
            {
                string id = xnode.Attributes["ID"].Value;
                string srcId = xnode.Attributes["SrcID"].Value;
                string dstId = xnode.Attributes["DstID"].Value;
                string srcPortId = xnode.Attributes["SrcPortID"].Value;
                string dstPortId = xnode.Attributes["DstPortID"].Value;

                var srcNode = new DataVertex();
                srcNode.Text = srcId;
                if (srcId.Contains("NetworkNode"))
                    srcNode.DataImage = new BitmapImage(new Uri(@"pack://application:,,,/NetworkModelViewer;component/Images/node.png", UriKind.Absolute)) { CacheOption = BitmapCacheOption.OnLoad };
                else if(srcId.Contains("ClientNode"))
                    srcNode.DataImage = new BitmapImage(new Uri(@"pack://application:,,,/NetworkModelViewer;component/Images/client.png", UriKind.Absolute)) { CacheOption = BitmapCacheOption.OnLoad };
                if (!nodeNames.Contains(srcId))
                {
                    dataGraph.AddVertex(srcNode);
                    nodeNames.Add(srcId);
                }

                var dstNode = new DataVertex();
                dstNode.Text = dstId;
                if (dstId.Contains("NetworkNode"))
                    dstNode.DataImage = new BitmapImage(new Uri(@"pack://application:,,,/NetworkModelViewer;component/Images/node.png", UriKind.Absolute)) { CacheOption = BitmapCacheOption.OnLoad };
                else if (dstId.Contains("ClientNode"))
                    dstNode.DataImage = new BitmapImage(new Uri(@"pack://application:,,,/NetworkModelViewer;component/Images/client.png", UriKind.Absolute)) { CacheOption = BitmapCacheOption.OnLoad };
                if (!nodeNames.Contains(dstId))
                {
                    dataGraph.AddVertex(dstNode);
                    nodeNames.Add(dstId);
                }
                var vlist = dataGraph.Vertices.ToList();
                int src = 0;
                int dst = 0;
                for (int i=0; i<vlist.Count; i++)
                {
                    if (vlist[i].Text == srcId)
                        src = i;
                    if (vlist[i].Text == dstId)
                        dst = i;
                }
                var link = new DataEdge(vlist[src], vlist[dst]);
                dataGraph.AddEdge(link);
            }
        }

        private UIElement GenerateWpfVisuals()
        {
            zoomctrl = new ZoomControl();
            ZoomControl.SetViewFinderVisibility(zoomctrl, System.Windows.Visibility.Collapsed);
            /* ENABLES WINFORMS HOSTING MODE --- >*/
            var logic = new GXLogicCore<DataVertex, DataEdge, BidirectionalGraph<DataVertex, DataEdge>>();
            gArea = new GraphAreaExample() { EnableWinFormsHostingMode = true, LogicCore = logic };
            logic.Graph = dataGraph;
            logic.DefaultLayoutAlgorithm = GraphX.LayoutAlgorithmTypeEnum.KK;
            logic.DefaultLayoutAlgorithmParams = logic.AlgorithmFactory.CreateLayoutParameters(GraphX.LayoutAlgorithmTypeEnum.KK);
            ((KKLayoutParameters)logic.DefaultLayoutAlgorithmParams).MaxIterations = 200;
            logic.DefaultOverlapRemovalAlgorithm = GraphX.OverlapRemovalAlgorithmTypeEnum.FSA;
            logic.DefaultOverlapRemovalAlgorithmParams = logic.AlgorithmFactory.CreateOverlapRemovalParameters(GraphX.OverlapRemovalAlgorithmTypeEnum.FSA);
            ((OverlapRemovalParameters)logic.DefaultOverlapRemovalAlgorithmParams).HorizontalGap = 50;
            ((OverlapRemovalParameters)logic.DefaultOverlapRemovalAlgorithmParams).VerticalGap = 50;
            logic.DefaultEdgeRoutingAlgorithm = GraphX.EdgeRoutingAlgorithmTypeEnum.SimpleER;
            logic.AsyncAlgorithmCompute = false;
            logic.EnableParallelEdges = true;
            logic.ParallelEdgeDistance = 10;
            zoomctrl.Content = gArea;
            gArea.RelayoutFinished += gArea_RelayoutFinished;


            var myResourceDictionary = new ResourceDictionary { Source = new Uri("template.xaml", UriKind.Relative) };
            zoomctrl.Resources.MergedDictionaries.Add(myResourceDictionary);

            return zoomctrl;
        }

        void gArea_RelayoutFinished(object sender, EventArgs e)
        {
        }

        private void button3_Click(object sender, EventArgs e)
        {
            gArea.RelayoutGraph();
            zoomctrl.ZoomToFill();
        }

        private void loadConfiguration(string path)
        {
            try
            {
                readTopology(path);
                elementHost1.Child = GenerateWpfVisuals();
                gArea.GenerateGraph(true);
                gArea.SetVerticesDrag(true, true);
                zoomctrl.ZoomToFill();
                button1.Enabled = false;
                button3.Enabled = true;
            }
            catch(Exception)
            { }
        }
        
    }
}
