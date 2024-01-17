using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Core
{
    public struct SceneInfo
    {
        public string Tag;

        public int Id;

        public SceneInfo(string tag, int id)
        {
            Tag = tag;
            Id = id;
        }
    }
}
