using OpenTK.Mathematics;
using OpenTK.Graphics.OpenGL4;

namespace Open_TK
{
    public class Coin : Model
    {
        public float MoveSpeedZ = 3.0f; // Cкорость движения к игроку
        public float RotationY = 0f; 
        public float RotationSpeed = 0.01f; // Cкорость вращения 

        private float verticalSpeed = 2.0f; // Cкорость вертикального покачивания
        private float verticalAmplitude = 0.5f; // Амплитуда вертикального покачивания
        private float verticalOffset = 0f; // Текущее смещение по вертикали

        private float time = 0f; // Внутренний таймер для эффекта покачивания
        public void UpdateRotation(float deltaTime) {
            RotationY += RotationSpeed * 360 * deltaTime; // Увеличиваем угол вращения
            RotationY %= 360; // Ограничиваем угол до 360 градусов
        }
        private void UpdateVertical(float deltaTime) {
            time += deltaTime * verticalSpeed;
            verticalOffset = MathF.Sin(time) * verticalAmplitude;
        }
        public void Update(float deltaTime) {
            UpdateRotation(deltaTime);
            UpdateVertical(deltaTime);
        }

        public Coin(Vector3 startPosition) : base(startPosition) {
            LoadModelFromFile("../../../Models/Coin/Low Poly Coin_000001.obj");
            Initialize();
            LoadTexture("../../../Textures/coin_texture.jpg");
        }

        public override void Render(Shader shader) {
            Matrix4 model = Matrix4.CreateScale(0.3f);
            model *= Matrix4.CreateRotationY(RotationY);
            model *= Matrix4.CreateTranslation(Position + Vector3.UnitY * verticalOffset); // добавляем вертикальное смещение

            int modelLocation = GL.GetUniformLocation(shader.shaderHandle, "model");
            GL.UniformMatrix4(modelLocation, true, ref model);

            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, TextureId);

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