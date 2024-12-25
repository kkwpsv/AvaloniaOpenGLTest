using Silk.NET.OpenGL;
using System;
using System.Numerics;

namespace AvaloniaOpenGLTest.LearnOpenGL.Helpers
{
    public class ShaderProgram : IDisposable
    {
        private readonly GL _gl;
        private uint _shaderProgram;

        public ShaderProgram(GL gl)
        {
            _gl = gl;
        }

        public void Compile(string vertexShaderString, string fragmentShaderString)
        {
            if (_shaderProgram != 0)
            {
                throw new InvalidOperationException("Already compiled");
            }


            var vertexShader = _gl.CreateShader(GLEnum.VertexShader);
            var fragmentShader = _gl.CreateShader(GLEnum.FragmentShader);
            var shaderProgram = _gl.CreateProgram();

            try
            {
                _gl.ShaderSource(vertexShader, vertexShaderString);
                _gl.CompileShader(vertexShader);

                if (_gl.GetShader(vertexShader, GLEnum.CompileStatus) == 0)
                {
                    throw new InvalidOperationException("Compile Vertex Shader Failed.");
                }


                _gl.ShaderSource(fragmentShader, fragmentShaderString);
                _gl.CompileShader(fragmentShader);

                if (_gl.GetShader(fragmentShader, GLEnum.CompileStatus) == 0)
                {
                    throw new InvalidOperationException("Compile Fragment Shader Failed.");
                }

                _gl.AttachShader(shaderProgram, vertexShader);
                _gl.AttachShader(shaderProgram, fragmentShader);
                _gl.LinkProgram(shaderProgram);

                if (_gl.GetProgram(shaderProgram, GLEnum.LinkStatus) == 0)
                {
                    _gl.DeleteProgram(shaderProgram);
                    throw new InvalidOperationException("Link program Failed.");
                }
            }
            finally
            {
                _gl.DeleteShader(vertexShader);
                _gl.DeleteShader(fragmentShader);
            }

            _shaderProgram = shaderProgram;
        }

        public void Dispose()
        {
            _gl.DeleteProgram(_shaderProgram);
        }

        public void Use()
        {
            _gl.UseProgram(_shaderProgram);
        }

        internal void SetUniform(string name, int value)
        {
            var location = _gl.GetUniformLocation(_shaderProgram, name);
            _gl.Uniform1(location, value);
        }

        internal void SetUniform(string name, float value)
        {
            var location = _gl.GetUniformLocation(_shaderProgram, name);
            _gl.Uniform1(location, value);
        }

        internal void SetUniform(string name, Vector3 value)
        {
            var location = _gl.GetUniformLocation(_shaderProgram, name);
            _gl.Uniform3(location, value);
        }

        internal unsafe void SetUniform(string name, Matrix4x4 value)
        {
            var location = _gl.GetUniformLocation(_shaderProgram, name);

            _gl.UniformMatrix4(location, 1, false, (float*)&value);
        }
    }
}
