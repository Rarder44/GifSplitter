using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GifSplitter
{
    static class Common
    {
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


    }

    public delegate void ProgressChangeEventHandler<t>(t This, int Now, int Total);
    public delegate void StateChangeEventHandler<t>(t This, State state);
    public enum State
    {
        NotWorking,
        InWorking,
        Finish
    }

}
