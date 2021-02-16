using Harmony;
using BattleTech;
using BattleTech.UI;
using System;
using System.Reflection;

namespace CustomVoicesFixAIVoice {
    public static class Main {
        public static void Init(string directory, string settingsJSON)
        {
            var harmony = HarmonyInstance.Create("com.bzy-xyz.CustomVoicesFixAIVoice");
            harmony.PatchAll(Assembly.GetExecutingAssembly());

            MethodInfo PilotRepresentation__PlayPilotVO__T = typeof(PilotRepresentation).GetMethod("PlayPilotVO");
            MethodInfo PilotRepresentation__PlayPilotVO__AudioSwitch_dialog_lines_pilots = PilotRepresentation__PlayPilotVO__T.MakeGenericMethod(typeof(AudioSwitch_dialog_lines_pilots));
            MethodInfo PilotRepresentation__PlayPilotVO__Patch__Prefix = typeof(PilotRepresentation__PlayPilotVO__Patch).GetMethod("Prefix");

            harmony.Patch(
                PilotRepresentation__PlayPilotVO__AudioSwitch_dialog_lines_pilots, 
                new HarmonyMethod(PilotRepresentation__PlayPilotVO__Patch__Prefix),
                null,
                null
            );
        }
    }

    public static class PilotRepresentation__PlayPilotVO__Patch {        
        public static bool Prefix(ref PilotRepresentation __instance, ref bool ___startedVOStatic, AudioSwitch_dialog_lines_pilots VOEnumValue, int priority, AkCallbackManager.EventCallback callback, object in_cookie)
        {
            string voice = __instance.pilot.pilotDef.Voice;
            UnityEngine.Debug.Log($"PilotRepresentation__PlayPilotVO__Patch {voice} {VOEnumValue}");
            if (string.IsNullOrEmpty(voice) || voice != "pilot_player_computer") {
                return true;
            }

            UnityEngine.Debug.Log($"PilotRepresentation__PlayPilotVO__Patch {__instance.pilot.pilotVoice}");
            UnityEngine.Debug.Log($"PilotRepresentation__PlayPilotVO__Patch doing the thing");

            if (!___startedVOStatic)
			{
				___startedVOStatic = true;
				WwiseManager.PostEvent(AudioEventList_vo.vo_static_start_pilot, __instance.audioObject);
			}
			AudioEventManager.InterruptPilotVOForTeam(__instance.pilot.ParentActor.team, null);

            WwiseManager.SetSwitch(VOEnumValue, __instance.audioObject);
            WwiseManager.PostEvent(AudioEventList_vo.vo_play_pilot_player_character, __instance.audioObject, (callback != null) ? callback : new AkCallbackManager.EventCallback(__instance.AudioCallback), in_cookie);

            MethodInfo _isPlayingVO_Setter = typeof(PilotRepresentation).GetProperty("IsPlayingVO", BindingFlags.Instance | BindingFlags.Public).GetSetMethod(nonPublic: true);
            MethodInfo _currentVOPriority_Setter = typeof(PilotRepresentation).GetProperty("CurrentVOPriority", BindingFlags.Instance | BindingFlags.Public).GetSetMethod(nonPublic: true);
            
            _isPlayingVO_Setter.Invoke(__instance, new object[1] {
                true
            });
            _currentVOPriority_Setter.Invoke(__instance, new object[1] {
                priority
            });

            return false;
        }
    }

    [HarmonyPatch(typeof(SG_HiringHall_DetailPanel), "PlayPilotSelectionVO")]
    public static class SG_HiringHall_DetailPanel__PlayPilotSelectionVO__Patch
    {
        public static bool Prefix(ref SG_HiringHall_DetailPanel __instance, Pilot p)
        {
            string voice = p.pilotDef.Voice;
            if (string.IsNullOrEmpty(voice) || voice != "pilot_player_computer") {
                return true;
            }

            UnityEngine.Debug.Log($"SG_HiringHall_DetailPanel__PlayPilotSelectionVO__Patch doing the thing");

            WwiseManager.PostEvent(AudioEventList_vo.vo_stop_pilots, WwiseManager.GlobalAudioObject);

            WwiseManager.SetSwitch(AudioSwitch_dialog_lines_computer_ai.welcome_commander, WwiseManager.GlobalAudioObject);
            WwiseManager.PostEvent(AudioEventList_vo.vo_play_computer_ai, WwiseManager.GlobalAudioObject);

            return false;
        }
    }

    [HarmonyPatch(typeof(SGBarracksDossierPanel), "PlayVO")]
    public static class SGBarracksDossierPanel__PlayVO__Patch
    {
        public static bool Prefix(String voice)
        {
            if (string.IsNullOrEmpty(voice) || voice != "pilot_player_computer") {
                return true;
            }

            UnityEngine.Debug.Log($"SGBarracksDossierPanel__PlayVO__Patch doing the thing");

            WwiseManager.PostEvent(AudioEventList_vo.vo_stop_pilots, WwiseManager.GlobalAudioObject);

            WwiseManager.SetSwitch(AudioSwitch_dialog_lines_computer_ai.welcome_commander, WwiseManager.GlobalAudioObject);
            WwiseManager.PostEvent(AudioEventList_vo.vo_play_computer_ai, WwiseManager.GlobalAudioObject);

            return false;
        }
    }

    [HarmonyPatch(typeof(PilotRepresentation), "LoadSoundbanks")]
    public static class PilotRepresentation__LoadSoundbanks__Patch
    {
        public static bool Prefix(ref PilotRepresentation __instance, ref bool ___startedVOStatic)
        {
            string voice = __instance.pilot.pilotDef.Voice;
            if (string.IsNullOrEmpty(voice) || voice != "pilot_player_computer") {
                return true;
            }

            UnityEngine.Debug.Log($"PilotRepresentation__LoadSoundbanks__Patch doing the thing");

            AudioBankList bankId = AudioBankList.vo_pilot_player_character;
            HBS.SceneSingletonBehavior<WwiseManager>.Instance.LoadBank(bankId);
            HBS.SceneSingletonBehavior<WwiseManager>.Instance.voBanks.Add("vo_pilot_player_character");

            ___startedVOStatic = true;

            return false;
        }
    }
};