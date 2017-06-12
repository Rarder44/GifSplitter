using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GifSplitterNameSpace
{
    class ImageCollectionManager
    {
        public int Colonne=10;

        ImageCollection ic;

        public ImageCollectionManager( ImageCollection ic )
        {
            this.ic = ic;
        }


        /// <summary>
        /// Permette di esportare tutte le immagini della collezione in una cartella
        /// </summary>
        /// <param name="PathOut"></param>
        /// <param name="OriginalName"></param>
        /// <param name="FormatoOutput"></param>
        public void Export(String PathOut, String OriginalName, FormatoOut FormatoOutput = FormatoOut.jpg)
        {
            StateChange?.Invoke(this, State.InWorking);


            if (!Directory.Exists(PathOut))
                Directory.CreateDirectory(PathOut);

            if(ic.IsEmpty)
            {
                StateChange?.Invoke(this, State.Finish);
                return;
            }


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


            int current = 0;
            int tot = ic.Collection.Count;
            
            foreach( Image i in ic.Collection)
            {
                ProgressChange?.Invoke(this, current++, tot);
                i.Save(Path.Combine(PathOut, OriginalName + "_" + current + "." + Ext), format);
            }




            StateChange?.Invoke(this, State.Finish);

        }


        /// <summary>
        /// Permette di esportare tutte le immagini in un unico file ( file sprite ); i dati vengono inviati ad uno stream
        /// </summary>
        /// <param name="sw"></param>
        /// <param name="FormatoOutput"></param>
        public void Export(StreamWriter sw, FormatoOut FormatoOutput = FormatoOut.jpg)
        {
            StateChange?.Invoke(this, State.InWorking);

            if (ic.IsEmpty)
            {
                StateChange?.Invoke(this, State.Finish);
                return;
            }

            //Ottengo dimensione reale di un frame
            Size OriginalSize = ic.Collection[0].Size;

            //Ottengo il numero di frame
            int NumeroFrame = ic.Count;



            //Calcolo le righe
            
            int Righe = (int)Math.Ceiling(NumeroFrame / (float)Colonne);

            //Calcolo grandezza immagine finale
            Size FinalSize = new Size(Colonne * OriginalSize.Width, Righe * OriginalSize.Height);


            Bitmap b = new Bitmap(FinalSize.Width, FinalSize.Height);

            Graphics g = Graphics.FromImage(b);

            int r = 0, c = 0;

            int current = 0;

            foreach ( Image i in ic.Collection)
            {
                ProgressChange?.Invoke(this, current++, NumeroFrame);

                //Calcolo il punto di inserimento del frame nell'immagine grande
                Point p= new Point(c * OriginalSize.Width, r * OriginalSize.Height);
                g.DrawImageUnscaled(i,p);
                
                c++;
                if (c >= Colonne)
                {
                    c = 0;
                    r++;
                }

                if (r >= Righe)
                {
                    break;
                }


            }


            if (FormatoOutput == FormatoOut.jpg)
                b.Save(sw.BaseStream, ImageFormat.Jpeg);
            else if (FormatoOutput == FormatoOut.png)
                b.Save(sw.BaseStream, ImageFormat.Png);
            else if (FormatoOutput == FormatoOut.bmp)
                b.Save(sw.BaseStream, ImageFormat.Bmp);



            g.Dispose();
            b.Dispose();

            StateChange?.Invoke(this, State.Finish);
        }



        /// <summary>
        /// Permette di esportare tutte le immagini in un unico file ( file sprite );
        /// </summary>
        /// <param name="FileName"></param>
        /// <param name="FormatoOutput"></param>
        public void Export(String FileName, FormatoOut FormatoOutput = FormatoOut.jpg)
        {
            using (StreamWriter sw = new StreamWriter(FileName))
            {
                Export(sw, FormatoOutput);
            }
        }



        public event ProgressChangeEventHandler<ImageCollectionManager> ProgressChange;
        public event StateChangeEventHandler<ImageCollectionManager> StateChange;



        public void SetCollection(ImageCollection ic)
        {
            this.ic = ic;
            
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
