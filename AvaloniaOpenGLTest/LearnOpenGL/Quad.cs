using Silk.NET.OpenGL;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AvaloniaOpenGLTest.LearnOpenGL
{
    internal class Quad
    {
        static uint _vao;
        static uint _vbo;
        static uint _ebo;

        static uint _shaderProgram;
        public unsafe static void Init(GL gl)
        {
            Span<float> vertices = [
                0.5f, 0.5f, 0.0f,   // 右上角
                0.5f, -0.5f, 0.0f,  // 右下角
                -0.5f, -0.5f, 0.0f, // 左下角
                -0.5f, 0.5f, 0.0f   // 左上角
            ];

            Span<uint> indices = [
                0, 1, 3, // 第一个三角形
                1, 2, 3  // 第二个三角形
            ];

            _vao = gl.GenVertexArray();
            gl.BindVertexArray(_vao);

            _vbo = gl.GenBuffer();
            gl.BindBuffer(BufferTargetARB.ArrayBuffer, _vbo);
            gl.BufferData<float>(BufferTargetARB.ArrayBuffer, vertices, BufferUsageARB.StaticDraw);

            _ebo = gl.GenBuffer();
            gl.BindBuffer(BufferTargetARB.ElementArrayBuffer, _ebo);
            gl.BufferData<uint>(BufferTargetARB.ElementArrayBuffer, indices, BufferUsageARB.StaticDraw);

            gl.VertexAttribPointer(0, 3, GLEnum.Float, false, 3 * sizeof(float), 0);
            gl.EnableVertexAttribArray(0);

            gl.BindVertexArray(0);
            gl.BindBuffer(BufferTargetARB.ArrayBuffer, 0);
            gl.BindBuffer(BufferTargetARB.ElementArrayBuffer, 0);

            var vertexShader = gl.CreateShader(GLEnum.VertexShader);
            gl.ShaderSource(vertexShader, VertexShaderString);
            gl.CompileShader(vertexShader);

            if (gl.GetShader(vertexShader, GLEnum.CompileStatus) == 0)
            {
                Console.WriteLine("Compile Shader Failed.");
            }

            var fragmentShader = gl.CreateShader(GLEnum.FragmentShader);
            gl.ShaderSource(fragmentShader, FragmentShaderString);
            gl.CompileShader(fragmentShader);

            if (gl.GetShader(fragmentShader, GLEnum.CompileStatus) == 0)
            {
                Console.WriteLine("Compile Shader Failed.");
            }

            _shaderProgram = gl.CreateProgram();
            gl.AttachShader(_shaderProgram, vertexShader);
            gl.AttachShader(_shaderProgram, fragmentShader);
            gl.LinkProgram(_shaderProgram);

            if (gl.GetProgram(_shaderProgram, GLEnum.LinkStatus) == 0)
            {
                Console.WriteLine("Link program Failed.");
            }

            gl.DeleteShader(vertexShader);
            gl.DeleteShader(fragmentShader);
        }

        public unsafe static void Render(GL gl)
        {
            gl.PolygonMode(GLEnum.FrontAndBack, GLEnum.Fill);
            gl.UseProgram(_shaderProgram);
            gl.BindVertexArray(_vao);

            var red = 0.5f;
            var val = Environment.TickCount % 1000 / 1000d;
            red += (float)(Math.Sin(val * Math.PI * 2) / 2);
            gl.Uniform4(gl.GetUniformLocation(_shaderProgram, "color"), red, 0, 0, 1);


            gl.DrawElements(GLEnum.Triangles, 6, GLEnum.UnsignedInt, (void*)0);
        }

        static string VertexShaderString = """
            #version 330 core
            layout (location = 0) in vec3 aPos;

            out vec4 vertexColor;

            void main()
            {
                gl_Position = vec4(aPos.x, aPos.y, aPos.z, 1.0);
                vertexColor = vec4(0.5, 0.0, 0.0, 1.0);
            }
            """;

        static string FragmentShaderString = """
            #version 330 core
            out vec4 FragColor;

            in vec4 vertexColor;
            uniform vec4 color;

            void main()
            {
                FragColor = color;
            } 
            """;
    }
}
