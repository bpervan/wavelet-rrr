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

    static int mape;
    static int lutInts;
    static int lutBooleans;
    static int blokovi;
    static int superblokovi;
    static int booleanarrayevi;

    static int mapeBA;
    static int booleanarrayeviBA;

    /**
     * Usage:
     * 1. java hr.fer.bio.project.main.Main pathToFastaFile
     * 2. java hr.fer.bio.project.main.Main pathToFastaFile character endPosition
     * */
    public static void main(String[] args) throws IOException{
        if(args.length < 1 || args.length > 4){
            System.out.println("Arguments?");
            System.out.println("java hr.fer.bio.project.main FileName (R|S) Character EndBound (B|R)");
            System.exit(-1);
        }
        Fasta inputGenome = Fasta.fromFile(args[0]);
        long time1, time2, diff;
        double resultTime;
        int resultBytes;

        time1 = System.nanoTime();
        TreeNode<RRRBlock> rootNode = WaveletTreeBuilder.getInstance().fromStringRRR(inputGenome.getReadings());
        time2 = System.nanoTime();
        diff = time2 - time1;
        resultTime = (double) diff / 1000000000.0;
        System.out.println("Wavelet tree with RRR nodes created in " + resultTime + " s");
        inOrder(rootNode);
        resultBytes = (mape * 3) + (lutInts * 4) + (lutBooleans * 1) + (blokovi * 4 * 2) + (superblokovi * 4) + (booleanarrayevi * 1);
        System.out.println("Size(RRR): " + resultBytes);


        time1 = System.nanoTime();
        TreeNode<BooleanArray> rootNodeBooleanArray = WaveletTreeBuilder.getInstance().fromString(inputGenome.getReadings());
        time2 = System.nanoTime();
        diff = time2 - time1;
        resultTime = (double) diff / 1000000000.0;
        System.out.println("Wavelet tree with BooleanArray nodes created in " + resultTime + " s");
        inOrderBA(rootNodeBooleanArray);
        resultBytes = (mapeBA * 3) + (booleanarrayeviBA * 1);
        System.out.println("Size(BA): " + resultBytes);

        int resultRank;
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
        } else {
            System.out.println("Arguments?");
            System.exit(-1);
        }


        System.out.println("Over and out");
    }

    private static void inOrder(TreeNode<RRRBlock> rootNode){
        if(rootNode == null){
            return;
        }
        inOrder(rootNode.leftChild);
        mape += rootNode.charMap.size(); //char (2 bajta) boolean (1 bajt) = 3 bajta
        Map<Integer, Map<BooleanArray, Integer>> tempMap = rootNode.data.table.getTable();
        for(Map.Entry<Integer, Map<BooleanArray, Integer>> entry : tempMap.entrySet()){
            lutInts++; // integer (4 bajta)
            for(Map.Entry<BooleanArray, Integer> entry2 : entry.getValue().entrySet()){
                lutInts++; //integer (4 bajta)
                lutBooleans++; //boolean (4 bajta)
            }
        }
        blokovi += (rootNode.data.classes.size()); //integer (4 bajta) svaki dvaput jer je za classes i offsets
        superblokovi += rootNode.data.superblockData.size();// integer(4 bajta)
        booleanarrayevi += rootNode.data.booleanArray.data.length;//boolean(1 bajt)

        inOrder(rootNode.rightChild);
    }


    private static void inOrderBA(TreeNode<BooleanArray> rootNode){
        if(rootNode == null){
            return;
        }
        inOrderBA(rootNode.leftChild);
        mapeBA += rootNode.charMap.size();
        booleanarrayeviBA += rootNode.data.data.length;
        inOrderBA(rootNode.rightChild);
    }
}
