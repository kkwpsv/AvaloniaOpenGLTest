using Avalonia.Controls;
using AvaloniaOpenGLTest.LearnOpenGL.Helpers;
using Silk.NET.OpenGL;
using System;
using System.IO;
using System.Numerics;

namespace AvaloniaOpenGLTest.LearnOpenGL
{
    internal class Model
    {
        static Helpers.Model _model;
        static ShaderProgram _shaderProgram;
        static Helpers.Camera _camera;
        public unsafe static void Init(GL gl, Control control)
        {
            _model = new Helpers.Model(gl);
            _model.Load(Path.Combine("nanosuit", "nanosuit.obj"));

            _shaderProgram = new ShaderProgram(gl);
            _shaderProgram.Compile(VertexShaderString, FragmentShaderString);

            _camera = new Helpers.Camera(gl, control);
            _camera.CameraPos = new Vector3(0, 12, 20);
        }



        public unsafe static void Render(GL gl)
        {
            Span<int> viewports = stackalloc int[4];
            gl.GetInteger(GLEnum.Viewport, viewports);

            var view = _camera.GetViewMatrix();
            var projection = _camera.GetProjection((float)viewports[2] / viewports[3]);
            var lightPos = new Vector3(8f, 20f, 20f);

            _shaderProgram.Use();
            foreach (var mesh in _model.Meshes)
            {
                _shaderProgram.SetUniform("model", Matrix4x4.Identity);
                _shaderProgram.SetUniform("view", view);
                _shaderProgram.SetUniform("projection", projection);
                //_shaderProgram.SetUniform("material.diffuse", 0);
                //_shaderProgram.SetUniform("material.specular", 1);
                _shaderProgram.SetUniform("material.shininess", 32f);
                _shaderProgram.SetUniform("light.ambient", new Vector3(0.2f, 0.2f, 0.2f));
                _shaderProgram.SetUniform("light.diffuse", new Vector3(0.5f, 0.5f, 0.5f));
                _shaderProgram.SetUniform("light.specular", new Vector3(1.0f, 1.0f, 1.0f));
                _shaderProgram.SetUniform("light.position", lightPos);
                _shaderProgram.SetUniform("viewPos", _camera.CameraPos);

                mesh.Draw(_shaderProgram);
            }
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

                //FragColor = texture(material.diffuse, TexCoords);
            } 
            """;
    }
}
