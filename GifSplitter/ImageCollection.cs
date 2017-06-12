using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GifSplitterNameSpace
{
    public class ImageCollection : IDisposable
    {
        public List<Image> Collection;

        public ImageCollection()
        {
            Collection = new List<Image>();
        }


        public bool IsEmpty
        {
            get
            {
                return Collection.Count == 0;
            }
        }

        public int Count
        {
            get
            {
                return Collection.Count;
            }
        }

        public void Dispose()
        {
            foreach (Image i in Collection)
                i.Dispose();
        }
    }
}
