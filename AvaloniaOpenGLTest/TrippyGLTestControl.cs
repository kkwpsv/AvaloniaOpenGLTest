using Avalonia.Controls;
using Avalonia.OpenGL;
using Avalonia.OpenGL.Controls;
using Silk.NET.Maths;
using Silk.NET.OpenGL;
using System;
using System.Diagnostics;
using System.Numerics;
using TrippyGL;
using PrimitiveType = TrippyGL.PrimitiveType;

namespace AvaloniaOpenGLTest
{
    internal class TrippyGLTestControl : OpenGlControlBase
    {
        private GL? _gl;
        private GraphicsDevice? _graphicsDevice;
        VertexBuffer<VertexColor> _vertexBuffer;
        SimpleShaderProgram _shaderProgram;
        Stopwatch stopwatch;

        protected override void OnOpenGlInit(GlInterface gl)
        {
            _gl = GL.GetApi(gl.GetProcAddress);
            _graphicsDevice = new GraphicsDevice(_gl);
            _graphicsDevice.DebugMessagingEnabled = true;
            _graphicsDevice.DebugMessageReceived += GraphicsDevice_DebugMessageReceived;
            if (_graphicsDevice != null)
            {
                Span<VertexColor> cubeBufferData = stackalloc VertexColor[] {
                    new VertexColor(new Vector3(-0.5f, -0.5f, -0.5f), Color4b.LightBlue),//4
                    new VertexColor(new Vector3(-0.5f, -0.5f, 0.5f), Color4b.Lime),//3
                    new VertexColor(new Vector3(-0.5f, 0.5f, -0.5f), Color4b.White),//7
                    new VertexColor(new Vector3(-0.5f, 0.5f, 0.5f), Color4b.Black),//8
                    new VertexColor(new Vector3(0.5f, 0.5f, 0.5f), Color4b.Blue),//5
                    new VertexColor(new Vector3(-0.5f, -0.5f, 0.5f), Color4b.Lime),//3
                    new VertexColor(new Vector3(0.5f, -0.5f, 0.5f), Color4b.Red),//1
                    new VertexColor(new Vector3(-0.5f, -0.5f, -0.5f), Color4b.LightBlue),//4
                    new VertexColor(new Vector3(0.5f, -0.5f, -0.5f), Color4b.Yellow),//2
                    new VertexColor(new Vector3(-0.5f, 0.5f, -0.5f), Color4b.White),//7
                    new VertexColor(new Vector3(0.5f, 0.5f, -0.5f), Color4b.Pink),//6
                    new VertexColor(new Vector3(0.5f, 0.5f, 0.5f), Color4b.Blue),//5
                    new VertexColor(new Vector3(0.5f, -0.5f, -0.5f), Color4b.Yellow),//2
                    new VertexColor(new Vector3(0.5f, -0.5f, 0.5f), Color4b.Red),//1
                };

                _vertexBuffer = new VertexBuffer<VertexColor>(_graphicsDevice, cubeBufferData, BufferUsage.StaticCopy);
                _shaderProgram = SimpleShaderProgram.Create<VertexColor>(_graphicsDevice);

                _shaderProgram.View = Matrix4x4.CreateLookAt(new Vector3(0, 1.0f, -1.5f), Vector3.Zero, Vector3.UnitY);

                _graphicsDevice.DepthState = DepthState.Default;
                _graphicsDevice.BlendState = BlendState.Opaque;

                stopwatch = Stopwatch.StartNew();
            }
        }

        private void GraphicsDevice_DebugMessageReceived(TrippyGL.DebugSource debugSource, TrippyGL.DebugType debugType, int messageId, TrippyGL.DebugSeverity debugSeverity, string? message)
        {
            Console.WriteLine(message);
        }

        protected override void OnOpenGlRender(GlInterface gl, int fb)
        {
            if (_graphicsDevice != null)
            {
                _graphicsDevice.ClearDepth = 1;
                _graphicsDevice.ClearColor = Vector4.Zero;
                _graphicsDevice.Clear(ClearBuffers.Color | ClearBuffers.Depth);
                //_gl.Clear(ClearBufferMask.ColorBufferBit);
                //RequestNextFrameRendering();

                var size = Bounds.Size * VisualRoot.RenderScaling;
                _graphicsDevice.SetViewport(0, 0, (uint)size.Width, (uint)size.Height);
                _shaderProgram.Projection = Matrix4x4.CreatePerspectiveFieldOfView(MathF.PI / 2f, (float)(size.Width / size.Height), 0.01f, 100f);
                _shaderProgram.World = Matrix4x4.CreateRotationY(2 * (float)stopwatch.Elapsed.TotalSeconds);
                _graphicsDevice.ShaderProgram = _shaderProgram;
                _graphicsDevice.VertexArray = _vertexBuffer;

                _graphicsDevice.DrawArrays(PrimitiveType.TriangleStrip, 0, _vertexBuffer.StorageLength);

                RequestNextFrameRendering();
            }
        }

        protected override void OnOpenGlDeinit(GlInterface gl)
        {
        }

        protected override void OnOpenGlLost()
        {
        }
    }
}
