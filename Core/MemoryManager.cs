using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using Memory;

namespace ascendedAuth_v2.Core
{
    public class MemoryManager
    {
        private readonly Mem _memory = new Mem();
        public Mem Memory { get { return _memory; } }
        private readonly string _processName;
        public string LastError { get; private set; }

        public MemoryManager(string processName)
        {
            _processName = processName;
        }

        private bool IsAdministrator()
        {
            using (WindowsIdentity identity = WindowsIdentity.GetCurrent())
            {
                WindowsPrincipal principal = new WindowsPrincipal(identity);
                return principal.IsInRole(WindowsBuiltInRole.Administrator);
            }
        }

        public bool OpenProcess()
        {
            try
            {
                if (!IsAdministrator())
                {
                    LastError = "ERRO: Execute como ADMINISTRADOR!";
                    return false;
                }

                // Tenta abrir diretamente pelo nome (como no seu código original)
                if (_memory.OpenProcess(_processName))
                {
                    return true;
                }

                // Fallback: Tenta abrir pelo ID de qualquer processo encontrado com esse nome
                var processes = Process.GetProcessesByName(_processName);
                if (processes.Length == 0)
                {
                    LastError = "Emulador (" + _processName + ") não encontrado!";
                    return false;
                }

                foreach (var proc in processes)
                {
                    if (_memory.OpenProcess(proc.Id))
                    {
                        return true;
                    }
                }

                LastError = "Falha ao abrir processo. Verifique o Antivírus.";
                return false;
            }
            catch (Exception ex)
            {
                LastError = "Erro: " + ex.Message;
                return false;
            }
        }

        public async Task<bool> ApplyPattern(string search, string replace)
        {
            if (!OpenProcess()) return false;

            try
            {
                // Busca exatamente como o original fazia
                var results = await _memory.AoBScan(search, true, true);
                
                if (results == null || !results.Any())
                {
                    LastError = "Padrão não encontrado! Entre na partida.";
                    return false;
                }

                foreach (long addr in results)
                {
                    _memory.WriteMemory(addr.ToString("X"), "bytes", replace);
                }
                return true;
            }
            catch (Exception ex)
            {
                LastError = "Erro: " + ex.Message;
                return false;
            }
        }

        public async Task<bool> RevertPattern(string original, string modified)
        {
            // Para reverter, buscamos o padrão MODIFICADO e o substituímos pelo ORIGINAL
            if (!OpenProcess()) return false;

            try
            {
                var results = await _memory.AoBScan(modified, true, true);
                
                if (results == null || !results.Any())
                {
                    // Se não encontrar o padrão modificado, pode ser que já tenha sido revertido
                    // ou que não foi aplicado corretamente.
                    return true; 
                }

                foreach (long addr in results)
                {
                    _memory.WriteMemory(addr.ToString("X"), "bytes", original);
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<IEnumerable<long>> Scan(string search)
        {
            if (!OpenProcess()) return Enumerable.Empty<long>();

            try
            {
                var results = await _memory.AoBScan(search, true, true);
                
                if (results == null || !results.Any())
                {
                    LastError = "Padrão não encontrado! Entre na partida.";
                    return Enumerable.Empty<long>();
                }
                return results;
            }
            catch (Exception ex)
            {
                LastError = "Erro no Scan: " + ex.Message;
                return Enumerable.Empty<long>();
            }
        }

        public int ReadInt(string addressHex)
        {
            if (!OpenProcess()) return 0;
            try
            {
                return _memory.ReadMemory<int>(addressHex);
            }
            catch (Exception ex)
            {
                LastError = "Erro ao ler int: " + ex.Message;
                return 0;
            }
        }

        public void WriteInt(string addressHex, int value)
        {
            if (!OpenProcess()) return;
            try
            {
                _memory.WriteMemory(addressHex, "int", value.ToString());
            }
            catch (Exception ex)
            {
                LastError = "Erro ao escrever int: " + ex.Message;
            }
        }
    }
}
