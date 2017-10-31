var inGameScreenClassName = "in-game-screen";
var characterSelectionScreenClassName = "character-selection-screen";
var hideScreenClassName = "hide-screen";
var playerNameClassName = "player-name";
var backToLobbyButtonClassName = "back-to-lobby-button"
var backToLobbyScreenClassName = "back-to-lobby-screen";
var instructionsScreenClassName = "instructions-screen";

var backToLobbyElement = document.getElementsByClassName(backToLobbyScreenClassName)[0];
var backToLobbyButtonElement = document.getElementsByClassName(backToLobbyButtonClassName)[0];
var inGameScreenElement = document.getElementsByClassName(inGameScreenClassName)[0];
var characterSelectionScreenElement = document.getElementsByClassName(characterSelectionScreenClassName)[0];
var playerNameElement = document.getElementsByClassName(playerNameClassName)[0];
var instructionsScreenElement = document.getElementsByClassName(instructionsScreenClassName)[0];

function displayInGameScreen() {
  resetInGame();
  characterSelectionScreenElement.classList.add(hideScreenClassName);
  inGameScreenElement.classList.remove(hideScreenClassName);
  backToLobbyElement.classList.add(hideScreenClassName);
  instructionsScreenElement.classList.add(hideScreenClassName);
}

function displayCharacterSelectionScreen() {
  characterSelectionScreenElement.classList.remove(hideScreenClassName);
  inGameScreenElement.classList.add(hideScreenClassName);
  backToLobbyElement.classList.add(hideScreenClassName);
  instructionsScreenElement.classList.add(hideScreenClassName);
}

function displayBackToLobbyScreen() {
  characterSelectionScreenElement.classList.add(hideScreenClassName);
  inGameScreenElement.classList.add(hideScreenClassName);
  backToLobbyElement.classList.remove(hideScreenClassName);
  instructionsScreenElement.classList.add(hideScreenClassName);
}

function displayInstructionsScreen() {
  characterSelectionScreenElement.classList.add(hideScreenClassName);
  inGameScreenElement.classList.add(hideScreenClassName);
  backToLobbyElement.classList.add(hideScreenClassName);
  instructionsScreenElement.classList.remove(hideScreenClassName);
}

function displayPlayerName(playerName) {
  playerNameElement.textContent = playerName;
}

backToLobbyButtonElement.addEventListener("touchstart", function(){
  app.sendStartGame();
});

// displayInGameScreen();
// displayBackToLobbyScreen();
displayCharacterSelectionScreen();

