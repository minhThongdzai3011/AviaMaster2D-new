const uiStyles = `
canvas {
	pointer-events: none;
	border: #ffcc29 3px solid;
	box-sizing: border-box;
	border-top-width: 45px;
}

#replay-panel {
	height: 45px;
    z-index: 100;
    position: absolute;
    top: 0;
    font-family: 'Arial';
    color: #000000;
    display: flex;
	width: 100%;
    align-items: baseline;
    justify-content: center;
	font-size: 20px;
	font-weight: 700;
}

#replay-panel button {
	height: 35px;
    line-height: 1;
	cursor: pointer;
    padding: 6px 23px;
    background: #ff8f00;
    margin: 5px;
    border-radius: 18px;
    font-size: 17px;
	font-weight: 700;
}

#replay-panel button:hover {
	outline: 1px solid #774400;
	outline-offset: 2px;
}

#replay-panel button:disabled {
	pointer-events: none;
	cursor: unset;
	opacity: 0.5;
}

.highlight {
	animation: highlight 1s ease 0s infinite normal forwards;
	outline-offset: 2px;
	outline: 3px solid #ff0000;
}

#pause-message {
	animation: flash 1s ease 0s infinite normal forwards;
    position: absolute;
	font-family: 'Arial';
    color: #ffffff;
	top: 43vh;
    font-size: 400%;
    text-shadow: -1px -1px 0 #000, 1px -1px 0 #000, -1px 1px 0 #000, 1px 1px 0 #000;
}

@keyframes highlight {
	0%,
	100% {
		outline-color: #ff0000;
	}
	50% {
		outline-color: #ffcc29;
	}
}

@keyframes flash {
	0%,
	100% {
		opacity: 1;
	}
	50% {
		opacity: 0;
	}
}
`;

export default uiStyles;