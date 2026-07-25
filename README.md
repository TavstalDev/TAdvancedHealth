# TAdvancedHealth

![Release (latest by date)](https://img.shields.io/github/v/release/TavstalDev/TAdvancedHealth?style=plastic-square)
![Workflow Status](https://img.shields.io/github/actions/workflow/status/TavstalDev/TAdvancedHealth/release.yml?branch=stable&label=build&style=plastic-square)
![License](https://img.shields.io/github/license/TavstalDev/TAdvancedHealth?style=plastic-square)
![Downloads](https://img.shields.io/github/downloads/TavstalDev/TAdvancedHealth/total?style=plastic-square)
![Issues](https://img.shields.io/github/issues/TavstalDev/TAdvancedHealth?style=plastic-square)

### What is this?
This is the source code of a .NETFramework library written in C#. This library is a plugin made for Unturned 3.24.x+ servers. 

### Description
A custom tarkov like health system with database support.

### Features
* Async database
* Custom health system
* Revive system
* Respawn system (hospitals)
* Anti group friendly fire
* Custom UI

### Requirements
- Unturned 3.24.x or later
- [RocketMod](https://rocketmod.net/) installed on the server
- [Workshop](https://steamcommunity.com/sharedfiles/filedetails/?id=2067970311)

### Installation

1. Download the latest release and its libraries from the [Releases](https://github.com/TavstalDev/TAdvancedHealth/releases) page.
2. Place `TShop.dll` into your server's `Rocket/Plugins/` directory.
3. Extract the libraries archive into `Rocket/Libraries` directory.
4. Start or restart the server. The plugin will generate a default YAML configuration file on first load.
5. Edit the configuration file to your liking, then reload the plugin or restart the server.

### Commands
| - means <b>or</b></br>
[] - means <b>required</b></br>
<> - means <b>optional</b>

<details>
<summary>/cure <player></summary>
<b>Description:</b> Heal yourself or somebody else.
<br>
<b>Permission(s):</b> tadvancedhealth.commands.cure
</details>

<details>
<summary>/sethealth <player> [bodypart] [newHealth]</summary>
<b>Description:</b> Changes your health or somebody else's.
<br>
<b>Permission(s):</b> tadvancedhealth.commands.sethealth
<br>
</details>

<details>
<summary>/sethealthhud list <page> | [name]</summary>
<b>Description:</b> Checks the cost of a specific item.
<br>
<b>Permission(s):</b> tadvancedhealth.commands.sethealthhud
</details>

<details>
<summary>/sethospitalbed <hospitalname></summary>
<b>Description:</b> Sets a respawn point
<br>
<b>Permission(s):</b> tadvancedhealth.commands.sethospitalbed
</details>

<details>
<summary>/togglehealthhud</summary>
<b>Description:</b> Toggles the custom hud.
<br>
<b>Permission(s):</b> tadvancedhealth.commands.togglehud
</details>

## Building from Source

### Prerequisites

- .NET Framework 4.8 SDK / targeting pack

### Steps

1. Clone the repository:
   ```
   git clone https://github.com/TavstalDev/TAdvancedHealth.git
   ```
2. Open `TShop.sln` in your IDE.
3. Build the project:
   ```
   dotnet build -c Release
   ```
4. The output DLL will be at `TShop/bin/Release/net48/TShop.dll`.

## License

This project is licensed under the GNU General Public License v3.0. See the `LICENSE` file for more details.

## Contact

For issues or feature requests, please use the [GitHub issue tracker](https://github.com/TavstalDev/TAdvancedHealth/issues).