const fs = require('fs');

fs.readFile('input.txt', 'utf-8', (err, data) => {
  if (err) {
    console.log(err);
  }

  const lines = data.split("\n");
  let numbers = lines[0].split(',')
  const boardStates = [];
  let currentBoard = null;
  lines.slice(1).forEach((line) => {
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

  for(let numbersDrawn = 0; numbersDrawn < numbers.length; numbersDrawn++)
  {
    const drawnNumber = numbers[numbersDrawn];
    
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
  }

  console.log(boardStates.sort((a,b) => a.winOrder - b.winOrder));
});

function processWinstates(boards, numbersDrawn, nextNumber) {
  boards.filter(b => !b.hasWon).forEach(board => {
    let winnerFound = processBoard(board.board) || processBoard(rotate(board.board));
    
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
    if (line.every(s => s.includes("x"))) {
      winnerFound = true;
    }
  });
  return winnerFound;
}

function determineScore(board, nextNumber) {
  const unmarkedSum = board
              .flatMap(line => line)
              .filter(s => !s.includes("x"))
              .reduce((acc, x) => acc + Number.parseInt(x), 0);
  return unmarkedSum * nextNumber;
}