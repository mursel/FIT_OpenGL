using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace SoulEngine
{
    static class Program
    {
        [STAThread]
        public static void Main()
        {
            using (GlavnaForma mainForm = new GlavnaForma())
            {
                Application.Idle += new EventHandler(mainForm.Application_Idle);
                Application.Run(mainForm);
            }
        }
    }
}
