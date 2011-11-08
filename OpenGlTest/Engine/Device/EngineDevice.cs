using System;
using System.Collections.Generic;
using System.Text;
using SoulEngine.Engine.Driver;

namespace SoulEngine.Engine.Device
{
    public class EngineDevice
    {
        protected VideoDriver videoDriver;

        public VideoDriver GetVideoDriver()
        {
            return (videoDriver != null) ? videoDriver : null;
        }

        public void SetVideoDriver(VideoDriver v)
        {
            this.videoDriver = v;
        }
        
        public void SetWindowTitle(string text)
        {
            Core.MainForm.Text = text;                        
        }


    }
}
