﻿using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using StbImageSharp;

namespace Open_TK
{
    public class Plane : GameObject
    {
        float scale;
        int textureVbo;
        bool isVertical;
        public Plane(Vector3 startPosition, float scale = 1.0f, bool isVertical = false) : base() {
            Position = startPosition;
            this.scale = scale;
            this.isVertical = isVertical;
            GeneratePlaneData();
        }

        private void GeneratePlaneData() {
            if (!isVertical)
            {
                // Горизонтальная плоскость
                base.vertices = new List<Vector3>()
                {
                    new Vector3(-1.0f * scale, 0.0f, -1.0f * scale),
                    new Vector3( 1.0f * scale, 0.0f, -1.0f * scale),
                    new Vector3( 1.0f * scale, 0.0f,  1.0f * scale),
                    new Vector3(-1.0f * scale, 0.0f,  1.0f * scale)
                };

                base.texCoords = new List<Vector2>()
                {
                    new Vector2(0.0f, 1.0f),
                    new Vector2(1.0f, 1.0f),
                    new Vector2(1.0f, 0.0f),
                    new Vector2(0.0f, 0.0f)
                };

                base.indices = new uint[]
                {
                    0, 1, 2,
                    2, 3, 0
                };
            }
            else
            {
                // Вертикальная плоскость
                base.vertices = new List<Vector3>()
                {
                    new Vector3(-1.0f * scale, -1.0f * scale, 0.0f),
                    new Vector3( 1.0f * scale, -1.0f * scale, 0.0f),
                    new Vector3( 1.0f * scale,  1.0f * scale, 0.0f),
                    new Vector3(-1.0f * scale,  1.0f * scale, 0.0f)
                };

                base.texCoords = new List<Vector2>()
                {
                    new Vector2(0.0f, 0.0f),
                    new Vector2(1.0f, 0.0f),
                    new Vector2(1.0f, 1.0f),
                    new Vector2(0.0f, 1.0f)
                };

                base.indices = new uint[]
                {
                    0, 1, 2,
                    2, 3, 0
                };
            }
        }
        public void Initialize() {
            InitializeInternal();
        }

        public void LoadTexture(string path) {
            LoadTextureInternal(path, TextureUnit.Texture1, true);
        }

        public override Matrix4 GetModelMatrix() {
            return Matrix4.CreateTranslation(Position);
        }

        public override void Render(Shader shader) {
            base.Render(shader);
        }
        public override void CleanUp() {
            base.CleanUp();
        }

    }
}