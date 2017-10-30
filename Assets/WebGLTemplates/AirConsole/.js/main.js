var inGameScreenClassName = "in-game-screen";
var characterSelectionScreenClassName = "character-selection-screen";
var hideScreenClassName = "hide-screen";
var playerNameClassName = "player-name";

var inGameScreenElement = document.getElementsByClassName(inGameScreenClassName)[0];
var characterSelectionScreenElement = document.getElementsByClassName(characterSelectionScreenClassName)[0];
var playerNameElement = document.getElementsByClassName(playerNameClassName)[0];

function displayInGameScreen() {
  resetInGame();
  characterSelectionScreenElement.classList.add(hideScreenClassName);
  inGameScreenElement.classList.remove(hideScreenClassName);
}

function displayCharacterSelectionScreen() {
  characterSelectionScreenElement.classList.remove(hideScreenClassName);
  inGameScreenElement.classList.add(hideScreenClassName);
}

function displayPlayerName(playerName) {
  playerNameElement.textContent = playerName;
}

// displayInGameScreen();
displayCharacterSelectionScreen();

