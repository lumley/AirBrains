const previousButtonClassName = "previous-button";
const selectButtonClassName = "selection-button";
const nextButtonClassName = "next-button";
const characterContainerClassName = "character-container";
const characterContainerMoverClassNamePrefix = "character-container-";
const buttonDisabled = "button-disabled";
const characterPreviews = "character-previews";
const characterPreviewMoverClassNamePrefix = "character-preview-focus-";
const characterPreviewMoverClassName = "character-preview-focus";
const characterPreviewClassName = "character-preview";
const characterPreviewClassNamePrefix = "character-preview-";
const selectButtonSelectedClassName = "selection-button--selected";
const buttonDisabledClassName = "button-disabled";
const buttonSelectedClassName = "button-selected";
const characterPreviewSelectedClassName = "character-preview-selected";
const characterPreviewUnavailableClassName = "character-preview-unavailable";
const startGameButtonClassName = "start-game-button";

const characterContainerElement = document.getElementsByClassName(characterContainerClassName)[0];
const characterPreviewsElement = document.getElementsByClassName(characterPreviews)[0];
initializeCharacters();

const startGameButtonElement = document.getElementsByClassName(startGameButtonClassName)[0];
const previousElement = document.getElementsByClassName(previousButtonClassName)[0];
const selectElement = document.getElementsByClassName(selectButtonClassName)[0];
const nextElement = document.getElementsByClassName(nextButtonClassName)[0];
const characterPreview = document.getElementsByClassName(characterPreviewClassName);

const selectText = "Select";
const selectSelectedText = "✔";

const characterCount = 10;
var currentCharacterId = 1;
var currentPlayerSelectedCharacterId = undefined;
var playerSelections = [];

setCurrentCharacterId(currentCharacterId);
previousElement.classList.add(buttonDisabled);

previousElement.addEventListener("click", function(){
  onPreviousClick();
});

selectElement.addEventListener("click", function(){
  onSelectClick();
});

nextElement.addEventListener("click", function(){
  onNextClick();
});

startGameButtonElement.addEventListener("click", function(){
  onStartGameClick();
});


function onPreviousClick() {
  if (currentCharacterId > 1) {
    setCurrentCharacterId(currentCharacterId - 1);
  }

  if (currentCharacterId === 1) {
    previousElement.classList.add(buttonDisabled)
  }

  if (currentCharacterId === characterCount - 1) {
    nextElement.classList.remove(buttonDisabled)
  }
}

function onSelectClick() {
  if (playerSelections[currentCharacterId] == undefined) {
    selectCharacter();
  }
}

function onNextClick() {
  if (currentCharacterId < characterCount) {
    setCurrentCharacterId(currentCharacterId + 1);
  }

  if (currentCharacterId === characterCount) {
    nextElement.classList.add(buttonDisabled)
  }

  if (currentCharacterId === 2) {
    previousElement.classList.remove(buttonDisabled)
  }
}

function setCurrentCharacterId(newCharacterId) {
  characterContainerElement.classList.remove(characterContainerMoverClassNamePrefix + currentCharacterId);
  characterContainerElement.classList.add(characterContainerMoverClassNamePrefix + newCharacterId);
  characterPreviewsElement.classList.remove(characterPreviewMoverClassNamePrefix + currentCharacterId);
  characterPreviewsElement.classList.add(characterPreviewMoverClassNamePrefix + newCharacterId);

  characterPreview[currentCharacterId - 1].classList.remove(characterPreviewMoverClassName);
  characterPreview[newCharacterId - 1].classList.add(characterPreviewMoverClassName);

  currentCharacterId = newCharacterId;

  updateSelectButton();
}

function selectCharacter() {
  if (currentPlayerSelectedCharacterId != undefined) {
    characterPreview[currentPlayerSelectedCharacterId - 1].classList.remove(characterPreviewSelectedClassName);
  }

  currentPlayerSelectedCharacterId = currentCharacterId;

  characterPreview[currentPlayerSelectedCharacterId - 1].classList.add(characterPreviewSelectedClassName);

  updateSelectButton();

  onPlayerSelected(currentCharacterId);
}

function updateSelectButton() {
  const currentPlayerSelected = currentPlayerSelectedCharacterId === currentCharacterId;
  if (currentPlayerSelected) {
    selectElement.classList.add(selectButtonSelectedClassName);
  } else {
    selectElement.classList.remove(selectButtonSelectedClassName);
  }

  if (playerSelections[currentCharacterId] == undefined || playerSelections[currentCharacterId] === false) {
    selectElement.classList.remove(buttonDisabledClassName);
  } else {
    selectElement.classList.add(buttonDisabledClassName);
  }

  selectElement.textContent = currentPlayerSelected ? selectSelectedText : selectText;
}

// setPlayerSelection(0, true);

function setPlayerSelection(playerId, selected) {
  playerSelections[playerId] = selected;

  if (selected) {
    characterPreview[playerId - 1].classList.add(characterPreviewUnavailableClassName);
  } else {
    characterPreview[playerId - 1].classList.remove(characterPreviewUnavailableClassName);
  }


  updateSelectButton();
}

function initializeCharacters() {
  characters.forEach(function(character, index) {
    const characterElement = createElement("div", "character");
    characterElement.classList.add("character-" + (index + 1));

    const content = createElement("div", "character-content", characterElement);
    const name = createElement("div", "character-name", content);

    const detail = createElement("div", "character-detail", content);
    const image = createElement("div", "character-image", detail);
    const img = createElement("img", undefined, image);
    img.width = 270;
    img.height = 342;
    const information = createElement("div", "character-information", detail);

    const fromLabel = createElement("div", "character-label", information);
    fromLabel.textContent = "From:";
    const fromValue = createElement("div", "character-value", information);

    const weightLabel = createElement("div", "character-label", information);
    weightLabel.textContent = "Weight:";
    const weightValue = createElement("div", "character-value", information);

    const heightLabel = createElement("div", "character-label", information);
    heightLabel.textContent = "Height:";
    const heightValue = createElement("div", "character-value", information);

    const agenda = createElement("div", "character-agenda", content);
    const agendaLabel = createElement("span", "character-label", agenda);
    agendaLabel.textContent = "Agenda: ";
    const agendaValue = createElement("span", "character-value", agenda);

    name.textContent = character.name;
    img.src  = "images/characters_high/" + character.imageName + "_high.png";
    fromValue.textContent = character.from;
    weightValue.textContent = character.weight;
    heightValue.textContent = character.height;
    agendaValue.textContent = character.agenda;

    characterContainerElement.appendChild(characterElement);

    // Preview
    const characterPreviewElement = createElement("div", "character-preview", characterPreviewsElement);
    characterPreviewElement.classList.add("character-preview-" + (index + 1));
  });
}

function createElement(type, className, parent) {
  const element = document.createElement(type);

  if (className != undefined) {
    element.classList.add(className);
  }

  if (parent != undefined) {
    parent.appendChild(element);
  }

  return element;
}

// ****************************************************************
// These are relevant functions for communicating with the game :

function onPlayerSelected(characterId) {
  console.log("Player selected character " + characterId);
}

function onStartGameClick() {
  console.log("Player pressed Start Game button");
}