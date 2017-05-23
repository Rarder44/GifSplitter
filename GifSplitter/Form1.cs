using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GifSplitter
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            foreach (FormatoOut t in Enum.GetValues(typeof(FormatoOut)))
            {
                comboBox1.Items.Add(t);
            }
            comboBox1.SelectedItem = FormatoOut.jpg;

            foreach (TipoOut t in Enum.GetValues(typeof(TipoOut)))
            {
                comboBox2.Items.Add(t);
            }
            comboBox2.SelectedItem = TipoOut.File_Unico;
        }

        Thread Worker = null;
        private void button1_Click(object sender, EventArgs e)
        {
            if (Worker != null && Worker.IsAlive)
                Worker.Abort();

            Worker = new Thread(ThreadSplit);
            Worker.Start(new ThreadParameter(textBox_sorgente.Text,(TipoOut)comboBox2.SelectedItem, (FormatoOut)comboBox1.SelectedItem));
             
        }


        public void ThreadSplit(object o)
        {
            ThreadParameter tp = (ThreadParameter)o;

            this.SetEnableInvoke(false);
            MaximumSet = false;


            GifSplitter g = new GifSplitter();
            g.ProgressChange += ProgressChange;
            g.StateChange += StateChange;
            ImageCollection ic = g.Split(tp.Sorgente);

            ImageCollectionManager icm = new ImageCollectionManager(ic);
            icm.ProgressChange += ProgressChange;
            icm.StateChange += StateChange;

            String path = Directory.GetParent(tp.Sorgente).FullName;
            String FileName = Path.GetFileNameWithoutExtension(tp.Sorgente);

            if (tp.tipoOut == TipoOut.File_Separati)
            {
                String PathOut = Path.Combine(path, FileName);
                icm.Export(PathOut, FileName, tp.formatoOut);
            }
            else
            {
                String OutFile = Path.Combine(path, FileName + "." + tp.formatoOut);
                icm.Export(OutFile, tp.formatoOut);
            }


            g = null;
            ic.Dispose();


            this.SetEnableInvoke(true);
        }

        

        bool MaximumSet = false;
        private void ProgressChange(object Self, int Now, int Total)
        {
            progressBar1.SetValueInvoke(Now + 1);
            if (!MaximumSet)
            {
                MaximumSet = true;
                progressBar1.SetMaximumInvoke(Total);
            }
        }
        private void StateChange(object This, State state)
        {
            if(state==State.Finish)
            {
                progressBar1.SetValueInvoke(progressBar1.GetMaximumInvoke());
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            openFileDialog1.Filter = "Animated Image Files (*.gif)|*.gif";
            openFileDialog1.FileName = "";
            if ( openFileDialog1.ShowDialog()==DialogResult.OK)
            {
                textBox_sorgente.Text = openFileDialog1.FileName;
            }
        }
       
    }




    class ThreadParameter
    {
        public TipoOut tipoOut;
        public FormatoOut formatoOut;
        public String Sorgente;

        public ThreadParameter(String Sorgente,TipoOut TO, FormatoOut FO)
        {
            this.Sorgente = Sorgente;
            this.tipoOut = TO;
            this.formatoOut = FO;
        }

    }

}
