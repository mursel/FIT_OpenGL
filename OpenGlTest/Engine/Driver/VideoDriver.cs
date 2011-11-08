using System;
using System.Collections.Generic;
using System.Text;
using Tao.OpenGl;
using Tao.Platform.Windows;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using SoulEngine.Engine.Interfaces;
using System.Diagnostics;
using SoulEngine.Engine.Data;

namespace SoulEngine.Engine.Driver
{
    public class VideoDriver : Logger
    {
        public Form MainForm { get; set; }
        public bool IsFullscreen { get; set; }

        public virtual void DrawScene() { }
        public virtual void EndScene() { }
        public virtual void ResizeScene(int width, int height) { }

        public override void Write(string msg, params object[] args)
        {
            base.Write(msg, args);
        }
    }

    #region DirectX9Driver
    public class DirectX9Driver : VideoDriver //,IDriver
    {

        public bool Setup(Form mainForm, int width, int height, int bits, bool isfullscreen)
        {
            throw new NotImplementedException();
        }

        public bool Initialize()
        {
            throw new NotImplementedException();
        }

        public void Deinitilize()
        {
            throw new NotImplementedException();
        }

        public override void ResizeScene(int width, int height)
        {
            throw new NotImplementedException();
        }

        public override void DrawScene()
        {
            throw new NotImplementedException();
        }

        public override void EndScene()
        {
            throw new NotImplementedException();
        }
    }
    #endregion

    #region OpenGLDriver
    public class OpenGLDriver : VideoDriver, IDriver
    {
        private IntPtr hDC;
        private IntPtr hRC;
        
        public bool Setup(Form mainForm, int width, int height, int bits, bool isfullscreen)
        {
            int pixelFormat;

            this.IsFullscreen = isfullscreen;
            this.MainForm = mainForm;

            GC.Collect();
            Kernel.SetProcessWorkingSetSize(Process.GetCurrentProcess().Handle, -1, -1);

            Gl.glShadeModel(Gl.GL_SMOOTH);

            if (IsFullscreen)
            {
                Gdi.DEVMODE devMode = new Gdi.DEVMODE();
                devMode.dmSize = (short)Marshal.SizeOf(devMode);
                devMode.dmPelsWidth = width;
                devMode.dmPelsHeight = height;
                devMode.dmBitsPerPel = bits;
                devMode.dmFields = Gdi.DM_BITSPERPEL | Gdi.DM_PELSWIDTH | Gdi.DM_PELSHEIGHT;

                if (User.ChangeDisplaySettings(ref devMode, User.CDS_FULLSCREEN) != User.DISP_CHANGE_SUCCESSFUL)
                {
                    IsFullscreen = false;
                }

            }

            if (IsFullscreen)
            {
                MainForm.FormBorderStyle = FormBorderStyle.None;
            }
            else
            {
                MainForm.FormBorderStyle = FormBorderStyle.Sizable;
            }

            MainForm.Width = width;
            MainForm.Height = height;

            Gdi.PIXELFORMATDESCRIPTOR pfd = new Gdi.PIXELFORMATDESCRIPTOR();

            pfd.nSize = (short)Marshal.SizeOf(pfd);
            pfd.nVersion = 1;
            pfd.dwFlags = Gdi.PFD_SUPPORT_OPENGL | Gdi.PFD_DRAW_TO_WINDOW | Gdi.PFD_DOUBLEBUFFER;
            pfd.iPixelType = (byte)Gdi.PFD_TYPE_RGBA;
            pfd.cColorBits = (byte)bits;
            pfd.cAccumAlphaBits = 0;
            pfd.cAccumBits = 0;
            pfd.cAccumBlueBits = 0;
            pfd.cAccumGreenBits = 0;
            pfd.cAccumRedBits = 0;
            pfd.cAlphaBits = 0;
            pfd.cAlphaShift = 0;
            pfd.cAuxBuffers = 0;
            pfd.cBlueBits = 0;
            pfd.cBlueShift = 0;
            pfd.cDepthBits = 16;    // depth
            pfd.cGreenBits = 0;
            pfd.cGreenShift = 0;
            pfd.cRedBits = 0;
            pfd.cRedShift = 0;
            pfd.cStencilBits = 0;   // stencil
            pfd.iLayerType = (byte)Gdi.PFD_MAIN_PLANE;
            pfd.bReserved = 0;
            pfd.dwDamageMask = 0;
            pfd.dwLayerMask = 0;
            pfd.dwVisibleMask = 0;

            hDC = User.GetDC(MainForm.Handle);
            if (hDC == IntPtr.Zero)
            {
                Deinitialize();
                return false;
            }

            pixelFormat = Gdi.ChoosePixelFormat(hDC, ref pfd);
            if (pixelFormat == 0)
            {
                Deinitialize();
                return false;
            }

            if (!Gdi.SetPixelFormat(hDC, pixelFormat, ref pfd))
            {
                Deinitialize();
                return false;
            }

            hRC = Wgl.wglCreateContext(hDC);    // ako imamo validan hdc onda postavi rendering context
            if (hRC == IntPtr.Zero)
            {
                Deinitialize();
                return false;
            }

            if (!Wgl.wglMakeCurrent(hDC, hRC))
            {
                Deinitialize();
                return false;
            }

            MainForm.TopMost = true;
            MainForm.Focus();

            if (IsFullscreen)
            {
                Cursor.Hide();
            }

            ResizeScene(width, height);
            
            if (!Initialize())
            {
                Deinitialize();
                return false;
            }

            return true;
        }

