package hr.fer.bio.project.wavelet;

import hr.fer.bio.project.rankable.Rankable;

import java.util.Map;

/**
 * Created by Branimir on 21.10.2014..
 */
public class TreeNode<T extends Rankable> implements Rankable {
    protected TreeNode<T> treeRoot;
    public TreeNode<T> leftChild;
    public TreeNode<T> rightChild;
    public T data;
    public Map<Character, Boolean> charMap;

    public TreeNode(T data){
        this.data = data;
    }

    /*
    * Algorithm:
    * 1. Dohvati bit kojim je enkodiran char c
    * 2. Pozicioniraj se po superblokovima na kraj prethodnog superbloka
    * 3. Zbrajaj popcount do kraja prethodnog bloka
    * 4. Ako je pozicija u sredini bloka, dohvati klasu(popcount) i offset, odredi popcount do pozicije endPos
    * 5. Loop -> u točan dio stabla, ali do zadnji put određene pozicije :)
    * */
    /*public int rankRRR(char c, int endPos){
        TreeNode<RRRBlock> workingNode = (TreeNode<RRRBlock>) this;

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
            int endOffset = workingNode.data.classes.get(endBlockPosition);

            BooleanArray tempArray = null;
            Map<BooleanArray, Integer> tempMap = workingNode.data.table.getTable().get(endClass);
            for(Map.Entry<BooleanArray, Integer> entry : tempMap.entrySet()){
                if(entry.getValue() == endOffset){
                    tempArray = entry.getKey();
                    break;
                }
            }
            for(int i = 0; i < (rightBound % workingNode.data.blockSize + 1); ++i){
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
    }*/

    @Override
    public int rank(char c, int endPos, TreeNode rootNode){
        if(!charMap.containsKey(c)){
            return 0;
        }
        if(endPos < 0){
            throw new IllegalArgumentException("End position must be positive");
        }
        return this.data.rank(c, endPos, rootNode);
    }
}
