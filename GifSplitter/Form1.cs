using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
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

        GifSplitter g = null;
        private void button1_Click(object sender, EventArgs e)
        {
            g = new GifSplitter(textBox_sorgente.Text);
            g.ProgressChange += G_ProgressChange;
            g.StateChange += G_StateChange;

            String path=Directory.GetParent(textBox_sorgente.Text).FullName;
            String FileName = Path.GetFileNameWithoutExtension(textBox_sorgente.Text);

            if((TipoOut)comboBox2.SelectedItem== TipoOut.File_Separati)
            {
                String PathOut = Path.Combine(path, FileName);
                g.Split(PathOut, FileName,(FormatoOut)comboBox1.SelectedItem);
            }
            else
            {
                String OutFile=Path.Combine(path, FileName + "." + (FormatoOut)comboBox1.SelectedItem);
                using (StreamWriter sw = new StreamWriter(OutFile))
                {
                    g.Split(sw, (FormatoOut)comboBox1.SelectedItem);
                }

            }

        }

        private void G_StateChange(GifSplitter This,GifSplitter.State state)
        {
            if(state==GifSplitter.State.InWorking)
            {
                this.Enabled = false;
                progressBar1.Maximum = This.FrameCount;
                
            }
            else if(state == GifSplitter.State.Finish)
            {
                this.Enabled = true;
                progressBar1.Value = progressBar1.Maximum;
            }

        }

        private void G_ProgressChange(GifSplitter This, int Now, int Total)
        {
            progressBar1.Value = Now+1;
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




    public class GifSplitter
    {
        public int FrameCount
        {
            get
            {
                return frameCount;
            }
        }


        Size OriginalSize,FinalSize;
        int Colonne, Righe;
        int frameCount;

        Image gifImg;
        FrameDimension dimension;

        public GifSplitter(String Path)
        {
            gifImg = Image.FromFile(Path);
            dimension = new FrameDimension(gifImg.FrameDimensionsList[0]);
            frameCount = gifImg.GetFrameCount(dimension);


            CalcolaRigheColonne(frameCount);
            SetImageSize(gifImg.Size);

        }


        private void CalcolaRigheColonne(int NumeroFrame)
        {
            //Calcolo le Colonne
            Colonne = 10;

            //Calcolo le righe
            Righe = (NumeroFrame / Colonne) + 1;
        }
        private void SetImageSize(Size OriginalSize)
        {
            this.OriginalSize = OriginalSize;
            FinalSize = new Size(Colonne * OriginalSize.Width, Righe * OriginalSize.Height);
        }

        private Point GetPointToPrint(int r, int c)
        {
            return new Point(c * OriginalSize.Width, r * OriginalSize.Height);
        }

        public void Split(String PathOut,String OriginalName, FormatoOut FormatoOutput = FormatoOut.jpg)
        {
            StateChange?.Invoke(this, State.InWorking);


            if (!Directory.Exists(PathOut))
                Directory.CreateDirectory(PathOut);

            Bitmap b = new Bitmap(FinalSize.Width, FinalSize.Height);

            Graphics g = Graphics.FromImage(b);

           

            ImageFormat format;
            if (FormatoOutput == FormatoOut.jpg)
                format = ImageFormat.Jpeg;
            else if (FormatoOutput == FormatoOut.png)
                format = ImageFormat.Png;
            else if (FormatoOutput == FormatoOut.bmp)
                format = ImageFormat.Bmp;
            else
                format = ImageFormat.Jpeg;


            String Ext = FormatoOutput.ToString();



            for (int i = 0; i < frameCount; i++)
            {
                ProgressChange?.Invoke(this, i, frameCount);
                gifImg.SelectActiveFrame(dimension, i);
                gifImg.Save(Path.Combine(PathOut, OriginalName +"_"+i+ "." + Ext), format);   
            }

            StateChange?.Invoke(this, State.Finish);

        }

        public void Split(StreamWriter sw ,FormatoOut FormatoOutput =FormatoOut.jpg)
        {
            StateChange?.Invoke(this,State.InWorking);


            Bitmap b = new Bitmap(FinalSize.Width, FinalSize.Height);

            Graphics g = Graphics.FromImage(b);

            int r = 0, c = 0;

            for (int i = 0; i < frameCount; i++)
            {
                ProgressChange?.Invoke(this,i, frameCount);

                gifImg.SelectActiveFrame(dimension, i);
                g.DrawImage(gifImg, GetPointToPrint(r, c));

                c++;
                if(c>=Colonne)
                {
                    c = 0;
                    r++;
                }

                if(r>=Righe)
                {
                    break;
                }

            }
            if (FormatoOutput  == FormatoOut.jpg)
                b.Save(sw.BaseStream, ImageFormat.Jpeg);
            else if (FormatoOutput  == FormatoOut.png)
                b.Save(sw.BaseStream, ImageFormat.Png);
            else if (FormatoOutput  == FormatoOut.bmp)
                b.Save(sw.BaseStream, ImageFormat.Bmp);


            StateChange?.Invoke(this,State.Finish);
        }




        public delegate void ProgressChangeEventHandler(GifSplitter This, int Now,int Total);
        public delegate void StateChangeEventHandler(GifSplitter This, State state);

        public event ProgressChangeEventHandler ProgressChange;
        public event StateChangeEventHandler StateChange;


        public enum State
        {
            NotWorking,
            InWorking,
            Finish
        }
    }



    public enum FormatoOut
    {
        png,
        jpg,
        bmp
    }
    public enum TipoOut
    {
        File_Separati,
        File_Unico
    }


}
