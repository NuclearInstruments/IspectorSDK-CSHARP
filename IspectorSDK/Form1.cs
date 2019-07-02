using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using IspectorSDKLib;

using OxyPlot;
using OxyPlot.Series;
using OxyPlot.WindowsForms;

namespace IspectorSDK
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        PlotView myPlot;
        Ispector IS;
        private void Form1_Load(object sender, EventArgs e)
        {

            IS = new Ispector("192.168.50.2");
            Ispector.Channel chs = IS.getChannelStatus();

            

            IS.configureMCA(100, 100, 300, 10, 100, 10, 10, 10, Ispector.BaselineLength.BASELINE_512, Ispector.RunMode.FREE, 0);
            IS.resetSpectrum();
            
            List<int> spectrum = IS.getSpectrum();


            myPlot = new PlotView();
            var myModel = new PlotModel { Title = "Example 1" };
            FunctionSeries fs = new FunctionSeries();
            for (int i =0;i<spectrum.Count;i++)
                fs.Points.Add(new DataPoint(i, spectrum[i]));
            myModel.Series.Add(fs);
            myPlot.Model = myModel;
            //Set up plot for display
            myPlot.Dock = System.Windows.Forms.DockStyle.Fill ;
            myPlot.Location = new System.Drawing.Point(0, 0);
            myPlot.Size = new System.Drawing.Size(500, 500);
            myPlot.TabIndex = 0;

            //Add plot control to form
            Controls.Add(myPlot);

            timer1.Interval = 500;
            timer1.Enabled = true;

            
        }


        private void timer1_Tick(object sender, EventArgs e)
        {
            List<int> spectrum = IS.getSpectrum();
            var myModel = new PlotModel { Title = "Example 1" };
            FunctionSeries fs = new FunctionSeries();
            for (int i = 0; i < spectrum.Count; i++)
                fs.Points.Add(new DataPoint(i, spectrum[i]));
            myModel.Series.Add(fs);
            myPlot.Model = myModel;
        }
    }
}
