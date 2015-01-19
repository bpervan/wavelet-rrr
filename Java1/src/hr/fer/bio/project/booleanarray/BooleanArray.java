package hr.fer.bio.project.booleanarray;

import hr.fer.bio.project.rankable.Rankable;
import hr.fer.bio.project.wavelet.TreeNode;

import java.util.Arrays;

/**
 * Simple class for boolean[] encapsulation
 *
 * @author bpervan
 * @since 1.0
 */
public class BooleanArray implements Rankable {

    public boolean[] data;

    public BooleanArray(int size){
        data = new boolean[size];
    }

    public BooleanArray(boolean[] data){
        this.data = Arrays.copyOf(data, data.length);
    }

    @Override
    public int rank(char c, int endPos, TreeNode rootNode){
        TreeNode<BooleanArray> workingNode = rootNode;

        if(workingNode.data.data.length <= endPos){
            throw new IllegalArgumentException("End position larger than input data");
        }

        int counter = 0;
        int rightBound = endPos;

        while(workingNode != null){
            if(workingNode.charMap.get(c) == true){
                //char c is encoded as 1, count 1es, proceed to right child
                for(int i = 0; i < rightBound; ++i){
                    if(workingNode.data.data[i]){
                        counter++;
                    }
                }
                workingNode = workingNode.rightChild;
            } else {
                //char c is encoded as 0, count 0es, proceed to left child
                for(int i = 0; i < rightBound; ++i){
                    if(!workingNode.data.data[i]){
                        counter++;
                    }
                }
                workingNode = workingNode.leftChild;
            }
            rightBound = counter;
            counter = 0;
        }
        return rightBound;
    }

    @Override
    public int select(char c, int boundary, TreeNode rootNode) {
        TreeNode<BooleanArray> workingNode = rootNode;

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
        //workingNode now contains leaf with char c

        //count 0/1
        //parent with same operation count -> previously calculated boundary
        int counter = 0;
        int newBound = boundary;
        int Select = 0;
        while(workingNode != null){
            if(workingNode.charMap.get(c)){
                for(int i = 0; i < workingNode.data.data.length; ++i){
                    Select++;
                    if(workingNode.data.data[i]){
                        counter++;
                        if(counter == newBound){
                            break;
                        }
                    }
                }
            } else {
                for(int i = 0; i < workingNode.data.data.length; ++i){
                    Select++;
                    if(!workingNode.data.data[i]){
                        counter++;
                        if(counter == newBound){
                            break;
                        }
                    }
                }
            }
            workingNode = workingNode.parent;
            newBound = Select;
            counter = 0;
            Select = 0;
        }

        return newBound;
    }

    @Override
    public String toString(){
        StringBuilder stringBuilder = new StringBuilder();
        for(int i = 0; i < data.length; ++i){
            if(data[i]){
                stringBuilder.append(1);
            } else {
                stringBuilder.append(0);
            }
        }
        return stringBuilder.toString();
    }

    @Override
    public boolean equals(Object that){
        if(that == null){
            return false;
        }
        if(!(that instanceof BooleanArray)){
            return false;
        }

        BooleanArray thatBooleanArray = (BooleanArray) that;
        if(data.length != thatBooleanArray.data.length){
            return false;
        }

        for(int i = 0; i < data.length; ++i){
            if(data[i] != thatBooleanArray.data[i]){
                return false;
            }
        }
        return true;
    }
}
