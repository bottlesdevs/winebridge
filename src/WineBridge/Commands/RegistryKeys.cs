using System.Diagnostics.CodeAnalysis;
using System.Security.AccessControl;
using Microsoft.Win32;

namespace WineBridge.Commands;

[SuppressMessage("Interoperability", "CA1416:Validate platform compatibility")]
internal class RegistryKeys : ICommand
{
    public void Execute(string[]? args)
    {
        if (args is null)
        {
            Console.WriteLine("Error. Usage: WineBridge.exe GetKeys MacroArea");
            return;
        }
        if (args.Length < 2)
        {
            Console.WriteLine("Error. Usage: WineBridge.exe GetKeys MacroArea(HU, HCU, HLMS, HLM, HLMO)" +
                              " Action(list, modifystart)");
            return;
        }
        
        string subKey;
        RegistryKey? registry;
        
        switch (args[0].ToUpperInvariant())
        {
            case "HU":
                subKey = @"S-1-5-21-0-0-0-1000\Software\Microsoft\Windows\CurrentVersion\Run";
                registry = Registry.Users;
                break;
            case "HCU":
                subKey = @"Software\Microsoft\Windows\CurrentVersion\Run";
                registry = Registry.CurrentUser;
                break;
            case "HLMS":
                subKey = @"System\CurrentControlSet\Services";
                registry = Registry.LocalMachine;
                break;
            case "HLM":
                subKey = @"Software\Microsoft\Windows\CurrentVersion\Run";
                registry = Registry.LocalMachine;
                break;
            case "HLMO":
                subKey = @"Software\Microsoft\Windows\CurrentVersion\RunOnce";
                registry = Registry.LocalMachine;
                break;
            default:
                Console.WriteLine("Error. Unknown MacroArea");
                return;
        }

        switch (args[1].ToLowerInvariant())
        {
            case "list":
                ListKeys(registry, subKey);
                break;
            case "modifystart":
                ModifyKey(registry, subKey, args);
                break;
            default:
                Console.WriteLine("Error. Unknown action");
                return;
        }
    }

    private void ListKeys(RegistryKey registry, string subKey)
    {

        using var key = registry.OpenSubKey(subKey);
        if (key == null)
        {
            Console.WriteLine($"Error. Can't open registry key {registry} - {subKey}");
            return;
        }

        if (key.SubKeyCount == 0)
        {
            // print content
            foreach (var value in key.GetValueNames())
            {
                if(!string.IsNullOrEmpty(value))
                    Console.WriteLine($"{value}|{key.GetValue(value)}");
            }
        }
        else
        {
            // print sub keys DisplayName (if available)
            foreach(var subkeyName in key.GetSubKeyNames())
            {
                using var subSubKey = key.OpenSubKey(subkeyName);
                var displayName = subSubKey?.GetValue("DisplayName");
                if (displayName == null) continue;
                if(string.IsNullOrEmpty(displayName.ToString())) continue;
                
                var startValue = subSubKey?.GetValue("Start");
                if (startValue == null)
                {
                    Console.WriteLine($"{subkeyName}|{displayName}|-1");
                    continue;
                }

                if (string.IsNullOrEmpty(startValue.ToString()))
                {
                    Console.WriteLine($"{subkeyName}|{displayName}|-1");
                    continue;
                }
                
                Console.WriteLine($"{subkeyName}|{displayName}|{startValue}");
            }   
        }
    }

    private void ModifyKey(RegistryKey registry, string subKey, string[] args)
    {
        if (args.Length != 4)
        {
            Console.WriteLine("Error. Usage: WineBridge.exe GetKeys MacroArea(HU, HCU, HLMS, HLM, HLMO)" +
                              " Modify \"Key Name\" 0|1|2|3");
            return;
        }

        if (!short.TryParse(args[3], out var startValue))
        {
            Console.WriteLine("Error. Invalid start value. Usage: WineBridge.exe GetKeys MacroArea(HU, HCU, HLMS, HLM, HLMO)" +
                              " Modify \"Key Name\" 0|1|2|3");
            return;
        }
        
        using var key = registry.OpenSubKey(subKey);
        if (key == null)
        {
            Console.WriteLine($"Error. Can't open registry key {registry} - {subKey}");
            return;
        }

        var subSubKey = key.OpenSubKey(args[2], true);
        if (subSubKey == null)
        {
            Console.WriteLine($"Error. Can't open registry key {registry} - {subKey} - {args[2]}");
            return;
        }
        
        var currentStart = subSubKey.GetValue("Start");
        if (currentStart == null)
        {
            Console.WriteLine($"Error. Registry key {registry} - {subSubKey} doesn't have a Start value.");
            return;
        }
        
        try
        {
            subSubKey.SetValue("Start", startValue);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error. {ex.Message}");
        }
    }
}