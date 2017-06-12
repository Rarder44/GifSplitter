using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GifSplitterNameSpace
{
    static class Common
    {
       [DllImport("msvcrt.dll", CallingConvention=CallingConvention.Cdecl)]
static extern int memcmp(IntPtr b1, IntPtr b2, UIntPtr count);



        public static void SetValueInvoke(this ProgressBar p, int Value)
        {
            if (p.InvokeRequired)
                p.BeginInvoke((MethodInvoker)delegate { p.SetValueInvoke(Value); });
            else
                if (Value <= p.Maximum)
                    p.Value = Value;
        }
        public static void SetMaximumInvoke(this ProgressBar p, int Maximum)
        {
            if (p.InvokeRequired)
                p.BeginInvoke((MethodInvoker)delegate { p.SetMaximumInvoke(Maximum); });
            else
                p.Maximum = Maximum;
        }
        public static int GetMaximumInvoke(this ProgressBar p)
        {
            if (p.InvokeRequired)
                return (int)p.Invoke((Func<int>)delegate { return p.GetMaximumInvoke(); });
            else
                return p.Maximum;
        }
        public static int GetValueInvoke(this ProgressBar p)
        {
            if (p.InvokeRequired)
                return (int)p.Invoke((Func<int>)delegate { return p.GetValueInvoke(); });
            else
                return p.Value;
        }


        public static void SetValueNoAnimation(this ProgressBar p, int value)
        {
            if (value > p.Maximum)
                value = p.Maximum;

            if (value == p.Maximum)
            {
                p.Maximum = value + 1;
                p.Value = value + 1;
                p.Maximum = value;
            }
            else
            {
                p.Value = value + 1;
            }
            p.Value = value;
        }
        public static void SetValueNoAnimationInvoke(this ProgressBar p, int value)
        {
            if (p.InvokeRequired)
                p.BeginInvoke((MethodInvoker)delegate { p.SetValueNoAnimation(value); });
            else
            {
                p.SetValueNoAnimation(value);
            }
        }

        public static void SetEnableInvoke(this Control t, bool b)
        {
            if (t.InvokeRequired)
                t.BeginInvoke((MethodInvoker)delegate { t.SetEnableInvoke(b); });
            else
                t.Enabled = b;
        }

        public static void SetTextInvoke(this Control t, String s)
        {
            if (t.InvokeRequired)
                t.BeginInvoke((MethodInvoker)delegate { t.SetTextInvoke(s); });
            else
                t.Text = s;
        }

        public static String GetTextInvoke(this Control ctrl)
        {
            if (ctrl.InvokeRequired)
                return (String)ctrl.Invoke((Func<String>)delegate { return ctrl.GetTextInvoke(); });
            else
                return ctrl.Text;
        }



        public static Image CloneFast(this Image img)
        {

            int Width = img.Width;
            int Height = img.Height;
            Bitmap bmp = new Bitmap(Width, Height);
            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.DrawImageUnscaled(img, 0, 0);
            }
            return bmp;
        }
        

        public static bool EqualImage(this Image img,Image i2)
        {

            return CompareMemCmp((Bitmap) img, (Bitmap)i2);
        }
        

        public static bool CompareMemCmp(Bitmap b1, Bitmap b2)
        {
            if ((b1 == null) != (b2 == null)) return false;
            if (b1.Size != b2.Size) return false;

            var bd1 = b1.LockBits(new Rectangle(new Point(0, 0), b1.Size), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            var bd2 = b2.LockBits(new Rectangle(new Point(0, 0), b2.Size), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);

            try
            {
                IntPtr bd1scan0 = bd1.Scan0;
                IntPtr bd2scan0 = bd2.Scan0;
                
                int stride = bd1.Stride;
                int len = stride * b1.Height;

                return memcmp(bd1scan0, bd2scan0, new UIntPtr((uint)len)) == 0;
            }
            finally
            {
                b1.UnlockBits(bd1);
                b2.UnlockBits(bd2);
            }
        }
    }

    public delegate void ElementFinishEventHandler<t>(t This);
    public delegate void ProgressChangeEventHandler<t>(t This, int Now, int Total);
    public delegate void StateChangeEventHandler<t>(t This, State state);
    public enum State
    {
        NotWorking,
        InWorking,
        Finish
    }

}
