package hr.fer.bio.project.rrr;

import com.sun.org.apache.xpath.internal.operations.Bool;
import hr.fer.bio.project.booleanarray.BooleanArray;
import hr.fer.bio.project.main.Main;
import hr.fer.bio.project.rankable.Rankable;
import hr.fer.bio.project.wavelet.TreeNode;

import java.util.ArrayList;
import java.util.List;
import java.util.Map;

/**
 * RRRStructure model. Creates RRRStructure from bit vector(BooleanArray).
 * This class implements Rankable interface and can be used as node for WaveletTree
 *
 * @see hr.fer.bio.project.rrr.RRRLookUpTable
 * @author bpervan
 * @since 1.0
 */
public class RRRBlock implements Rankable {

    /** Reference to a RRR Look - up table */
    public RRRLookUpTable table;

    /** BooleanArray (BitVector) from which RRR block is built from */
    public BooleanArray booleanArray;

    /** Basic RRR structure attributes */
    /** Length of original BooleanArray */
    public int arrayLength;
    /** Number of booleans held by a single block */
    public int blockSize;
    /* Number of blocks held by a single superblock */
    public int superblockSize;
    /* Number of booleans held by a single superblock */
    public int superblockCapacity;

    /** RRR structure block classes */
    public List<Integer> classes;
    /** RRR structure block offsets */
    public List<Integer> offsets;
    /** Superblock cache. Not cumulative */
    public List<Integer> superblockData;

    /**
     * Constructs RRR structure from basic BooleanArray
     *
     * @param booleanArray Array of booleans which RRR structure will be build from
     * */
    public RRRBlock(BooleanArray booleanArray){
        this.booleanArray = booleanArray;
        this.arrayLength = booleanArray.data.length;
        this.blockSize = RRRLookUpTable.log2(arrayLength) / 2;
        if(this.blockSize == 0){
            this.blockSize = 1;
        }
        this.superblockSize = RRRLookUpTable.log2(arrayLength);
        this.superblockCapacity = superblockSize * blockSize;

        classes = new ArrayList<Integer>();
        offsets = new ArrayList<Integer>();
        superblockData = new ArrayList<Integer>();

        /**
         * Every node has his own RRRLookUpTable
         * */
        table = new RRRLookUpTable(blockSize);

        /**
         * Initialize RRR structure
         * */
        this.init();
    }

    /**
     * Initializes RRR structure
     * */
    private void init(){
        int superblockPopCount = 0;
        int passCount = 0;
        //For every block
        for(int i = 0; i < arrayLength; i += blockSize){
            BooleanArray tempArray = new BooleanArray(blockSize);
            int popCount = 0;
            ++passCount;
            //Get pop count, by the way extract BooleanArray
            for(int j = 0; j < blockSize; ++j){
                if(i + j < arrayLength){
                    tempArray.data[j] = booleanArray.data[i + j];
                    if(booleanArray.data[i + j]){
                        popCount++;
                    }
                } else {
                    tempArray.data[j] = false;
                }
            }
            classes.add(popCount);
            offsets.add(getOffset(tempArray, popCount));

            //We are superblocks, resistance is futile
            superblockPopCount += popCount;
            if(passCount == superblockSize){
                superblockData.add(superblockPopCount);
                superblockPopCount = 0;
                passCount = 0;
            }
        }
        if(Main.DEBUG){
            System.out.println("---------------------RESULT---------------------");
            for(int k = 0; k < classes.size(); ++k){
                System.out.println("Class: " + classes.get(k) + ", Offset: " + offsets.get(k));
            }
            for(int k = 0; k < superblockData.size(); ++k){
                System.out.println("Superblock: " + k + " " + superblockData.get(k));
            }
            System.out.println("------------------------------------------------");
        }
    }

    /**
     * Extracts offset for given BooleanArray in a specific class
     *
     * @param inputArray BooleanArray for which we are looking for offset
     * @param popCount Class
     *
     * @return Integer offset which has to be converted to binary
     * */
    private int getOffset(BooleanArray inputArray, int popCount){
        Map<BooleanArray, Integer> tempMap = this.table.getTable().get(popCount);
        for(Map.Entry<BooleanArray, Integer> entry : tempMap.entrySet()){
            if(entry.getKey().equals(inputArray)){
                return entry.getValue();
            }
        }
        return -1;
    }

