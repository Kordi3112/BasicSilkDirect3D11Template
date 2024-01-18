using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Core
{
    public struct GTime
    {

        public float TotalRealTime;
        public float TotalTime;

        public float RealDeltaTime;

        public float TimeSpeed;

        public float DeltaTime => RealDeltaTime * TimeSpeed;

    }
}
