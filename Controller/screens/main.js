const inGameScreenClassName = "in-game-screen";
const characterSelectionScreenClassName = "character-selection-screen";
const hideScreenClassName = "hide-screen";

const inGameScreenElement = document.getElementsByClassName(inGameScreenClassName)[0];
const characterSelectionScreenElement = document.getElementsByClassName(characterSelectionScreenClassName)[0];

function displayInGameScreen() {
  characterSelectionScreenElement.classList.add(hideScreenClassName);
  inGameScreenElement.classList.remove(hideScreenClassName);
}

function displayCharacterSelectionScreen() {
  characterSelectionScreenElement.classList.remove(hideScreenClassName);
  inGameScreenElement.classList.add(hideScreenClassName);
}

displayCharacterSelectionScreen();