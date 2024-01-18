using Engine.Video;
using Silk.NET.GLFW;
using Silk.NET.Input;
using Silk.NET.Maths;
using Silk.NET.SDL;
using Silk.NET.Windowing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Window = Silk.NET.Windowing.Window;

namespace Engine.Core
{
    public class GameManager : IDisposable
    {
        public GameLoop GameLoop { get; private set; }

        public SceneManager SceneManager { get; private set; }

        public VideoManager VideoManager { get; private set; }

        public IWindow WindowHandler { get; private set; }


        public event EventHandler InitScenes;
        public event EventHandler InitResources;

        public GameManager() 
        {
            GameLoop = new GameLoop();

            SceneManager = new SceneManager(this);

            VideoManager = new VideoManager(this);
        }



        public void Run(string title, Vector2i size)
        {
            var options = WindowOptions.Default;
            options.Size = size;
            options.Title = title;
            options.API = GraphicsAPI.None; // <-- This bit is important, as your window will be configured for OpenGL by default.

            // Create window
            WindowHandler = Window.Create(options);

            GameLoop.OnLoad += GameLoop_OnLoad;
            GameLoop.OnPreUpdate += PreUpdate;
            GameLoop.OnFixedUpdate += FixedUpdate;
            GameLoop.OnUpdate += Update;
            GameLoop.OnLateUpdate += LateUpdate;
            GameLoop.OnRender += Render;
            GameLoop.OnPostRender += PostRender;

            GameLoop.Init(WindowHandler);

            WindowHandler.FramebufferResize += Window_FramebufferResize;



            WindowHandler.Run();

            
        }

        private void Window_FramebufferResize(Vector2i size)
        {
            VideoManager.OnFramebufferResize(size);
        }

        private void GameLoop_OnLoad(object sender, EventArgs e)
        {
            // TODO: place somewhere else
            // Set-up input context.
            var input = WindowHandler.CreateInput();
            foreach (var keyboard in input.Keyboards)
            {
                keyboard.KeyDown += OnKeyDown;
            }


            VideoManager.Initialize(GameLoop.WindowHandle);

            InitResources?.Invoke(this, EventArgs.Empty);

            InitScenes?.Invoke(this, EventArgs.Empty);

            // Call Resize functions
            Window_FramebufferResize(WindowHandler.Size);
        }

        private void PreUpdate(GTime gTime)
        {
            SceneManager.PreUpdate(gTime);
        }

        private void FixedUpdate(GTime gTime)
        {
            Console.WriteLine("Fixed: {0}, {1}", gTime.RealDeltaTime, gTime.TotalRealTime);
            SceneManager.FixedUpdate(gTime);
        }

        private void Update(GTime gTime)
        {
            SceneManager.Update(gTime);
        }

        private void LateUpdate(GTime gTime)
        {
            SceneManager.LateUpdate(gTime);
        }


        private void Render(GTime gTime)
        {
            VideoManager.MainRenderTargetController.PrepareDraw();

            SceneManager.Render(gTime);
    
            VideoManager.FinalRender();
        }

        private void PostRender(GTime gTime)
        {

        }

        private void OnKeyDown(IKeyboard keyboard, Key key, int scancode)
        {
            // Check to close the window on escape.
            if (key == Key.Escape)
            {
                WindowHandler.Close();
            }
        }


        public void Dispose()
        {
            
        }
    }
}
