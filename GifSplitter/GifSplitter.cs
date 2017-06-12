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
                ic.Collection.Add(gifImg.CloneFast());

            }

            StateChange?.Invoke(this, State.Finish);

            gifImg.Dispose();
            return ic;

        }

        public event ElementFinishEventHandler<Image> ElementFinish;
        public event ProgressChangeEventHandler<GifSplitter> ProgressChange;
        public event StateChangeEventHandler<GifSplitter> StateChange;


        public void SplitOneByOne(String Path)
        {
            StateChange?.Invoke(this, State.InWorking);
            Image gifImg = Image.FromFile(Path);
            FrameDimension dimension = new FrameDimension(gifImg.FrameDimensionsList[0]);
            int frameCount = gifImg.GetFrameCount(dimension);

            for (int i = 0; i < frameCount; i++)
            {
                ProgressChange?.Invoke(this, i, frameCount);
                gifImg.SelectActiveFrame(dimension, i);

                Image tmp = gifImg.CloneFast();
                ElementFinish?.Invoke(tmp);
                tmp.Dispose();


            }

            StateChange?.Invoke(this, State.Finish);

            gifImg.Dispose();
           

        }

    }



    


}
