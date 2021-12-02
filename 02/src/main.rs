use std::io::BufWriter;
use ferris_says::say;
use std::io::stdout;
use std::fs;
use std::env;

fn main() {
    let mut horpos = 0;
    let mut depth  = 0;
    let mut aim = 0;

    let args: Vec<String> = env::args().collect();

    let input = fs::read_to_string(&args[1])
        .expect("Couldn't read file");


    for line in input.lines() {
        let terms = line.split_whitespace().collect::<Vec<&str>>();
        let direction = terms[0];
        let delta = terms[1].parse::<i32>().unwrap();

        if direction == "up" {
            aim -= delta;
        }
        if direction == "down" {
            aim += delta;
        }
        if direction == "forward" {
            horpos += delta;
            depth += aim * delta;
        }

        let mut output = "".to_owned();
        output.push_str(&horpos.to_string());
        output.push_str(" ");
        output.push_str(&depth.to_string());
        output.push_str(" ");
        output.push_str(&aim.to_string());
        output.push_str("=");
        output.push_str(&(&horpos * &depth).to_string());

        let stdout = stdout();
        let mut writer = BufWriter::new(stdout.lock());
        let outputwidth = output.chars().count();
        say(output.as_bytes(), outputwidth, &mut writer).unwrap();
    }

}
