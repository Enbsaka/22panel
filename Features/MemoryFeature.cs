using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ascendedAuth_v2.Core;
using ascendedAuth_v2.Models;

namespace ascendedAuth_v2.Features
{
    public class MemoryFeature : FeatureBase
    {
        private readonly MemoryManager _memoryManager;
        private readonly MemoryPattern _pattern;

        public MemoryFeature(string name, MemoryManager memoryManager, MemoryPattern pattern)
        {
            Name = name;
            _memoryManager = memoryManager;
            _pattern = pattern;
        }

        public override async Task<bool> Toggle(bool enable)
        {
            if (enable == IsEnabled) return true;

            bool success;
            if (enable)
            {
                success = await _memoryManager.ApplyPattern(_pattern.Search, _pattern.Replace);
            }
            else
            {
                success = await _memoryManager.RevertPattern(_pattern.Search, _pattern.Replace);
            }

            if (success)
            {
                IsEnabled = enable;
            }
            return success;
        }
    }
}
