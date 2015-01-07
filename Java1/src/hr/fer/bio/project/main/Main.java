package hr.fer.bio.project.main;

import hr.fer.bio.project.booleanarray.BooleanArray;
import hr.fer.bio.project.io.Fasta;
import hr.fer.bio.project.rrr.RRRBlock;
import hr.fer.bio.project.wavelet.TreeNode;
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

    public static void main(String[] args) throws IOException{
        Fasta eColi = Fasta.fromFile("d:\\ecoli.fa");
        long time1 = System.nanoTime();
        TreeNode<RRRBlock> rootNode = WaveletTreeBuilder.getInstance().fromStringRRR(eColi.getReadings());
        long time2 = System.nanoTime();
        System.out.println("Wavelet tree with RRR nodes created in " + (time2 - time1) + " ns");

        BufferedReader bufferedReader = new BufferedReader(new InputStreamReader(System.in));

        String c = " ";
        String p = " ";
        int resultRank;
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
