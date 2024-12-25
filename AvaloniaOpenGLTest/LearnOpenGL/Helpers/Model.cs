using Silk.NET.Assimp;
using Silk.NET.OpenGL;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AvaloniaOpenGLTest.LearnOpenGL.Helpers
{
    public class Model
    {
        private readonly GL _gl;
        private readonly Assimp _assimp;
        private string _dir;
        public List<Mesh> Meshes { get; } = new List<Mesh>();
        private Dictionary<string, Texture> _pathToTextureDic = new Dictionary<string, Texture>();

        public Model(GL gl)
        {
            _gl = gl;
            _assimp = Assimp.GetApi();
            //StbImageSharp.StbImage.stbi_set_flip_vertically_on_load(1);
        }

        public void Load(string path)
        {
            unsafe
            {
                _dir = Path.GetDirectoryName(path);
                var scene = _assimp.ImportFile(path, (uint)(PostProcessSteps.Triangulate | PostProcessSteps.FlipUVs));

                if (scene == null || (scene->MFlags & (uint)SceneFlags.Incomplete) != 0 || scene->MRootNode == null)
                {
                    throw new InvalidOperationException($"Load model failed: {_assimp.GetErrorStringS()}");
                }

                ProcessNode(scene->MRootNode, scene);
            }
        }

        private unsafe void ProcessNode(Node* node, Scene* scene)
        {
            for (var i = 0; i < node->MNumMeshes; i++)
            {
                var mesh = scene->MMeshes[node->MMeshes[i]];
                Meshes.Add(ProcessMesh(scene, mesh));
            }

            for (var i = 0; i < node->MNumChildren; i++)
            {
                ProcessNode(node->MChildren[i], scene);
            }
        }

        private unsafe Mesh ProcessMesh(Scene* scene, Silk.NET.Assimp.Mesh* mesh)
        {
            List<Vertex> vertices = new List<Vertex>();
            List<uint> indices = new List<uint>();
            List<Texture> textures = new List<Texture>();

            for (var i = 0; i < mesh->MNumVertices; i++)
            {
                Vertex vertex = new Vertex();

                vertex.Position = mesh->MVertices[i];
                vertex.Normal = mesh->MNormals[i];
                if (mesh->MTextureCoords[0] != null)
                {
                    vertex.TexCoords.X = mesh->MTextureCoords[0][i].X;
                    vertex.TexCoords.Y = mesh->MTextureCoords[0][i].Y;
                }

                vertices.Add(vertex);
            }

            for (var i = 0; i < mesh->MNumFaces; i++)
            {
                for (var j = 0; j < mesh->MFaces[i].MNumIndices; j++)
                {
                    indices.Add(mesh->MFaces[i].MIndices[j]);
                }
            }

            if (mesh->MMaterialIndex >= 0)
            {
                var material = scene->MMaterials[mesh->MMaterialIndex];
                textures = LoadTexture(material);
            }

            return new Mesh(_gl, vertices.ToArray(), indices.ToArray(), textures.ToArray());
        }

        private unsafe List<Texture> LoadTexture(Material* material)
        {
            List<Texture> result = new List<Texture>();
            for (var i = 0u; i < _assimp.GetMaterialTextureCount(material, Silk.NET.Assimp.TextureType.Diffuse); i++)
            {
                AssimpString path;
                _assimp.GetMaterialTexture(material, Silk.NET.Assimp.TextureType.Diffuse, i, &path, null, null, null, null, null, null);
                var pathStr = path.AsString;
                if (_pathToTextureDic.ContainsKey(pathStr))
                {
                    result.Add(_pathToTextureDic[pathStr]);
                }
                else
                {
                    var texture = LoadTextureFromFile(pathStr);
                    texture.Type = TextureType.Diffuse;
                    _pathToTextureDic[pathStr] = texture;
                    result.Add(texture);
                }
            }
            for (var i = 0u; i < _assimp.GetMaterialTextureCount(material, Silk.NET.Assimp.TextureType.Specular); i++)
            {
                AssimpString path;
                _assimp.GetMaterialTexture(material, Silk.NET.Assimp.TextureType.Specular, i, &path, null, null, null, null, null, null);
                var pathStr = path.AsString;
                if (_pathToTextureDic.ContainsKey(pathStr))
                {
                    result.Add(_pathToTextureDic[pathStr]);
                }
                else
                {
                    var texture = LoadTextureFromFile(pathStr);
                    texture.Type = TextureType.Specular;
                    _pathToTextureDic[pathStr] = texture;
                    result.Add(texture);
                }
            }
            return result;
        }

        private unsafe Texture LoadTextureFromFile(string pathStr)
        {
            var id = _gl.GenTexture();
            var imageResult = StbImageSharp.ImageResult.FromMemory(System.IO.File.ReadAllBytes(Path.Combine(_dir, pathStr)), StbImageSharp.ColorComponents.RedGreenBlueAlpha);
            fixed (byte* ptr = imageResult.Data)
            {
                _gl.BindTexture(GLEnum.Texture2D, id);
                _gl.TexParameter(GLEnum.Texture2D, GLEnum.TextureMinFilter, (int)TextureMinFilter.Nearest);
                _gl.TexParameter(GLEnum.Texture2D, GLEnum.TextureMagFilter, (int)TextureMagFilter.Nearest);
                _gl.TexImage2D(GLEnum.Texture2D, 0, InternalFormat.Rgba, (uint)imageResult.Width, (uint)imageResult.Height, 0, GLEnum.Rgba, GLEnum.UnsignedByte, ptr);
            }
            return new Texture { Id = id };
        }
    }
}
