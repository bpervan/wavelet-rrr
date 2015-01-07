package hr.fer.bio.project.rrr;

import hr.fer.bio.project.booleanarray.BooleanArray;

import java.math.BigInteger;
import java.util.HashMap;
import java.util.Map;

/**
 * Created by Branimir on 23.10.2014..
 */
public class RRRLookUpTable {

    /**
     * Map<Class, Map<Offset, CumulativeRank>>
     * */
    private Map<Integer, Map<Integer, BooleanArray>> table;

    private Map<Integer, Map<BooleanArray, Integer>> newTable;

    private RRRLookUpTable(){ }

    public RRRLookUpTable(int length){

        newTable = new HashMap<Integer, Map<BooleanArray, Integer>>();

        this.generateTable(length);
    }

    public Map<Integer, Map<BooleanArray, Integer>> getTable(){
        return this.newTable;
    }

    private static RRRLookUpTable instance;

    public static RRRLookUpTable getInstance(){
        if(instance == null){
            synchronized (RRRLookUpTable.class){
                if(instance == null){
                    instance = new RRRLookUpTable();
                }
            }
        }
        return instance;
    }

    public void generateTable(int length){
        //Temp map
        Map<BooleanArray, Integer> tempMap;
        //int blockSize = RRRLookUpTable.log2(length) / 2;
        int blockSize = length;
        //For every class (every popcount)
        for(int i = 0; i <= blockSize; ++i){
            //Create brand new map
            tempMap = new HashMap<BooleanArray, Integer>();
            //First element
            int seed = element0(i);
            //Binomial
            int nCr = binomial(blockSize, i).intValue();
            //Basically temp var
            int next = 0;
            //Loop and generate permutations
            //System.out.println(decodeInt(seed, blockSize) + " -> 0");
            tempMap.put(decodeInt(seed, blockSize), 0);
            for(int j = 0; j < (nCr - 1); ++j){
                next = nextPermutation(seed);
                seed = next;
                //Seed contains permutations, convert to binary
                int temp = j + 1;
                tempMap.put(decodeInt(seed, blockSize), temp);
                //System.out.println(decodeInt(seed, length) + " -> " + temp);
            }
            //System.out.println("-----------");
            newTable.put(i, tempMap);
        }
    }

    /**
     * Decode int to BooleanArray, with leading zeros
     * */
    public static BooleanArray decodeInt(int toDecode, int num){
        BooleanArray retVal = new BooleanArray(num);
        int counter = num - 1;
        while(toDecode > 0){
            int rem = toDecode % 2;
            toDecode = toDecode / 2;
            if(rem == 1){
                retVal.data[counter] = true;
            } else {
                retVal.data[counter] = false;
            }
            counter--;
        }
        return retVal;
    }

    public static int element0(int c){
        return (1 << c) - 1;
    }

    public static BigInteger binomial(int n, int k){
        BigInteger retVal = BigInteger.ONE;
        for(int i = 0; i < k; ++i){
            retVal = retVal.multiply(BigInteger.valueOf(n - i)).divide(BigInteger.valueOf(i + 1));
        }
        return retVal;
    }

    public static int nextPermutation(int v){
        int t = (v | (v - 1)) + 1;
        int w = t | ((((t & -t) / (v & -v)) >> 1) - 1);
        return w;
    }

    public static int log2(int n){
        if(n <= 0){
            throw new IllegalArgumentException();
        }
        return 31 - Integer.numberOfLeadingZeros(n);
    }
}
