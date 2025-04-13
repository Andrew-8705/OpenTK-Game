using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using StbImageSharp;


namespace Open_TK
{
    public class Sphere : GameObject
    {
        float radius;
        int horizontalSegments;
        int verticalSegments;

        public float RotationX = 0f;
        public float RotationY = 0f;

        public Sphere(Vector3 startPosition, float radius = 0.5f, int horizontalSegments = 36, int verticalSegments = 18) : base() {
            Position = startPosition;
            this.radius = radius;
            this.horizontalSegments = horizontalSegments;
            this.verticalSegments = verticalSegments;
            GenerateSphereData();
        }

        private void GenerateSphereData() {
            List<Vector3> verticesList = new List<Vector3>();
            List<uint> indicesList = new List<uint>();
            List<Vector2> texCoordsList = new List<Vector2>();

            for (int i = 0; i <= verticalSegments; i++)
            {
                float theta = i * MathF.PI / verticalSegments;

                for (int j = 0; j <= horizontalSegments; j++)
                {
                    float phi = j * 2 * MathF.PI / horizontalSegments;

                    float x = radius * MathF.Cos(phi) * MathF.Sin(theta);
                    float y = radius * MathF.Cos(theta);
                    float z = radius * MathF.Sin(phi) * MathF.Sin(theta);

                    verticesList.Add(new Vector3(x, y, z));
                    texCoordsList.Add(new Vector2((float)j / horizontalSegments, (float)i / verticalSegments));
                }
            }

            for (int i = 0; i < verticalSegments; i++)
            {
                for (int j = 0; j < horizontalSegments; j++)
                {
                    uint first = (uint)((i * (horizontalSegments + 1)) + j);
                    uint second = (uint)(first + horizontalSegments + 1);

                    indicesList.Add(first);
                    indicesList.Add(second);
                    indicesList.Add(first + 1);

                    indicesList.Add(second);
                    indicesList.Add(second + 1);
                    indicesList.Add(first + 1);
                }
            }

            base.vertices = verticesList;
            base.indices = indicesList.ToArray();
            base.texCoords = texCoordsList;
        }

        public void Initialize() {
            InitializeInternal();
        }

        public void LoadTexture(string path) {
            LoadTextureInternal(path, TextureUnit.Texture0, false);
        }

        public override void Render(Shader shader) {
            Matrix4 translation = Matrix4.CreateTranslation(Position);
            Matrix4 rotationX = Matrix4.CreateRotationX(MathHelper.DegreesToRadians(RotationX));
            Matrix4 rotationY = Matrix4.CreateRotationY(MathHelper.DegreesToRadians(RotationY));

            Matrix4 model = rotationY * rotationX * translation;

            int modelLocation = GL.GetUniformLocation(shader.shaderHandle, "model");
            GL.UniformMatrix4(modelLocation, true, ref model);

            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, TextureId);
            int textureSelectorLocation = GL.GetUniformLocation(shader.shaderHandle, "textureSelector");
            GL.Uniform1(textureSelectorLocation, 0);

            GL.BindVertexArray(Vao);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, Ebo);
            GL.DrawElements(PrimitiveType.Triangles, indices.Length, DrawElementsType.UnsignedInt, 0);
            GL.BindVertexArray(0);
        }
        public override void CleanUp() {
            base.CleanUp();
        }
    }
}