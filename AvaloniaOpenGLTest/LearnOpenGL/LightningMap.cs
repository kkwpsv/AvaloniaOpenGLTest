using Avalonia.Controls;
using AvaloniaOpenGLTest.LearnOpenGL.Helpers;
using Silk.NET.OpenGL;
using System;
using System.IO;
using System.Numerics;

namespace AvaloniaOpenGLTest.LearnOpenGL
{
    internal class LightningMap
    {
        static uint _vao;
        static uint _lightVao;
        static uint _vbo;
        static uint _diffuseMap;
        static uint _specularMap;

        static ShaderProgram _shaderProgram;
        static ShaderProgram _lightShaderProgram;
        static Helpers.Camera _camera;
        public unsafe static void Init(GL gl, Control control)
        {
            Span<float> vertices = [
                // positions          // normals           // texture coords
                -0.5f, -0.5f, -0.5f,  0.0f,  0.0f, -1.0f,  0.0f, 0.0f,
                 0.5f, -0.5f, -0.5f,  0.0f,  0.0f, -1.0f,  1.0f, 0.0f,
                 0.5f,  0.5f, -0.5f,  0.0f,  0.0f, -1.0f,  1.0f, 1.0f,
                 0.5f,  0.5f, -0.5f,  0.0f,  0.0f, -1.0f,  1.0f, 1.0f,
                -0.5f,  0.5f, -0.5f,  0.0f,  0.0f, -1.0f,  0.0f, 1.0f,
                -0.5f, -0.5f, -0.5f,  0.0f,  0.0f, -1.0f,  0.0f, 0.0f,

                -0.5f, -0.5f,  0.5f,  0.0f,  0.0f, 1.0f,   0.0f, 0.0f,
                 0.5f, -0.5f,  0.5f,  0.0f,  0.0f, 1.0f,   1.0f, 0.0f,
                 0.5f,  0.5f,  0.5f,  0.0f,  0.0f, 1.0f,   1.0f, 1.0f,
                 0.5f,  0.5f,  0.5f,  0.0f,  0.0f, 1.0f,   1.0f, 1.0f,
                -0.5f,  0.5f,  0.5f,  0.0f,  0.0f, 1.0f,   0.0f, 1.0f,
                -0.5f, -0.5f,  0.5f,  0.0f,  0.0f, 1.0f,   0.0f, 0.0f,

                -0.5f,  0.5f,  0.5f, -1.0f,  0.0f,  0.0f,  1.0f, 0.0f,
                -0.5f,  0.5f, -0.5f, -1.0f,  0.0f,  0.0f,  1.0f, 1.0f,
                -0.5f, -0.5f, -0.5f, -1.0f,  0.0f,  0.0f,  0.0f, 1.0f,
                -0.5f, -0.5f, -0.5f, -1.0f,  0.0f,  0.0f,  0.0f, 1.0f,
                -0.5f, -0.5f,  0.5f, -1.0f,  0.0f,  0.0f,  0.0f, 0.0f,
                -0.5f,  0.5f,  0.5f, -1.0f,  0.0f,  0.0f,  1.0f, 0.0f,

                 0.5f,  0.5f,  0.5f,  1.0f,  0.0f,  0.0f,  1.0f, 0.0f,
                 0.5f,  0.5f, -0.5f,  1.0f,  0.0f,  0.0f,  1.0f, 1.0f,
                 0.5f, -0.5f, -0.5f,  1.0f,  0.0f,  0.0f,  0.0f, 1.0f,
                 0.5f, -0.5f, -0.5f,  1.0f,  0.0f,  0.0f,  0.0f, 1.0f,
                 0.5f, -0.5f,  0.5f,  1.0f,  0.0f,  0.0f,  0.0f, 0.0f,
                 0.5f,  0.5f,  0.5f,  1.0f,  0.0f,  0.0f,  1.0f, 0.0f,

                -0.5f, -0.5f, -0.5f,  0.0f, -1.0f,  0.0f,  0.0f, 1.0f,
                 0.5f, -0.5f, -0.5f,  0.0f, -1.0f,  0.0f,  1.0f, 1.0f,
                 0.5f, -0.5f,  0.5f,  0.0f, -1.0f,  0.0f,  1.0f, 0.0f,
                 0.5f, -0.5f,  0.5f,  0.0f, -1.0f,  0.0f,  1.0f, 0.0f,
                -0.5f, -0.5f,  0.5f,  0.0f, -1.0f,  0.0f,  0.0f, 0.0f,
                -0.5f, -0.5f, -0.5f,  0.0f, -1.0f,  0.0f,  0.0f, 1.0f,

                -0.5f,  0.5f, -0.5f,  0.0f,  1.0f,  0.0f,  0.0f, 1.0f,
                 0.5f,  0.5f, -0.5f,  0.0f,  1.0f,  0.0f,  1.0f, 1.0f,
                 0.5f,  0.5f,  0.5f,  0.0f,  1.0f,  0.0f,  1.0f, 0.0f,
                 0.5f,  0.5f,  0.5f,  0.0f,  1.0f,  0.0f,  1.0f, 0.0f,
                -0.5f,  0.5f,  0.5f,  0.0f,  1.0f,  0.0f,  0.0f, 0.0f,
                -0.5f,  0.5f, -0.5f,  0.0f,  1.0f,  0.0f,  0.0f, 1.0f
            ];

            _vao = gl.GenVertexArray();
            gl.BindVertexArray(_vao);

            _vbo = gl.GenBuffer();
            gl.BindBuffer(BufferTargetARB.ArrayBuffer, _vbo);
            gl.BufferData<float>(BufferTargetARB.ArrayBuffer, vertices, BufferUsageARB.StaticDraw);

            gl.VertexAttribPointer(0, 3, GLEnum.Float, false, 8 * sizeof(float), 0);
            gl.EnableVertexAttribArray(0);
            gl.VertexAttribPointer(1, 3, GLEnum.Float, false, 8 * sizeof(float), 3 * sizeof(float));
            gl.EnableVertexAttribArray(1);
            gl.VertexAttribPointer(2, 2, GLEnum.Float, false, 8 * sizeof(float), 6 * sizeof(float));
            gl.EnableVertexAttribArray(2);

            _lightVao = gl.GenVertexArray();
            gl.BindVertexArray(_lightVao);
            gl.VertexAttribPointer(0, 3, GLEnum.Float, false, 8 * sizeof(float), 0);
            gl.EnableVertexAttribArray(0);

            gl.BindVertexArray(0);
            gl.BindBuffer(BufferTargetARB.ArrayBuffer, 0);

            StbImageSharp.StbImage.stbi_set_flip_vertically_on_load(1);
            var imageResult = StbImageSharp.ImageResult.FromMemory(File.ReadAllBytes("container2.png"), StbImageSharp.ColorComponents.RedGreenBlue);
            fixed (byte* ptr = imageResult.Data)
            {
                _diffuseMap = gl.GenTexture();
                gl.BindTexture(GLEnum.Texture2D, _diffuseMap);
                gl.TexParameter(GLEnum.Texture2D, GLEnum.TextureMinFilter, (int)TextureMinFilter.Nearest);
                gl.TexParameter(GLEnum.Texture2D, GLEnum.TextureMagFilter, (int)TextureMagFilter.Nearest);
                gl.TexImage2D(GLEnum.Texture2D, 0, InternalFormat.Rgb, (uint)imageResult.Width, (uint)imageResult.Height, 0, GLEnum.Rgb, GLEnum.UnsignedByte, ptr);
            }
            imageResult = StbImageSharp.ImageResult.FromMemory(File.ReadAllBytes("container2_specular.png"), StbImageSharp.ColorComponents.RedGreenBlue);
            fixed (byte* ptr = imageResult.Data)
            {
                _specularMap = gl.GenTexture();
                gl.BindTexture(GLEnum.Texture2D, _specularMap);
                gl.TexParameter(GLEnum.Texture2D, GLEnum.TextureMinFilter, (int)TextureMinFilter.Nearest);
                gl.TexParameter(GLEnum.Texture2D, GLEnum.TextureMagFilter, (int)TextureMagFilter.Nearest);
                gl.TexImage2D(GLEnum.Texture2D, 0, InternalFormat.Rgb, (uint)imageResult.Width, (uint)imageResult.Height, 0, GLEnum.Rgb, GLEnum.UnsignedByte, ptr);
            }

            _shaderProgram = new ShaderProgram(gl);
            _shaderProgram.Compile(VertexShaderString, FragmentShaderString);

            _lightShaderProgram = new ShaderProgram(gl);
            _lightShaderProgram.Compile(VertexShaderString, LightFragmentShaderString);

            _camera = new Helpers.Camera(gl, control);
            _camera.CameraPos = new Vector3(0, 0, 5);
        }



