using System;
using System.Windows.Forms;

namespace Se7enRedLines.UI.Native
{
    public delegate bool WndProcHandler(object sender, Message msg);

    public class WinProcHandlerForm : Form
    {
        //======================================================
        #region _Public properties_

        public event WndProcHandler WndProcHandler;

        #endregion

        //======================================================
        #region _Private, protected, internal methods_

        protected override void WndProc(ref Message m)
        {
            if (WndProcHandler != null)
            {
                if (WndProcHandler(this, m))
                {
                    m.Result = new IntPtr(1);
                    return;
                }
            }

            base.WndProc(ref m);
        }

        #endregion
    }
}