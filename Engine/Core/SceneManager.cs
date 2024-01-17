using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Core
{
    public class SceneManager : IDisposable
    {
        private GameManager gameManager;

        private List<Scene> scenes;

        public Scene HeadScene { get; private set; }

        public Scene CurrentScene { get; private set; }

        public SceneManager(GameManager gameManager)
        {
            this.gameManager = gameManager;

            scenes = new List<Scene>();
        }

        public void SetHeadScene(Scene scene)
        {
            HeadScene = scene;
            scene.GameManager = gameManager;
            scene.Init();
        }

        public void AddScene(Scene scene)
        {
            scenes.Add(scene);
            scene.GameManager = gameManager;
            scene.Init();
        }

        public void ApplyScene(string tag)
        {
            try
            {
                var scene = scenes.Find(_scene => _scene.Info.Tag == tag ? true : false);

                SetScene(scene);
            }
            catch { throw new Exception($"Tag: {tag} is valid"); }
        }



        public void ApplyScene(int id)
        {
            try
            {
                var scene = scenes.Find(_scene => _scene.Info.Id == id ? true : false);

                SetScene(scene);
            }
            catch { throw new Exception($"Id: {id} is valid"); }
        }

        public void RestartHeadScene(bool disposeBeforeLoad = false)
        {
            HeadScene?.Close();

            if (disposeBeforeLoad)
                HeadScene?.Dispose();

            HeadScene?.Load();
        }

        public void RestartCurrentScene(bool disposeBeforeLoad = false)
        {
            CurrentScene?.Close();

            if (disposeBeforeLoad)
                CurrentScene?.Dispose();

            CurrentScene?.Load();
        }

        internal void PreUpdate(GTime deltaTime)
        {
            HeadScene?.PreUpdate(deltaTime);
            CurrentScene?.PreUpdate(deltaTime);
        }

        internal void FixedUpdate(GTime deltaTime)
        {
            HeadScene?.FixedUpdate(deltaTime);
            CurrentScene?.FixedUpdate(deltaTime);
        }

        internal void Update(GTime deltaTime)
        {
            HeadScene?.Update(deltaTime);
            CurrentScene?.Update(deltaTime);
        }

        internal void LateUpdate(GTime deltaTime)
        {
            HeadScene?.LateUpdate(deltaTime);
            CurrentScene?.LateUpdate(deltaTime);
        }

        internal void Render(GTime deltaTime)
        {
            CurrentScene?.Render(deltaTime);
            HeadScene?.Render(deltaTime);
        }

        internal void OnResized()
        {
            HeadScene?.OnResized();
            CurrentScene?.OnResized();
        }

        private void SetScene(Scene scene)
        {
            // Close previous scene
            CurrentScene?.Close();

            // Set new scene
            CurrentScene = scene;

            // Call load
            CurrentScene.Load();
        }

        public void Dispose()
        {
            scenes.ForEach(scene => scene.Dispose());
        }
    }
}
