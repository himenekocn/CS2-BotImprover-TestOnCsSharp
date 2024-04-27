using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Attributes.Registration;
using CounterStrikeSharp.API.Modules.Commands;
using CounterStrikeSharp.API.Modules.Utils;
using CounterStrikeSharp.API.Modules.Memory;
using CounterStrikeSharp.API.Modules.Memory.DynamicFunctions;
using Vector = CounterStrikeSharp.API.Modules.Utils.Vector;
using Microsoft.Extensions.Logging;

namespace BotImprover;

public enum DispositionType
{
    ENGAGE_AND_INVESTIGATE, // engage enemies on sight and investigate enemy noises
    OPPORTUNITY_FIRE,       // engage enemies on sight, but only look towards enemy noises, dont investigate
    SELF_DEFENSE,           // only engage if fired on, or very close to enemy
    IGNORE_ENEMIES,         // ignore all enemies - useful for ducking around corners, running away, etc
    NUM_DISPOSITIONS
};

public enum PriorityType
{
    PRIORITY_LOW,
    PRIORITY_MEDIUM,
    PRIORITY_HIGH,
    PRIORITY_UNINTERRUPTABLE
};

public class BotImprover : BasePlugin
{
    public override string ModuleName => "CS2 BotImprover Plugin";

    public override string ModuleVersion => "0.0.1";

    public override string ModuleAuthor => "LynchMus";

    public override string ModuleDescription => "BotImprover plugin";

    //client, desc, pos, pri, dur, clearIfClose, angleTolerance, attack
    private MemoryFunctionVoid<nint, string, Vector, PriorityType, float, bool, float, bool> CCSBot_SetLookAtFunc =
        new("55 48 89 E5 41 57 49 89 FF 41 56 45 89 C6 41 55 41 54 49 89 F4", Addresses.ServerPath);

    private MemoryFunctionVoid<nint> CCSBot_PickNewAimSpotFunc =
        new("55 48 89 E5 41 57 41 56 41 55 41 54 53 48 83 EC 58 80 3D E8 38 1F 01 00", Addresses.ServerPath);

    //In CCSBot::Upkeep code bottom
    private MemoryFunctionWithReturn<float, float> BotCOSFunc =
        new("F3 0F 5C 05 ? ? ? ? F3 0F 10 0D ? ? ? ?", Addresses.ServerPath);
    //In CCSBot::Upkeep code bottom
    private MemoryFunctionWithReturn<float, float> BotSINFunc =
        new("F3 0F 5C 05 D4 9E C1 00", Addresses.ServerPath);

    //private MemoryFunctionVoid CCSBot_UpKeepFuncVoid =
    //    new("55 48 89 E5 41 57 41 56 41 55 41 54 49 89 FC 53 48 83 EC 38 4C 8B 2D 7D D9 FA 00", Addresses.ServerPath);

    public override void Load(bool hotReload)
    {
        Logger.LogInformation("======================================");
        Logger.LogInformation("HIME BotImprover Load Start!");
        
        try
        {
            Logger.LogInformation("HIME BotImprover StartHook SetLookAt!");
            CCSBot_SetLookAtFunc.Hook(Hook_CCSBot_SetLookAt, HookMode.Pre);
            Logger.LogInformation("HIME BotImprover StartHook BotCOS!");
            BotCOSFunc.Hook(Hook_BotCOS, HookMode.Pre);
            //Logger.LogInformation("HIME BotImprover StartHook BotSIN!");
            //BotSINFunc.Hook(Hook_BotSIN, HookMode.Pre);
            Logger.LogInformation("HIME BotImprover StartHook PickNewAimSpot!");
            CCSBot_PickNewAimSpotFunc.Hook(Hook_CCSBot_PickNewAimSpot, HookMode.Post);
            //CCSBot_UpKeepFuncVoid.Hook(Hook_CCSBot_UpKeepVoid, HookMode.Pre);
        }
        catch (Exception ex)
        {
            if (ex.Message != "Invalid game event")
            {
                Logger.LogInformation("[BotImprover] HookFailed: " + ex.Message);
            }
        }

        RegisterListener<Listeners.OnTick>(OnTick);
        Logger.LogInformation("HIME BotImprover OnTick Listening!");

        Logger.LogInformation("HIME BotImprover Load Finish!");
        Logger.LogInformation("======================================");
    }

