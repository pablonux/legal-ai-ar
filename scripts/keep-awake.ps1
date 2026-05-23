<#
.SYNOPSIS
    Prevents screen/system sleep using Windows SetThreadExecutionState API.
.DESCRIPTION
    Calls SetThreadExecutionState in a loop to tell Windows the system is in use.
    ES_CONTINUOUS | ES_SYSTEM_REQUIRED | ES_DISPLAY_REQUIRED keeps both
    the screen on and the system from sleeping.

    Run as background job:
      Start-Job -Name 'KeepAwake' -FilePath .\scripts\keep-awake.ps1
    Stop:
      Get-Job -Name 'KeepAwake' | Stop-Job | Remove-Job
#>

$code = @'
using System;
using System.Runtime.InteropServices;
public class SleepPreventer {
    [DllImport("kernel32.dll", SetLastError = true)]
    public static extern uint SetThreadExecutionState(uint esFlags);

    public const uint ES_CONTINUOUS        = 0x80000000;
    public const uint ES_SYSTEM_REQUIRED   = 0x00000001;
    public const uint ES_DISPLAY_REQUIRED  = 0x00000002;

    public static void PreventSleep() {
        SetThreadExecutionState(ES_CONTINUOUS | ES_SYSTEM_REQUIRED | ES_DISPLAY_REQUIRED);
    }

    public static void AllowSleep() {
        SetThreadExecutionState(ES_CONTINUOUS);
    }
}
'@

Add-Type -TypeDefinition $code -Language CSharp -ErrorAction Stop

[SleepPreventer]::PreventSleep()
Write-Host "KeepAwake: sleep/hibernation prevented via SetThreadExecutionState"

while ($true) {
    [SleepPreventer]::PreventSleep()
    Start-Sleep -Seconds 30
}
