package hr.fer.bio.project.wavelet;

import hr.fer.bio.project.booleanarray.BooleanArray;
import hr.fer.bio.project.rrr.RRRBlock;

import java.util.ArrayList;
import java.util.HashMap;
import java.util.List;
import java.util.Map;

/**
 * Created by Branimir on 21.10.2014..
 */
public class WaveletTreeBuilder {

    private WaveletTreeBuilder(){}
    public static WaveletTreeBuilder getInstance(){
        return new WaveletTreeBuilder();
    }

    public TreeNode<BooleanArray> fromString(String string){
        return fromString(string, toMap(string), 0, string.length() - 1);
    }

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

    //TEST TEST TEST
    public TreeNode<RRRBlock> fromStringRRR(String string){
        return fromStringRRR(string, toMap(string), 0, string.length() - 1);
    }
    //TEST TEST TEST
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
    //TEST TEST TEST
    private RRRBlock encodeStringRRR(String string, Map<Character, Boolean> charMap){
        return new RRRBlock(encodeString(string, charMap));
    }

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


    private BooleanArray encodeString(String string, Map<Character, Boolean> charMap){
        BooleanArray booleanArray = new BooleanArray(string.length());
        for(int i = 0; i < string.length(); ++i){
            booleanArray.data[i] = charMap.get(string.charAt(i));
        }

        return booleanArray;
    }

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


    private boolean isInArray(List<Character> haystack, char needle, int low, int high){
        return haystack.subList(low, high).contains(needle);
    }

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
