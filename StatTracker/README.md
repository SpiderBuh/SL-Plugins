# Stat Tracker
Basic plugin to track player stats.
Stats are NOT stored server side, and are cleared at the end of each round. This plugin must be used with a custom API to store and handle the stat data.

# Config
| Config  | Default Value | Description |
| ------------- | ------------- | ------------- |
| api_endpoint  | "https://google.co.uk/" | Endpoint for the plugin to send the stat data to |

# Commands
This plugin has no commands

# Events
This plugin uses the following events:
 - ServerWaitingForPlayers
 - ServerRoundStarted
 - ServerRoundEnded
 - PlayerSpawned
 - PlayerJoined
 - PlayerLeft
 - PlayerHurt
 - PlayerDeath
 - PlayerEscaped
 - PlayerCuffed
 - PlayerUsedItem

# Developers
Stat data is sent as a JSON string to the endpoint provided in the config file. The layout of the JSON can be found at the bottom of the `StatTrackerPlugin.cs` file.
<br>The keys for the 4 dictionary lists are the ID for each role. So `1` is equal to `ClassD`.
<br>The default state for the `DNT` option is true. Meaning, if the plugin, for whatever reason, is unable to check if the player has DNT enabled, it will assume they have it enabled and thus will not send the data. Players with DNT data WILL still have their stats tracked, however their data will not be sent at the end of the round, and will be cleared.
