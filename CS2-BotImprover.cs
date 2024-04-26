using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Attributes.Registration;
using CounterStrikeSharp.API.Modules.Commands;
using CounterStrikeSharp.API.Modules.Utils;
using CounterStrikeSharp.API.Modules.Memory;
using CounterStrikeSharp.API.Modules.Memory.DynamicFunctions;

using Microsoft.Extensions.Logging;

namespace BotImprover;
public class BotImprover : BasePlugin
{
    public override string ModuleName => "CS2 BotImprover Plugin";

    public override string ModuleVersion => "0.0.1";

    public override string ModuleAuthor => "LynchMus";

    public override string ModuleDescription => "BotImprover plugin";

    private MemoryFunctionWithReturn<nint, string, vector, int, float, bool, float, bool> CCSBot_SetLookAtFunc =
        new("55 48 89 E5 41 57 49 89 FF 41 56 45 89 C6 41 55 41 54 49 89 F4", Addresses.ServerPath);

    private MemoryFunctionVoid CCSBot_UpKeepFuncVoid =
        new("55 48 89 E5 41 57 41 56 41 55 41 54 49 89 FC 53 48 83 EC 38 4C 8B 2D 7D D9 FA 00", Addresses.ServerPath);

    public override void Load(bool hotReload)
    {
        Console.WriteLine("======================================");
        Console.WriteLine("HIME BotImprover Load Start!");
        try
        {
            CCSBot_SetLookAtFunc.Hook(Hook_CCSBot_SetLookAt, HookMode.Pre);
            CCSBot_UpKeepFuncVoid.Hook(Hook_CCSBot_UpKeepVoid, HookMode.Pre);
        }
        catch (Exception ex)
        {
            if (ex.Message != "Invalid game event")
            {
                Console.WriteLine("[BotImprover] HookFailed: " + ex.Message);
                Logger.LogInformation("[BotImprover] HookFailed: " + ex.Message);
            }
        }
        Console.WriteLine("HIME BotImprover Load Finish!");
        Console.WriteLine("======================================");
    }

    private HookResult Hook_CCSBot_UpKeepVoid(DynamicHook hook)
    {
        /*
        try
        {
            //Console.WriteLine("===============================================");
            //Console.WriteLine("CCSBot_UpKeep was fired!");
            //Console.WriteLine("===============================================");
        }
        catch (Exception ex)
        {
            if (ex.Message != "Invalid game event")
            {
                Console.WriteLine("[BotImprover] Hook Failed: " + ex.Message);
                Logger.LogInformation("[BotImprover] Hook Failed: " + ex.Message);
            }
        }
        */
        return HookResult.Continue;
    }

    private HookResult Hook_CCSBot_SetLookAt(DynamicHook hook)
    {
        try
        {
            Console.WriteLine("[BotImprover] SetLookAt: " + hook.GetReturn<string>(1));
            //hook.SetReturn<float>(0.0f);
            return HookResult.Continue;
        }
        catch (Exception ex)
        {
            if (ex.Message != "Invalid game event")
            {
                Console.WriteLine("[BotImprover] SetLookAt Failed: " + ex.Message);
                Logger.LogInformation("[BotImprover] SetLookAt Failed: " + ex.Message);
            }
        }
        return HookResult.Continue;
    }
}