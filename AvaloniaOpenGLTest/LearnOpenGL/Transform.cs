using AvaloniaOpenGLTest.LearnOpenGL.Helpers;
using Silk.NET.OpenGL;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace AvaloniaOpenGLTest.LearnOpenGL
{
    internal class Transform
    {
        static uint _vao;
        static uint _vbo;
        static uint _ebo;
        static uint _texture1;
        static uint _texture2;

        static ShaderProgram _shaderProgram;
        public unsafe static void Init(GL gl)
        {
            Span<float> vertices = [
            //     ---- 位置 ----       ---- 颜色 ----     - 纹理坐标 -
                 0.5f,  0.5f, 0.0f,   1.0f, 0.0f, 0.0f,   1.0f, 1.0f,   // 右上
                 0.5f, -0.5f, 0.0f,   0.0f, 1.0f, 0.0f,   1.0f, 0.0f,   // 右下
                -0.5f, -0.5f, 0.0f,   0.0f, 0.0f, 1.0f,   0.0f, 0.0f,   // 左下
                -0.5f,  0.5f, 0.0f,   1.0f, 1.0f, 0.0f,   0.0f, 1.0f    // 左上
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

            gl.VertexAttribPointer(0, 3, GLEnum.Float, false, 8 * sizeof(float), 0);
            gl.EnableVertexAttribArray(0);
            gl.VertexAttribPointer(1, 3, GLEnum.Float, false, 8 * sizeof(float), 3 * sizeof(float));
            gl.EnableVertexAttribArray(1);
            gl.VertexAttribPointer(2, 2, GLEnum.Float, false, 8 * sizeof(float), 6 * sizeof(float));
            gl.EnableVertexAttribArray(2);

            gl.BindVertexArray(0);
            gl.BindBuffer(BufferTargetARB.ArrayBuffer, 0);
            gl.BindBuffer(BufferTargetARB.ElementArrayBuffer, 0);

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
        }

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

            var transform = Matrix4x4.CreateRotationZ(Environment.TickCount / 5000f) * Matrix4x4.CreateTranslation(0.5f, -0.5f, 0);
            _shaderProgram.SetUniform("transform", transform);
            gl.DrawElements(GLEnum.Triangles, 6, GLEnum.UnsignedInt, (void*)0);

            var transform2 = Matrix4x4.CreateRotationZ(Environment.TickCount / 5000f) * Matrix4x4.CreateTranslation(0.5f, 0.5f, 0);
            _shaderProgram.SetUniform("transform", transform2);
            gl.DrawElements(GLEnum.Triangles, 6, GLEnum.UnsignedInt, (void*)0);

            var transform3 = Matrix4x4.CreateRotationZ(Environment.TickCount / 5000f) * Matrix4x4.CreateTranslation(-0.5f, 0.5f, 0);
            _shaderProgram.SetUniform("transform", transform3);
            gl.DrawElements(GLEnum.Triangles, 6, GLEnum.UnsignedInt, (void*)0);

            var transform4 = Matrix4x4.CreateRotationZ(Environment.TickCount / 5000f) * Matrix4x4.CreateTranslation(-0.5f, -0.5f, 0);
            _shaderProgram.SetUniform("transform", transform4);
            gl.DrawElements(GLEnum.Triangles, 6, GLEnum.UnsignedInt, (void*)0);
        }

        static string VertexShaderString = """
            #version 330 core
            layout (location = 0) in vec3 aPos;
            layout (location = 1) in vec3 aColor;
            layout (location = 2) in vec2 aTexCoord;

            out vec3 color;
            out vec2 texCoord;

            uniform mat4 transform;

            void main()
            {
                gl_Position = transform * vec4(aPos.x, aPos.y, aPos.z, 1.0);
                color = aColor;
                texCoord = aTexCoord;
            }
            """;

        static string FragmentShaderString = """
            #version 330 core
            out vec4 FragColor;

            in vec3 color;
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
