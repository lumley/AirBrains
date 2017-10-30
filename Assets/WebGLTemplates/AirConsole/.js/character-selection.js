var previousButtonClassName = "previous-button";
var selectButtonClassName = "selection-button";
var nextButtonClassName = "next-button";
var characterContainerClassName = "character-container";
var characterContainerMoverClassNamePrefix = "character-container-";
// var buttonDisabled = "button-disabled";
var characterPreviews = "character-previews";
var characterPreviewMoverClassNamePrefix = "character-preview-focus-";
var characterPreviewMoverClassName = "character-preview-focus";
var characterPreviewClassName = "character-preview";
var characterPreviewClassNamePrefix = "character-preview-";
var selectButtonSelectedClassName = "selection-button--selected";
var buttonDisabledClassName = "button-disabled";
var buttonSelectedClassName = "button-selected";
var characterPreviewSelectedClassName = "character-preview-selected";
var characterPreviewUnavailableClassName = "character-preview-unavailable";
var startGameButtonClassName = "start-game-button";

var characterContainerElement = document.getElementsByClassName(characterContainerClassName)[0];
var characterPreviewsElement = document.getElementsByClassName(characterPreviews)[0];
initializeCharacters();

var startGameButtonElement = document.getElementsByClassName(startGameButtonClassName)[0];
var previousElement = document.getElementsByClassName(previousButtonClassName)[0];
var selectElement = document.getElementsByClassName(selectButtonClassName)[0];
var nextElement = document.getElementsByClassName(nextButtonClassName)[0];
var characterPreview = document.getElementsByClassName(characterPreviewClassName);

var selectText = "Select";
var selectSelectedText = "✔";

var characterCount = 10;
var currentCharacterId = 1;
var currentPlayerSelectedCharacterId = undefined;
var playerSelections = [];

setCurrentCharacterId(currentCharacterId);
previousElement.classList.add(buttonDisabled);

previousElement.addEventListener("touchstart", function(){
  onPreviousClick();
});

selectElement.addEventListener("touchstart", function(){
  onSelectClick();
});

nextElement.addEventListener("touchstart", function(){
  onNextClick();
});

startGameButtonElement.addEventListener("touchstart", function(){
  onStartGameClick();
});


function onPreviousClick() {
  if (currentCharacterId > 1) {
    setCurrentCharacterId(currentCharacterId - 1);
  }

  if (currentCharacterId === 1) {
    previousElement.classList.add(buttonDisabled);
  }

  if (currentCharacterId === characterCount - 1) {
    nextElement.classList.remove(buttonDisabled);
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
    nextElement.classList.add(buttonDisabled);
  }

  if (currentCharacterId === 2) {
    previousElement.classList.remove(buttonDisabled);
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
  var currentPlayerSelected = currentPlayerSelectedCharacterId === currentCharacterId;
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
    var characterElement = createElement("div", "character");
    characterElement.classList.add("character-" + (index + 1));

    var content = createElement("div", "character-content", characterElement);
    var name = createElement("div", "character-name", content);

    var detail = createElement("div", "character-detail", content);
    var image = createElement("div", "character-image", detail);
    var img = createElement("img", undefined, image);
    img.width = 270 / 3;
    img.height = 342 / 3;
    var information = createElement("div", "character-information", detail);

    var fromLabel = createElement("div", "character-label", information);
    fromLabel.textContent = "From:";
    var fromValue = createElement("div", "character-value", information);

    var weightLabel = createElement("div", "character-label", information);
    weightLabel.textContent = "Weight:";
    var weightValue = createElement("div", "character-value", information);

    var heightLabel = createElement("div", "character-label", information);
    heightLabel.textContent = "Height:";
    var heightValue = createElement("div", "character-value", information);

    var agenda = createElement("div", "character-agenda", content);
    var agendaLabel = createElement("span", "character-label", agenda);
    agendaLabel.textContent = "Agenda: ";
    var agendaValue = createElement("span", "character-value", agenda);

    name.textContent = character.name;
    img.src  = "images/characters_high/" + character.imageName + "_high.png";
    fromValue.textContent = character.from;
    weightValue.textContent = character.weight;
    heightValue.textContent = character.height;
    agendaValue.textContent = character.agenda;

    characterContainerElement.appendChild(characterElement);

    // Preview
    var characterPreviewElement = createElement("div", "character-preview", characterPreviewsElement);
    characterPreviewElement.classList.add("character-preview-" + (index + 1));
  });
}

function createElement(type, className, parent) {
  var element = document.createElement(type);

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

  app.sendSetAvatarIndex(characterId);
}

function onStartGameClick() {
  console.log("Player pressed Start Game button");

  app.sendStartGame();
}