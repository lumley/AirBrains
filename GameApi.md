# Air Console Player Properties

| Property | Type | Description |
| -------- | ---- | ----------- |


# Air Console Messaging
## Mobile

 - SetReady: indicates that the player is ready (on set up screen or on each round). 

| Property | Type | Description |
| -------- | ---- | ----------- |
| type | string | Type of this message |
| isReady | boolean | Indicates if the player is ready or not |

 - SetAvatarIndex: chooses the avatar index for a given player, the screen will reply with the actual avatar of the player.

| Property | Type | Description |
| -------- | ---- | ----------- |
| type | string | Type of this message |
| avatarIndex | int | Index of the avatar of this player |

 - StartGame: a player requests to start the game, only succeedes when all players are actually ready.

| Property | Type | Description |
| -------- | ---- | ----------- |
| type | string | Type of this message |

 - SendChosenActions: device tells the screen what actions have been chosen.

| Property | Type | Description |
| -------- | ---- | ----------- |
| type | string | Type of this message |
| actions | string[] | "up", "down", "left", "right", "wait" |

## TV

 - AvatarChosen: Screen indicates the device which is its avatar. This message is sent when the device connects or whenever it requests to change its avatar.

| Property | Type | Description |
| -------- | ---- | ----------- |
| type | string | Type of this message |
| avatarIndex | int | Index of the avatar of this player |

 - GameStarted: Screen indicates the devices that a new game has started, indicate all the rules, etc.

| Property | Type | Description |
| -------- | ---- | ----------- |
| type | string | Type of this message |
| fundingGoal | int | Amount of funds required to trigger the round countdown |
| roundsRemaining | int | Amount of rounds that are remaining from now, just in case there is no funding goal |

 - StartRound: A new round has started, client should block any input.

| Property | Type | Description |
| -------- | ---- | ----------- |
| type | string | Type of this message |
| round | int | Round that has started (beginning with 0) |
| turnCount | int | Amount of turns on this round |
| donorCount | int | Amount of donors this player has |
| fundsRaised | int | Amount of funds already raised by this player (in Millions) |

 - GiveMeChosenActions: Screen requests the chosen actions from the device.

| Property | Type | Description |
| -------- | ---- | ----------- |
| type | string | Type of this message |

 - YourActionResolutionForTurn: Screen indicates a device what was the result of the given turns.

| Property | Type | Description |
| -------- | ---- | ----------- |
| type | string | Type of this message |
| turns | int[] | Index of the turns that have a result (starting with zero) |
| results | string[] | Results of each turn ("not started", "completed", "failed", "discarded") |

 - RoundEnded: Screen indicates the final values for the round (while displaying the result on screen).

| Property | Type | Description |
| -------- | ---- | ----------- |
| type | string | Type of this message |
| round | int | Round that has started (beginning with 0) |
| donorCount | int | Amount of donors this player has |
| fundsRaised | int | Amount of funds already raised by this player (in Millions) |

 - GameFinished: The game has finished, it provides the information that is relevant to the players.

| Property | Type | Description |
| -------- | ---- | ----------- |
| type | string | Type of this message |
| fundsRaised | int | Amount of funds raised |
| winnerDeviceId | int | Device Id of the winner (may be you) |
| placement | int | This device placement (0 for first) |