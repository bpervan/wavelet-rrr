package hr.fer.bio.project.rankable;

import hr.fer.bio.project.wavelet.TreeNode;

/**
 * Created by Branimir on 8.11.2014..
 */
public interface Rankable {
    public int rank(char c, int endPos, TreeNode rootNode);
}
