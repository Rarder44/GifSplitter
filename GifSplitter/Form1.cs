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

namespace GifSplitterNameSpace
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

            if (textBox_sorgente.Text.Trim() == "")
            {
                MessageBox.Show("Inserire immagine valida");
                return;
            }

            if (Worker != null && Worker.IsAlive)
                Worker.Abort();

            Worker = new Thread(ThreadSplit);
            Worker.Start(new ThreadParameter(textBox_sorgente.Text,(TipoOut)comboBox2.SelectedItem, (FormatoOut)comboBox1.SelectedItem));
             
        }


        public void ThreadSplit(object o)
        {
            ThreadParameter tp = (ThreadParameter)o;

            this.SetEnableInvoke(false);
            namedProgressBar1.Text = "Split";
            namedProgressBar2.Text = "Merge";
            namedProgressBar1.ResetMaximum();


            GifSplitter g = new GifSplitter();
            g.ProgressChange += (GifSplitter Self, int Now, int Total) =>
            {
                ProgressChangeGeneral(Self, Now, Total, namedProgressBar1);
            };
            g.StateChange += (GifSplitter This, State state) =>
            {
                StateChangeGeneral(This, state, namedProgressBar1);
            };
        



            ImageCollection ic = g.Split(tp.Sorgente);

            ImageCollectionManager icm = new ImageCollectionManager(ic);
            icm.ProgressChange += (ImageCollectionManager Self, int Now, int Total)=>
            {
                ProgressChangeGeneral(Self, Now, Total, namedProgressBar2);
            };
            icm.StateChange += (ImageCollectionManager This, State state) =>
            {
                StateChangeGeneral(This, state, namedProgressBar2);
            };


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

        


        private void ProgressChangeGeneral(object Self, int Now, int Total,NamedProgressBar n)
        {

            if (!n.IsMaxSet)
            {
                n.Maximum = Total;
            }

            n.Value = Now + 1;
            n.Value = Now + 1;
        }


        private void StateChangeGeneral(object This, State state, NamedProgressBar n)
        {
            if(state==State.Finish)
            {
                namedProgressBar1.Value = namedProgressBar1.Maximum;
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

        private void button3_Click(object sender, EventArgs e)
        {
            if (textBox_sorgente.Text.Trim() == "")
            {
                MessageBox.Show("Inserire immagine valida");
                return;
            }


            if (Worker != null && Worker.IsAlive)
                Worker.Abort();

            Worker = new Thread(ThreadSplitOneByOne);
            Worker.Start(new ThreadParameter(textBox_sorgente.Text, (TipoOut)comboBox2.SelectedItem, (FormatoOut)comboBox1.SelectedItem, (int)Row.Value, (int)Column.Value));

        }

        public void ThreadSplitOneByOne(object o)
        {
            ThreadParameter tp = (ThreadParameter)o;

            this.SetEnableInvoke(false);
            namedProgressBar1.ResetMaximum();
            namedProgressBar2.ResetMaximum();
            namedProgressBar1.Text = "Lettura Frame";
            namedProgressBar2.Text = "Salvataggio Frame";


            GifSplitter g = new GifSplitter();
            g.ProgressChange += (GifSplitter Self, int Now, int Total) =>
            {
                ProgressChangeGeneral(Self, Now, Total, namedProgressBar1);
            };
            g.StateChange += (GifSplitter This, State state) =>
            {
                StateChangeGeneral(This, state, namedProgressBar1);
            };



            String path = Directory.GetParent(tp.Sorgente).FullName;
            String FileName = Path.GetFileNameWithoutExtension(tp.Sorgente);
            String PathOut = Path.Combine(path, FileName);
            if (!Directory.Exists(PathOut))
                Directory.CreateDirectory(PathOut);



            int i = 0;
            int MaxElement = tp.Righe * tp.Colonne;

            ImageCollection ic = new ImageCollection();



            ImageCollectionManager icm = new ImageCollectionManager(ic);
            icm.Colonne = tp.Colonne;

            icm.ProgressChange += (ImageCollectionManager Self, int Now, int Total) =>
            {
                ProgressChangeGeneral(Self, Now, Total, namedProgressBar2);
            };
            icm.StateChange += (ImageCollectionManager This2, State state) =>
            {
                if (state == State.Finish)
                {
                    namedProgressBar2.Value = 0;
                }
            };



            g.ElementFinish += (Image This)=>{

                /*
                 * TODO: se si vuole implementare un ottimizzazione di frame ( controllo se il frame precedente è uguale al successivo
                 * if (ic.Count != 0)
                {
                    if( !This.EqualImage(ic.Collection.Last()))
                        ic.Collection.Add(This.CloneFast());
                }
                else
                    ic.Collection.Add(This.CloneFast());*/

                ic.Collection.Add(This.CloneFast());


                if ( ic.Count>=MaxElement)
                {
                    namedProgressBar2.ResetMaximum();
                    icm.SetCollection(ic);
                    icm.Export(Path.Combine(PathOut, FileName + "_" + i + "." + tp.formatoOut), tp.formatoOut);
                    ic.Dispose();
                    ic = new ImageCollection();
                    i++;
                }

            };
            g.StateChange += (GifSplitter This, State state) => {

                if (state == State.Finish)
                {
                    icm.SetCollection(ic);

                   
                    /*icm.StateChange += (ImageCollectionManager This2, State state2) =>
                    {
                        StateChangeGeneral(This2, state2, namedProgressBar2);
                    };*/
                    icm.Export(Path.Combine(PathOut, FileName + "_" + i + "." + tp.formatoOut), tp.formatoOut);
                    ic.Dispose();
                    this.SetEnableInvoke(true);
                }
            };

            g.SplitOneByOne(tp.Sorgente);

            

        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (Worker != null && Worker.IsAlive)
                Worker.Abort();
        }
    }




    class ThreadParameter
    {
        public TipoOut tipoOut;
        public FormatoOut formatoOut;
        public String Sorgente;

        public int Righe;
        public int Colonne;


        public ThreadParameter(String Sorgente,TipoOut TO, FormatoOut FO)
        {
            this.Sorgente = Sorgente;
            this.tipoOut = TO;
            this.formatoOut = FO;
        }


        public ThreadParameter(String Sorgente, TipoOut TO, FormatoOut FO,int Righe,int Colonne)
        {
            this.Sorgente = Sorgente;
            this.tipoOut = TO;
            this.formatoOut = FO;
            this.Righe = Righe;
            this.Colonne = Colonne;

        }
    }

}
