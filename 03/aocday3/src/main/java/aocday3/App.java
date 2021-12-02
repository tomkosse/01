package aocday3;

import java.io.BufferedReader;
import java.io.File;
import java.io.FileReader;
import java.io.IOException;

public class App {
    public static void main(String[] args) {
        try (var fr = new FileReader(new File(args[0]))) {
            var br = new BufferedReader(fr);
            while (br.ready()) {
                System.out.println(br.readLine());
            }
        } catch (IOException e) {
            e.printStackTrace();
        }
    }
}
