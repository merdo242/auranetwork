[Setup]
AppName=Merdo Launcher
AppVersion=6.2
AppPublisher=MerdoNetwork
AppPublisherURL=https://merdonetwork.com
DefaultDirName={autopf}\Merdo Launcher
DefaultGroupName=Merdo Launcher
OutputDir=..\installer
OutputBaseFilename=MerdoLauncher_Setup
SetupIconFile=
Compression=lzma2/ultra64
SolidCompression=yes
WizardStyle=modern
DisableProgramGroupPage=yes
PrivilegesRequired=lowest
UninstallDisplayIcon={app}\MerdoLauncher.exe
ArchitecturesInstallIn64BitMode=x64compatible

[Languages]
Name: "turkish"; MessagesFile: "compiler:Languages\Turkish.isl"

[Tasks]
Name: "desktopicon"; Description: "Masaüstüne kısayol oluştur"; GroupDescription: "Ek görevler:"
Name: "startmenu"; Description: "Başlat menüsüne kısayol ekle"; GroupDescription: "Ek görevler:"

[Files]
Source: "..\publish\*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs createallsubdirs
Source: "..\MerdoClient\icon.ico"; DestDir: "{app}"; Flags: ignoreversion

[Icons]
Name: "{group}\Merdo Launcher"; Filename: "{app}\MerdoLauncher.exe"; IconFilename: "{app}\icon.ico"
Name: "{autodesktop}\Merdo Launcher"; Filename: "{app}\MerdoLauncher.exe"; IconFilename: "{app}\icon.ico"; Tasks: desktopicon

[Run]
Filename: "{app}\MerdoLauncher.exe"; Description: "Merdo Launcher'ı başlat"; Flags: nowait postinstall skipifsilent
