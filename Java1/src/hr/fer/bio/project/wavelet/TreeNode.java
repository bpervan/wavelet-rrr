package hr.fer.bio.project.wavelet;

import hr.fer.bio.project.rankable.Rankable;

import java.util.Map;

/**
 * Created by Branimir on 21.10.2014..
 */
public class TreeNode<T extends Rankable> implements Rankable {
    protected TreeNode<T> treeRoot;
    public TreeNode<T> parent;
    public TreeNode<T> leftChild;
    public TreeNode<T> rightChild;
    public T data;
    public Map<Character, Boolean> charMap;

    public TreeNode(T data){
        this.data = data;
    }

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

    @Override
    public int select(char c, int boundary, TreeNode rootNode) {
        return this.data.select(c, boundary, rootNode);
    }
}
