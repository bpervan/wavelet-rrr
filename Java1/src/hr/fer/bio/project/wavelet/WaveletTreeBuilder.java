package hr.fer.bio.project.wavelet;

import hr.fer.bio.project.booleanarray.BooleanArray;
import hr.fer.bio.project.rrr.RRRBlock;

import java.util.ArrayList;
import java.util.HashMap;
import java.util.List;
import java.util.Map;

/**
 * Builder class for WaveletTree. This class builds Wavelet tree from input string sequence.
 * At this moment, separate methods for building trees with BooleanArray nodes and with RRRBlock
 * nodes are used
 *
 * @see hr.fer.bio.project.wavelet.TreeNode
 * @see hr.fer.bio.project.booleanarray.BooleanArray
 * @see hr.fer.bio.project.rrr.RRRBlock
 *
 * @author bpervan
 * @since 1.0
 */
public class WaveletTreeBuilder {

    private WaveletTreeBuilder(){}

    /** Factory method */
    public static WaveletTreeBuilder getInstance(){
        return new WaveletTreeBuilder();
    }

    /**
     * Delegates method call to internal/private method
     * */
    public TreeNode<BooleanArray> fromString(String string){
        return fromString(string, toMap(string), 0, string.length() - 1);
    }

    /**
     * Recursively builds Wavelet tree from input string sequence
     * Node is BooleanArray
     *
     * @param string Input sequence
     * @param charMap Character -> Boolean mappings
     * @param min Left bound
     * @param max Right bound
     *
     * @return Tree root
     * */
    private TreeNode<BooleanArray> fromString(String string, Map<Character, Boolean> charMap, int min, int max){
        if(charMap.size() < 2){
            return null;
        }
        int mid = min + (max - min) / 2;
        TreeNode<BooleanArray> tempnode = new TreeNode<BooleanArray>(encodeString(string, charMap));
        tempnode.charMap = charMap;
        tempnode.leftChild = fromString(stringDivider(string, charMap)[0], toMap(stringDivider(string, charMap)[0]), min, mid - 1);
        if(tempnode.leftChild != null) {
            tempnode.leftChild.parent = tempnode;
        }
        tempnode.rightChild = fromString(stringDivider(string, charMap)[1], toMap(stringDivider(string, charMap)[1]), mid + 1, max);
        if(tempnode.rightChild != null) {
            tempnode.rightChild.parent = tempnode;
        }
        return tempnode;
    }

    /**
     * Delegates method call to internal/private method
     * */
    public TreeNode<RRRBlock> fromStringRRR(String string){
        return fromStringRRR(string, toMap(string), 0, string.length() - 1);
    }

    /**
     * Recursively builds Wavelet tree from input string sequence
     * Node is RRRBlock
     *
     * @param string Input sequence
     * @param charMap Character -> Boolean mappings
     * @param min Left bound
     * @param max Right bound
     *
     * @return Tree root
     * */
    private TreeNode<RRRBlock> fromStringRRR(String string, Map<Character, Boolean> charMap, int min, int max){
        if(charMap.size() < 2){
            return null;
        }
        int mid = min + (max - min) / 2;
        TreeNode<RRRBlock> tempnode = new TreeNode<RRRBlock>(encodeStringRRR(string, charMap));
        tempnode.charMap = charMap;
        tempnode.leftChild = fromStringRRR(stringDivider(string, charMap)[0], toMap(stringDivider(string, charMap)[0]), min, mid - 1);
        if(tempnode.leftChild != null){
            tempnode.leftChild.parent = tempnode;
        }
        tempnode.rightChild = fromStringRRR(stringDivider(string, charMap)[1], toMap(stringDivider(string, charMap)[1]), mid + 1, max);
        if(tempnode.rightChild != null){
            tempnode.rightChild.parent = tempnode;
        }
        return tempnode;
    }

    /**
     * Adapter for encodeString method call, used for building RRRBlock
     * */
    private RRRBlock encodeStringRRR(String string, Map<Character, Boolean> charMap){
        return new RRRBlock(encodeString(string, charMap));
    }

    /**
     * Divides string based on mapping in charMap
     *
     * @param string string for division
     * @param charMap mappings
     *
     * @return divided string as string array (on indices 0 and 1)
     * */
    private String[] stringDivider(String string, Map<Character, Boolean> charMap){
        StringBuilder sb1 = new StringBuilder();
        StringBuilder sb2 = new StringBuilder();
        String[] returnArray = new String[2];

        for(char c : string.toCharArray()){
            if(charMap.get(c)){
                sb2.append(c);
            } else {
                sb1.append(c);
            }
        }

        returnArray[0] = sb1.toString();
        returnArray[1] = sb2.toString();
        return returnArray;
    }

    /**
     * Encodes string as BooleanArray
     *
     * @param string string to be encoded
     * @param charMap mappings
     *
     * @return BooleanArray representing encoded string
     * */
    private BooleanArray encodeString(String string, Map<Character, Boolean> charMap){
        BooleanArray booleanArray = new BooleanArray(string.length());
        for(int i = 0; i < string.length(); ++i){
            booleanArray.data[i] = charMap.get(string.charAt(i));
        }

        return booleanArray;
    }

    /**
     * Extracts mapping(dictionary) from input string
     *
     * @param string input string
     *
     * @return HashMap with mappings between chars in input string and booleans
     * */
    private Map<Character, Boolean> toMap(String string){
        List<Character> charList = extractAlphabet(string);
        Map<Character, Boolean> charMap = new HashMap<Character, Boolean>();
        int mid = charList.size() / 2;
        for(char c : string.toCharArray()){
            if(isInArray(charList, c, 0, mid)){
                charMap.put(c, false);
            } else {
                charMap.put(c, true);
            }
        }
        return charMap;
    }

    /**
     * Chechs if char is present in a list of chars
     *
     * @param haystack list of chars
     * @param needle char we are looking for
     * @param low left bound
     * @param high right bound
     *
     * @return true if char needle is present in haystack surrounded by left and right bound false otherwise
     * */
    private boolean isInArray(List<Character> haystack, char needle, int low, int high){
        return haystack.subList(low, high).contains(needle);
    }

    /**
     * Extracts alphabet from input string
     *
     * @param s input string
     * @return List of characters from extracted string
     * */
    private List<Character> extractAlphabet(String s){
        List<Character> charList = new ArrayList<Character>();
        for(char c : s.toCharArray()){
            if(!charList.contains(c)){
                charList.add(c);
            }
        }
        //Collections.sort(charList);
        return charList;
    }
}
