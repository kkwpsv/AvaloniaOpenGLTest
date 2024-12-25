using Avalonia;
using Avalonia.Input;
using Avalonia.OpenGL;
using Avalonia.OpenGL.Controls;
using Avalonia.Rendering;
using Avalonia.Threading;
using AvaloniaOpenGLTest.LearnOpenGL;
using Silk.NET.OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrippyGL;

namespace AvaloniaOpenGLTest
{
    internal class OpenGLTestControl : OpenGlControlBase, ICustomHitTest
    {
        private GL? _gl;

        protected override void OnOpenGlInit(GlInterface gl)
        {
            //Console.WriteLine("OnOpenGlInit");
            _gl = GL.GetApi(gl.GetProcAddress);

            if (_gl != null)
            {
                var version = _gl.GetStringS(GLEnum.Version);
                var vendor = _gl.GetStringS(GLEnum.Vendor);
                var renderer = _gl.GetStringS(GLEnum.Renderer);
                Console.WriteLine($"OpenGL Init.");
                Console.WriteLine($"Vendor: {vendor}");
                Console.WriteLine($"Version: {version}");
                Console.WriteLine($"Renderer: {renderer}");

                _gl.Enable(EnableCap.DepthTest);

                try
                {
                    LearnOpenGL.Model.Init(_gl, this);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            }
        }

        protected override void OnOpenGlRender(GlInterface gl, int fb)
        {
            //Console.WriteLine("OnOpenGlRender");
            if (_gl != null)
            {
                var size = Bounds.Size * VisualRoot.RenderScaling;
                _gl.Viewport(0, 0, (uint)size.Width, (uint)size.Height);
                _gl.ClearColor(0.2f, 0.3f, 0.3f, 1);
                _gl.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

                LearnOpenGL.Model.Render(_gl);

                //Console.WriteLine("RequestNextFrameRendering");
                RequestNextFrameRendering();
            }
        }

        public bool HitTest(Point point)
        {
            return true;
        }
    }
}
