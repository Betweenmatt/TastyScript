using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TastyScript.TastyScriptNPP
{
    internal class RTBToggleSelect : RichTextBox
    {
        public RTBToggleSelect()
        {
            Selectable = true;
        }

        private const int WM_SETFOCUS = 0x0007;
        private const int WM_KILLFOCUS = 0x0008;

        [DefaultValue(true)]
        public bool Selectable { get; set; }

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == WM_SETFOCUS && !Selectable)
                m.Msg = WM_KILLFOCUS;

            base.WndProc(ref m);
        }
    }
}