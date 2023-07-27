# Terraria Depot Downloader - Terraria 1.1.2 1.2X 1.3X 1.4X+

[![Build status](https://ci.appveyor.com/api/projects/status/4je4mgn8thq15cf9?svg=true)](https://ci.appveyor.com/project/RussDev7/terrariadepotdownloader) [![GitHub Version](https://img.shields.io/github/tag/RussDev7/TerrariaDepotDownloader.svg?label=GitHub)](https://github.com/RussDev7/TerrariaDepotDownloader) [![Contributors](https://img.shields.io/github/contributors/RussDev7/TerrariaDepotDownloader)](https://github.com/RussDev7/TerrariaDepotDownloader)

![TDD](https://raw.githubusercontent.com/RussDev7/TerrariaDepotDownloader/main/src/TerrariaDepotDownloader/Resources/custom-terraria-logo1.png)

**TerrariaDepotDownloader** - Downgrade your game to any of the past game versions where the steam manifest IDs are known.

## How It Works / Legal
Steam allows the downloading of previous game versions through the use of depots. Depots are a collection of files pertaining to a group; for steam, the game Terraria. When a game gets updated, steam archives each patch as something called a manifest ID. Using steam [console](steam//nav/console) and the correct manifest ID, you can downgrade/download any game version that you own on your account. Using an open source utility called [DepotDownloader](https://github.com/SteamRE/DepotDownloader), you can download depots with a lot more flexibility.

Included in this application's directory is a file named `ManifestVersions.cfg`. This file is in charge of all the game versions and their data which is loaded into the program. This can be updated to include future versions or previously unknown earlier versions of the game without the need to download a new tool version.

**Note to Terraria's team:** I have created this courtesyware in hopes to diminish pirating and bring easability to downgrading. If their is any issues with this tool please do not hessitate to contact me.

## Other Information

<details><summary>Startup Guide</summary><div align="center">
  
  ![custom-terraria-logo (1)](https://user-images.githubusercontent.com/33048298/203858054-033ec2e0-ac09-40a2-bda5-b42c7ccabe8e.png)
  
  **Step 1)** Make sure you have [.NET 6.0 Runtime](https://dotnet.microsoft.com/en-us/download/dotnet/6.0) installed on your computer.
  
  ![05v2](https://user-images.githubusercontent.com/33048298/203858119-6d572a85-b2c9-40b0-bb35-1cc88b81b805.PNG)
  
  \
  **Step 2)** Click the Settings tab and enter your Steam account name and password.\
  This is used to download the game versions from steam.
  
  ![01](https://user-images.githubusercontent.com/33048298/203858142-d79f6d46-f229-48de-8c26-bd73b499b3a7.jpg)

  \
  **Step 3)** In the Downloader tab, select the version you want to download\
  and click the download button in the bottom right corner.
    
 ![02](https://user-images.githubusercontent.com/33048298/203858163-b669c202-6b15-4e63-89f6-8cd35fb0fab2.jpg)

  \
  **Step 4)** A command prompt window will appear. If your Steam account has 2-way verification-\
  Enter your Steam Guard authentication code into the command prompt and hit enter.
  
  ![03](https://user-images.githubusercontent.com/33048298/203858174-7eeaa9cd-42ef-411d-8a61-364a70883b2f.jpg)

  \
  **Step 5)** The game version will download to the TerrariaDepots folder.\
  This Can be changed to any desired location in the settings tab!
  
  ![04](https://user-images.githubusercontent.com/33048298/203858187-85b457df-4906-4e7d-8b8d-eda102935fd6.jpg)

  \
  **Step 6)** If the game does not start and you receive this error message-\
  backup and paste depot files to your game directory and run via steam.\
`C:\Program Files (x86)\Steam\steamapps\common\Terraria`
  
  ![ErrorMsg](https://user-images.githubusercontent.com/33048298/203858221-5686f990-915b-4596-bb38-e2f70630104f.PNG)\
(Or check out the Overwrite Steam Directory feature)
</div></details>
<details><summary>Troubleshooting</summary><p></p>
  
  **Bellow are some useful troubleshooting guides. If you find issues with this application please let me know!**
  <p></p>
  <details><summary>Console appears then immediately closes.</summary><p></p>
    
`Fix #1:`\
Ensure both username & password are correct in the settings tab.
    
  ![USERANDPASS](https://user-images.githubusercontent.com/33048298/203860279-0def82b9-7d9d-4108-aee4-59998103e476.JPG)

`Fix #2:`\
Try installing [.NET 6.0 Runtime -> Run console apps](https://dotnet.microsoft.com/en-us/download/dotnet/6.0/runtime?cid=getdotnetcore).\
IMPORTANT: Make sure to download the correct x64 / x86 version for your system.
    
   ![ConsoleApps](https://user-images.githubusercontent.com/33048298/203860302-f68c6761-2d78-490d-8133-c6e46d3b9d70.JPG)
  </details>
  <details><summary>Windows is blocking me from opening the application.</summary><p></p>
    
`Fix #1:`\
Clicking More info -> Run anyway
    
<img width="319" alt="01" src="https://user-images.githubusercontent.com/33048298/203860340-c2e81606-b416-4a63-bb66-886f643aba44.png">
<img width="319" alt="02 (1)" src="https://user-images.githubusercontent.com/33048298/203860348-d9181eec-45ef-4635-98e3-c571e65ba84c.png">

<p></p>
Why this is happening?
<p></p>

Built into Windows 10, is something called Windows Defender SmartScreen. Each time an application is ran Windows 10, SmartScreen will check if it is a “good” application against their catalog of applications. It’s a good security measure and is particular helpful at stopping malware spreading through email attachments – where some users do not understand the difference between a legitimate document and an application. Sometimes SmartScreen will prevent applications you know are not bad – for example, it’s a CMD or VBS script you wrote, or a program from a trusted source.

Explanation Credits: [Adrian Gordon](https://www.itsupportguides.com/blog/author/agordon/)
  </details>
  <details><summary>Clicking single player / multiplayer crashes my game.</summary><p></p>
    
`Fix #1:`\
Backup or rename existing game saves.\
`C:\Users\%username%\Documents\My Games\Terraria`
    
  ![R B](https://user-images.githubusercontent.com/33048298/203860437-77038bc0-e3d5-4ddf-b7de-4e8e29aeb688.PNG)![R B2](https://user-images.githubusercontent.com/33048298/203860438-5cb611fd-7b05-45c7-82ad-8282a8a865a4.PNG)

Why this is happening?

This issue is typically caused by an existing `\Documents\My Games\Terraria\` directory.
  </details>
  <details><summary>Please launch game from steam client.</summary><p></p>

`Fix #1:`\
Enable Overwrite Steam Directory option from within settings tab.
    
  ![Capture](https://user-images.githubusercontent.com/33048298/203860460-b5a34672-f293-4207-aa11-51a70d0eb837.PNG)

Why this is happening?

This issue is caused from one of the hardcoded checks Terraria does to make sure you own the game. You need to have the game inside your steamapps directory to prevent this error.
  </details>
  <details><summary>Older versions crash when selecting single player.</summary><p></p>
    
`Fix #1:`\
To fix this its super simple. You need to move your Terraria folder (\Documents\My Games\Terraria) to a different location for safe keeping. Then try and reload the game back up. You will have to create a new player and a new world.

Why this is happening?

The older versions can crash and or now show all the UI options when attempting to read newer player and world files.
  </details>
  <details><summary>Steam won't let me play older versions until it "Updates".</summary><p></p>
    
`Fix #1:`\
Within your steam client, go to `Library > Terraria > Properties > Updates` and change `Automatic Updates` to Only `update this game when I launch it` and turn `Background Downloads` to `Never allow background downloads`.

`Fix #2:`\
Launching steam in offline mode it will prevent the searching of a new update. You can do this within your steam client by navigating to Steam > Go offline or by closing steam, disconnecting from the internet, and re-launching steam.

Why this is happening?

For some people, steam will try to keep Terraria up to date automatically. This can very on your settings within steam.
  </details>
</details>
<details><summary>Changelog</summary>

    v1.8.5.5
     - Added an "Use Steam Directory" checkbox to fix "Please launch the game from your steam client".
     - Added dark mode theme.
     - Fixed refreshing not disabling the download button.

    v​1.8.5.4
     - Added the ability to download via GitHub links.
     - Added resilience to non-default install locations.
     - Added an save login option.
     - Fixed intelsense project warning messages.
     - Updated decompiler messages.
     - Fixed some GUI text.

    v1.8.5.3
     - Fixed an issue where the launch button was not correctly launching desired versions.
     
    v1.8.5.2
     - Fixed an issue where passwords using special characters would cause terminal crashes.
     
    v1.8.5.1
     - Added missing tooltips to checkbox.
     - Bug fixes.
     
    v1.8.5
     - Fixed right clicking for tools within listview.
     - Added tooltips (can be disabled via checkbox).
     - Added checkbox to disable DepotDownloader API updates checks.
     - Fixed tab indexes along with some logged items.
     - Bug fixes.
     
    v1.8.4
     - Fixed versions 1.3.0.1 & 1.4.0.1 not being displayed as downloaded.
     - Removed maximization.
     
    v1.8.3
     - Remove game now terminates only games running within target directory.
     - Remove All will now terminate any running games prior.
     - Fixed File > Download App not being displayed.
     - Fixed File > Remove App not being displayed.
     - .NET Framework check is now done via filesystem over registry.
     - Added .NET version to output log.
     - Exception handling issues fixed.
     - Fixed log spellings.
     - General bugs.
     
    v1.8.2
     - Loading application now checks for DepotDownloader API updates.
     - Overwrite Steam Directory bug not removing previous versions prior to installing a new copy.
     
    v1.6.0 - v1.8.1
     - Updated DepotDownloader API.
     - General bug fixes.
     
    v1.5.0
     - Added a Overwrite Steam Directory option.
     - Fixed some logging typos.
     
    v1.4.0
     - Fixed an issue with properly finding .NET versions.
     - Updated DepotDownloader API.
     - General bug fixes.
     
    v1.2.0 - v1.3.0
     - Initial release.
</details>

## Important Links

- [Forums Link](https://forums.terraria.org/index.php?threads/terrariadepotdownloader-downgrade-to-any-version.107519/)
- [Startup Guide](https://forums.terraria.org/index.php?threads/terrariadepotdownloader-downgrade-to-any-version.107519/)
- [Discord](https://discord.gg/fEK6eE7W)

## Download

- [GitHub Releases](https://github.com/RussDev7/TerrariaDepotDownloader/releases)
- [Terraria Forums](https://forums.terraria.org/index.php?threads/terrariadepotdownloader-downgrade-to-any-version.107519/)

## Support & Credits

- [DepotDownloader API](https://github.com/SteamRE/DepotDownloader)
- [Donations](https://www.paypal.com/cgi-bin/webscr?cmd=_donations&business=imthedude030@gmail.com&lc=US&item_name=Donation&currency_code=USD&bn=PP%2dDonationsBF)
