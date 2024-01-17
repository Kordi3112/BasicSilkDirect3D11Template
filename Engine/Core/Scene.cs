using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Core
{
    public abstract class Scene : IDisposable
    {
        public GameManager GameManager { get; internal set; }

        public SceneInfo Info { get; private set; }

        public Scene(SceneInfo info)
        {
            Info = info;
        }

        public abstract void Init();

        public abstract void Load();

        public abstract void PreUpdate(GTime deltaTime);


        public abstract void FixedUpdate(GTime deltaTime);


        public abstract void Update(GTime deltaTime);

        public abstract void LateUpdate(GTime deltaTime);

        public abstract void Render(GTime deltaTime);

        public abstract void Close();
        public abstract void OnResized();

        public void Dispose()
        {

        }
    }
}
