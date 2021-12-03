const fs = require('fs');

console.log("Hello world");

fs.readFile('input.txt', 'utf-8', (err, data) =>
{
  if(err) {
    console.log(err);
  }
  console.log(data);
  const lines = data.split("\n");
  lines.forEach((line, idx) => console.log('Regel ' + idx + ': ' + line));
});