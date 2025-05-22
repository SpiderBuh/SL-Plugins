# Dynamic Tags
Adds the ability to provide in-game tags and permissions for users from a remote endpoint without needing to edit the server's main config files.

> [!IMPORTANT] 
The plugin requires extensive use of an external API which must be custom made. We will not be providing an API for you to use.

# Config
| Config  | Default Value | Description |
| ------------- | ------------- | ------------- |
| api_url  | "https://google.co.uk/" | Base URL for the API to interact with. |
| tracker_enabled | false | Used to enable tracking for users and those with Remote Admin |
| tags_enabled | true | Should the tag system be enabled |
| automatic_northwood_reserved_slot | true | Should anyone connecting with an `@northwood` ID automatically be given a reserved slot |

# Endpoints
Endpoints used by the plugin. These will be appended to the end of the base URL provided.

| Endpoint  | System | Description |
| ------------- | ------------- | ------------- |
| "games/gettags" | Tags | Letch the list of tags |
| "scpsl/report | Reporting | Send details of in-game reports made against players (Does not include cheater reports) |
| "scpsl/playerjoin" | Tracking | Sends details of a player when they join the server |
| "scpsl/playerleave" | Tracking | Sends details of a player when they leave the server |
| "scpsl/playerbanned" | Tracking | Sends details of a new player ban, and who issued it |
| "scpsl/playerkick" | Tracking | Sends details of when a player gets kicked from the server |
| "scpsl/playermute" | Tracking | Sends details of when a player gets server muted by an admin |
| "scpsl/playerunmute" | Tracking | Sends details of when a player gets server unmuted by an admin |


# Commands
| Command | Alias | Permission | Description |
| ------------- | ------------- | ------------- | ------------- |
| dynamictag | dtag / dt | NONE | Shows a player's Dynamic Tag. Can be run in the client console with `.dtag` |
| dynamictaglist | dtaglist / dtl | NONE | List all dynamic tags currently loaded on the server |
| dynamictagupdate | dtagupdate / dtu | NONE | Forcibly refetches the list of dynamic tags. This is done automatically during the `WaitingForPlayers` screen |

# Events
This plugin uses the following events:
 - PlayerPreAuthenticating
 - PlayerJoined
 - PlayerReportingPlayer
 - PlayerReportedPlayer
 - PlayerLeft
 - PlayerBanned
 - PlayerKicked
 - PlayerMuted
 - PlayerUnmuted

# Developers
Data formats can be found in the `DataClasses.cs` file. For further assistance with developing the API for this plugin, please contact us on our discord.
