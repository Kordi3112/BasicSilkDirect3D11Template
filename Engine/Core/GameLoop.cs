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
    public class GameLoop
    {
        public delegate void d_Update(GTime time);

        internal event EventHandler OnLoad;

        internal event d_Update OnPreUpdate;
        internal event d_Update OnFixedUpdate;
        internal event d_Update OnUpdate;
        internal event d_Update OnLateUpdate;
        internal event d_Update OnRender;
        internal event d_Update OnPostRender;

        private float totalTime;
        private float totalRealTime;

        private IWindow window;

        internal IWindow WindowHandle => window;

        internal GameLoop()
        {
                

        }


        internal void Init(IWindow window)
        {

            this.window = window;
            // Create window        
  
            window.Load += Window_Load;
            window.Update += Window_Update;
            window.Render += Window_Render;
            window.FramebufferResize += Window_FramebufferResize;

            window.UpdatesPerSecond = 0;
            window.FramesPerSecond = 0;
            

        }


        private void Window_Load()
        {
            OnLoad?.Invoke(this, EventArgs.Empty);
        }

        private void Window_Update(double deltaSeconds)
        {
            float gameSpeed = 1.0f;
            float delta = (float)deltaSeconds;

            totalRealTime += delta;
            totalTime += delta * gameSpeed;

            var gTime = new GTime()
            {
                TimeSpeed = gameSpeed,
                RealDeltaTime = delta,
                TotalRealTime = totalRealTime,
                TotalTime = totalTime,
            };

            // TODO: Make another timer for fixed Update

            OnPreUpdate?.Invoke(gTime);
            OnFixedUpdate?.Invoke(gTime);
            OnUpdate?.Invoke(gTime);
            OnLateUpdate?.Invoke(gTime);
            OnRender?.Invoke(gTime);
            OnPostRender?.Invoke(gTime);

        }


        private void Window_Render(double deltaSeconds)
        {
            // Render
            // PostRender
        }

        private void Window_FramebufferResize(Vector2D<int> newSize)
        {

        }


    }
}
