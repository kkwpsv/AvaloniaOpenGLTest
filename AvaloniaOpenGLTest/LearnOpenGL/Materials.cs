using Avalonia.Controls;
using AvaloniaOpenGLTest.LearnOpenGL.Helpers;
using Silk.NET.OpenGL;
using System;
using System.IO;
using System.Numerics;

namespace AvaloniaOpenGLTest.LearnOpenGL
{
    internal class Materials
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
                -0.5f, -0.5f, -0.5f,  0.0f,  0.0f, -1.0f,
                 0.5f, -0.5f, -0.5f,  0.0f,  0.0f, -1.0f,
                 0.5f,  0.5f, -0.5f,  0.0f,  0.0f, -1.0f,
                 0.5f,  0.5f, -0.5f,  0.0f,  0.0f, -1.0f,
                -0.5f,  0.5f, -0.5f,  0.0f,  0.0f, -1.0f,
                -0.5f, -0.5f, -0.5f,  0.0f,  0.0f, -1.0f,

                -0.5f, -0.5f,  0.5f,  0.0f,  0.0f, 1.0f,
                 0.5f, -0.5f,  0.5f,  0.0f,  0.0f, 1.0f,
                 0.5f,  0.5f,  0.5f,  0.0f,  0.0f, 1.0f,
                 0.5f,  0.5f,  0.5f,  0.0f,  0.0f, 1.0f,
                -0.5f,  0.5f,  0.5f,  0.0f,  0.0f, 1.0f,
                -0.5f, -0.5f,  0.5f,  0.0f,  0.0f, 1.0f,

                -0.5f,  0.5f,  0.5f, -1.0f,  0.0f,  0.0f,
                -0.5f,  0.5f, -0.5f, -1.0f,  0.0f,  0.0f,
                -0.5f, -0.5f, -0.5f, -1.0f,  0.0f,  0.0f,
                -0.5f, -0.5f, -0.5f, -1.0f,  0.0f,  0.0f,
                -0.5f, -0.5f,  0.5f, -1.0f,  0.0f,  0.0f,
                -0.5f,  0.5f,  0.5f, -1.0f,  0.0f,  0.0f,

                 0.5f,  0.5f,  0.5f,  1.0f,  0.0f,  0.0f,
                 0.5f,  0.5f, -0.5f,  1.0f,  0.0f,  0.0f,
                 0.5f, -0.5f, -0.5f,  1.0f,  0.0f,  0.0f,
                 0.5f, -0.5f, -0.5f,  1.0f,  0.0f,  0.0f,
                 0.5f, -0.5f,  0.5f,  1.0f,  0.0f,  0.0f,
                 0.5f,  0.5f,  0.5f,  1.0f,  0.0f,  0.0f,

                -0.5f, -0.5f, -0.5f,  0.0f, -1.0f,  0.0f,
                 0.5f, -0.5f, -0.5f,  0.0f, -1.0f,  0.0f,
                 0.5f, -0.5f,  0.5f,  0.0f, -1.0f,  0.0f,
                 0.5f, -0.5f,  0.5f,  0.0f, -1.0f,  0.0f,
                -0.5f, -0.5f,  0.5f,  0.0f, -1.0f,  0.0f,
                -0.5f, -0.5f, -0.5f,  0.0f, -1.0f,  0.0f,

                -0.5f,  0.5f, -0.5f,  0.0f,  1.0f,  0.0f,
                 0.5f,  0.5f, -0.5f,  0.0f,  1.0f,  0.0f,
                 0.5f,  0.5f,  0.5f,  0.0f,  1.0f,  0.0f,
                 0.5f,  0.5f,  0.5f,  0.0f,  1.0f,  0.0f,
                -0.5f,  0.5f,  0.5f,  0.0f,  1.0f,  0.0f,
                -0.5f,  0.5f, -0.5f,  0.0f,  1.0f,  0.0f
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

            _lightVao = gl.GenVertexArray();
            gl.BindVertexArray(_lightVao);
            gl.VertexAttribPointer(0, 3, GLEnum.Float, false, 6 * sizeof(float), 0);
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

            var lightPos = new Vector3(1.2f, 1.0f, 2.0f);

            var view = _camera.GetViewMatrix();
            var projection = _camera.GetProjection((float)viewports[2] / viewports[3]);

            _shaderProgram.SetUniform("model", Matrix4x4.Identity);
            _shaderProgram.SetUniform("view", view);
            _shaderProgram.SetUniform("projection", projection);
            _shaderProgram.SetUniform("material.ambient", new Vector3(1.0f, 0.5f, 0.31f));
            _shaderProgram.SetUniform("material.diffuse", new Vector3(1.0f, 0.5f, 0.31f));
            _shaderProgram.SetUniform("material.specular", new Vector3(0.5f, 0.5f, 0.5f));
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

            out vec3 FragPos;
            out vec3 Normal;

            uniform mat4 model;
            uniform mat4 view;
            uniform mat4 projection;

            void main()
            {
                gl_Position = projection * view * model * vec4(aPos, 1.0);
                FragPos = vec3(model * vec4(aPos, 1.0));
                Normal = mat3(transpose(inverse(model))) * aNormal;
                //Normal = aNormal;
            }
            """;

        static string FragmentShaderString = """
            #version 330 core
            struct Material {
                vec3 ambient;
                vec3 diffuse;
                vec3 specular;
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

            out vec4 FragColor;

            uniform vec3 viewPos;
            uniform Material material;
            uniform Light light;

            void main()
            {
                // ambient
                vec3 ambient = material.ambient * light.ambient;

                // diffuse 
                vec3 norm = normalize(Normal);
                vec3 lightDir = normalize(light.position - FragPos);
                float diff = max(dot(norm, lightDir), 0.0);
                vec3 diffuse = (diff * material.diffuse) * light.diffuse;

                // specular
                vec3 viewDir = normalize(viewPos - FragPos);
                vec3 reflectDir = reflect(-lightDir, norm);  
                float spec = pow(max(dot(viewDir, reflectDir), 0.0), material.shininess);
                vec3 specular = material.specular * spec * light.specular;  

                vec3 result = ambient + diffuse + specular;
                FragColor = vec4(result, 1.0);
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
