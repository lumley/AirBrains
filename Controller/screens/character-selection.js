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

const previousElement = document.getElementsByClassName(previousButtonClassName)[0];
const selectElement = document.getElementsByClassName(selectButtonClassName)[0];
const nextElement = document.getElementsByClassName(nextButtonClassName)[0];
const characterContainerElement = document.getElementsByClassName(characterContainerClassName)[0];
const characterPreviewsElement = document.getElementsByClassName(characterPreviews)[0];
const characterPreview = document.getElementsByClassName(characterPreviewClassName);

const selectText = "Select";
const selectSelectedText = "✔";

const characterCount = 3;
var currentCharacterId = 0;
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

function onPreviousClick() {
  if (currentCharacterId > 0) {
    setCurrentCharacterId(currentCharacterId - 1);
  }

  if (currentCharacterId === 0) {
    previousElement.classList.add(buttonDisabled)
  }

  if (currentCharacterId === characterCount - 2) {
    nextElement.classList.remove(buttonDisabled)
  }

  console.log("onPreviousClick");
}

function onSelectClick() {
  if (playerSelections[currentCharacterId] == undefined) {
    selectCharacter();
  }
}

function onNextClick() {
  if (currentCharacterId < characterCount - 1) {
    setCurrentCharacterId(currentCharacterId + 1);
  }

  if (currentCharacterId === characterCount - 1) {
    nextElement.classList.add(buttonDisabled)
  }

  if (currentCharacterId === 1) {
    previousElement.classList.remove(buttonDisabled)
  }
  console.log("onNextClick");
}

function setCurrentCharacterId(newCharacterId) {
  characterContainerElement.classList.remove(characterContainerMoverClassNamePrefix + currentCharacterId);
  characterContainerElement.classList.add(characterContainerMoverClassNamePrefix + newCharacterId);
  characterPreviewsElement.classList.remove(characterPreviewMoverClassNamePrefix + currentCharacterId);
  characterPreviewsElement.classList.add(characterPreviewMoverClassNamePrefix + newCharacterId);

  characterPreview[currentCharacterId].classList.remove(characterPreviewMoverClassName);
  characterPreview[newCharacterId].classList.add(characterPreviewMoverClassName);

  currentCharacterId = newCharacterId;

  updateSelectButton();
}

function selectCharacter() {
  if (currentPlayerSelectedCharacterId != undefined) {
    characterPreview[currentPlayerSelectedCharacterId].classList.remove(characterPreviewSelectedClassName);
  }

  currentPlayerSelectedCharacterId = currentCharacterId;

  characterPreview[currentPlayerSelectedCharacterId].classList.add(characterPreviewSelectedClassName);

  updateSelectButton();

}

function updateSelectButton() {
  const currentPlayerSelected = currentPlayerSelectedCharacterId === currentCharacterId;
  if (currentPlayerSelected) {
    selectElement.classList.add(selectButtonSelectedClassName);
  } else {
    selectElement.classList.remove(selectButtonSelectedClassName);
  }

  console.log(playerSelections);

  if (playerSelections[currentCharacterId] == undefined || playerSelections[currentCharacterId] === false) {
    selectElement.classList.remove(buttonDisabledClassName);
  } else {
    selectElement.classList.add(buttonDisabledClassName);
  }

  selectElement.textContent = currentPlayerSelected ? selectSelectedText : selectText;
}

setPlayerSelection(0, true);

function setPlayerSelection(id, selected) {
  playerSelections[id] = selected;

  if (selected) {
    characterPreview[id].classList.add(characterPreviewUnavailableClassName);
  } else {
    characterPreview[id].classList.remove(characterPreviewUnavailableClassName);
  }


  updateSelectButton();
}