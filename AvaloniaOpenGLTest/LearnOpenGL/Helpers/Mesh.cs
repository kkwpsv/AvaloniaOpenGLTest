using Silk.NET.OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AvaloniaOpenGLTest.LearnOpenGL.Helpers
{
    public class Mesh
    {
        private readonly GL _gl;
        private readonly Vertex[] _vertices;
        private readonly uint[] _indices;
        private readonly Texture[] _textures;

        private uint _vao;
        private uint _vbo;
        private uint _ebo;

        public Mesh(GL gl, Vertex[] vertices, uint[] indices, Texture[] textures)
        {
            _gl = gl;
            _vertices = vertices;
            _indices = indices;
            _textures = textures;
            Init();
        }

        private unsafe void Init()
        {
            _vao = _gl.GenVertexArray();
            _vbo = _gl.GenBuffer();
            _ebo = _gl.GenBuffer();

            _gl.BindVertexArray(_vao);
            _gl.BindBuffer(GLEnum.ArrayBuffer, _vbo);

            fixed (void* ptr = _vertices)
            {
                _gl.BufferData(GLEnum.ArrayBuffer, (uint)(sizeof(Vertex) * _vertices.Length), ptr, BufferUsageARB.StaticDraw);
            }

            _gl.BindBuffer(GLEnum.ElementArrayBuffer, _ebo);
            fixed (void* ptr = _indices)
            {
                _gl.BufferData(GLEnum.ElementArrayBuffer, (uint)(sizeof(uint) * _indices.Length), ptr, BufferUsageARB.StaticDraw);
            }

            _gl.EnableVertexAttribArray(0);
            _gl.VertexAttribPointer(0, 3, GLEnum.Float, false, (uint)(sizeof(Vertex)), 0);
            _gl.EnableVertexAttribArray(1);
            _gl.VertexAttribPointer(1, 3, GLEnum.Float, false, (uint)(sizeof(Vertex)), 3 * sizeof(float));
            _gl.EnableVertexAttribArray(2);
            _gl.VertexAttribPointer(2, 2, GLEnum.Float, false, (uint)(sizeof(Vertex)), 6 * sizeof(float));

            _gl.BindVertexArray(0);
        }

        public void Draw(ShaderProgram shaderProgram)
        {
            bool hasDiffuse = false;
            bool hasSpecular = false;
            for (var i = 0; i < _textures.Length; i++)
            {
                if (_textures[i].Type == TextureType.Diffuse)
                {
                    if (hasDiffuse)
                    {
                        // Not support now.
                    }
                    else
                    {
                        _gl.ActiveTexture(GLEnum.Texture0 + i);
                        _gl.BindTexture(GLEnum.Texture2D, _textures[i].Id);
                        shaderProgram.SetUniform("material.diffuse", i);
                    }
                }
                else if (_textures[i].Type == TextureType.Specular)
                {
                    if (hasSpecular)
                    {
                        // Not support now.
                    }
                    else
                    {
                        _gl.ActiveTexture(GLEnum.Texture0 + i);
                        _gl.BindTexture(GLEnum.Texture2D, _textures[i].Id);
                        shaderProgram.SetUniform("material.specular", i);
                    }
                }
            }

            _gl.BindVertexArray(_vao);
            _gl.DrawArrays(GLEnum.Triangles, 0, (uint)_indices.Length);
        }
    }
}
