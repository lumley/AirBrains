# Air Console Player Properties

| Property | Type | Description |
| -------- | ---- | ----------- |


# Air Console Messaging
## Sent by Mobile

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

## Sent by TV

 - AvatarChosen: Screen indicates the device which is its avatar. This message is sent when the device connects or whenever it requests to change its avatar.

| Property | Type | Description |
| -------- | ---- | ----------- |
| type | string | Type of this message |
| avatarIndex | int | Index of the avatar of this player (starting from 1) |

 - CharacterSetChanged: Screen indicates to all devices (through broadcast) which characters are currently available.

| Property | Type | Description |
| -------- | ---- | ----------- |
| type | string | Type of this message |
| availableAvatarIndexes | int[] | Array of integers with all indexes that are available (starting from 1) |
| notReadyDeviceIds | int[] | Device Ids that are still not ready |

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

 - BlockRound: Round will start being displayed on the screen, no more actions are accepted. Broadcasted to all devices.

| Property | Type | Description |
| -------- | ---- | ----------- |
| type | string | Type of this message |

 - GameFinished: The game has finished, it provides the information that is relevant to the players.

| Property | Type | Description |
| -------- | ---- | ----------- |
| type | string | Type of this message |
| fundsRaised | int | Amount of funds raised |
| winnerDeviceId | int | Device Id of the winner (may be you) |
| placement | int | This device placement (0 for first) |