using Silk.NET.Maths;
using Silk.NET.SDL;
using Silk.NET.Windowing;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Window = Silk.NET.Windowing.Window;

namespace Engine.Core
{
    public class GameLoop
    {
        public delegate void d_Update(GTime time);

        public float TimeSpeed { get; set; } = 1;
        public float MinimumUpdateRealDeltaTime { get; set; } = 0;
        public float MinimumFixedRealDeltaTime { get; set; } = 0;

        internal event EventHandler OnLoad;

        internal event d_Update OnPreUpdate;
        internal event d_Update OnFixedUpdate;
        internal event d_Update OnUpdate;
        internal event d_Update OnLateUpdate;
        internal event d_Update OnRender;
        internal event d_Update OnPostRender;

        private float totalUpdateTime;
        private float totalUpdateRealTime;

        private float totalRenderTime;
        private float totalRenderRealTime;

        private float updateDT;
        private float fixedDT;



        /// <summary>
        /// After calling update methods:
        /// if true : updateDeltaTime -= updateMinimumRealDeltaTime;
        /// if false : updateDeltaTime = 0;
        /// </summary>
        public bool DeltaTimeSubstractionMode { get; set; }

        private IWindow window;

        internal IWindow WindowHandle => window;

        internal GameLoop()
        {
            MinimumFixedRealDeltaTime = 0.5f;

            DeltaTimeSubstractionMode = true;
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
            float delta = (float)deltaSeconds;

            totalUpdateRealTime += delta;
            totalUpdateTime += delta * TimeSpeed;


            GTime updateGTime = new();
            GTime fixedGTime = new();

            updateDT += delta;
            fixedDT += delta;

            if (updateDT >= MinimumUpdateRealDeltaTime)
            {
                updateGTime = new GTime
                {
                    RealDeltaTime = updateDT,
                    TimeSpeed = TimeSpeed,
                    TotalRealTime = totalUpdateRealTime,
                    TotalTime = totalUpdateTime
                };


                if (DeltaTimeSubstractionMode)
                    updateDT -= MinimumUpdateRealDeltaTime;
                else updateDT = 0;
            }

            if (fixedDT >= MinimumFixedRealDeltaTime)
            {
                fixedGTime = new GTime
                {
                    RealDeltaTime = fixedDT,
                    TimeSpeed = TimeSpeed,
                    TotalRealTime = totalUpdateRealTime,
                    TotalTime = totalUpdateTime
                };


                if (DeltaTimeSubstractionMode)
                    fixedDT -= MinimumFixedRealDeltaTime;
                else fixedDT = 0;
            }


            if(updateGTime.RealDeltaTime != 0)
                OnPreUpdate?.Invoke(updateGTime);

            if(fixedGTime.RealDeltaTime != 0)
                OnFixedUpdate?.Invoke(fixedGTime);

            if (updateGTime.RealDeltaTime != 0)
            {
                OnUpdate?.Invoke(updateGTime);
                OnLateUpdate?.Invoke(updateGTime);
            }
                


        }


        private void Window_Render(double deltaSeconds)
        {
            float delta = (float)deltaSeconds;

            totalRenderRealTime += delta;
            totalRenderTime += delta * TimeSpeed;

            var gTime = new GTime()
            {
                TimeSpeed = TimeSpeed,
                RealDeltaTime = delta,
                TotalRealTime = totalRenderRealTime,
                TotalTime = totalRenderTime,
            };

            OnRender?.Invoke(gTime);
            OnPostRender?.Invoke(gTime);
        }

        private void Window_FramebufferResize(Vector2D<int> newSize)
        {

        }


    }
}