        public unsafe static void Render(GL gl)
        {
            Span<int> viewports = stackalloc int[4];
            gl.GetInteger(GLEnum.Viewport, viewports);

            _shaderProgram.Use();
            gl.BindVertexArray(_vao);
            gl.ActiveTexture(GLEnum.Texture0);
            gl.BindTexture(GLEnum.Texture2D, _diffuseMap);
            gl.ActiveTexture(GLEnum.Texture1);
            gl.BindTexture(GLEnum.Texture2D, _specularMap);

            var lightPos = new Vector3(1.2f, 1.0f, 2.0f);

            var view = _camera.GetViewMatrix();
            var projection = _camera.GetProjection((float)viewports[2] / viewports[3]);

            _shaderProgram.SetUniform("model", Matrix4x4.Identity);
            _shaderProgram.SetUniform("view", view);
            _shaderProgram.SetUniform("projection", projection);
            _shaderProgram.SetUniform("material.diffuse", 0);
            _shaderProgram.SetUniform("material.specular", 1);
            _shaderProgram.SetUniform("material.shininess", 32f);
            _shaderProgram.SetUniform("light.ambient", new Vector3(0.2f, 0.2f, 0.2f));
            _shaderProgram.SetUniform("light.diffuse", new Vector3(0.5f, 0.5f, 0.5f));
            _shaderProgram.SetUniform("light.specular", new Vector3(1.0f, 1.0f, 1.0f));
            _shaderProgram.SetUniform("light.position", lightPos);
            _shaderProgram.SetUniform("viewPos", _camera.CameraPos);
            gl.DrawArrays(GLEnum.Triangles, 0, 36);

            _lightShaderProgram.Use();
            gl.BindVertexArray(_lightVao);

            _lightShaderProgram.SetUniform("model", Matrix4x4.CreateScale(0.2f) * Matrix4x4.CreateTranslation(lightPos));
            _lightShaderProgram.SetUniform("view", view);
            _lightShaderProgram.SetUniform("projection", projection);
            gl.DrawArrays(GLEnum.Triangles, 0, 36);

        }

