using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GifSplitterNameSpace
{
    public partial class NamedProgressBar : UserControl
    {
        int DefaultMax = 100;



        public override String Text
        {
            get
            {
                return label.GetTextInvoke();
            }
            set
            {
                label.SetTextInvoke(value);
            }
        }

        bool _MaxSet = false;
        public bool IsMaxSet
        {
            get
            {
                return _MaxSet;
            }
        }

        public int Maximum
        {
            get
            {
                return progressBar.GetMaximumInvoke();
            }
            set
            {
                _MaxSet = true;
                progressBar.SetMaximumInvoke(value);
            }
        }

        public int Value
        {
            get
            {
                return progressBar.GetValueInvoke();
            }
            set
            {
                progressBar.SetValueNoAnimationInvoke(value);
            }
        }








        public NamedProgressBar()
        {
            InitializeComponent();
        }

       
        public void ResetMaximum()
        {
            progressBar.SetMaximumInvoke(DefaultMax);
            _MaxSet = false;

        }



    }
}
