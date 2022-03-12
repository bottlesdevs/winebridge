# 🌉 winebridge
A .Net bridge to run commands in Wine

# ❓Why
_winebridge_ offers a set of commands that you, as user, usually run trough [_winedbg_](https://wiki.winehq.org/Winedbg). winedbg is a heavy tool that requires much time to execute simple tasks because it's born as debugger, not as "runtime utility". winebridge is executed directly in the Wine prefix and, calling Windows' APIs, simply returns the resuls of the command as console output.