        static string VertexShaderString = """
            #version 330 core
            layout (location = 0) in vec3 aPos;
            layout (location = 1) in vec3 aNormal;
            layout (location = 2) in vec2 aTexCoords;

            out vec3 FragPos;
            out vec3 Normal;
            out vec2 TexCoords;

            uniform mat4 model;
            uniform mat4 view;
            uniform mat4 projection;

            void main()
            {
                gl_Position = projection * view * model * vec4(aPos, 1.0);
                FragPos = vec3(model * vec4(aPos, 1.0));
                Normal = mat3(transpose(inverse(model))) * aNormal;
                //Normal = aNormal;
                TexCoords = aTexCoords;
            }
            """;

        static string FragmentShaderString = """
            #version 330 core
            struct Material {
                sampler2D diffuse;
                sampler2D specular;
                float shininess;
            }; 

            struct Light {
                vec3 position;
                vec3 ambient;
                vec3 diffuse;
                vec3 specular;
            };

            in vec3 Normal;
            in vec3 FragPos;
            in vec2 TexCoords;

            out vec4 FragColor;

            uniform vec3 viewPos;
            uniform Material material;
            uniform Light light;

            void main()
            {
                // ambient
                vec3 ambient = vec3(texture(material.diffuse, TexCoords)) * light.ambient;

                // diffuse 
                vec3 norm = normalize(Normal);
                vec3 lightDir = normalize(light.position - FragPos);
                float diff = max(dot(norm, lightDir), 0.0);
                vec3 diffuse = diff * vec3(texture(material.diffuse, TexCoords)) * light.diffuse;

                // specular
                vec3 viewDir = normalize(viewPos - FragPos);
                vec3 reflectDir = reflect(-lightDir, norm);  
                float spec = pow(max(dot(viewDir, reflectDir), 0.0), material.shininess);
                vec3 specular = vec3(texture(material.specular, TexCoords)) * spec * light.specular;  

                FragColor = vec4(ambient + diffuse + specular, 1.0);
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
