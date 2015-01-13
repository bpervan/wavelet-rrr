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
        if(args.length < 1 || args.length > 4){
            System.out.println("Arguments?");
            System.out.println("java hr.fer.bio.project.main FileName (R|S) Character EndBound");
            System.exit(-1);
        }

        Fasta inputGenome = Fasta.fromFile(args[0]);
        long time1 = System.nanoTime();
        TreeNode<RRRBlock> rootNode = WaveletTreeBuilder.getInstance().fromStringRRR(inputGenome.getReadings());
        long time2 = System.nanoTime();
        long diff = time2 - time1;
        double resultTime = (double) diff / 1000000000.0;
        System.out.println("Wavelet tree with RRR nodes created in " + resultTime + " s");


        time1 = System.nanoTime();
        TreeNode<BooleanArray> rootNodeBooleanArray = WaveletTreeBuilder.getInstance().fromString(inputGenome.getReadings());
        time2 = System.nanoTime();
        diff = time2 - time1;
        resultTime = (double) diff / 1000000000.0;
        System.out.println("Wavelet tree with BooleanArray nodes created in " + resultTime + " s");

        int resultRank;
        int resultSelect;
        if(args.length == 4){
            if(args[1].equals("R")){
                //Rank
                time1 = System.nanoTime();
                resultRank = rootNode.rank(args[2].charAt(0), Integer.parseInt(args[3]), rootNode);
                time2 = System.nanoTime();
                diff = time2 - time1;
                resultTime = (double) diff / 1000000000.0;
                System.out.println("Rank(RRR): " + resultRank + ". Calculated in " + resultTime + " s");

                time1 = System.nanoTime();
                resultRank = rootNodeBooleanArray.rank(args[2].charAt(0), Integer.parseInt(args[3]), rootNodeBooleanArray);
                time2 = System.nanoTime();
                diff = time2 - time1;
                resultTime = (double) diff / 1000000000.0;
                System.out.println("Rank(BooleanArray): " + resultRank + ". Calculated in " + resultTime + " s");

                inOrder(rootNode);
                System.out.println(mape);
                System.out.println(lutovi);
                System.out.println(blokovi);
                System.out.println(superblokovi);
                System.out.println(booleanarrayevi);
            } else if(args[1].equals("S")){
                //Select
                time1 = System.nanoTime();
                resultRank = rootNode.select(args[2].charAt(0), Integer.parseInt(args[3]), rootNode);
                time2 = System.nanoTime();
                diff = time2 - time1;
                resultTime = (double) diff / 1000000000.0;
                System.out.println("Select(RRR): " + resultRank + ". Calculated in " + resultTime + " s");

                time1 = System.nanoTime();
                resultRank = rootNodeBooleanArray.select(args[2].charAt(0), Integer.parseInt(args[3]), rootNodeBooleanArray);
                time2 = System.nanoTime();
                diff = time2 - time1;
                resultTime = (double) diff / 1000000000.0;
                System.out.println("Select(BooleanArray): " + resultRank + ". Calculated in " + resultTime + " s");
            } else {
                System.out.println("R or S");
                System.exit(-1);
            }

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

    //Memory analysis
    //Charmap
    //RRRRLuT
    //BooleanArray

    static int mape;
    static int lutovi;
    static int blokovi;
    static int superblokovi;
    static int booleanarrayevi;

    private static void inOrder(TreeNode<RRRBlock> rootNode){
        if(rootNode == null){
            return;
        }
        inOrder(rootNode.leftChild);
        /*System.out.println("Velicina mape: " + rootNode.charMap.size() + " (Character, Boolean) pairs");
        System.out.println("Velicina LuT - a" + rootNode.data.table.getTable().size() + " Map<Integer, Map<BooleanArray, Integer>> pairs");
        System.out.println("Velicina blokova " + rootNode.data.classes.size() + " " + rootNode.data.offsets.size() + " (class, offset) pairs");
        System.out.println("Velicina superblokova " + rootNode.data.superblockData.size() + " integers");
        System.out.println("Velicina BooleanArraya " + rootNode.data.booleanArray.data.length + " booleans");
        System.out.println("-----------------------");*/
        mape += rootNode.charMap.size();
        lutovi += rootNode.data.table.getTable().size();
        blokovi += (rootNode.data.classes.size() * 2);
        superblokovi += rootNode.data.superblockData.size();
        booleanarrayevi += rootNode.data.booleanArray.data.length;

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
