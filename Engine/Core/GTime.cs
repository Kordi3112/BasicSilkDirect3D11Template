using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Core
{
    public struct GTime
    {

        public float TotalRealTime { get; internal set; }
        public float TotalTime { get; internal set; }

        public float RealDeltaTime { get; internal set; }

        public float TimeSpeed { get; internal set; }

        public float DeltaTime => RealDeltaTime * TimeSpeed;

    }
}