    @Override
    public int rank(char c, int endPos, TreeNode rootNode){
        TreeNode<RRRBlock> workingNode = rootNode;

        if(workingNode.data.arrayLength <= endPos){
            throw new IllegalArgumentException("End position larger than input data");
        }

        int counter = 0;
        int rightBound = endPos;
        //Iterate until
        while(workingNode != null){
            //Superblock level
            int endSuperBlockPosition = rightBound / workingNode.data.superblockCapacity;
            for(int i = 0; i < endSuperBlockPosition; ++i){
                counter += workingNode.data.superblockData.get(i);
            }

            //Block level
            int endBlockPosition = rightBound / workingNode.data.blockSize;
            for(int i = endSuperBlockPosition * workingNode.data.superblockSize;
                i < endBlockPosition;
                ++i){
                counter += workingNode.data.classes.get(i);
            }
            int endClass = workingNode.data.classes.get(endBlockPosition);
            int endOffset = workingNode.data.offsets.get(endBlockPosition);

            //Inner block level
            BooleanArray tempArray = null;
            Map<BooleanArray, Integer> tempMap = workingNode.data.table.getTable().get(endClass);
            for(Map.Entry<BooleanArray, Integer> entry : tempMap.entrySet()){
                if(entry.getValue() == endOffset){
                    tempArray = entry.getKey();
                    break;
                }
            }
            for(int i = 0; i < (rightBound % workingNode.data.blockSize); ++i){
                if(tempArray.data[i]){
                    counter++;
                }
            }

            //Set appropriate leaf as next node to be iterated through
            if(workingNode.charMap.get(c) == true){
                workingNode = workingNode.rightChild;
            } else {

                counter = rightBound - counter;
                workingNode = workingNode.leftChild;
            }
            rightBound = counter;
            counter = 0;
        }
        return rightBound;
    }

    @Override
    public int select(char c, int boundary, TreeNode rootNode) {
        TreeNode<RRRBlock> workingNode = rootNode;

        //Find out appropriate leaf
        while(true){
            if(!workingNode.charMap.get(c)){
                //left
                if(workingNode.leftChild != null){
                    workingNode = workingNode.leftChild;
                } else {
                    break;
                }
            } else {
                //right
                if(workingNode.rightChild != null){
                    workingNode = workingNode.rightChild;
                } else {
                    break;
                }
            }
        }

        //Working node now contains leaf with char c
        int counter = 0;
        int newBound = boundary;
        int select = 0;
        //Iterate until we reach root
        while(workingNode != null){
            //Find out appropriate superblock
            int numSuperBlocks = workingNode.data.arrayLength / workingNode.data.superblockCapacity;
            int superblockCounter = 0;
            for(int i = 0; i < numSuperBlocks; ++i){

                if(workingNode.charMap.get(c)){
                    //Ones
                    superblockCounter += workingNode.data.superblockData.get(i);
                } else {
                    //Zeros
                    superblockCounter += workingNode.data.superblockCapacity - workingNode.data.superblockData.get(i);
                }

                if(superblockCounter > newBound){
                    break;
                } else {
                    counter = superblockCounter;
                    select++;
                }
            }
            select = select * workingNode.data.superblockCapacity;

            //Block level
            int index = select / workingNode.data.blockSize;
            int blockCounter = 0;
            int iCache = 0;
            for(int i = index; i < (index + workingNode.data.superblockSize); i += 1){
                if(counter == newBound){
                    break;
                }
                if(workingNode.charMap.get(c)){
                    //Ones
                    blockCounter = workingNode.data.classes.get(i);
                } else {
                    //Zeros
                    blockCounter = workingNode.data.blockSize - workingNode.data.classes.get(i);
                }

                if(blockCounter + counter > newBound){
                    iCache = i;
                    break;
                } else {
                    counter += blockCounter;
                    select += workingNode.data.blockSize;
                }
            }

            //Inner block level
            BooleanArray tempArray = null;
            Map<BooleanArray, Integer> block = workingNode.data.table.getTable().get(workingNode.data.classes.get(iCache));
            //Extract appropriate offset
            for(Map.Entry<BooleanArray, Integer> entry : block.entrySet()){
                if(entry.getValue().equals(workingNode.data.offsets.get(iCache))){
                    tempArray = entry.getKey();
                    break;
                }
            }
            //Count in permutation(offset)
            for(int i = 0; i < tempArray.data.length; ++i){
                if(counter == newBound){
                    break;
                }
                if(tempArray.data[i] && workingNode.charMap.get(c)){
                    counter++;
                } else if (!tempArray.data[i] && !workingNode.charMap.get(c)){
                    counter++;
                }
                select++;
            }
            workingNode = workingNode.parent;
            newBound = select;
            select = 0;
            counter = 0;
        }
        return newBound;
    }
}
