using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using StbImageSharp;

namespace Open_TK
{
    public abstract class GameObject
    {
        public Vector3 Position;
        protected int Vao;
        protected int Vbo;
        protected int Ebo;
        protected int TextureId;

        public List<Vector3> vertices;
        public List<Vector2> texCoords;
        public uint[] indices;
        public int textureVbo;
        public abstract Matrix4 GetModelMatrix();
        public virtual void Render(Shader shader) {
            Matrix4 model = GetModelMatrix();
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

        protected void LoadTextureInternal(string path, TextureUnit textureUnit = TextureUnit.Texture0, bool flipVertical = false) {
            TextureId = GL.GenTexture();
            GL.ActiveTexture(textureUnit);
            GL.BindTexture(TextureTarget.Texture2D, TextureId);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)OpenTK.Graphics.OpenGL4.TextureWrapMode.Repeat);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)OpenTK.Graphics.OpenGL4.TextureWrapMode.Repeat);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);
            StbImage.stbi_set_flip_vertically_on_load(1);
            ImageResult texture = ImageResult.FromStream(File.OpenRead(path), ColorComponents.RedGreenBlueAlpha);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, texture.Width, texture.Height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, texture.Data);
            GL.BindTexture(TextureTarget.Texture2D, 0);
        }

        protected void InitializeInternal() {
            Vao = GL.GenVertexArray();
            GL.BindVertexArray(Vao);

            Vbo = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, Vbo);
            GL.BufferData(BufferTarget.ArrayBuffer, vertices.Count * OpenTK.Mathematics.Vector3.SizeInBytes, vertices.ToArray(), BufferUsageHint.StaticDraw);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 0, 0);
            GL.EnableVertexAttribArray(0);

            textureVbo = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, textureVbo);
            GL.BufferData(BufferTarget.ArrayBuffer, texCoords.Count * OpenTK.Mathematics.Vector2.SizeInBytes, texCoords.ToArray(), BufferUsageHint.StaticDraw); // Используем sphereTexCoords напрямую
            GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, 0, 0);
            GL.EnableVertexAttribArray(1);

            Ebo = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, Ebo);
            GL.BufferData(BufferTarget.ElementArrayBuffer, indices.Length * sizeof(uint), indices, BufferUsageHint.StaticDraw);

            GL.BindVertexArray(0);
        }
        public virtual void CleanUp() {
            if (Vao != 0) GL.DeleteVertexArray(Vao);
            if (Vbo != 0) GL.DeleteBuffer(Vbo);
            if (textureVbo != 0) GL.DeleteBuffer(textureVbo);
            if (Ebo != 0) GL.DeleteBuffer(Ebo);
            if (TextureId != 0) GL.DeleteTexture(TextureId);
        }
    }
}