    public void OnTick()
    {
        try
        {
        }
        catch (Exception ex)
        {
            if (ex.Message != "Invalid game event")
            {
                Logger.LogInformation("[BotImprover] OnTick Failed: " + ex.Message);
            }
        }
    }

    private HookResult Hook_CCSBot_PickNewAimSpot(DynamicHook hook)
    {
        try
        {
            CCSBot bot = new CCSBot(hook.GetParam<nint>(0));
            Schema.SetSchemaValue(bot.Handle, "CCSBot", "m_targetSpot", new Vector(120, 120, 120));
            return HookResult.Handled;
        }
        catch (Exception ex)
        {
            if (ex.Message != "Invalid game event")
            {
                Logger.LogInformation("[BotImprover] CCSBot_PickNewAimSpot Failed: " + ex.Message);
            }
        }
        return HookResult.Continue;
    }

    private HookResult Hook_BotCOS(DynamicHook hook)
    {
        try
        {
            hook.SetReturn<float>(0f);
            return HookResult.Changed;
        }
        catch (Exception ex)
        {
            if (ex.Message != "Invalid game event")
            {
                Logger.LogInformation("[BotImprover] BotCOS Failed: " + ex.Message);
            }
        }
        return HookResult.Continue;
    }

    private HookResult Hook_BotSIN(DynamicHook hook)
    {
        try
        {
            hook.SetReturn<float>(0f);
            return HookResult.Changed;
        }
        catch (Exception ex)
        {
            if (ex.Message != "Invalid game event")
            {
                Logger.LogInformation("[BotImprover] BotSIN Failed: " + ex.Message);
            }
        }
        return HookResult.Continue;
    }

