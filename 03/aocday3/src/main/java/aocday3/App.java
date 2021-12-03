package aocday3;

import java.io.BufferedReader;
import java.io.File;
import java.io.FileReader;
import java.io.IOException;
import java.util.ArrayList;
import java.util.stream.Collectors;

public class App {
    public static void main(String[] args) {
        
        var matrix = readFile(args[0]);
        
        var rotatedMatrix = rotateMatrix(matrix);

        var gamma = determineGamma(rotatedMatrix);
        var epsilon = determineEpsilon(rotatedMatrix);

        System.out.println("Gamma: " + gamma + " Epsilon: " + epsilon + " Power consumption: " + gamma * epsilon);
        
        var o2genrating = determineO2GeneratorRating(matrix);
        var co2scrubrating = determineCO2ScrubberRating(matrix);
        System.out.println("O2 Generator Rating: " + o2genrating + " CO2 Scrubber Rating: " + co2scrubrating + " Life support rating:" + o2genrating * co2scrubrating);
    }

    private static int determineO2GeneratorRating(ArrayList<ArrayList<Integer>> matrix)
    {
        var reducedMatrix = reduceMatrixByMajorities(matrix, 0).get(0);
        return bitsToNumber(reducedMatrix);
    }

    private static int determineCO2ScrubberRating(ArrayList<ArrayList<Integer>> matrix)
    {
        var reducedMatrix = reduceMatrixByMinorities(matrix, 0).get(0);
        return bitsToNumber(reducedMatrix);
    }

    private static int bitsToNumber(ArrayList<Integer> bits)
    {
        int result = 0;
        for(int i=0; i < bits.size(); i++)
        {
            result = result << 1;
            result += bits.get(i);
        }
        return result;
    }

    private static ArrayList<ArrayList<Integer>> reduceMatrixByMajorities(ArrayList<ArrayList<Integer>> matrix, int index)
    {
        var rotatedMatrix = rotateMatrix(matrix);

        if(index == rotatedMatrix.size())
        {
            return matrix;
        }

        var bits = rotatedMatrix.get(index);
        
        var majorityNumber = getMajorityNumber(bits);

        var reducedMatrix = matrix.stream().filter(line -> line.get(index) == majorityNumber).collect(Collectors.toCollection(ArrayList::new));    
        return reduceMatrixByMajorities(reducedMatrix, index + 1);
    }

    private static ArrayList<ArrayList<Integer>> reduceMatrixByMinorities(ArrayList<ArrayList<Integer>> matrix, int index)
    {
        if(matrix.size() == 1)
        {
            return matrix;
        }

        var rotatedMatrix = rotateMatrix(matrix);
        var bits = rotatedMatrix.get(index);        
        
        var minorityNumber = getMinorityNumber(bits);

        var reducedMatrix = matrix.stream().filter(line -> line.get(index) == minorityNumber).collect(Collectors.toCollection(ArrayList::new));
        System.out.println(reducedMatrix);
        return reduceMatrixByMinorities(reducedMatrix, index + 1);
    }

    private static int getMajorityNumber(ArrayList<Integer> bits)
    {
        long zeroes = bits.stream().filter(x -> x == 0).count();
        long ones = bits.stream().filter(x -> x == 1).count();
        return ones >= zeroes ? 1 : 0; // Ones take precedent over zeroes if it comes to a tie
    }

    private static int getMinorityNumber(ArrayList<Integer> bits)
    {
        long zeroes = bits.stream().filter(x -> x == 0).count();
        long ones = bits.stream().filter(x -> x == 1).count();
        return zeroes <= ones ? 0 : 1; // Zeroes take precedent over ones if it comes to a tie
    }

    private static int determineGamma(ArrayList<ArrayList<Integer>> matrix)
    {
        int gamma = 0;
        for(int i=0; i < matrix.size(); i++)
        {
            var bits = matrix.get(i);
            int majorityNumber = getMajorityNumber(bits);

            gamma = gamma << 1;
            gamma += majorityNumber;
        }
        return gamma;
    }

    private static int determineEpsilon(ArrayList<ArrayList<Integer>> matrix)
    {
        int epsilon = 0;
        for(int i=0; i < matrix.size(); i++)
        {
            var bits = matrix.get(i);
            int majorityNumber = getMajorityNumber(bits);

            epsilon = epsilon << 1;
            epsilon += majorityNumber == 1 ? 0 : 1;
        }
        return epsilon;
    }

    private static ArrayList<ArrayList<Integer>> rotateMatrix(ArrayList<ArrayList<Integer>> lines){
        var lineLength = lines.get(0).size();

        ArrayList<ArrayList<Integer>> rotatedArray = new ArrayList<>();
        for(int i=0; i < lineLength; i++)
        {
            ArrayList<Integer> bits = new ArrayList<>();
            for (ArrayList<Integer> line : lines) 
            {
                int intVal = Integer.valueOf(line.get(i));
                bits.add(intVal);
            }

            rotatedArray.add(bits);
        }
        return rotatedArray;
    }

    private static ArrayList<ArrayList<Integer>> readFile(String filepath) {
        ArrayList<ArrayList<Integer>> matrix = new ArrayList<>();
        try (var br = new BufferedReader(new FileReader(new File(filepath)))) {
            while (br.ready()) {
                ArrayList<Integer> bits = new ArrayList<>();
                for(char c : br.readLine().toCharArray()){
                    bits.add(c == 48 ? 0 : 1);
                }
                matrix.add(bits);
            }
        } catch (IOException e) {
            e.printStackTrace();
        }
        return matrix;
    }
}
