# TAdvancedHealth 

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
- [Workshop](https://steamcommunity.com/sharedfiles/filedetails/?id=2067970311)

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
