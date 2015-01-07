package hr.fer.bio.project.wavelet;

import hr.fer.bio.project.rankable.Rankable;

/**
 * Created by Branimir on 21.10.2014..
 */
public class WaveletTree<T extends Rankable> {
    public TreeNode<T> root;

    public WaveletTree(TreeNode<T> root){
        this.root = root;
    }
}
