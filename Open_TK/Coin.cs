using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using StbImageSharp;
using System;
using System.Collections.Generic;

namespace Open_TK
{
    public class Coin : GameObject
    {
        private float radius;
        private float height;
        private int segments;

        public float MoveSpeedZ = 3.0f;
        public float RotationY = 0f;
        public float RotationSpeed = 1.15f;

        private float verticalSpeed = 2.15f;
        private float verticalAmplitude = 0.225f;
        private float verticalOffset = 0f;
        private float time = 0f;

        public Coin(Vector3 startPosition, float radius = 0.505f, float height = 0.115f, int segments = 36) : base() {
            Position = startPosition;
            this.radius = radius;
            this.height = height;
            this.segments = segments;
            GenerateCoinData();
        }

        private void GenerateCoinData() {
            List<Vector3> verticesList = new List<Vector3>();
            List<uint> indicesList = new List<uint>();
            List<Vector2> texCoordsList = new List<Vector2>();

            // Верхняя крышка
            verticesList.Add(Vector3.Zero); // Центр (индекс 0)
            texCoordsList.Add(new Vector2(0.5f, 0.5f));
            for (int i = 0; i <= segments; i++)
            {
                float angle = i * 2 * MathF.PI / segments;
                float x = radius * MathF.Cos(angle);
                float z = radius * MathF.Sin(angle);
                verticesList.Add(new Vector3(x, height / 2, z)); // Индексы с 1 по segments + 1
                texCoordsList.Add(new Vector2(0.5f + 0.5f * MathF.Cos(angle), 0.5f + 0.5f * MathF.Sin(angle)));
                if (i > 0)
                {
                    indicesList.Add(0);
                    indicesList.Add((uint)i);
                    indicesList.Add((uint)i + 1);
                }
            }

            // Нижняя крышка
            int bottomCenterIndex = verticesList.Count; // Индекс segments + 2
            verticesList.Add(new Vector3(0, -height / 2, 0)); // Центр
            texCoordsList.Add(new Vector2(0.5f, 0.5f));
            for (int i = 0; i <= segments; i++)
            {
                float angle = i * 2 * MathF.PI / segments;
                float x = radius * MathF.Cos(angle);
                float z = radius * MathF.Sin(angle);
                verticesList.Add(new Vector3(x, -height / 2, z)); // Индексы с bottomCenterIndex + 1
                texCoordsList.Add(new Vector2(0.5f + 0.5f * MathF.Cos(angle), 0.5f + 0.5f * MathF.Sin(angle)));
                if (i > 0)
                {
                    indicesList.Add((uint)bottomCenterIndex);
                    indicesList.Add((uint)bottomCenterIndex + (uint)i + 1);
                    indicesList.Add((uint)bottomCenterIndex + (uint)i);
                }
            }

            // Добавляем вершины для боковой поверхности
            int sideStartIndex = verticesList.Count;
            for (int i = 0; i < segments; i++)
            {
                float angle = i * 2 * MathF.PI / segments;
                float x = radius * MathF.Cos(angle);
                float z = radius * MathF.Sin(angle);
                verticesList.Add(new Vector3(x, height / 2, z)); // Верхняя вершина
                texCoordsList.Add(new Vector2((float)i / segments, 1.0f));
                verticesList.Add(new Vector3(x, -height / 2, z)); // Нижняя вершина
                texCoordsList.Add(new Vector2((float)i / segments, 0.0f));
            }

            // Генерируем индексы для боковой поверхности
            for (int i = 0; i < segments; i++)
            {
                uint currentTop = (uint)(sideStartIndex + i * 2);
                uint currentBottom = currentTop + 1;
                uint nextTop = (uint)(sideStartIndex + ((i + 1) % segments) * 2);
                uint nextBottom = nextTop + 1;

                // Первый треугольник
                indicesList.Add(currentTop);
                indicesList.Add(currentBottom);
                indicesList.Add(nextTop);

                // Второй треугольник
                indicesList.Add(nextTop);
                indicesList.Add(currentBottom);
                indicesList.Add(nextBottom);
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

        public void UpdateRotation(float deltaTime) {
            RotationY += RotationSpeed * 360 * deltaTime;
            RotationY %= 360;
        }

        private void UpdateVertical(float deltaTime) {
            time += deltaTime * verticalSpeed;
            verticalOffset = MathF.Sin(time) * verticalAmplitude;
        }

        public void Update(float deltaTime) {
            UpdateRotation(deltaTime);
            UpdateVertical(deltaTime);
        }

        public override Matrix4 GetModelMatrix() {
            Matrix4 model = Matrix4.CreateRotationX(MathHelper.DegreesToRadians(90f));
            model *= Matrix4.CreateScale(0.5f);
            model *= Matrix4.CreateRotationY(MathHelper.DegreesToRadians(RotationY));
            model *= Matrix4.CreateTranslation(Position + Vector3.UnitY * verticalOffset);
            return model;
        }
        public override void Render(Shader shader) {
            base.Render(shader);
        }

        public override void CleanUp() {
            base.CleanUp();
        }
    }
}
