using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using StbImageSharp;

namespace Open_TK
{
    public class Obstacle : GameObject
    {
        public Obstacle(Vector3 startPosition) : base() {
            Position = startPosition;
            base.vertices = new List<Vector3>()
            {
                // Front face
                new Vector3(-0.5f, -0.5f,  0.5f),
                new Vector3( 0.5f, -0.5f,  0.5f),
                new Vector3( 0.5f,  0.5f,  0.5f),
                new Vector3(-0.5f,  0.5f,  0.5f),

                // Back face
                new Vector3(-0.5f, -0.5f, -0.5f),
                new Vector3( 0.5f, -0.5f, -0.5f),
                new Vector3( 0.5f,  0.5f, -0.5f),
                new Vector3(-0.5f,  0.5f, -0.5f),

                // Left face
                new Vector3(-0.5f,  0.5f,  0.5f),
                new Vector3(-0.5f,  0.5f, -0.5f),
                new Vector3(-0.5f, -0.5f, -0.5f),
                new Vector3(-0.5f, -0.5f,  0.5f),

                // Right face
                new Vector3( 0.5f,  0.5f,  0.5f),
                new Vector3( 0.5f,  0.5f, -0.5f),
                new Vector3( 0.5f, -0.5f, -0.5f),
                new Vector3( 0.5f, -0.5f,  0.5f),

                // Bottom face
                new Vector3(-0.5f, -0.5f, -0.5f),
                new Vector3( 0.5f, -0.5f, -0.5f),
                new Vector3( 0.5f, -0.5f,  0.5f),
                new Vector3(-0.5f, -0.5f,  0.5f),

                // Top face
                new Vector3(-0.5f,  0.5f, -0.5f),
                new Vector3( 0.5f,  0.5f, -0.5f),
                new Vector3( 0.5f,  0.5f,  0.5f),
                new Vector3(-0.5f,  0.5f,  0.5f)
            };

            base.texCoords = new List<Vector2>()
            {
                // Front
                new Vector2(0.0f, 0.0f),
                new Vector2(1.0f, 0.0f),
                new Vector2(1.0f, 1.0f),
                new Vector2(0.0f, 1.0f),

                // Back
                new Vector2(1.0f, 0.0f),
                new Vector2(0.0f, 0.0f),
                new Vector2(0.0f, 1.0f),
                new Vector2(1.0f, 1.0f),

                // Left
                new Vector2(0.0f, 0.0f),
                new Vector2(1.0f, 0.0f),
                new Vector2(1.0f, 1.0f),
                new Vector2(0.0f, 1.0f),

                // Right
                new Vector2(1.0f, 0.0f),
                new Vector2(0.0f, 0.0f),
                new Vector2(0.0f, 1.0f),
                new Vector2(1.0f, 1.0f),

                // Bottom
                new Vector2(0.0f, 0.0f),
                new Vector2(1.0f, 0.0f),
                new Vector2(1.0f, 1.0f),
                new Vector2(0.0f, 1.0f),

                // Top
                new Vector2(0.0f, 0.0f),
                new Vector2(1.0f, 0.0f),
                new Vector2(1.0f, 1.0f),
                new Vector2(0.0f, 1.0f)
            };

            base.indices = new uint[]
            {
                0, 1, 2, 2, 3, 0, // Front
                4, 5, 6, 6, 7, 4, // Back
                8, 9, 10, 10, 11, 8, // Left
                12, 13, 14, 14, 15, 12, // Right
                16, 17, 18, 18, 19, 16, // Bottom
                20, 21, 22, 22, 23, 20  // Top
            };
        }

        public void Initialize() {
            InitializeInternal();
        }

        public void LoadTexture(string path) {
            LoadTextureInternal(path, TextureUnit.Texture1, true);
        }

        public override void Render(Shader shader) {
            Matrix4 model = Matrix4.CreateTranslation(Position);
            int modelLocation = GL.GetUniformLocation(shader.shaderHandle, "model");
            GL.UniformMatrix4(modelLocation, true, ref model);

            GL.ActiveTexture(TextureUnit.Texture1);
            GL.BindTexture(TextureTarget.Texture2D, TextureId);
            int textureSelectorLocation = GL.GetUniformLocation(shader.shaderHandle, "textureSelector");
            GL.Uniform1(textureSelectorLocation, 1);

            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, TextureId);
            GL.Uniform1(textureSelectorLocation, 1);

            GL.BindVertexArray(Vao);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, Ebo);
            GL.DrawElements(OpenTK.Graphics.OpenGL4.PrimitiveType.Triangles, indices.Length, DrawElementsType.UnsignedInt, 0);
        }

        public override void CleanUp() {
            base.CleanUp();
        }
    }
}