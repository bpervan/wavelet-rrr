package hr.fer.bio.project.rankable;

import hr.fer.bio.project.wavelet.TreeNode;

/**
 * Operations that should be supported by Wavelet Tree and every
 * data structure that which will be Wavelet Tree node
 *
 * @see hr.fer.bio.project.rankable.Rankable
 * @see hr.fer.bio.project.rankable.Selectable
 *
 * @author Branimir
 * @since 1.0
 */
public interface TreeOperations {
    /**
     * Calculates rank on Wavelet Tree where nodes are data structures supporting
     * Rank operation
     *
     * @param c character
     * @param endPos right bound
     * @param rootNode root node of a tree
     *
     * @return Rank of char c by endPos at tree given by rootNode
     * */
    public int rank(char c, int endPos, TreeNode rootNode);

    /**
     * Performs select operation on Wavelet Tree where nodes are data structures
     * supporting Rank operation
     *
     * @param c
     * @param boundary
     * @param rootNode
     *
     * @return Select of char c by boundary at tree given by rootNode
     * */
    public int select(char c, int boundary, TreeNode rootNode);
}
