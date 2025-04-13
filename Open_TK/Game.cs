using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace Open_TK
{
    internal class Game : GameWindow
    {
        int width;
        int height;

        Shader shaderProgram = new Shader();
        Camera camera;

        Sphere sphere;
        Plane plane;
        Plane background;
        Random random = new Random();

        // -- Препятствия --
        List<Obstacle> obstacles = new List<Obstacle>();
        private float obstacleSpawnTimer = 0f; // Прошедшее время
        private float obstacleSpawnInterval = 0.75f; // Интервал спавна препятствий
        private float obstacleSpeed = 3.0f; // Скорость приближения 

        // -- Монетки --
        private List<Coin> coins = new List<Coin>();
        private float coinSpawnTimer = 0f; // Прошедшее время
        private float coinSpawnInterval = 2f; // Интервал спавна монет
        private int maxCoins = 10; // Максимальное количество монет на карте

        // -- Сфера --
        float sphereYPosition = 0.5f;
        float sphereXPosition = 0f;
        private float xBoundary = 7.0f; // Ограничение по x
        float initialJumpVelocity = 6.5f; // Скорость прыжка
        float gravity = -9.8f; // Аналог ускорения свободного падения
        float currentVelocityY = 0f; // Начальная скорость
        bool isJumping = false;
        float moveSpeedX = 5.0f; // Cкорость перемещения по x

        // -- Параметры игры -- 
        private bool gameOver = false;
        private bool isPaused = false;
        private int coinCount = 0;
        private float gameOverTimer = 0f;
        private float gameOverDelay = 2f;
        private bool isFreeCamera = false;

        private OpenTK.Mathematics.Vector3 cameraOffset = new OpenTK.Mathematics.Vector3(0f, 2f, 5f); // немного выше и позади
        public Game(int width, int height) : base(GameWindowSettings.Default, NativeWindowSettings.Default) {
            CenterWindow(new Vector2i(width, height));
            this.height = height;
            this.width = width;
        }

        private void SpawnObstacle() {
            float obstacleX = random.Next(-7, 7);
            float obstacleZ = -random.Next(20, 25);
            Obstacle obstacle = new Obstacle(new OpenTK.Mathematics.Vector3(obstacleX, 0.5f, obstacleZ));
            obstacle.Initialize();
            obstacle.LoadTexture("../../../Textures/box.jpg");
            obstacles.Add(obstacle);
        }

        private void SpawnCoin() {
            if (coins.Count < maxCoins)
            {
                float coinX = random.Next(-7, 7);
                float coinZ = -random.Next(15, 25); 
                float coinY = 0.5f;

                Coin newCoin = new Coin(new OpenTK.Mathematics.Vector3(coinX, coinY, coinZ));
                newCoin.MoveSpeedZ = obstacleSpeed; // Используем ту же скорость, что и у препятствий
                coins.Add(newCoin);
            }
        }
        protected override void OnLoad() {
            base.OnLoad();

            // -- Инициализация объектов --
            sphere = new Sphere(new OpenTK.Mathematics.Vector3(0f, 0.5f, 0f));
            sphere.Initialize();
            

            // -- Инициализация нижней плоскости --
            plane = new Plane(new OpenTK.Mathematics.Vector3(0f, 0f, -5f), 30f);
            plane.Initialize();

            // -- Инициализация задней плоскости --
            background = new Plane(new OpenTK.Mathematics.Vector3(0f, 1.80f, -20f), 30f, true);
            background.Initialize();

            // -- Загрузка шейдеров --
            shaderProgram.LoadShader();

            // -- Загрузка текстур --
            sphere.LoadTexture("../../../Textures/kolobok4.jpg");
            plane.LoadTexture("../../../Textures/grass.jpeg");
            background.LoadTexture("../../../Textures/forest3.jpeg");

            // -- Инициализация камеры --
            OpenTK.Mathematics.Vector3 initialSpherePosition = sphere.Position;
            camera = new Camera(width, height, initialSpherePosition + cameraOffset);
            //CursorState = CursorState.Grabbed;


            // -- Создание начальных препятствий --
            for (int i = 0; i < 3; i++)
            {
                SpawnObstacle();
            }

            GL.Enable(EnableCap.DepthTest);
        }

        protected override void OnUnload() {
            base.OnUnload();

            sphere.CleanUp();
            plane.CleanUp();
            background.CleanUp();

            foreach (var obstacle in obstacles)
            {
                obstacle.CleanUp();
            }
            obstacles.Clear();

            // -- Очистка монет --
            foreach (var coin in coins)
            {
                coin.CleanUp();
            }
            coins.Clear();

            shaderProgram.DeleteShader();
        }

        protected override void OnRenderFrame(FrameEventArgs args) {
            GL.ClearColor(0.3f, 0.3f, 1f, 1f);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            shaderProgram.UseShader();

            Matrix4 view = camera.GetViewMatrix();
            Matrix4 projection = camera.GetProjection();

            int viewLocation = GL.GetUniformLocation(shaderProgram.shaderHandle, "view");
            int projectionLocation = GL.GetUniformLocation(shaderProgram.shaderHandle, "projection");
            GL.UniformMatrix4(viewLocation, true, ref view);
            GL.UniformMatrix4(projectionLocation, true, ref projection);

            // -- Отрисовка сферы --
            sphere.Position = new OpenTK.Mathematics.Vector3(sphereXPosition, sphereYPosition, 0f);
            sphere.Render(shaderProgram);

            // -- Отрисовка нижней плоскости --
            plane.Render(shaderProgram);

            // -- Отрисовка задней плокости --
            background.Render(shaderProgram);

            // -- Отрисовка препятствий --
            foreach (var obstacle in obstacles)
            {
                obstacle.Render(shaderProgram);
            }

            // -- Отрисовка монет --
            foreach (var coin in coins)
            {
                coin.Render(shaderProgram);
            }

            Context.SwapBuffers();

            base.OnRenderFrame(args);
            
        }

        protected override void OnUpdateFrame(FrameEventArgs args) {

            if (gameOver)
            {
                gameOverTimer += (float)args.Time;
                if (gameOverTimer > gameOverDelay)
                {
                    Close();
                }
                return;
            }

            if (KeyboardState.IsKeyDown(Keys.Escape))
            {
                Close();
            }

            if (KeyboardState.IsKeyPressed(Keys.Space))
            {
                isPaused = !isPaused;
            }

            if (KeyboardState.IsKeyPressed(Keys.C)) // Нажатие 'C' переключает режим камеры
            {
                isFreeCamera = !isFreeCamera;
                if (isFreeCamera)
                {
                    CursorState = CursorState.Grabbed;
                }
                else
                {
                    CursorState = CursorState.Normal;
                    camera.firstMove = true;
                }
            }


            if (!isFreeCamera)
            {
                // -- Настройка камеры -- 
                OpenTK.Mathematics.Vector3 spherePosition = sphere.Position;

                // Вычисляем желаемое положение камеры
                OpenTK.Mathematics.Vector3 desiredCameraPosition = spherePosition + cameraOffset;

                // Плавное перемещение камеры 
                float smoothSpeed = 5.0f; // скорость сглаживания
                camera.position = OpenTK.Mathematics.Vector3.Lerp(camera.position, desiredCameraPosition, smoothSpeed * (float)args.Time);

                // Заставляем камеру смотреть на сферу
                camera.front = OpenTK.Mathematics.Vector3.Normalize(spherePosition - camera.position);
                camera.right = OpenTK.Mathematics.Vector3.Normalize(OpenTK.Mathematics.Vector3.Cross(camera.front, OpenTK.Mathematics.Vector3.UnitY));
                camera.up = OpenTK.Mathematics.Vector3.Normalize(OpenTK.Mathematics.Vector3.Cross(camera.right, camera.front));
            }
            else
            {
                MouseState mouse = MouseState;
                KeyboardState input = KeyboardState;
                camera.Update(input, mouse, args);
            }

            if (isPaused)
            {
                return;
            }

            if (KeyboardState.IsKeyPressed(Keys.Up) && !isJumping)
            {
                isJumping = true;
                currentVelocityY = initialJumpVelocity;
            }

            float moveStep = moveSpeedX * (float)args.Time;
            float rotationSpeed = -200f; // Скорость вращения в градусах в секунду
            sphere.RotationY = 270f;
            sphere.RotationX += rotationSpeed * (float)args.Time;
            if (KeyboardState.IsKeyDown(Keys.Left))
            {
                sphereXPosition -= moveStep;
            }
            if (KeyboardState.IsKeyDown(Keys.Right))
            {
                sphereXPosition += moveStep;
            }
            // Ограничение движения по x
            if (sphereXPosition < -xBoundary + 0.5f)
            {
                sphereXPosition = -xBoundary + 0.5f;
            }
            if (sphereXPosition > xBoundary - 0.5f)
            {
                sphereXPosition = xBoundary - 0.5f;
            }


            if (isJumping)
            {
                currentVelocityY += gravity * (float)args.Time;
                sphereYPosition += currentVelocityY * (float)args.Time;

                // Проверяем, достигла ли нижняя точка сферы уровня земли
                if (sphereYPosition < 0.5f)
                {
                    sphereYPosition = 0.5f; // Устанавливаем центр сферы на 0.5f
                    isJumping = false;
                    currentVelocityY = 0f;
                }
            }
            else if (sphereYPosition > 0.5f) // Если не прыгаем и находимся выше земли
            {
                currentVelocityY += gravity * (float)args.Time;
                sphereYPosition += currentVelocityY * (float)args.Time;

                // Снова проверяем достижение земли
                if (sphereYPosition < 0.5f)
                {
                    sphereYPosition = 0.5f;
                    currentVelocityY = 0f;
                }
            }


            // -- Обновление и проверка столкновений с препятствиями --
            for (int i = obstacles.Count - 1; i >= 0; i--)
            {
                Obstacle obstacle = obstacles[i];
                obstacle.Position += new OpenTK.Mathematics.Vector3(0f, 0f, obstacleSpeed * (float)args.Time); // Двигаем к игроку

                // Проверка столкновения
                float distanceX = Math.Abs(sphereXPosition - obstacle.Position.X);
                float distanceZ = Math.Abs(0f - obstacle.Position.Z); // Сфера всегда на Z = 0

                if (distanceX < 0.5f + 0.5f && distanceZ < 0.5f + 0.5f && sphereYPosition < 0.5f + 0.5f)
                {
                    gameOver = true;
                    Console.WriteLine("Game Over!");
                    break;
                }
                // Удаление препятствий, прошедших мимо игрока
                if (obstacle.Position.Z > 5.0f) // Немного дальше игрока по Z
                {
                    obstacles.RemoveAt(i);
                }
            }

            // -- Спавн новых препятствий --
            obstacleSpawnTimer += (float)args.Time;
            if (obstacleSpawnTimer >= obstacleSpawnInterval)
            {
                SpawnObstacle();
                obstacleSpawnTimer = 0f;
            }

            // -- Спавн новых монет --
            coinSpawnTimer += (float)args.Time;
            if (coinSpawnTimer >= coinSpawnInterval)
            {
                SpawnCoin();
                coinSpawnTimer = 0f;
            }

            // -- Обновление положения и вращения монет --
            for (int i = coins.Count - 1; i >= 0; i--)
            {
                Coin coin = coins[i];
                coin.Position += new OpenTK.Mathematics.Vector3(0f, 0f, coin.MoveSpeedZ * (float)args.Time);
                coin.Update((float)args.Time);

                // Проверка столкновения с монетой
                float distanceX = Math.Abs(sphereXPosition - coin.Position.X);
                float distanceZ = Math.Abs(0f - coin.Position.Z);
                float distanceY = Math.Abs(sphereYPosition - coin.Position.Y);

                if (distanceX < 0.5f + 0.5f && distanceZ < 0.5f + 0.5f && distanceY < 0.5f + 0.5f)
                {
                    Console.Clear();
                    Console.WriteLine("Coin Collected!");
                    coinCount++;
                    Console.WriteLine($"Coins: {coinCount}");
                    coins.RemoveAt(i);
                }
                else if (coin.Position.Z > 5.0f)
                {
                    coins.RemoveAt(i);
                }
            }
            
            base.OnUpdateFrame(args);
        }

        protected override void OnResize(ResizeEventArgs e) {
            base.OnResize(e);
            GL.Viewport(0, 0, e.Width, e.Height);
            this.width = e.Width;
            this.height = e.Height;
        }
    }
}