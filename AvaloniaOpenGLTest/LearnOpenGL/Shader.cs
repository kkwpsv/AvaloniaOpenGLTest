using AvaloniaOpenGLTest.LearnOpenGL.Helpers;
using Silk.NET.OpenGL;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AvaloniaOpenGLTest.LearnOpenGL
{
    internal class Shader
    {
        static uint _vao;
        static uint _vbo;

        static ShaderProgram _shaderProgram;
        public unsafe static void Init(GL gl)
        {
            Span<float> vertices = [
                // 位置              // 颜色
                 0.5f, -0.5f, 0.0f,  1.0f, 0.0f, 0.0f,   // 右下
                -0.5f, -0.5f, 0.0f,  0.0f, 1.0f, 0.0f,   // 左下
                 0.0f,  0.5f, 0.0f,  0.0f, 0.0f, 1.0f    // 顶部
            ];

            _vao = gl.GenVertexArray();
            gl.BindVertexArray(_vao);

            _vbo = gl.GenBuffer();
            gl.BindBuffer(BufferTargetARB.ArrayBuffer, _vbo);
            gl.BufferData<float>(BufferTargetARB.ArrayBuffer, vertices, BufferUsageARB.StaticDraw);

            gl.VertexAttribPointer(0, 3, GLEnum.Float, false, 6 * sizeof(float), 0);
            gl.EnableVertexAttribArray(0);

            gl.VertexAttribPointer(1, 3, GLEnum.Float, false, 6 * sizeof(float), 3 * sizeof(float));
            gl.EnableVertexAttribArray(1);

            gl.BindVertexArray(0);
            gl.BindBuffer(BufferTargetARB.ArrayBuffer, 0);

            _shaderProgram = new ShaderProgram(gl);
            _shaderProgram.Compile(VertexShaderString, FragmentShaderString);
        }

        public unsafe static void Render(GL gl)
        {
            _shaderProgram.Use();
            gl.BindVertexArray(_vao);

            gl.DrawArrays(GLEnum.Triangles, 0, 3);
        }

        static string VertexShaderString = """
            #version 330 core
            layout (location = 0) in vec3 aPos;
            layout (location = 1) in vec3 aColor;

            out vec3 vertexColor;

            void main()
            {
                gl_Position = vec4(aPos.x, aPos.y, aPos.z, 1.0);
                vertexColor = aColor;
            }
            """;

        static string FragmentShaderString = """
            #version 330 core
            out vec4 FragColor;

            in vec3 vertexColor;

            void main()
            {
                FragColor = vec4(vertexColor, 1.0);
            } 
            """;
    }
}
