using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using Assimp;
using StbImageSharp;

namespace Open_TK
{
    public class Model : GameObject
    {
        public Model(Vector3 startPosition) : base() {
            Position = startPosition;
            base.vertices = new List<Vector3>();
            base.texCoords = new List<Vector2>();
        }

        public void LoadModelFromFile(string path) {
            AssimpContext importer = new AssimpContext();
            Scene scene = importer.ImportFile(path, PostProcessSteps.Triangulate | PostProcessSteps.GenerateNormals | PostProcessSteps.GenerateUVCoords);

            Mesh mesh = scene.Meshes[0];

            List<uint> indicesList = new List<uint>();

            for (int i = 0; i < mesh.VertexCount; i++)
            {
                vertices.Add(new Vector3(mesh.Vertices[i].X, mesh.Vertices[i].Y, mesh.Vertices[i].Z));
                if (mesh.HasTextureCoords(0))
                {
                    texCoords.Add(new Vector2(mesh.TextureCoordinateChannels[0][i].X, mesh.TextureCoordinateChannels[0][i].Y));
                }
                else
                {
                    texCoords.Add(new Vector2(0.0f, 0.0f));
                }
            }

            for (int i = 0; i < mesh.FaceCount; i++)
            {
                Face face = mesh.Faces[i];
                for (int j = 0; j < face.IndexCount; j++)
                {
                    indicesList.Add((uint)face.Indices[j]);
                }
            }
            base.indices = indicesList.ToArray();
        }

        public void Initialize() {
            InitializeInternal();
        }

        public void LoadTexture(string path) {
            LoadTextureInternal(path, TextureUnit.Texture0, false);
        }

        public override void Render(Shader shader) {
            Matrix4 model = Matrix4.CreateTranslation(Position);
            int modelLocation = GL.GetUniformLocation(shader.shaderHandle, "model");
            GL.UniformMatrix4(modelLocation, true, ref model);

            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, TextureId);

            GL.BindVertexArray(Vao);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, Ebo);
            GL.DrawElements(OpenTK.Graphics.OpenGL4.PrimitiveType.Triangles, indices.Length, DrawElementsType.UnsignedInt, 0);

            GL.BindVertexArray(0);
        }

        public override void CleanUp() {
            base.CleanUp();
        }
    }
}