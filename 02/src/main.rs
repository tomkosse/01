use std::io::BufWriter;
use ferris_says::say;
use std::io::stdout;

struct Arguments {
    extra_message: String
}

fn main() {
    let stdout = stdout();
    let message = String::from("Hello friends! My name is CrabTom!");    
    let width = message.chars().count();

    let mut writer = BufWriter::new(stdout.lock());
    say(message.as_bytes(), width, &mut writer).unwrap();

    let args = Arguments {
        extra_message: std::env::args().nth(1).expect("No extra message supplied")
    };

    say(args.extra_message.as_bytes(), args.extra_message.chars().count(), &mut writer).unwrap();
}
