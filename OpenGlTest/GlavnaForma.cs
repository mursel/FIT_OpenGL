using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using SoulEngine.Engine.Driver;
using SoulEngine.Engine.Device;
using SoulEngine.Engine.Data;
using SoulEngine.Engine.Native;
using Tao.Platform.Windows;
using Tao.OpenGl;

namespace SoulEngine
{
    public class GlavnaForma : Form
    {
        private bool[] keys = new bool[256];
        private EngineDevice device;
        private VideoDriver driver;

        public GlavnaForma()
        {
            SetupForm();
            Init();
        }
        
        public void Init()
        {
            if (MessageBox.Show("Fullscreen?", "SoulEngineGL Test", MessageBoxButtons.OKCancel) == System.Windows.Forms.DialogResult.OK)
                device = Core.CreateDevice(EngineParameters.DRIVERTYPE.OPENGL, this, 800, 600, 16, true);
            else
                device = Core.CreateDevice(EngineParameters.DRIVERTYPE.OPENGL, this, 800, 600, 16, false);

            if (device != null)
            {
                driver = device.GetVideoDriver();
            }
            else
            {
                MessageBox.Show("CreateDevice failed!");
                return;
            }
        }
                
        public void Application_Idle(object sender, EventArgs e)
        {
            while (AppStillIdle)
            {
                if (keys[(int)Keys.Escape])
                {
                    Application.Exit();
                }

                driver.DrawScene();
				
                Gl.glTranslatef(-1.5f, 0, -6); 
                Gl.glBegin(Gl.GL_TRIANGLES);   
                Gl.glVertex3f(0, 1, 0);        
                Gl.glVertex3f(-1, -1, 0);      
                Gl.glVertex3f(1, -1, 0);       
                Gl.glEnd();                    
                Gl.glTranslatef(3, 0, 0);      
                Gl.glBegin(Gl.GL_QUADS);       
                Gl.glVertex3f(-1, 1, 0);       
                Gl.glVertex3f(1, 1, 0);        
                Gl.glVertex3f(1, -1, 0);       
                Gl.glVertex3f(-1, -1, 0);      
                Gl.glEnd();  

                driver.EndScene();

            }
        }

        private bool AppStillIdle
        {
            get
            {
                Win32.Message msg;
                return !Win32.PeekMessage(out msg, IntPtr.Zero, 0, 0, 0);
            }
        }

        private void Form_KeyDown(object sender, KeyEventArgs e)
        {
            keys[e.KeyValue] = true;
        }

        private void Form_KeyUp(object sender, KeyEventArgs e)
        {
            keys[e.KeyValue] = false;
        }

        #region Main form setup
        private void SetupForm()
        {
            this.CreateParams.ClassStyle = this.CreateParams.ClassStyle |
                User.CS_HREDRAW | User.CS_VREDRAW | User.CS_OWNDC;
            
            this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            this.SetStyle(ControlStyles.Opaque, true);
            this.SetStyle(ControlStyles.ResizeRedraw, true);
            this.SetStyle(ControlStyles.UserPaint, true);
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Form_KeyDown);
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.Form_KeyUp);
            //this.Resize += new EventHandler(this.Form_Resize);
        }

        #endregion
    }
}
