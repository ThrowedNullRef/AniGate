How to update the installer:

1. Run "heat dir . -o MyHarvestedStuff.wxs -scom -frag -srd -sreg -gg -cg AniGateComponentGroup -dr InstallFolder" on the target binaries
2. Copy the content of MyHarvestedStuff.wsx to Product.wsx
3. Replace ..\AniGate.WpfClient\bin\Release\net6.0-windows with the actual relative path to the binaries
4. Make sure you did not replace "..\AniGate.WpfClient\bin\Release\net6.0-windows" from the directory "TARGETDIR"

