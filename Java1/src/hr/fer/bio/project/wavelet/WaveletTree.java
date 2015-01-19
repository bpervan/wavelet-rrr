package hr.fer.bio.project.wavelet;

import hr.fer.bio.project.rankable.Rankable;
import hr.fer.bio.project.rankable.TreeOperations;

/**
 * Simple class encapsulating Wavelet Tree. Currently not used
 *
 * @author bpervan
 * @since 1.0
 */
public class WaveletTree<T extends Rankable> implements TreeOperations{
    public TreeNode<T> root;

    public WaveletTree(TreeNode<T> root){
        this.root = root;
    }

    @Override
    public int rank(char c, int endPos, TreeNode rootNode) {
        return this.root.rank(c, endPos, this.root);
    }

    @Override
    public int select(char c, int boundary, TreeNode rootNode) {
        return this.root.select(c, boundary, this.root);
    }
}
