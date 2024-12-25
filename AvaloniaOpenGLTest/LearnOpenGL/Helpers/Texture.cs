using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AvaloniaOpenGLTest.LearnOpenGL.Helpers
{
    public struct Texture
    {
        public uint Id;
        public TextureType Type;
    }

    public enum TextureType
    {
        Diffuse,
        Specular,
    }
}
