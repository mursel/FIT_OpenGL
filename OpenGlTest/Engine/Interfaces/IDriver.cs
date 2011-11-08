using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace SoulEngine.Engine.Interfaces
{
    public interface IDriver
	{
        bool Setup(Form mainForm, int width, int height, int bits, bool isfullscreen);
        
        bool Initialize();
        void Deinitialize();

        void ResizeScene(int width, int height);
        void DrawScene();
        void EndScene();
	} 
}
