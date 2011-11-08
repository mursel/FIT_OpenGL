using System;
using System.Collections.Generic;
using System.Text;
using SoulEngine.Engine.Device;
using SoulEngine.Engine.Driver;
using SoulEngine.Engine.Data;
using System.Windows.Forms;

namespace SoulEngine
{
    public class Core
    {
        public static Form MainForm { get; set; }

        public static EngineDevice CreateDevice(EngineParameters.DRIVERTYPE driverType, Form mainForm, int width, int height, int bits, bool isFullscreen)
        {
            switch (driverType)
            {
                case EngineParameters.DRIVERTYPE.OPENGL:
                    {
                        OpenGLDriver openGl = new OpenGLDriver();
                        if (openGl.Setup(mainForm, width, height, bits, isFullscreen))
                        {
                            EngineDevice ed = new EngineDevice();
                            ed.SetVideoDriver(openGl);
                            return ed;
                        }
                    }
                    break;
                case EngineParameters.DRIVERTYPE.DIRECT3D9:
                    break;
                case EngineParameters.DRIVERTYPE.DIRECT3D10:
                    break;
            }

            return null;
        }
    }
}
