package hr.fer.bio.project.main;

import hr.fer.bio.project.booleanarray.BooleanArray;
import hr.fer.bio.project.io.Fasta;
import hr.fer.bio.project.rrr.RRRBlock;
import hr.fer.bio.project.wavelet.TreeNode;
import hr.fer.bio.project.wavelet.WaveletTree;
import hr.fer.bio.project.wavelet.WaveletTreeBuilder;

import java.io.BufferedReader;
import java.io.IOException;
import java.io.InputStreamReader;
import java.util.Map;

/**
 * Created by Branimir on 21.10.2014..
 */
public class Main {

    public static final boolean DEBUG = false;

    /**
     * Usage:
     * 1. java hr.fer.bio.project.main.Main pathToFastaFile
     * 2. java hr.fer.bio.project.main.Main pathToFastaFile character endPosition
     * */
    public static void main(String[] args) throws IOException{
        if(args.length < 1 || args.length > 3){
            System.out.println("Arguments?");
            System.exit(-1);
        }

        Fasta inputGenome = Fasta.fromFile(args[0]);
        long time1 = System.nanoTime();
        TreeNode<RRRBlock> rootNode = WaveletTreeBuilder.getInstance().fromStringRRR(inputGenome.getReadings());
        TreeNode<BooleanArray> rootNode2 = WaveletTreeBuilder.getInstance().fromString(inputGenome.getReadings());
        long time2 = System.nanoTime();
        System.out.println("Wavelet tree with RRR nodes created in " + (time2 - time1) + " ns");

        System.out.println(rootNode.rank('t', 4000000, rootNode));
        System.out.println(rootNode.select('a', 40000, rootNode));
        System.out.println(rootNode.rank('a', 543210, rootNode));
        System.out.println(rootNode.select('c', 543210, rootNode));

        System.out.println(rootNode2.rank('t', 4000000, rootNode2));
        System.out.println(rootNode2.select('a', 40000, rootNode2));
        System.out.println(rootNode2.rank('a', 543210, rootNode2));
        System.out.println(rootNode2.select('c', 543210, rootNode2));
        System.exit(0);

        int resultRank;
        if(args.length == 3){
            time1 = System.nanoTime();
            resultRank = rootNode.rank(args[2].charAt(0), Integer.parseInt(args[3]), rootNode);
            time2 = System.nanoTime();
            System.out.println("Rank: " + resultRank + ". Calculated in " + (time2 - time1) + " ns");
        } else if(args.length == 1){
            BufferedReader bufferedReader = new BufferedReader(new InputStreamReader(System.in));

            String c = " ";
            String p = " ";

            while(true) {
                System.out.println("Char: ");
                c = bufferedReader.readLine();
                if(c.length() == 0){
                    break;
                }
                System.out.println("Position: ");
                p = bufferedReader.readLine();
                time1 = System.nanoTime();
                resultRank = rootNode.rank(c.charAt(0), Integer.parseInt(p), rootNode);
                time2 = System.nanoTime();
                System.out.println("Rank: " + resultRank + ". Calculated in " + (time2 - time1) + " ns");
            }
        } else {
            System.out.println("Arguments?");
            System.exit(-1);
        }


        System.out.println("Over and out");
    }

    private static void inOrder(TreeNode<BooleanArray> rootNode){
        if(rootNode == null){
            return;
        }
        inOrder(rootNode.leftChild);
        printBoolArray(rootNode.data);
        printHashMap(rootNode.charMap);
        System.out.println("-----");
        inOrder(rootNode.rightChild);
    }

    private static void printBoolArray(BooleanArray array){
        for(int i = 0; i < array.data.length; ++i){
            if(array.data[i]){
                System.out.print(1);
            } else {
                System.out.print(0);
            }
        }
        System.out.println();
    }

    public static void printHashMap(Map<Character, Boolean> charMap){
        for(Map.Entry<Character, Boolean> entry : charMap.entrySet()){
            if(entry.getValue()){
                System.out.println(entry.getKey() + ": " + 1);
            } else {
                System.out.println(entry.getKey() + ": " + 0);
            }
        }
        System.out.println();
    }
}
