const slotClassPrefix = "slot-";
const slotClassName = "slot";
const slotButtonClassName = "slot-button";
const activeSlotClassName = "slot-active";
const optionClassName = "option";
const activeOptionClassName = "option-active";
const readyClassName = "ready";
const readyButtonClassName = "ready-button";
const buttonDisabled = "button-disabled";
const readyButtonActiveClassName = "ready-button--active";

const optionTexts = [
  "<img src=\"images/icon_up.png\" />",
  "<img src=\"images/icon_left.png\" />",
  "<img src=\"images/icon_stop.png\" />",
  "<img src=\"images/icon_right.png\" />",
  "<img src=\"images/icon_down.png\" />",
  "<img src=\"images/icon_random_lightblue.png\" />"
];

const optionUp = 0;
const optionLeft = 3;
const optionWait = 4;
const optionRight = 1;
const optionDown = 2;
const optionUndefined = 5;

var selectedSlotId = undefined;
var selectedOptions = [optionUndefined, optionUndefined, optionUndefined, optionUndefined];
var ready = false;
var allSlotSelected = false;

const slotElements = document.getElementsByClassName(slotClassName);
const optionElements = document.getElementsByClassName(optionClassName);
const readyElement = document.getElementsByClassName(readyClassName)[0];
const readyButtonElement = readyElement.getElementsByClassName(readyButtonClassName)[0];

[].slice.call(slotElements).forEach(function(slot, index) {
  slot.addEventListener("click", function(){
    selectSlot(index);
  });
});

[].slice.call(optionElements).forEach(function(option, index) {
  option.addEventListener("click", function(){
      selectOption(index, true);
  });
});

readyElement.addEventListener("click", function(){
  activateReady();
});

function selectSlot(slotId) {
  if (selectedSlotId != undefined) {
    unselectSlot();
  }

  slotElements[slotId].classList.add(activeSlotClassName);

  selectedSlotId = slotId;

  selectOption(selectedOptions[slotId]);
}

function unselectSlot() {
  unselectOption(selectedOptions[selectedSlotId]);
  const slot = document.getElementsByClassName(activeSlotClassName)[0];
  slot.classList.remove(activeSlotClassName);
  selectedSlotId = undefined;
}

function selectOption(optionId, autoSelectNextSlot) {
  if (selectedSlotId != undefined) {
    unselectOption(selectedOptions[selectedSlotId])
  }

  if (optionId == optionUndefined) {
    return;
  }

  optionElements[optionId].classList.add(activeOptionClassName);

  slotElements[selectedSlotId].getElementsByClassName(slotButtonClassName)[0].innerHTML = optionTexts[optionId];

  selectedOptions[selectedSlotId] = optionId;

  onActionSelected(optionId);

  const newAllSlotSelected = (selectedOptions.filter(function(option) {return option === optionUndefined}).length === 0);

  if (autoSelectNextSlot && !allSlotSelected)
  {
    if (selectedSlotId < slotElements.length - 1) {
      selectSlot(selectedSlotId + 1);
    }
  }

  if (allSlotSelected === false && newAllSlotSelected) {
    enableReady();
  }

  allSlotSelected = newAllSlotSelected;
}

function unselectOption(optionId) {
  if (optionId != optionUndefined) {
    optionElements[optionId].classList.remove(activeOptionClassName);
  }
}

function activateReady() {
  ready = !ready;

  if (ready) {
    readyButtonElement.classList.add(readyButtonActiveClassName);
    onPlayerReady();
  } else {
    readyButtonElement.classList.remove(readyButtonActiveClassName);
    onPlayerNotReady();
  }
}

function enableReady() {
  readyButtonElement.disabled = false;

  readyButtonElement.classList.remove(buttonDisabled);
}

selectSlot(0);

function onActionSelected(optionId) {
  console.log("Action " + optionTexts[optionId] + " selected for action slot " + selectedSlotId);
}

function onPlayerReady() {
  console.log("Player is ready");
}

function onPlayerNotReady() {
  console.log("Player is not ready");
}