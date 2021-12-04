const fs = require('fs');
const { cwd } = require('process');

console.log("Hello world");

fs.readFile('input.txt', 'utf-8', (err, data) => {
  if (err) {
    console.log(err);
  }

  const lines = data.split("\n");
  let numbers = lines[0].split(',')
  const boardStates = [];
  let currentBoard = null;
  lines.slice(1).forEach((line, idx) => {
    if (line === "") {
      if (currentBoard !== null) {
        boardStates.push({ board: currentBoard, hasWon: false, winOrder: null });
      }
      currentBoard = [];
    }
    else {
      var numbers = line.split(" ").filter(el => el.length > 0)
      currentBoard.push(numbers);
    }
  });
  boardStates.push({ board: currentBoard, hasWon: false, winOrder: null });

  let numbersDrawn = 0;

  while (numbers.length > 0) {
    const drawnNumber = numbers[0];
    numbers = numbers.slice(1);
    boardStates.forEach((board, bidx) =>
      board.board.forEach((line, lidx) => {
        line.forEach((square, sidx) => {
          if (square == drawnNumber) {
            boardStates[bidx].board[lidx][sidx] = square + "x";
          }
        })
      })
    );
    processWinstates(boardStates, numbersDrawn, drawnNumber);
    numbersDrawn++;
  }

  console.log(boardStates.sort((a,b) => a.winOrder - b.winOrder));
});

function processWinstates(boards, numbersDrawn, nextNumber) {
  boards.filter(b => !b.hasWon).forEach(board => {
    let winnerFound = processBoard(board.board);
    if (!winnerFound) {
      winnerFound = processBoard(rotate(board.board));
    }
    if (winnerFound) {
      board.hasWon = true;
      board.winOrder = numbersDrawn;
      board.score = determineScore(board.board, nextNumber);
    }
  });
}

function rotate(board) {
  return board[0].map((el, idx) => board.map(row => row[idx]));
}

function processBoard(board) {
  let winnerFound = false;
  board.forEach(line => {
    let amountCrossedOff = 0;
    line.forEach(square => {
      if (square.includes("x")) {
        amountCrossedOff++;
      }
    });
    if (amountCrossedOff == line.length) {
      winnerFound = true;
    }
  });
  return winnerFound;
}

function determineScore(board, nextNumber) {
  let unmarkedSum = 0;
  board.forEach(line => {
    line.forEach(square => {
      if (!square.includes("x")) {
        unmarkedSum += Number.parseInt(square);
      }
    });
  });
  return unmarkedSum * nextNumber;
}