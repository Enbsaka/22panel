using System;
using System.Threading.Tasks;
using ascendedAuth_v2.Core;

namespace ascendedAuth_v2.Features
{
    public class DllFeature : FeatureBase
    {
        private readonly string _resourceName;
        private readonly string _outputFileName;
        private readonly string _processName;

        public DllFeature(string name, string resourceName, string outputFileName, string processName)
        {
            Name = name;
            _resourceName = resourceName;
            _outputFileName = outputFileName;
            _processName = processName;
        }

        public override async Task<bool> Toggle(bool enable)
        {
            if (enable == IsEnabled) return true;

            if (enable)
            {
                try
                {
                    string path = Injector.ExtractEmbeddedDll(_resourceName, _outputFileName);
                    Injector.Inject(path, _processName);
                    IsEnabled = true;
                    return true;
                }
                catch (Exception ex)
                {
                    System.Windows.Forms.MessageBox.Show("Erro na injeção: " + ex.Message);
                    return false;
                }
            }
            else
            {
                try
                {
                    Injector.Unload(_outputFileName, _processName);
                    IsEnabled = false;
                    return true;
                }
                catch (Exception ex)
                {
                    System.Windows.Forms.MessageBox.Show("Erro ao desativar DLL: " + ex.Message);
                    return false;
                }
            }
        }
    }
}
