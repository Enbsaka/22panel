using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;

namespace ascendedAuth_v2.Core
{
    public class Injector
    {
        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr OpenProcess(uint dwDesiredAccess, bool bInheritHandle, int dwProcessId);

        [DllImport("kernel32.dll", SetLastError = true, ExactSpelling = true)]
        private static extern IntPtr VirtualAllocEx(IntPtr hProcess, IntPtr lpAddress, uint dwSize, uint flAllocationType, uint flProtect);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] lpBuffer, uint nSize, out IntPtr lpNumberOfBytesWritten);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string lpModuleName);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr GetProcAddress(IntPtr hModule, string lpProcName);

        [DllImport("kernel32.dll")]
        private static extern IntPtr CreateRemoteThread(IntPtr hProcess, IntPtr lpThreadAttributes, uint dwStackSize, IntPtr lpStartAddress, IntPtr lpParameter, uint dwCreationFlags, IntPtr lpThreadId);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool CloseHandle(IntPtr hObject);

        private const uint PROCESS_ALL_ACCESS = 0x001F0FFF;
        private const uint MEM_COMMIT = 0x00001000;
        private const uint PAGE_READWRITE = 0x04;

        public static string ExtractEmbeddedDll(string resourceName, string outputFileName)
        {
            string baseDir = Path.Combine(Path.GetTempPath(), "22");
            Directory.CreateDirectory(baseDir);
            string destination = Path.Combine(baseDir, outputFileName);
            using (Stream resourceStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName))
            {
                if (resourceStream == null) throw new Exception("Recurso não encontrado: " + resourceName);
                using (FileStream fileStream = new FileStream(destination, FileMode.Create))
                {
                    resourceStream.CopyTo(fileStream);
                }
            }
            return destination;
        }

        public static void Inject(string dllPath, string processName)
        {
            if (!File.Exists(dllPath)) throw new FileNotFoundException("DLL não encontrada", dllPath);

            Process[] targetProcess = Process.GetProcessesByName(processName);
            if (targetProcess.Length == 0) throw new Exception("Processo não encontrado: " + processName);

            Process process = targetProcess[0];
            IntPtr hProcess = OpenProcess(PROCESS_ALL_ACCESS, false, process.Id);
            if (hProcess == IntPtr.Zero) throw new Exception("Falha ao abrir processo.");

            IntPtr addr = VirtualAllocEx(hProcess, IntPtr.Zero, (uint)((dllPath.Length + 1) * Marshal.SizeOf(typeof(char))), MEM_COMMIT, PAGE_READWRITE);
            if (addr == IntPtr.Zero) throw new Exception("Falha ao alocar memória no processo remoto.");

            byte[] dllBytes = Encoding.ASCII.GetBytes(dllPath);
            IntPtr bytesWritten;
            if (!WriteProcessMemory(hProcess, addr, dllBytes, (uint)dllBytes.Length, out bytesWritten))
                throw new Exception("Falha ao escrever na memória do processo remoto.");

            IntPtr loadLibraryAddr = GetProcAddress(GetModuleHandle("kernel32.dll"), "LoadLibraryA");
            if (loadLibraryAddr == IntPtr.Zero) throw new Exception("Falha ao obter endereço de LoadLibraryA.");

            IntPtr hThread = CreateRemoteThread(hProcess, IntPtr.Zero, 0, loadLibraryAddr, addr, 0, IntPtr.Zero);
            if (hThread != IntPtr.Zero) CloseHandle(hThread);
            CloseHandle(hProcess);
        }

        public static void Unload(string dllName, string processName)
        {
            // Nota: O descarregamento de DLLs de terceiros pode ser instável e causar crashes 
            // se a DLL tiver threads ativos ou hooks não removidos.
            // Para segurança máxima, estamos desativando o Unload por FreeLibrary remoto 
            // e apenas marcando como desabilitado na UI.
            
            /*
            Process[] targetProcess = Process.GetProcessesByName(processName);
            if (targetProcess.Length == 0) return;

            Process process = targetProcess[0];
            process.Refresh();
            
            var module = process.Modules.Cast<ProcessModule>()
                .FirstOrDefault(m => m.ModuleName.IndexOf(dllName, StringComparison.OrdinalIgnoreCase) >= 0);

            if (module == null) return;

            IntPtr hProcess = OpenProcess(PROCESS_ALL_ACCESS, false, process.Id);
            if (hProcess == IntPtr.Zero) return;

            IntPtr freeLibraryAddr = GetProcAddress(GetModuleHandle("kernel32.dll"), "FreeLibrary");
            if (freeLibraryAddr != IntPtr.Zero)
            {
                IntPtr hThread = CreateRemoteThread(hProcess, IntPtr.Zero, 0, freeLibraryAddr, module.BaseAddress, 0, IntPtr.Zero);
                if (hThread != IntPtr.Zero) CloseHandle(hThread);
            }

            CloseHandle(hProcess);
            */
        }
    }
}
