using Raylib_cs;

namespace TSHARP.CodeAnalysis
{
    internal sealed class WindowController
    {
        private Image _image;
        private Texture2D _texture => Raylib.LoadTextureFromImage(_image);

        internal WindowController(int width, int height, string name)
        {
            Width = width;
            Height = height;
            Name = name;
            SetWindow();
        }

        private void SetWindow()
        {
            Raylib.SetTraceLogLevel(TraceLogLevel.None);
            Raylib.InitWindow(Width, Height, Name);
            _image = Raylib.GenImageColor(Width, Height, Color.White);
        }

        internal void SetAPixel(int x, int y, int R, int G, int B)
        {
            if (x < 0 || x > Width - 1)
                return;

            if (y < 0 || y > Height - 1)
                return;

            Raylib.ImageDrawPixel(ref _image, x, y, new Color(R, G, B, 255));
        }

        internal bool IsWinClose()
            => Raylib.WindowShouldClose();
        
        internal void CloseWin()
        {
            Raylib.UnloadImage(_image);
            Raylib.UnloadTexture(_texture);
            Raylib.CloseWindow();
        }

        internal void Render()
        {
            Raylib.BeginDrawing();

            Raylib.ClearBackground(Color.White);

            Raylib.DrawTexture(_texture, 0, 0, Color.White);

            Raylib.EndDrawing();
        }

        internal int Width { get; }
        internal int Height { get; }
        internal string Name { get; }
    }
}
