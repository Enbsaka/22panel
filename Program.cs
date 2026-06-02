using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Forms;
using ascendedAuth_v2.UI;

namespace ascendedAuth_v2
{
    internal static class Program
    {
        /// <summary>
        /// Ponto de entrada principal para o aplicativo.
        /// </summary>
        [STAThread]
        static void Main()
        {
            AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new LoginForm());
        }

        private static Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            try
            {
                var requestedName = new AssemblyName(args.Name).Name + ".dll";
                var asm = Assembly.GetExecutingAssembly();
                var resourceName = asm.GetManifestResourceNames()
                    .FirstOrDefault(n => n.EndsWith(requestedName, StringComparison.OrdinalIgnoreCase));

                if (string.IsNullOrWhiteSpace(resourceName)) return null;

                using (var stream = asm.GetManifestResourceStream(resourceName))
                {
                    if (stream == null) return null;
                    using (var ms = new MemoryStream())
                    {
                        stream.CopyTo(ms);
                        return Assembly.Load(ms.ToArray());
                    }
                }
            }
            catch
            {
                return null;
            }
        }
    }
}
