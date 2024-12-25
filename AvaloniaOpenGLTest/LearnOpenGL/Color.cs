using Avalonia.Controls;
using AvaloniaOpenGLTest.LearnOpenGL.Helpers;
using Silk.NET.OpenGL;
using System;
using System.IO;
using System.Numerics;

namespace AvaloniaOpenGLTest.LearnOpenGL
{
    internal class Color
    {
        static uint _vao;
        static uint _lightVao;
        static uint _vbo;

        static ShaderProgram _shaderProgram;
        static ShaderProgram _lightShaderProgram;
        static Helpers.Camera _camera;
        public unsafe static void Init(GL gl, Control control)
        {
            Span<float> vertices = [
                -0.5f, -0.5f, -0.5f,  0.0f, 0.0f,
                 0.5f, -0.5f, -0.5f,  1.0f, 0.0f,
                 0.5f,  0.5f, -0.5f,  1.0f, 1.0f,
                 0.5f,  0.5f, -0.5f,  1.0f, 1.0f,
                -0.5f,  0.5f, -0.5f,  0.0f, 1.0f,
                -0.5f, -0.5f, -0.5f,  0.0f, 0.0f,

                -0.5f, -0.5f,  0.5f,  0.0f, 0.0f,
                 0.5f, -0.5f,  0.5f,  1.0f, 0.0f,
                 0.5f,  0.5f,  0.5f,  1.0f, 1.0f,
                 0.5f,  0.5f,  0.5f,  1.0f, 1.0f,
                -0.5f,  0.5f,  0.5f,  0.0f, 1.0f,
                -0.5f, -0.5f,  0.5f,  0.0f, 0.0f,

                -0.5f,  0.5f,  0.5f,  1.0f, 0.0f,
                -0.5f,  0.5f, -0.5f,  1.0f, 1.0f,
                -0.5f, -0.5f, -0.5f,  0.0f, 1.0f,
                -0.5f, -0.5f, -0.5f,  0.0f, 1.0f,
                -0.5f, -0.5f,  0.5f,  0.0f, 0.0f,
                -0.5f,  0.5f,  0.5f,  1.0f, 0.0f,

                 0.5f,  0.5f,  0.5f,  1.0f, 0.0f,
                 0.5f,  0.5f, -0.5f,  1.0f, 1.0f,
                 0.5f, -0.5f, -0.5f,  0.0f, 1.0f,
                 0.5f, -0.5f, -0.5f,  0.0f, 1.0f,
                 0.5f, -0.5f,  0.5f,  0.0f, 0.0f,
                 0.5f,  0.5f,  0.5f,  1.0f, 0.0f,

                -0.5f, -0.5f, -0.5f,  0.0f, 1.0f,
                 0.5f, -0.5f, -0.5f,  1.0f, 1.0f,
                 0.5f, -0.5f,  0.5f,  1.0f, 0.0f,
                 0.5f, -0.5f,  0.5f,  1.0f, 0.0f,
                -0.5f, -0.5f,  0.5f,  0.0f, 0.0f,
                -0.5f, -0.5f, -0.5f,  0.0f, 1.0f,

                -0.5f,  0.5f, -0.5f,  0.0f, 1.0f,
                 0.5f,  0.5f, -0.5f,  1.0f, 1.0f,
                 0.5f,  0.5f,  0.5f,  1.0f, 0.0f,
                 0.5f,  0.5f,  0.5f,  1.0f, 0.0f,
                -0.5f,  0.5f,  0.5f,  0.0f, 0.0f,
                -0.5f,  0.5f, -0.5f,  0.0f, 1.0f
            ];

            _vao = gl.GenVertexArray();
            gl.BindVertexArray(_vao);

            _vbo = gl.GenBuffer();
            gl.BindBuffer(BufferTargetARB.ArrayBuffer, _vbo);
            gl.BufferData<float>(BufferTargetARB.ArrayBuffer, vertices, BufferUsageARB.StaticDraw);

            gl.VertexAttribPointer(0, 3, GLEnum.Float, false, 5 * sizeof(float), 0);
            gl.EnableVertexAttribArray(0);

            _lightVao = gl.GenVertexArray();
            gl.BindVertexArray(_lightVao);
            gl.VertexAttribPointer(0, 3, GLEnum.Float, false, 5 * sizeof(float), 0);
            gl.EnableVertexAttribArray(0);

            gl.BindVertexArray(0);
            gl.BindBuffer(BufferTargetARB.ArrayBuffer, 0);

            _shaderProgram = new ShaderProgram(gl);
            _shaderProgram.Compile(VertexShaderString, FragmentShaderString);

            _lightShaderProgram = new ShaderProgram(gl);
            _lightShaderProgram.Compile(VertexShaderString, LightFragmentShaderString);

            _camera = new Helpers.Camera(gl, control);
            _camera.CameraPos = new Vector3(0, 0, 6);
        }



        public unsafe static void Render(GL gl)
        {
            Span<int> viewports = stackalloc int[4];
            gl.GetInteger(GLEnum.Viewport, viewports);

            _shaderProgram.Use();
            gl.BindVertexArray(_vao);

            var view = _camera.GetViewMatrix();
            var projection = _camera.GetProjection((float)viewports[2] / viewports[3]);

            _shaderProgram.SetUniform("model", Matrix4x4.Identity);
            _shaderProgram.SetUniform("view", view);
            _shaderProgram.SetUniform("projection", projection);
            _shaderProgram.SetUniform("objectColor", new Vector3(1.0f, 0.5f, 0.31f));
            _shaderProgram.SetUniform("lightColor", new Vector3(1.0f, 1.0f, 1.0f));
            gl.DrawArrays(GLEnum.Triangles, 0, 36);

            _lightShaderProgram.Use();
            gl.BindVertexArray(_lightVao);

            _lightShaderProgram.SetUniform("model", Matrix4x4.CreateScale(0.2f) * Matrix4x4.CreateTranslation(1.2f, 1.0f, 2.0f));
            _lightShaderProgram.SetUniform("view", view);
            _lightShaderProgram.SetUniform("projection", projection);
            gl.DrawArrays(GLEnum.Triangles, 0, 36);

        }

        static string VertexShaderString = """
            #version 330 core
            layout (location = 0) in vec3 aPos;

            uniform mat4 model;
            uniform mat4 view;
            uniform mat4 projection;

            void main()
            {
                gl_Position = projection * view * model * vec4(aPos.x, aPos.y, aPos.z, 1.0);
            }
            """;

        static string FragmentShaderString = """
            #version 330 core
            out vec4 FragColor;

            uniform vec3 objectColor;
            uniform vec3 lightColor;

            void main()
            {
                FragColor = vec4(lightColor * objectColor, 1.0);
            } 
            """;

        static string LightFragmentShaderString = """
            #version 330 core
            out vec4 FragColor;

            void main()
            {
                FragColor = vec4(1.0);
            } 
            """;
    }
}
