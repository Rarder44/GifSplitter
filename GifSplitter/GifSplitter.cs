using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GifSplitter
{
    public class GifSplitter
    {

        public ImageCollection Split(String Path)
        {
            StateChange?.Invoke(this, State.InWorking);
            Image gifImg = Image.FromFile(Path);
            FrameDimension dimension = new FrameDimension(gifImg.FrameDimensionsList[0]);
            int frameCount = gifImg.GetFrameCount(dimension);

            ImageCollection ic = new ImageCollection();

            for (int i = 0; i < frameCount; i++)
            {
                ProgressChange?.Invoke(this, i, frameCount);
                gifImg.SelectActiveFrame(dimension, i);
                ic.Collection.Add((Image)gifImg.Clone());
            }

            StateChange?.Invoke(this, State.Finish);

            gifImg.Dispose();
            return ic;

        }

        public event ProgressChangeEventHandler<GifSplitter> ProgressChange;
        public event StateChangeEventHandler<GifSplitter> StateChange;

    }



   

}
