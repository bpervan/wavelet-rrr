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
 * Created by Branimir on 21.10.2014..
 * Algorithm steps:
 * 1. Take input BitArray
 * 2. Get global lookup table for input.length
 * 3. Divide input to blocks
 * 4. Iterate through blocks and encode each block as (Class, Offset) pair
 */
public class RRRBlock implements Rankable {

    public RRRLookUpTable table;

    public BooleanArray booleanArray;
    public int arrayLength;
    public int blockSize;
    public int superblockSize;
    public int superblockCapacity;

    public List<Integer> classes;
    public List<Integer> offsets;
    public List<Integer> superblockData;

    public RRRBlock(BooleanArray booleanArray){
        this.booleanArray = booleanArray;
        this.arrayLength = booleanArray.data.length;
        this.blockSize = RRRLookUpTable.log2(arrayLength) / 2;
        if(this.blockSize == 0){
            this.blockSize = 1;
        }
        //this.blockSize = 5;
        //broj blokova u superbloku
        //this.superblockSize = this.blockSize * RRRLookUpTable.log2(arrayLength);
        this.superblockSize = RRRLookUpTable.log2(arrayLength);
        //this.superblockSize = 2;
        //broj true/false u superbloku
        this.superblockCapacity = superblockSize * blockSize;

        classes = new ArrayList<Integer>();
        offsets = new ArrayList<Integer>();
        superblockData = new ArrayList<Integer>();

        table = new RRRLookUpTable(blockSize);
        /*table = RRRLookUpTable.getInstance();
        table.initialize(blockSize);*/

        this.init();
    }

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


    private int getOffset(BooleanArray inputArray, int popCount){
        Map<BooleanArray, Integer> tempMap = this.table.getTable().get(popCount);
        for(Map.Entry<BooleanArray, Integer> entry : tempMap.entrySet()){
            if(entry.getKey().equals(inputArray)){
                return entry.getValue();
            }
        }
        return 58;
    }

    @Override
    public int rank(char c, int endPos, TreeNode rootNode){
        TreeNode<RRRBlock> workingNode = rootNode;

        if(workingNode.data.arrayLength <= endPos){
            throw new IllegalArgumentException("End position larger than input data");
        }


        int counter = 0;
        int rightBound = endPos;

        while(workingNode != null){
            int endSuperBlockPosition = rightBound / workingNode.data.superblockCapacity;
            for(int i = 0; i < endSuperBlockPosition; ++i){
                counter += workingNode.data.superblockData.get(i);
            }
            int endBlockPosition = rightBound / workingNode.data.blockSize;
            for(int i = endSuperBlockPosition * workingNode.data.superblockSize;
                i < endBlockPosition;
                ++i){
                counter += workingNode.data.classes.get(i);
            }
            int endClass = workingNode.data.classes.get(endBlockPosition);
            int endOffset = workingNode.data.offsets.get(endBlockPosition);

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
            if(workingNode.charMap.get(c) == true){
                //char c is encoded as 1, count 1es, proceed to right child

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
        //working node now contains leaf with char c
        int counter = 0;
        int newBound = boundary;
        int select = 0;
        while(workingNode != null){
            int numSuperBlocks = workingNode.data.arrayLength / workingNode.data.superblockCapacity;
            int superblockCounter = 0;
            for(int i = 0; i < numSuperBlocks; ++i){

                if(workingNode.charMap.get(c)){
                    superblockCounter += workingNode.data.superblockData.get(i);
                } else {
                    superblockCounter += workingNode.data.superblockCapacity - workingNode.data.superblockData.get(i);
                }

                if(superblockCounter > newBound){
                    break;
                } else {
                    counter = superblockCounter;
                    select++;
                }
            }
            //select--;
            //problem s nulama i jedinicama
            select = select * workingNode.data.superblockCapacity;

            int index = select / workingNode.data.blockSize;
            int blockCounter = 0;
            int iCache = 0;
            for(int i = index; i < (index + workingNode.data.superblockSize); i += 1){
                if(counter == newBound){
                    break;
                }
                if(workingNode.charMap.get(c)){
                    blockCounter = workingNode.data.classes.get(i);
                } else {
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


            BooleanArray tempArray = null;
            Map<BooleanArray, Integer> block = workingNode.data.table.getTable().get(workingNode.data.classes.get(iCache));
            for(Map.Entry<BooleanArray, Integer> entry : block.entrySet()){
                if(entry.getValue().equals(workingNode.data.offsets.get(iCache))){
                    tempArray = entry.getKey();
                    break;
                }
            }
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
            //System.out.println(select);
            workingNode = workingNode.parent;
            newBound = select;
            select = 0;
            counter = 0;
        }
        return newBound;
    }
}
