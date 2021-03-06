var slotClassPrefix = "slot-";
var slotClassName = "slot";
var slotButtonClassName = "slot-button";
var activeSlotClassName = "slot-active";
var optionClassName = "option";
var optionsClassName = "options";
var activeOptionClassName = "option-active";
var readyClassName = "ready";
var readyButtonClassName = "ready-button";
var buttonDisabled = "button-disabled";
var readyButtonActiveClassName = "ready-button--active";
var inGameAvatarClassName = "in-game-avatar";
var lookAtScreenTextClassName = "look-at-screen-text";
var hiddenClass = "hidden";


var optionTexts = [
    "<img src=\"images/icon_up.png\" width=\"28\" height=\"28\"/>",
    "<img src=\"images/icon_left.png\" width=\"28\" height=\"28\"/>",
    "<img src=\"images/icon_stop.png\" width=\"28\" height=\"28\"/>",
    "<img src=\"images/icon_right.png\" width=\"28\" height=\"28\"/>",
    "<img src=\"images/icon_down.png\" width=\"28\" height=\"28\"/>",
    "<img src=\"images/icon_random_lightblue.png\" width=\"28\" height=\"28\"/>"
];

var optionIds = ["up", "left", "wait", "right", "down"];

var optionUp = 0;
var optionLeft = 3;
var optionWait = 4;
var optionRight = 1;
var optionDown = 2;
var optionUndefined = 5;

var selectedSlotId = undefined;
var selectedOptions = [optionUndefined, optionUndefined, optionUndefined, optionUndefined];
var ready = false;
var allSlotSelected = false;

var slotElements = document.getElementsByClassName(slotClassName);
var optionElements = document.getElementsByClassName(optionClassName);
var readyElement = document.getElementsByClassName(readyClassName)[0];
var readyButtonElement = readyElement.getElementsByClassName(readyButtonClassName)[0];
var inGameAvatarElement = document.getElementsByClassName(inGameAvatarClassName)[0];
var optionsElement = document.getElementsByClassName(optionsClassName)[0];
var lookAtScreenTextElement = document.getElementsByClassName(lookAtScreenTextClassName)[0];

[].slice.call(slotElements).forEach(function (slot, index) {
    slot.addEventListener("touchstart", function () {
        selectSlot(index);
    });
});

[].slice.call(optionElements).forEach(function (option, index) {
    option.addEventListener("touchstart", function () {
        selectOption(index, true);
    });
});

readyElement.addEventListener("touchstart", function () {
    activateReady();
});

selectSlot(0);

function selectSlot(slotId) {
    if (selectedSlotId !== undefined) {
        unselectSlot();
    }

    slotElements[slotId].classList.add(activeSlotClassName);

    selectedSlotId = slotId;

    selectOption(selectedOptions[slotId]);
}

function unselectSlot() {
    unselectOption(selectedOptions[selectedSlotId]);
    var slot = document.getElementsByClassName(activeSlotClassName)[0];
    slot.classList.remove(activeSlotClassName);
    selectedSlotId = undefined;
}

function selectOption(optionId, autoSelectNextSlot) {
    if (selectedSlotId != undefined) {
        unselectOption(selectedOptions[selectedSlotId]);
    }

    if (optionId == optionUndefined) {
        return;
    }

    optionElements[optionId].classList.add(activeOptionClassName);

    slotElements[selectedSlotId].getElementsByClassName(slotButtonClassName)[0].innerHTML = optionTexts[optionId];

    selectedOptions[selectedSlotId] = optionId;

    onActionSelected(optionId, selectedSlotId);

    var newAllSlotSelected = (selectedOptions.filter(function (option) {
        return option === optionUndefined;
    }).length === 0);

    if (autoSelectNextSlot && !allSlotSelected) {
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
    // ready = !ready;
    //
    // if (ready) {
    //   readyButtonElement.classList.add(readyButtonActiveClassName);
    //   onPlayerReady();
    // } else {
    //   readyButtonElement.classList.remove(readyButtonActiveClassName);
    //   onPlayerNotReady();
    // }
}

function enableReady() {
    ready = true;
    onPlayerReady();
    readyButtonElement.disabled = false;

    readyButtonElement.classList.remove(buttonDisabled);
}

function onPlayerNotReady() {
    console.log("Player is not ready");
}

// ****************************************************************
// These are relevant functions for communicating with the game :

function onActionSelected(optionId, slotId) {
    // optionId :
    // 0 = up;
    // 1 = right;
    // 2 = down;
    // 3 = left;
    // 4 = stop;
    // slotId :
    // from 0 to 3
    app.setDefinedAction(slotId + 1, optionIds[optionId]);
    console.log("Action " + optionIds[optionId] + " selected for action slot " + slotId);
}

function onPlayerReady() {
    console.log("Player is ready");
    app.sendDeviceIsReady(true);
}

function resetInGame() {
    lookAtScreenTextElement.classList.add(hiddenClass);
    optionsElement.classList.remove(hiddenClass);
    selectSlot(0);
    selectedOptions = [optionUndefined, optionUndefined, optionUndefined, optionUndefined];
    ready = false;
    allSlotSelected = false;
    slotElements[0].getElementsByClassName(slotButtonClassName)[0].innerHTML = optionTexts[5];
    slotElements[1].getElementsByClassName(slotButtonClassName)[0].innerHTML = optionTexts[5];
    slotElements[2].getElementsByClassName(slotButtonClassName)[0].innerHTML = optionTexts[5];
    slotElements[3].getElementsByClassName(slotButtonClassName)[0].innerHTML = optionTexts[5];

    inGameAvatarElement.classList.add("character-preview-" + currentPlayerSelectedCharacterId);
}

function blockActionSelection() {
    optionsElement.classList.add(hiddenClass);
    lookAtScreenTextElement.classList.remove(hiddenClass);
}

function test() {
    window.location.href = "";
}