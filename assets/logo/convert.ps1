Add-Type -AssemblyName System.Drawing
$img = [System.Drawing.Bitmap]::FromFile("c:\Users\QS\Downloads\POS\DrMusa\assets\logo\DrMusa-logo.jpg")
$iconHandle = $img.GetHicon()
$icon = [System.Drawing.Icon]::FromHandle($iconHandle)
$fs = [System.IO.FileStream]::new("c:\Users\QS\Downloads\POS\DrMusa\assets\logo\DrMusa-logo.ico", [System.IO.FileMode]::Create)
$icon.Save($fs)
$fs.Close()
$icon.Dispose()
$img.Dispose()
