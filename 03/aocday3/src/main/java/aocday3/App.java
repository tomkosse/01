package aocday3;

import java.io.BufferedReader;
import java.io.File;
import java.io.FileReader;
import java.io.IOException;
import java.util.ArrayList;
import java.util.function.Function;
import java.util.stream.Collectors;

public class App {
    public static void main(String[] args) {
        var matrix = readFileIntoMatrix(args[0]);

        var rotatedMatrix = rotateMatrix(matrix);

        var gamma = determineNumberByDiscriminator(rotatedMatrix, App::getMajorityNumber);
        var epsilon = determineNumberByDiscriminator(rotatedMatrix, App::getMinorityNumber);

        System.out.println("Gamma: " + gamma + " Epsilon: " + epsilon + " Power consumption: " + gamma * epsilon);

        var o2genrating = bitsToNumber(reduceMatrixByDiscriminator(matrix, App::getMajorityNumber, 0).get(0));
        var co2scrubrating = bitsToNumber(reduceMatrixByDiscriminator(matrix, App::getMinorityNumber, 0).get(0));
        System.out.println("O2 Generator Rating: " + o2genrating + " CO2 Scrubber Rating: " + co2scrubrating
                + " Life support rating:" + o2genrating * co2scrubrating);
    }

    private static int bitsToNumber(ArrayList<Integer> bits) {
        int result = 0;
        for (var bit : bits) {
            result = result << 1;
            result += bit;
        }
        return result;
    }

    private static ArrayList<ArrayList<Integer>> reduceMatrixByDiscriminator(ArrayList<ArrayList<Integer>> matrix,
            Function<ArrayList<Integer>, Integer> discriminatorFinder, int index) {
        if (matrix.size() == 1) {
            return matrix;
        }

        var rotatedMatrix = rotateMatrix(matrix);

        var bits = rotatedMatrix.get(index);
        var discriminatorNumber = discriminatorFinder.apply(bits);

        var reducedMatrix = matrix.stream()
                .filter(line -> line.get(index) == discriminatorNumber)
                .collect(Collectors.toCollection(ArrayList::new));

        return reduceMatrixByDiscriminator(reducedMatrix, discriminatorFinder, index + 1);
    }

    private static int getMajorityNumber(ArrayList<Integer> bits) {
        long zeroes = bits.stream().filter(x -> x == 0).count();
        long ones = bits.stream().filter(x -> x == 1).count();
        return ones >= zeroes ? 1 : 0; // Ones take precedent over zeroes if it comes to a tie
    }

    private static int getMinorityNumber(ArrayList<Integer> bits) {
        long zeroes = bits.stream().filter(x -> x == 0).count();
        long ones = bits.stream().filter(x -> x == 1).count();
        return zeroes <= ones ? 0 : 1; // Zeroes take precedent over ones if it comes to a tie
    }

    private static int determineNumberByDiscriminator(ArrayList<ArrayList<Integer>> matrix,
            Function<ArrayList<Integer>, Integer> discriminatorFinder) {
        ArrayList<Integer> numbers = new ArrayList<>();
        for (int i = 0; i < matrix.size(); i++) {
            var bits = matrix.get(i);
            int majorityNumber = discriminatorFinder.apply(bits);
            numbers.add(majorityNumber);
        }
        return bitsToNumber(numbers);
    }

    private static ArrayList<ArrayList<Integer>> rotateMatrix(ArrayList<ArrayList<Integer>> lines) {
        var lineLength = lines.get(0).size(); // Assumes "square" (non-jagged) matrix

        ArrayList<ArrayList<Integer>> rotatedArray = new ArrayList<>();
        for (int i = 0; i < lineLength; i++) {
            ArrayList<Integer> bits = new ArrayList<>();
            for (ArrayList<Integer> line : lines) {
                bits.add(line.get(i));
            }

            rotatedArray.add(bits);
        }
        return rotatedArray;
    }

    private static ArrayList<ArrayList<Integer>> readFileIntoMatrix(String filepath) {
        ArrayList<ArrayList<Integer>> matrix = new ArrayList<>();
        try (var br = new BufferedReader(new FileReader(new File(filepath)))) {
            while (br.ready()) {
                ArrayList<Integer> bits = new ArrayList<>();
                for (char c : br.readLine().toCharArray()) {
                    bits.add(c == 48 ? 0 : 1); // Converts char value for 0 to int 0. Assumes that it can only be 0 or 1
                }
                matrix.add(bits);
            }
        } catch (IOException e) {
            e.printStackTrace();
        }
        return matrix;
    }
}
