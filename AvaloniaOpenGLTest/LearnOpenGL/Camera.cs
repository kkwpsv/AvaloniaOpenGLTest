using Avalonia.Controls;
using AvaloniaOpenGLTest.LearnOpenGL.Helpers;
using Silk.NET.OpenGL;
using System;
using System.IO;
using System.Numerics;

namespace AvaloniaOpenGLTest.LearnOpenGL
{
    internal class Camera
    {
        static uint _vao;
        static uint _vbo;
        static uint _texture1;
        static uint _texture2;

        static ShaderProgram _shaderProgram;
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
            gl.VertexAttribPointer(1, 2, GLEnum.Float, false, 5 * sizeof(float), 3 * sizeof(float));
            gl.EnableVertexAttribArray(1);

            gl.BindVertexArray(0);
            gl.BindBuffer(BufferTargetARB.ArrayBuffer, 0);

            StbImageSharp.StbImage.stbi_set_flip_vertically_on_load(1);
            var imageResult = StbImageSharp.ImageResult.FromMemory(File.ReadAllBytes("container.jpg"), StbImageSharp.ColorComponents.RedGreenBlue);
            fixed (byte* ptr = imageResult.Data)
            {
                _texture1 = gl.GenTexture();
                gl.BindTexture(GLEnum.Texture2D, _texture1);
                gl.TexParameter(GLEnum.Texture2D, GLEnum.TextureMinFilter, (int)TextureMinFilter.Nearest);
                gl.TexParameter(GLEnum.Texture2D, GLEnum.TextureMagFilter, (int)TextureMagFilter.Nearest);
                gl.TexImage2D(GLEnum.Texture2D, 0, InternalFormat.Rgb, (uint)imageResult.Width, (uint)imageResult.Height, 0, GLEnum.Rgb, GLEnum.UnsignedByte, ptr);
            }

            var imageResult2 = StbImageSharp.ImageResult.FromMemory(File.ReadAllBytes("awesomeface.png"), StbImageSharp.ColorComponents.RedGreenBlueAlpha);
            fixed (byte* ptr = imageResult2.Data)
            {
                _texture2 = gl.GenTexture();
                gl.BindTexture(GLEnum.Texture2D, _texture2);
                gl.TexParameter(GLEnum.Texture2D, GLEnum.TextureMinFilter, (int)TextureMinFilter.Nearest);
                gl.TexParameter(GLEnum.Texture2D, GLEnum.TextureMagFilter, (int)TextureMagFilter.Nearest);
                gl.TexImage2D(GLEnum.Texture2D, 0, InternalFormat.Rgba, (uint)imageResult2.Width, (uint)imageResult2.Height, 0, GLEnum.Rgba, GLEnum.UnsignedByte, ptr);
            }

            _shaderProgram = new ShaderProgram(gl);
            _shaderProgram.Compile(VertexShaderString, FragmentShaderString);

            _camera = new Helpers.Camera(gl, control);
        }

        static Vector3[] _positions =
        [
            new Vector3 (0.0f,  0.0f, 0.0f),
            new Vector3( 2.0f,  5.0f, -15.0f),
            new Vector3(-1.5f, -2.2f, -2.5f),
            new Vector3(-3.8f, -2.0f, -12.3f),
            new Vector3( 2.4f, -0.4f, -3.5f),
            new Vector3(-1.7f,  3.0f, -7.5f),
            new Vector3( 1.3f, -2.0f, -2.5f),
            new Vector3( 1.5f,  2.0f, -2.5f),
            new Vector3( 1.5f,  0.2f, -1.5f),
            new Vector3(-1.3f,  1.0f, -1.5f),
        ];



        public unsafe static void Render(GL gl)
        {
            _shaderProgram.Use();
            gl.BindVertexArray(_vao);
            gl.ActiveTexture(GLEnum.Texture1);
            gl.BindTexture(GLEnum.Texture2D, _texture1);
            gl.ActiveTexture(GLEnum.Texture2);
            gl.BindTexture(GLEnum.Texture2D, _texture2);
            _shaderProgram.SetUniform("texture1", 1);
            _shaderProgram.SetUniform("texture2", 2);

            Span<int> viewports = stackalloc int[4];
            gl.GetInteger(GLEnum.Viewport, viewports);


            _shaderProgram.SetUniform("view", _camera.GetViewMatrix());
            _shaderProgram.SetUniform("projection", _camera.GetProjection((float)viewports[2] / viewports[3]));

            for (int i = 0; i < _positions.Length; i++)
            {
                var model = Matrix4x4.CreateFromAxisAngle(new Vector3(1.0f, 0.3f, 0.5f), (float)(20f * i / 180 * Math.PI)) * Matrix4x4.CreateTranslation(_positions[i]);
                _shaderProgram.SetUniform("model", model);
                gl.DrawArrays(GLEnum.Triangles, 0, 36);
            }
        }

        static string VertexShaderString = """
            #version 330 core
            layout (location = 0) in vec3 aPos;
            layout (location = 1) in vec2 aTexCoord;

            out vec2 texCoord;

            uniform mat4 model;
            uniform mat4 view;
            uniform mat4 projection;

            void main()
            {
                gl_Position = projection * view * model * vec4(aPos.x, aPos.y, aPos.z, 1.0);
                texCoord = aTexCoord;
            }
            """;

        static string FragmentShaderString = """
            #version 330 core
            out vec4 FragColor;

            in vec2 texCoord;

            uniform sampler2D texture1;
            uniform sampler2D texture2;

            void main()
            {
                FragColor = mix(texture(texture1, texCoord), texture(texture2, texCoord), 0.2);
                FragColor.a = 1.0;
            } 
            """;
    }
}
