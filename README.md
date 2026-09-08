# 22 Panel — Painel de Cheats para Free Fire

Painel desktop (Windows x64, .NET Framework 4.7.2) com autenticação KeyAuth, injeção de DLL,
manipulação de memória (AoB scan) e features de aimbot, chams, ESP, no recoil, camera,
tracking, stream mode e bypass de segurança.

Desenvolvido para rodar em emuladores (BlueStacks / HD-Player, etc.) com elevação de admin.

## Estrutura

```
Core/
  Injector.cs        -> P/Invoke + ExtractEmbeddedDll + LoadLibraryA remote injection
  KeyAuth.cs         -> API KeyAuth 1.2 (auth, checksum, hmac, webhook, chat, file...)
  MemoryManager.cs   -> Wrapper da Memory.dll (AoB scan, read/write int, pattern apply/revert)
Features/
  FeatureBase.cs         -> Abstract Toggle contract
  MemoryFeature.cs       -> Aplica byte pattern via MemoryManager (pixel, antena, camera, tracking...)
  MemoryScanFeature.cs   -> Scan + offset para aimbot
  DllFeature.cs          -> Extrai e injeta DLLs embutidas (chams, ESP)
Models/
  MemoryPattern.cs   -> DTO (Search / Replace)
UI/
  LoginForm.cs       -> Tela de login com KeyAuth e key cache
  MainForm.cs        -> Painel principal com tabs (Aim / Visual / Misc / Safety)
                     -> Hotkey INSERT show/hide + particle background + stream mode (DisplayAffinity)
```

## DLLs nativas embutidas

- `Memory.dll` — lib de scan/write em memória
- `MENU CHAMS 64 BITS.dll`
- `BLUE_AND_WHITE.dll`
- `transparent.dll`
- `hakulines.dll` — lines ESP

## Requisitos

- Windows 10+ x64
- .NET Framework 4.7.2
- Visual Studio 2022 (para buildar)
- Emulador HD-Player rodando Free Fire
- **Executar como ADMINISTRADOR** (MemoryManager exige)

## Build

```bash
msbuild ascendedAuth_v2.sln /p:Configuration=Release /p:Platform=x64
# ou abre ascendedAuth_v2.sln no Visual Studio e compila como Release x64
```

## Autenticação

Credenciais KeyAuth (editar em `UI/LoginForm.cs` se for trocar):
- App: `ascendedAuth`
- OwnerID / Secret / Version configurados no `LoginForm()`

Licença digitada é persistida em `Properties.Settings.Default.LicenseKey` em caso de sucesso.

## Features por aba

| Aba       | Features                                                                 |
|-----------|--------------------------------------------------------------------------|
| **Aim**   | Aimbot Ombro, Aimbot Rage, Aimbot Legit, No Recoil                      |
| **Visual**| Chams (Menu / B&W / Basic), Lines ESP                                   |
| **Misc**  | Pixel Extendido, Antena Male, Camera Direita, 2x Tracking, Stream Mode |
| **Safety**| Bypass / Panic (desliga tudo, limpa key, mata o painel)                 |

Hotkey global: **INSERT** — mostra / esconde o painel.

## Modo Stream

Esconde sombra da janela e aplica `SetWindowDisplayAffinity(17/1)` pra proteger a tela
de ser capturada por OBS / gravadores.

## Aviso

Projeto destinado **apenas para fins acadêmicos / de estudo**. O uso indevido em jogos
online viola Termos de Serviço. Usa por tua conta e risco.
