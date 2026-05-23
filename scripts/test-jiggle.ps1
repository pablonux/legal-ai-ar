Add-Type -AssemblyName System.Windows.Forms

$code = @'
using System;
using System.Runtime.InteropServices;
public class TestJiggle {
    [DllImport("user32.dll")]
    static extern void mouse_event(uint dwFlags, int dx, int dy, uint dwData, UIntPtr dwExtraInfo);
    const uint MOUSEEVENTF_MOVE = 0x0001;

    public static void Move(int dx, int dy) {
        mouse_event(MOUSEEVENTF_MOVE, dx, dy, 0, UIntPtr.Zero);
    }
}
'@
Add-Type -TypeDefinition $code -Language CSharp -ErrorAction Stop

$p1 = [System.Windows.Forms.Cursor]::Position
Write-Host "Antes: X=$($p1.X) Y=$($p1.Y)"

Write-Host "Llamando mouse_event(+5, +5)..."
[TestJiggle]::Move(5, 5)
Start-Sleep -Milliseconds 200

$p2 = [System.Windows.Forms.Cursor]::Position
Write-Host "Despues: X=$($p2.X) Y=$($p2.Y)"

if ($p1.X -ne $p2.X -or $p1.Y -ne $p2.Y) {
    Write-Host "RESULTADO: mouse_event FUNCIONA"
    [TestJiggle]::Move(-5, -5)
} else {
    Write-Host "RESULTADO: mouse_event NO funciona - probando SetCursorPos..."
    [System.Windows.Forms.Cursor]::Position = New-Object System.Drawing.Point(($p1.X + 5), ($p1.Y + 5))
    Start-Sleep -Milliseconds 200
    $p3 = [System.Windows.Forms.Cursor]::Position
    Write-Host "Con SetCursorPos: X=$($p3.X) Y=$($p3.Y)"
    if ($p1.X -ne $p3.X -or $p1.Y -ne $p3.Y) {
        Write-Host "RESULTADO: SetCursorPos SI funciona (pero no genera input events)"
    } else {
        Write-Host "RESULTADO: Ninguno funciona - posible problema de permisos/sesion"
    }
    [System.Windows.Forms.Cursor]::Position = New-Object System.Drawing.Point($p1.X, $p1.Y)
}
