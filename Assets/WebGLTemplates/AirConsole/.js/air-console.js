
var turn1Action = "wait";
var turn2Action = "wait";
var turn3Action = "wait";
var turn4Action = "wait";
var airconsole;

/**
 * Sets up the communication to the screen.
 */
function App() {
  var me = this;
  me.airconsole = new AirConsole({"orientation": "portrait", "synchronize_time": "true"});


  me.airconsole.onMessage = function (from, data) {
    console.log("onMessage", from, data);
    // document.getElementById("content").innerHTML = "device " + from + " says: " + data;

    var parsedMessage = JSON.parse(data);

    var messageType = parsedMessage["type"].toLowerCase();
    if (messageType === "avatarchosen") {

      app.sendDeviceIsReady(true);
      // document.getElementById("avatar").innerHTML = "Chosen avatar: " + parsedMessage["avatarIndex"];
    } else if (messageType === "startround") {
        displayInGameScreen();
        resetInGame();
      // document.getElementById("turn1").innerHTML = "Turn 1: wait";
      // document.getElementById("turn2").innerHTML = "Turn 2: wait";
      // document.getElementById("turn3").innerHTML = "Turn 3: wait";
      // document.getElementById("turn4").innerHTML = "Turn 4: wait";
      // turn1Action = "wait";
      // turn2Action = "wait";
      // turn3Action = "wait";
      // turn4Action = "wait";
      // document.getElementById("current-game").innerHTML = "Round: " + parsedMessage["round"] + " Donors: " + parsedMessage["donorCount"] + " Funds Raised: " + parsedMessage["fundsRaised"] + "M";
    } else if (messageType === "backtolobby") {
      displayCharacterSelectionScreen();
    } else if (messageType === "gamefinished") {
      displayBackToLobbyScreen();
    }
  };

  me.airconsole.onReady = function (code) {
    console.log("onReady", code);
    app.displayNickname();
  };

  me.airconsole.onDeviceStateChange = function (device_id, device_data) {
    console.log("onDeviceStateChange", device_id, device_data);
  };

  /**
   * Here we are adding support for mouse events manually.
   * WE STRONGLY ENCOURAGE YOU TO USE THE AIRCONSOLE CONTROLS LIBRARY
   * WHICH IS EVEN BETTER (BUT WE DONT WANT TO BLOAT THE CODE HERE).
   * https://github.com/AirConsole/airconsole-controls/
   *
   * NO MATTER WHAT YOU DECIDE, DO NOT USE ONCLICK HANDLERS.
   * THEY HAVE A 200MS DELAY!
   */
  if (!("ontouchstart" in document.createElement("div"))) {
    var elements = document.getElementsByTagName("*");
    for (var i = 0; i < elements.length; ++i) {
      var element = elements[i];
      var ontouchstart = element.getAttribute("ontouchstart");
      if (ontouchstart) {
        element.setAttribute("onmousedown", ontouchstart);
      }
      var ontouchend = element.getAttribute("ontouchend");
      if (ontouchend) {
        element.setAttribute("onmouseup", ontouchend);
      }
    }
  }
}

// - - - - - - GAME API SECTION - - - - - -
App.prototype.sendDeviceIsReady = function (isReady) {
  var jsonMessage = {
    'type': 'SetReady',
    'isReady': isReady
  };

  this.airconsole.message(AirConsole.SCREEN, jsonMessage);
};

App.prototype.sendStartGame = function () {
  var jsonMessage = {
    'type': 'StartGame'
  };

  this.airconsole.message(AirConsole.SCREEN, jsonMessage);
};

App.prototype.sendSetAvatarIndex = function (avatarIndex) {
  var jsonMessage = {
    'type': 'SetAvatarIndex',
    'avatarIndex': avatarIndex
  };

  this.airconsole.message(AirConsole.SCREEN, jsonMessage);
};

App.prototype.sendSendChosenActions = function (chosenActions) {
  var jsonMessage = {
    'type': 'SendChosenActions',
    'actions': chosenActions // Array of strings with  "up", "down", "left", "right" or "wait" (all actions for one round)
  };

  this.airconsole.message(AirConsole.SCREEN, jsonMessage);
};


// - - - - - - END GAME API SECTION - - - - - -

App.prototype.displayDeviceId = function () {
  var id = this.airconsole.getDeviceId();
  // document.getElementById("content").innerHTML = "My ID is: " + id;
};

App.prototype.displayNickname = function () {
  var name = this.airconsole.getNickname();
  // document.getElementById("content").innerHTML = "My name is: " + name;
  displayPlayerName(name);
};

App.prototype.setDefinedAction = function (turn, action) {
  switch (turn) {
    case 1:
      turn1Action = action;
      // document.getElementById("turn1").innerHTML = "Turn 1: " + action;
      break;
    case 2:
      turn2Action = action;
      // document.getElementById("turn2").innerHTML = "Turn 2: " + action;
      break;
    case 3:
      turn3Action = action;
      // document.getElementById("turn3").innerHTML = "Turn 3: " + action;
      break;
    default:
      turn4Action = action;
      // document.getElementById("turn4").innerHTML = "Turn 4: " + action;
      break;
  }
  this.sendSendChosenActions([turn1Action, turn2Action, turn3Action, turn4Action]);
};