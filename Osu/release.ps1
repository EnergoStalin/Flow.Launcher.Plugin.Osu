dotnet publish Flow.Launcher.Plugin.Osu -c Release -r win-x64
Compress-Archive -LiteralPath Flow.Launcher.Plugin.Osu/bin/Release/win-x64/publish -DestinationPath Flow.Launcher.Plugin.Osu/bin/Osu.zip -Force