var inGameScreenClassName = "in-game-screen";
var characterSelectionScreenClassName = "character-selection-screen";
var hideScreenClassName = "hide-screen";

var inGameScreenElement = document.getElementsByClassName(inGameScreenClassName)[0];
var characterSelectionScreenElement = document.getElementsByClassName(characterSelectionScreenClassName)[0];

function displayInGameScreen() {
  characterSelectionScreenElement.classList.add(hideScreenClassName);
  inGameScreenElement.classList.remove(hideScreenClassName);
}

function displayCharacterSelectionScreen() {
  characterSelectionScreenElement.classList.remove(hideScreenClassName);
  inGameScreenElement.classList.add(hideScreenClassName);
}

displayCharacterSelectionScreen();

