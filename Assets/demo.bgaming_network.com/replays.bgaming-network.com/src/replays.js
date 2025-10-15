import uiStyles from "./replays-ui-styles.js";
import uiHtml from "./replays-ui-html.js";
import GameEvent from "./game-events";

class Replays {
  constructor() {
    if (!this.isReplayMode()) {
      return;
    }

    this.gameEventHandler = this.gameEventHandler.bind(this);

    if (!window.trackGameEventListeners) {
      window.trackGameEventListeners = [];
    }

    window.trackGameEventListeners.push(this.gameEventHandler);
  }

  gameEventHandler(eventName, targetName, context) {
    switch (eventName) {
      case GameEvent.START_LOADING:
        this.initStyles();
        break;

      case GameEvent.GAME_INITIALIZED:
        this.initGame();
        break;

      default:
    }
  }

  sendEvent(eventName, targetName, context) {
    window.dispatchEvent(
      new CustomEvent(eventName, {
        detail: {
          targetName,
          context,
        },
      })
    );
  }

  initGame() {
    this.speed = 1;
    this.started = false;

    this.initView();
  }

  isReplayMode() {
    return Boolean(window.__OPTIONS__?.replay);
  }

  initStyles() {
    if (this.stylesInitialized) {
      return;
    }

    this.applyCSS(uiStyles);
    this.stylesInitialized = true;
  }

  initView() {
    if (this.viewInitialized) {
      return;
    }

    const replayPanel = document.createElement("div");
    replayPanel.id = "replay-panel";
    replayPanel.innerHTML = uiHtml;

    document.body.appendChild(replayPanel);

    this.x1btn = document.getElementById("x1btn");
    this.x2btn = document.getElementById("x2btn");
    this.x4btn = document.getElementById("x4btn");
    this.restartButton = document.getElementById("restartButton");
    this.playBtn = document.getElementById("playBtn");
    this.pauseMessage = document.getElementById("pause-message");

    this.addButtonEventListeners();
    this.refreshView();

    this.viewInitialized = true;
  }

  addButtonEventListeners() {
    this.x1btn.addEventListener("pointerdown", () => {
      this.setSpeed(1);
    });

    this.x2btn.addEventListener("pointerdown", () => {
      this.setSpeed(2);
    });

    this.x4btn.addEventListener("pointerdown", () => {
      this.setSpeed(4);
    });

    this.restartButton.addEventListener("pointerdown", () => {
      this.paused = false;
      this.started = false;
      this.sendEvent(GameEvent.RESTART_GAME, "restartButton", {});
    });

    this.playBtn.addEventListener("pointerdown", () => {
      if (!this.started) {
        this.startReplay();
      } else {
        this.paused = !this.paused;
        this.sendEvent(GameEvent.SET_SPEED, "", {
          speed: this.paused ? 0 : this.speed,
        });
        this.refreshView();
      }
    });
  }

  startReplay() {
    this.started = true;
    this.sendEvent(GameEvent.PLAY_REPLAY, "", {});
    this.refreshView();
  }

  setSpeed(val) {
    this.speed = val;
    this.sendEvent(GameEvent.SET_SPEED, "", {
      speed: val,
    });
    this.refreshView();
  }

  refreshView() {
    this.playBtn.innerText = !this.started || this.paused ? "⏵" : "⏸";
    this.pauseMessage.style.display =
      !this.started || this.paused ? "block" : "none";

    if (!this.started) {
      this.playBtn.classList.add("highlight");
    } else {
      this.playBtn.classList.remove("highlight");
    }
    this.restartButton.disabled = !this.started;
    this.x1btn.disabled = this.speed === 1;
    this.x2btn.disabled = this.speed === 2;
    this.x4btn.disabled = this.speed === 4;
  }

  applyCSS(css) {
    let head = document.head || document.getElementsByTagName("head")[0];
    let style = document.createElement("style");

    style.type = "text/css";
    if (style.styleSheet) {
      style.styleSheet.cssText = css;
    } else {
      style.appendChild(document.createTextNode(css));
    }
    head.appendChild(style);
  }
}

const replays = new Replays();

export default replays;
