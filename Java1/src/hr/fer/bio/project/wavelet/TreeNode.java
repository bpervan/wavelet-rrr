package hr.fer.bio.project.wavelet;

import hr.fer.bio.project.rankable.Rankable;

import java.util.Map;

/**
 * Models every node in Wavelet Tree. Parametrized with param T which by extending rankable
 * enables that tree node can be every data structure on which rank and select operation could
 * be performed
 *
 * @author bpervan
 * @since 1.0
 */
public class TreeNode<T extends Rankable> implements Rankable {

    /** Root node, currently not used */
    protected TreeNode<T> treeRoot;
    /** Reference to parent node, used for select operation */
    public TreeNode<T> parent;
    /** Reference to left child */
    public TreeNode<T> leftChild;
    /** Reference to right child */
    public TreeNode<T> rightChild;
    /** Data holder */
    public T data;
    /** Dictionary with mapping rules between characters and booleans (bits) */
    public Map<Character, Boolean> charMap;

    /**
     * Constructs new tree node with given data
     *
     * @param data Data assigned to TreeNode
     * */
    public TreeNode(T data){
        this.data = data;
    }

    /**
     * Rank operation performed on appropriate data structure
     * */
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

    /** Select operation performed on appropriate data structure */
    @Override
    public int select(char c, int boundary, TreeNode rootNode) {
        return this.data.select(c, boundary, rootNode);
    }
}