        public bool Initialize()
        {
            Gl.glShadeModel(Gl.GL_SMOOTH);
            Gl.glClearColor(0, 128, 128, 0);     // "ocisti" pozadinu sa tirkiznom bojom
            Gl.glClearDepth(1);
            Gl.glEnable(Gl.GL_DEPTH_TEST);
            Gl.glDepthFunc(Gl.GL_LEQUAL);
            Gl.glHint(Gl.GL_PERSPECTIVE_CORRECTION_HINT, Gl.GL_NICEST);
            return true;
        }

        public override void ResizeScene(int width, int height)
        {
            if (height == 0)
            {
                height = 1;
            }

            Gl.glViewport(0, 0, width, height);

            Gl.glMatrixMode(Gl.GL_PROJECTION);      // postavi projekcijsku matricu na stog
            Gl.glLoadIdentity();                    // postavi jedinicnu matricu (resetuj projekcijsku)

            Glu.gluPerspective(
                45.0,                       // ugao vidljivosti
                width / (double)height,    // omjer vidljivosti
                0.1,                        // pocetak vidljivosti
                100                        // kraj vidljivosti
                );

            Gl.glMatrixMode(Gl.GL_MODELVIEW);       // model-view matrica
            Gl.glLoadIdentity();                    // resetuj mv matricu

        }

        public override void DrawScene() {
            Gl.glClear(Gl.GL_COLOR_BUFFER_BIT | Gl.GL_DEPTH_BUFFER_BIT);
            Gl.glLoadIdentity();
        }

        public override void EndScene() 
        {
            Gdi.SwapBuffers(hDC);
        }

        public void Deinitialize()
        {
            if (IsFullscreen)
            {
                User.ChangeDisplaySettings(IntPtr.Zero, 0);
            }

            if (hRC != IntPtr.Zero)
            {
                Wgl.wglMakeCurrent(IntPtr.Zero, IntPtr.Zero);
                Wgl.wglDeleteContext(hRC);
                hRC = IntPtr.Zero;
            }

            if (hDC != IntPtr.Zero)
            {
                User.ReleaseDC(MainForm.Handle, hDC);
                hDC = IntPtr.Zero;
            }

            if (MainForm != null)
            {
                MainForm.Close();
                MainForm = null;
            }

        }

    }
#endregion  

}
