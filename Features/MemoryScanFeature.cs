using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ascendedAuth_v2.Core;

namespace ascendedAuth_v2.Features
{
    public class MemoryScanFeature : FeatureBase
    {
        private readonly MemoryManager _memoryManager;
        private readonly string _searchPattern;
        private readonly int _readOffset;
        private readonly int _writeOffset;
        // Armazena os valores originais para restauração
        private readonly Dictionary<string, int> _originalValues = new Dictionary<string, int>();

        public MemoryScanFeature(string name, MemoryManager memoryManager, string searchPattern, int readOffset, int writeOffset)
        {
            Name = name;
            _memoryManager = memoryManager;
            _searchPattern = searchPattern;
            _readOffset = readOffset;
            _writeOffset = writeOffset;
        }

        public override async Task<bool> Toggle(bool enable)
        {
            if (enable == IsEnabled) return true;

            if (enable)
            {
                var results = await _memoryManager.Scan(_searchPattern);
                if (results == null || !results.Any()) return false;

                _originalValues.Clear();

                foreach (long currentAddress in results)
                {
                    string targetAddrHex = (currentAddress + _writeOffset).ToString("X");
                    long partAddr = currentAddress + _readOffset;

                    // Salva o valor original antes de sobrescrever
                    int originalValue = _memoryManager.ReadInt(targetAddrHex);
                    _originalValues[targetAddrHex] = originalValue;

                    // Aplica o novo valor
                    int newValue = _memoryManager.ReadInt(partAddr.ToString("X"));
                    _memoryManager.WriteInt(targetAddrHex, newValue);
                }
                IsEnabled = true;
                return true;
            }
            else
            {
                // Restaura os valores originais salvos
                if (_originalValues.Count > 0)
                {
                    foreach (var entry in _originalValues)
                    {
                        _memoryManager.WriteInt(entry.Key, entry.Value);
                    }
                }
                IsEnabled = false;
                return true;
            }
        }
    }
}
