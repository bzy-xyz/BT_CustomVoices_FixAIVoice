CustomVoices_FixAIVoice
=======================

(Will be obsoleted by a future release of CustomVoices; the essential changes were PR'd upstream.)

Fixes the AI voice with CustomVoices installed.

Why
---
The AI voice gets its own VO AudioEvent type and sound bank, and does
_not_ work if you try to treat it as an ordinary voice.

Specifically, you have to send `vo_play_pilot_player_character` event
and have the `vo_pilot_player_character` AudioBank loaded for the voice
to play correctly in-mission.

(Out-of-mission voice can be done with `vo_play_computer_ai` instead.)

Dependencies
------------
* CustomVoices (doesn't do anything useful without it...)

Build from source
-----------------
* Fix any wrong reference hints in the `CustomVoices_FixAIVoice.csproj`
  (not needed when cloning to `Mods`).
* Run `dotnet build CustomVoices_FixAIVoice`.
