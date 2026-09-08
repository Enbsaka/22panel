using System;
using System.Threading.Tasks;

namespace ascendedAuth_v2.Features
{
    public abstract class FeatureBase
    {
        public string Name { get; protected set; }
        public bool IsEnabled { get; protected set; }

        public abstract Task<bool> Toggle(bool enable);
    }
}
