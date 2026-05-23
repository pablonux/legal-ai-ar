$code = @'
using System;
using System.Runtime.InteropServices;

public class Jiggler {
    [DllImport("user32.dll")]
    static extern void mouse_event(uint dwFlags, int dx, int dy, uint dwData, UIntPtr dwExtraInfo);

    [DllImport("kernel32.dll", SetLastError = true)]
    public static extern uint SetThreadExecutionState(uint esFlags);

    const uint MOUSEEVENTF_MOVE = 0x0001;
    public const uint ES_CONTINUOUS       = 0x80000000;
    public const uint ES_SYSTEM_REQUIRED  = 0x00000001;
    public const uint ES_DISPLAY_REQUIRED = 0x00000002;

    public static void Jiggle() {
        mouse_event(MOUSEEVENTF_MOVE, 1, 0, 0, UIntPtr.Zero);
        System.Threading.Thread.Sleep(50);
        mouse_event(MOUSEEVENTF_MOVE, -1, 0, 0, UIntPtr.Zero);
    }

    public static void PreventSleep() {
        SetThreadExecutionState(ES_CONTINUOUS | ES_SYSTEM_REQUIRED | ES_DISPLAY_REQUIRED);
    }
}
'@

Add-Type -TypeDefinition $code -Language CSharp -ErrorAction Stop

[Jiggler]::PreventSleep()
[Jiggler]::Jiggle()
Write-Host "Mouse jiggler activo (mouse_event + SetThreadExecutionState) - cada 25 segundos..."

while ($true) {
    [Jiggler]::PreventSleep()
    [Jiggler]::Jiggle()
    Start-Sleep -Seconds 25
}