    private HookResult Hook_CCSBot_SetLookAt(DynamicHook hook)
    {
        try
        {
            CCSBot bot = new CCSBot(hook.GetParam<nint>(0));
            //CCSPlayerController player = bot.Controller;
            string Desc = hook.GetParam<string>(1);
            //Logger.LogInformation("[BotImprover] "+ bot.Name +" SetLookAt: " + hook.GetParam<string>(1));
            if (Desc.Equals("Defuse bomb", StringComparison.OrdinalIgnoreCase))
            {
                return HookResult.Continue;
            }
            else if (Desc.Equals("Use entity", StringComparison.OrdinalIgnoreCase))
            {
                return HookResult.Continue;
            }
            else if (Desc.Equals("Open door", StringComparison.OrdinalIgnoreCase))
            {
                return HookResult.Continue;
            }
            else if (Desc.Equals("Hostage", StringComparison.OrdinalIgnoreCase))
            {
                return HookResult.Continue;
            }
            else if (Desc.Equals("Avoid Flashbang", StringComparison.OrdinalIgnoreCase))
            {
                Vector fNadePos = hook.GetParam<Vector>(2);
                Logger.LogInformation("[BotImprover] Avoid Flashbang: " + fNadePos.X + " " + fNadePos.Y + " " + fNadePos.Z);
                CCSBot_BendLineOfSight(bot, bot.EyePosition, fNadePos, fNadePos, 140.0f);
                Logger.LogInformation("[BotImprover] Avoid Flashbang Bend: " + fNadePos.X + " " + fNadePos.Y + " " + fNadePos.Z);
                hook.SetParam<Vector>(2, fNadePos);
                hook.SetParam<float>(4, 2.0f);
                hook.SetParam<float>(5, 1);
                hook.SetParam<float>(6, 1.2f);
                hook.SetParam<PriorityType>(3, PriorityType.PRIORITY_HIGH);
                return HookResult.Changed;
            }
            else if (Desc.Equals("Blind", StringComparison.OrdinalIgnoreCase))
            {
                return HookResult.Handled;
            }
            else if (Desc.Equals("Face outward", StringComparison.OrdinalIgnoreCase))
            {
                return HookResult.Handled;
            }
            else if (Desc.Equals("Breakable", StringComparison.OrdinalIgnoreCase))
            {
                return HookResult.Continue;
            }
            else if (Desc.Equals("Plant bomb on floor", StringComparison.OrdinalIgnoreCase))
            {
                return HookResult.Continue;
            }
            else if (Desc.Equals("GrenadeThrowBend", StringComparison.OrdinalIgnoreCase))
            {
                Vector fNadePos = hook.GetParam<Vector>(2);
                Logger.LogInformation("[BotImprover] GrenadeThrow: " + fNadePos.X + " " + fNadePos.Y + " " + fNadePos.Z);
                CCSBot_BendLineOfSight(bot, bot.EyePosition, fNadePos, fNadePos, 135.0f);
                Logger.LogInformation("[BotImprover] GrenadeThrow Bend: " + fNadePos.X + " " + fNadePos.Y + " " + fNadePos.Z);
                hook.SetParam<Vector>(2, fNadePos);
                hook.SetParam<float>(4, 3.0f);
                hook.SetParam<float>(6, 1.5f);
                hook.SetParam<PriorityType>(3, PriorityType.PRIORITY_HIGH);
                return HookResult.Changed;
            }
            else if (Desc.Equals("Noise", StringComparison.OrdinalIgnoreCase))
            {
                Vector fNoisePos = hook.GetParam<Vector>(2);
                hook.SetParam<Vector>(2, new Vector(fNoisePos.X, fNoisePos.Y, fNoisePos.Z + 25.0f));
                return HookResult.Changed;
            }
            else if (Desc.Equals("Panic", StringComparison.OrdinalIgnoreCase))
            {
                Vector fPanicPos = hook.GetParam<Vector>(2);
                hook.SetParam<Vector>(2, new Vector(fPanicPos.X, fPanicPos.Y, fPanicPos.Z + 25.0f));
                return HookResult.Changed;
            }
            else if (Desc.Equals("Last Enemy Position", StringComparison.OrdinalIgnoreCase))
            {
                Vector fClientPos = hook.GetParam<Vector>(2);
                hook.SetParam<Vector>(2, new Vector(fClientPos.X, fClientPos.Y, fClientPos.Z + 25.0f));
                return HookResult.Changed;
            }
            else if (Desc.Equals("Nearby enemy gunfire", StringComparison.OrdinalIgnoreCase))
            {
                Vector fClientPos = hook.GetParam<Vector>(2);
                hook.SetParam<Vector>(2, new Vector(fClientPos.X, fClientPos.Y, fClientPos.Z + 25.0f));
                return HookResult.Changed;
            }
            else
            {
                Vector fClientPos = hook.GetParam<Vector>(2);
                hook.SetParam<Vector>(2, new Vector(fClientPos.X, fClientPos.Y, fClientPos.Z + 25.0f));
                return HookResult.Changed;
            }
        }
        catch (Exception ex)
        {
            if (ex.Message != "Invalid game event")
            {
                Logger.LogInformation("[BotImprover] SetLookAt Failed: " + ex.Message);
            }
        }
        return HookResult.Continue;
    }

    public bool CCSBot_BendLineOfSight(CCSBot bot, Vector Eye, Vector Point, Vector Bend, float AngleLimit)
    {
        try
        {
            var CCSBot_BendLineOfSightFunc = VirtualFunction.Create<nint, Vector, Vector, Vector, float, bool>(
                "55 48 89 E5 41 57 49 89 D7 41 56 41 55 41 54 49 89 FC 53 48 89 F3 48 81 EC D8 01 00 00", Addresses.ServerPath
            );

            return CCSBot_BendLineOfSightFunc(bot.Handle, Eye, Point, Bend, AngleLimit);
        }
        catch (Exception ex)
        {
            if (ex.Message != "Invalid game event")
            {
                Logger.LogInformation("[BotImprover] SetLookAt Failed: " + ex.Message);
            }
            return false;
        }
    }

    public void CCSBot_Attack(CCSBot bot, CCSPlayerController player)
    {
        try
        {
            var CCSBot_BendLineOfSightFunc = VirtualFunction.CreateVoid<nint, nint>(
                "48 85 F6 0F 84 ? ? ? ? 55 48 89 E5 41 55 49 89 F5", Addresses.ServerPath
            );
            CCSBot_BendLineOfSightFunc(bot.Handle, player.Handle);
        }
        catch (Exception ex)
        {
            if (ex.Message != "Invalid game event")
            {
                Logger.LogInformation("[BotImprover] Attack Failed: " + ex.Message);
            }
        }
    }


}